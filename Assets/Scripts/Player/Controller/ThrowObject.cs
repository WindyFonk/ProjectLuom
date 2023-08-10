using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float throwStrengh;
    [SerializeField] LayerMask targetMask;
    [SerializeField] Transform handRootTransform;
    [SerializeField] Transform castOrigin;
    [SerializeField] Camera _camera;
    private Animator animator;


    [SerializeField] LayerMask groundMask;
    [SerializeField] GameObject cursor;
    // Start is called before the first frame update
    private GameObject _object,holdingObject;
    private bool isHolding;
    public bool isAiming;
    Vector3 throwDirection;
    Vector3 lookDirection;

    RaycastHit hit;

    void Start()
    {
        animator= GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        checkThrowObject();
        showUIThrow();
        grabAndThrowObject();

    }

    private void checkThrowObject()
    {
        if (isHolding) return;

        Collider[] check = Physics.OverlapSphere(castOrigin.position, radius, targetMask);
        float minDistance = radius;

        if (check.Length > 0)
        {
            Transform target = check[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            foreach (Collider collider in check)
            {
                float distance = Vector3.Distance(collider.transform.position, transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _object = collider.gameObject;
                }
            }
        }
        else
        {
            _object = null;
        }
    }

    private void showUIThrow()
    {
        cursor.SetActive(isAiming);

        if (!isAiming) return;

        Ray camRay = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(camRay, out hit, 100f, groundMask))
        {
            cursor.transform.position = hit.point + Vector3.up * 0.1f;

            throwDirection = CalculateVelocity(hit.point, handRootTransform.position, 1f);
        }
    }

    private void grabAndThrowObject()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isHolding) 
        {
            animator.SetTrigger("PickUp");
        }

        if (Input.GetKeyDown(KeyCode.R) && isHolding)
        {
            dropObject();
        }

        if (Input.GetMouseButton(1) && isHolding)
        {
            isAiming = true;
            if (!isHolding) return;
            lookDirection = hit.point;


            if (Input.GetMouseButton(0))
            {
                animator.Play("Throw");
            }
        }
        else
        {
            isAiming = false;
        }
    }

    public void grabObject()
    {
        holdingObject = _object;
        holdingObject.transform.SetParent(handRootTransform, false);
        holdingObject.GetComponent<Collider>().enabled = false;
        holdingObject.GetComponent<Rigidbody>().isKinematic = true;
        isHolding = true;
        holdingObject.transform.position = handRootTransform.position;

    }

    private void dropObject()
    {
        holdingObject.GetComponent<Collider>().enabled = true;
        holdingObject.transform.SetParent(null); 
        holdingObject.GetComponent<Rigidbody>().isKinematic = false;
        isHolding = false;
        holdingObject = null;
    }

    private void throwObject()
    {

        holdingObject.GetComponent<Collider>().enabled = true;
        holdingObject.transform.SetParent(null);
        holdingObject.GetComponent<Rigidbody>().isKinematic = false;
        holdingObject.GetComponent<Rigidbody>().velocity = throwDirection;
        isHolding = false;
        holdingObject = null;      
    }

    private Vector3 CalculateVelocity (Vector3 target, Vector3 origin, float time) { 
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;


        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f*Mathf.Abs(Physics.gravity.y)*time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }
}
