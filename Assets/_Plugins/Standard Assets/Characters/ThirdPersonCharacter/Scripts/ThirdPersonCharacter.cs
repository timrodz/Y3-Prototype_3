using System.Collections;
using UnityEngine;


namespace UnityStandardAssets.Characters.ThirdPerson
{

    public enum MoveState
    {
        WALKING,
        JUMPING,
        SPRINTING
    }

    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class ThirdPersonCharacter : MonoBehaviour
    {
        [SerializeField] float m_MovingTurnSpeed = 360;
        [SerializeField] float m_StationaryTurnSpeed = 180;
        [SerializeField] float m_JumpPower = 12f;
        [Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
        [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] float m_MoveSpeedMultiplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;

        Rigidbody m_Rigidbody;
        public Animator m_Animator;
        bool m_IsGrounded;
        float m_OrigGroundCheckDistance;
        const float k_Half = 0.5f;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;
        float m_CapsuleHeight;
        Vector3 m_CapsuleCenter;
        CapsuleCollider m_Capsule;
        bool m_Crouching;

        MoveState state;

        private bool m_CanMove = true;
        float m_MovementPushForce = 0.1f;

        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            EventManager.StartListening(EventName.DialogueStart, PreventMovement);
            EventManager.StartListening(EventName.DialogueEnd, AllowMovement);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            EventManager.StopListening(EventName.DialogueStart, PreventMovement);
            EventManager.StopListening(EventName.DialogueEnd, AllowMovement);
        }

        private void AllowMovement()
        {
            m_CanMove = true;
			m_MovementPushForce = 0.1f;
        }

        private void PreventMovement()
        {
            m_CanMove = false;
			StartCoroutine(ReduceMoveForce(4));
        }
		
		private IEnumerator ReduceMoveForce(float duration)
        {
            for (float t = 0; t <= 1; t += (Time.deltaTime / duration))
            {
                m_MovementPushForce -= t;
                if (m_MovementPushForce < 0)
                {
                    m_MovementPushForce = 0;
                }
				
                m_Animator.SetFloat("Forward", m_ForwardAmount, m_MovementPushForce, Time.deltaTime);
                m_Animator.SetFloat("Turn", m_TurnAmount, m_MovementPushForce, Time.deltaTime);
				
                yield return null;
            }
            m_MovementPushForce = 0;
        }

        private IEnumerator JumpTime(float duration, float delay)
        {

            // jump!
            m_IsGrounded = false;
            m_GroundCheckDistance = 0.1f;
            EventManager.Invoke(EventName.PlayerJump);

            m_Animator.Play("Jump");

            float delayTime = 0.0f;

            while(delayTime < delay)
            {
                delayTime += Time.deltaTime;

                yield return null;
            }

            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);

            float time = 0.0f;

            while(time < duration)
            {
                time += Time.deltaTime;

                m_Animator.SetFloat("Jump", 1 - (time / duration));
                
                yield return null;
            }
        }

        Vector3 prevMove = Vector3.zero;
        public void Move(Vector3 move, bool crouch, bool sprint, bool jump)
        {

            //Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + move, Color.blue);

            if (move.magnitude > 1f) move.Normalize();

            //Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + move, Color.red);

            move = transform.InverseTransformDirection(move);
            //Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + move, Color.green);

            CheckGroundStatus();

            move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            //Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + move, Color.yellow);

            m_TurnAmount = Mathf.Atan2(move.x, move.z);
            m_ForwardAmount = move.z;


            ApplyExtraTurnRotation();

            float sprintModifier = 1.0f;
            sprintModifier = (sprint == true) ? 2.5f : 1.0f;

            Vector3 _jumpAmount = Vector3.zero;

            //if(jump && state != MoveState.JUMPING)
            //{
            //    m_Animator.SetFloat("Jump", 1.0f);
            //    state = MoveState.JUMPING;
            //    StartCoroutine("JumpTime", 1.0f);

            //    _jumpAmount.y += m_JumpPower;
            //}

            if(move != prevMove)
            {
                prevMove = move;
                m_Rigidbody.velocity = (((transform.forward) * m_ForwardAmount) * m_MoveSpeedMultiplier * sprintModifier) + new Vector3(0, m_Rigidbody.velocity.y, 0);
            }


            //m_Rigidbody.AddForce(_jumpAmount, ForceMode.Impulse);

            if (m_IsGrounded)
                HandleGroundedMovement(crouch, jump);
            else
                HandleAirborneMovement();
        }




        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }

        void HandleGroundedMovement(bool crouch, bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (jump && !crouch)
            {

                StartCoroutine(JumpTime(1.0f, .25f));
            }
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.05f) + (Vector3.down * m_GroundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.05f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
            }
            else
            {
                print("FALSE" + gameObject.name);
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
            }
        }
    }
}