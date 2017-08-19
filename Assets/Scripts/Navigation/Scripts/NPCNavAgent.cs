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



    }


    public void SetFollowTarget(Transform target)
    {
        FollowTarget = target;

        //Enable auto repathing - Update path when it becomes invalid
        m_agent.autoRepath = true;

        //Set a high quality avoidance algorithm
        m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        //Set agent target destination
        StartCoroutine("UpdateFollowTarget");

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
