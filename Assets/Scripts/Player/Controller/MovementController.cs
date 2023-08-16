using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class MovementController : MonoBehaviour
{
    [Header("Movement")]
    private float speed;
    public float walkspeed;
    public float runSpeed = 18f;
    public float crouchSpeed = 8f;
    public Transform orientation;
    public Transform cam;
    public bool isAiming;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    public float GroundedRadius = 0.28f;
    public float GroundedOffset = -0.14f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;


    [SerializeField] float turnSpeed = 0.1f;
    private float turnSmoothVelocity;

    float horizontal;
    float vertical;

    Rigidbody rb;
    private bool isWalking;
    public bool isCrouching;
    private bool isRunning;
    Vector3 direction;
    private GameObject _mainCamera;
    public bool Grounded;


    public Animator modelAnimator;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    public GameObject CinemachineCameraTarget;

    private bool canControl = true;
    private bool canStandUp = true;


    private ThrowObject throwObject;

    [SerializeField] Collider standingCollider;
    [SerializeField] Collider crouchingCollider;
    [SerializeField] Transform headPoint;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = walkspeed;
        isAiming = false;

        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/

        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        throwObject = GetComponent<ThrowObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!canControl)
        {
            return;
        }
        Move();
        AnimationController();
        Aiming();
        GroundedCheck();
    }

    private void Move()
    {
        horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");
        vertical = UnityEngine.Input.GetAxisRaw("Vertical");


        direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude > 0)
        {
            //Face toward moving direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
            Vector3 forwardDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            rb.AddForce(forwardDirection.normalized * speed * 100, ForceMode.Force);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        if (UnityEngine.Input.GetKey(KeyCode.LeftShift) &&!isCrouching)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.C) && canStandUp)
        {
            isCrouching = !isCrouching;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            isCrouching = false;
        }


        if (isCrouching)
        {
            speed = crouchSpeed;
        }
        else if (isRunning)
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkspeed;
        }

        standingCollider.enabled = !isCrouching;
        crouchingCollider.enabled = isCrouching;


    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (true)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += UnityEngine.Input.GetAxis("Mouse X") * deltaTimeMultiplier;
            _cinemachineTargetPitch += UnityEngine.Input.GetAxis("Mouse X") * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }
    private void AnimationController()
    {
        if (direction != new Vector3(0, 0, 0))
        {
            isWalking = true;
        }
        else isWalking = false;
        modelAnimator.SetBool("isWalking", isWalking);
        modelAnimator.SetBool("isCrouching", isCrouching);
        modelAnimator.SetBool("isRunning", isRunning);
    }

    public void setControl(bool canControl)
    {
        this.canControl = canControl;
        GetComponent<Collider>().enabled = canControl;
    }


    private void Aiming()
    {
        Vector3 mouseWorldPos = Vector3.zero;
        if (isAiming)
        {
            Vector3 worldAimTarget = mouseWorldPos;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        canStandUp = !Physics.Raycast(headPoint.position, transform.up, 20f);

        modelAnimator.SetBool("isGrounded", Grounded);
    }
}
