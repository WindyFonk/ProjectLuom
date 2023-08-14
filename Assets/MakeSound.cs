using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSound : MonoBehaviour
{
    public bool canMakeSound;
    EnemyPatrol enemyPatrol;
    public float radius = 8;
    public LayerMask targetMask;
    private GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && canMakeSound)
        {
            Debug.Log("Landed");
            canMakeSound= false;
            checkEnemy();
        }
    }

    private void checkEnemy()
    {
        Collider[] check = Physics.OverlapSphere(transform.position, radius, targetMask);
        float minDistance = radius;

        if (check.Length > 0)
        {
            foreach (Collider collider in check)
            {
                float distance = Vector3.Distance(collider.transform.position, transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    enemy = collider.gameObject;
                    enemy.GetComponent<EnemyPatrol>().state = 1;
                }
            }
        }
        else
        {
            enemy = null;
        }
    }
}
