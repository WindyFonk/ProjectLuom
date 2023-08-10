using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform[] wayPoints;
    public int waypointIndex;
    public float timeBetweenMove;
    private Vector3 targetDestination;

    public int state = 0;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        UpdateDestination();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 0:
                PatrolState();
                break;
        }

    }

    private void PatrolState()
    {
        if (Vector3.Distance(transform.position, targetDestination) < 2)
        {
            waypointCircle();
            StartCoroutine(Patrol());
        }
    }

    private IEnumerator Patrol()
    {
        yield return new WaitForSeconds(timeBetweenMove);
        UpdateDestination();
    }

    private void UpdateDestination()
    {
        targetDestination = wayPoints[waypointIndex].position;
        agent.SetDestination(targetDestination);
    }

    private void waypointCircle()
    {
        waypointIndex++;
        if (waypointIndex == wayPoints.Length)
        {
            waypointIndex = 0;
        }
    }

}
