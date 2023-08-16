using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float radius;
    [Range(0f, 360f)]
    public float angle;

    public GameObject player;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public bool seePlayer;
    public float distance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVrountine());
    }

    private IEnumerator FOVrountine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length > 0 )
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position- transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distancetoTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distancetoTarget, obstacleMask))
                {
                    seePlayer= true;
                    distance = Vector3.Distance(transform.position, target.transform.position);
                }
                else
                {
                    seePlayer= false;
                }
            }
            else
            {
                seePlayer = false;
            }
        }
        else if (seePlayer)
        {
            seePlayer= false;
        }
    }
}
