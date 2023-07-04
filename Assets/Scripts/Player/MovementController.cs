using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MovementController : MonoBehaviour
{
    [Header("Movement")]
    private float speed = 5f;
    public float walkspeed;
    public float runSpeed = 18f;
    public Transform orientation;
    public Transform cam;


    [SerializeField] float turnSpeed = 0.1f;
    private float turnSmoothVelocity;

    float horizontal;
    float vertical;

    Vector3 moveDirection;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = walkspeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * vertical + orientation.right* horizontal;

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude > 0)
        {
            //Face toward moving direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 forwardDirection = Quaternion.Euler(0,targetAngle,0)*Vector3.forward;
            rb.AddForce(forwardDirection.normalized * speed, ForceMode.Force);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkspeed;
        }
    }
}
