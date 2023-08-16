using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform[] wayPoints;
    public int waypointIndex = 0;
    public float timeBetweenMove;
    private Vector3 targetDestination;
    private Vector3 currentDestination;
    public Vector3 checkPosition;

    public int state = 0;

    private bool returning;

    public float detection;
    EnemyFOV enemyFOV;
    void Start()
    {
        checkPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        enemyFOV = GetComponent<EnemyFOV>();
    }

    // Update is called once per frame
    void Update()
    {

        switch (state)
        {
            case 0:
                PatrolState();
                break;
            case 1:
                CheckingState();
                break;
        }

    }

    private void FixedUpdate()
    {
        DetectMeter();
    }

    private void DetectMeter()
    {
        if (enemyFOV.seePlayer)
        {
            detection += (70 / enemyFOV.distance) * Time.deltaTime;
        }
        else
        {
            detection -= (70 / enemyFOV.distance) * Time.deltaTime;
        }

        if (detection > 10)
        {
            detection = 10;
        }

        if (detection < 0)
        {
            detection = 0;
        }
    }

    //Patrol State
    private void PatrolState()
    {
        UpdateDestination();

        if (Vector3.Distance(transform.position, targetDestination) < 1.5f || returning)
        {
            waypointCircle();
            Invoke("UpdateDestination", timeBetweenMove);
            Invoke("goToDestination", timeBetweenMove);
            returning = false;
        }
    }


    private void UpdateDestination()
    {
        targetDestination = wayPoints[waypointIndex].position;
    }
    private void goToDestination()
    {
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
        agent.isStopped = true;

        StartCoroutine(CheckLocation());
    }

    private IEnumerator CheckLocation()
    {
        yield return new WaitForSeconds(3f);
        agent.ResetPath();
        agent.SetDestination(checkPosition);
        if (Vector3.Distance(transform.position, checkPosition) < 2)
        {
            yield return new WaitForSeconds(5f);
            state = 0;
            goToDestination();
        }
    }

}
