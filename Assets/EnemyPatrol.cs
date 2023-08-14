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
        /*switch (state)
        {
            case 0:
                PatrolState();
                break;
            case 1:
                CheckingState();
                break;
        }*/

        if (state == 0)
        {
            PatrolState();

        }
        else if (state== 1)
        {
            CheckingState();
        }

        Debug.Log(state);
    }

    //Patrol State
    private void PatrolState()
    {
        if (Vector3.Distance(transform.position, targetDestination) < 2)
        {
            StartCoroutine(Patrol());
        }
    }

    private IEnumerator Patrol()
    {
        waypointCircle();
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


    //Checking State
    private void CheckingState()
    {
        StartCoroutine(CheckLocation());
    }

    private IEnumerator CheckLocation()
    {
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(3f);
        state = 0;
        UpdateDestination();
    }

}
