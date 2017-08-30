using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam; // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward; // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump; // the world-relative desired move direction, calculated from the camForward and user input.

        private bool m_CanMove = true;
        private bool m_IsMoving = false;

        private AudioSource m_AudioSource;
        private AudioManager m_audioManager;
       

        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();

            m_AudioSource = GetComponent<AudioSource>();
            m_audioManager = FindObjectOfType<AudioManager>();
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

        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                m_Jump = Input.GetKeyDown(KeyCode.Space);
            }

            if (m_IsMoving && !m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }

            //if (m_IsMoving && !m_audioManager.IsPlaying("Footstep"))
            //{
            //    m_audioManager.Play("Footstep");
            //}
        }

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (!m_CanMove) { return; }
            
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            if (h == 0 && v == 0)
            {
                m_IsMoving = false;
            }
            else
            {
                m_IsMoving = true;
            }

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }

//#if !MOBILE_INPUT
            bool sprint = false;
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) sprint = true;
//#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, false, sprint, m_Jump);
            m_Jump = false;
        }

        private void AllowMovement()
        {
            //Debug.Log("Allowing player movement");
            m_CanMove = true;
        }

        private void PreventMovement()
        {
            //Debug.Log("Preventing player movement");
            m_CanMove = false;
            m_Move = Vector3.zero;
            m_Character.Move(Vector3.zero, false, false, false);
        }

    }

}