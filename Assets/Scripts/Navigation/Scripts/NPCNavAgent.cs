using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCNavAgent : MonoBehaviour {

    NavMeshAgent m_agent;

    public Transform FollowTarget;
    public float FollowDistance;



	// Use this for initialization
	void Start ()
    {
        m_agent = GetComponent<NavMeshAgent>();


        //For Testing
        if (FollowTarget)
            SetFollowTarget(FollowTarget);

    }

    // Update is called once per frame
    void Update () {

        //if (m_agent.pathPending || m_agent.remainingDistance > 0.1f)
        //    return;

        if(Input.GetKey(KeyCode.T))
        {
            StopFollowTarget();
        }

    }


    public void SetFollowTarget(Transform target)
    {
        FollowTarget = target;

        m_agent.isStopped = false;

        //Enable auto repathing - Update path when it becomes invalid
        m_agent.autoRepath = true;

        //Set a high quality avoidance algorithm
        m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        StartCoroutine("UpdateFollowTarget");
    }

    public void StopFollowTarget()
    {
        StopCoroutine("UpdateFollowTarget");

        //m_agent.SetDestination(transform.position + transform.forward * 3.0f);

        //StartCoroutine("FadeNPC");
    }


    private IEnumerator FadeNPC()
    {
        Material mat = GetComponent<MeshRenderer>().material;

        Color color = mat.color;

        while(color.a > 0.0f)
        {
            color.a -= Time.deltaTime / 3.0f;
            mat.color = color;

            yield return null;
        }
    }


    private IEnumerator UpdateFollowTarget()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            m_agent.SetDestination(FollowTarget.position - (FollowTarget.forward * FollowDistance));
        }
    }
    


}
