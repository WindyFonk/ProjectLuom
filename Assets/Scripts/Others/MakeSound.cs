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
    private Vector3 soundPosition;
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

            ContactPoint contact = collision.contacts[0];
            soundPosition = contact.point;
            Debug.Log(soundPosition);
            canMakeSound = false;
            checkEnemy(soundPosition);
        }
    }

    private void checkEnemy(Vector3 soundPos)
    {
        Collider[] check = Physics.OverlapSphere(soundPos, radius, targetMask);
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
                    if (enemy)
                    {
                        
                        Debug.Log(enemy.name);                      
                    }
                    enemy.GetComponent<EnemyPatrol>().state = 1;
                    enemy.GetComponent<EnemyPatrol>().checkPosition = soundPos;
                }
            }

        }
        else
        {
            enemy = null;
            return;
        }
    }
}
