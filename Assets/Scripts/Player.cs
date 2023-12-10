using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject cameraLocation;
    public GameObject mesh;
    public Camera cameraComponent;
    public GameObject hand;

    [Header("Camera Settings")]
    public float sensitivity = 1.0f;
    public float defaultFov = 60;
    public float sprintFov = 70;

    [Header("Movement")]
    public float maxMoveSpeed;
    public float maxJumpSpeed;
    public float maxCrouchSpeed;
    public float sprintSpeedMultiplier = 1.5f;
    public float jumpSpeedMultiplier = 7;
    public float sprintMaxSpeedMultiplier;
    public float jumpMaxSpeedMultiplier;
    public float playerHeight = 1;
    public float cameraHeight = 0.72f;
    public float cameraHeightCrouch = 0f;
    public float crouchSpeed = 3;
    private float timeSinceSprintChange = 0;
    public float timeSinceCrouchChange = 0;

    [Header("Acceleration")]
    public float moveAcceleration;
    public float moveDecceleration;
    public float crouchAcceleration;
    public float crouchDecceleration;
    public float jumpAcceleration;
    public float jumpDecceleration;
    public float minInputThreshold;
    public float minHorizontalVelocity;

    [Header("Jumping")]
    public float jumpForce = 100;
    public float gravityForce = 10;
    public bool isGrounded = true;

    private bool wasSprinting = false;
    private bool wasCrouching = false;
    private bool isAttacking = false;
    private float xRotation = 0;
    private CharacterController characterController;

    public Vector3 velocity;
    float verticalVelocity = 0;



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: check proper velocity/accel time.deltaTime usecase
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.C);
        isGrounded = characterController.isGrounded;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity = jumpForce;
        }

        float verticalInput = Input.GetAxis("Vertical");
        float horizonalInput = Input.GetAxis("Horizontal");
        Vector3 horizontalMovement = (verticalInput * transform.forward + horizonalInput * transform.right);


        float speedupVelocity = 0;
        float slowdownVelocity = 0;
        if (!isGrounded)
        {
            //Jumping
            verticalVelocity -= gravityForce * Time.deltaTime;
            speedupVelocity = jumpAcceleration;
            slowdownVelocity = jumpDecceleration;

        }
        else if (isCrouching)
        {
            //Couching
            speedupVelocity = crouchAcceleration;
            slowdownVelocity = crouchDecceleration;
        }
        else
        {
            //Moving
            speedupVelocity = moveAcceleration * (isSprinting ? sprintSpeedMultiplier : 1);
            slowdownVelocity = moveDecceleration * (isSprinting ? sprintSpeedMultiplier : 1);
        }
        if (horizontalMovement.magnitude > minInputThreshold)
        {
            velocity += horizontalMovement * speedupVelocity * Time.deltaTime;
        }
        else
        {
            velocity -= velocity * slowdownVelocity * Time.deltaTime;
        }
        //Vector3 moveDirection = verticalInput * forward + horizonalInput * right;
        //characterController.Move((moveDirection * (isSprinting ? sprintSpeedMultiplier : 1)*moveSpeed*(isGrounded?1:jumpSpeedMultiplier) + transform.up * verticalVelocity) * Time.deltaTime);

        //TODO: max speed not working
        //Handle max speed
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        float maxSpeed = (isSprinting ? sprintSpeedMultiplier : 1) * (isGrounded ? 1 : jumpMaxSpeedMultiplier) * maxMoveSpeed;
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity *= maxSpeed / horizontalVelocity.magnitude;
        }
        if (horizontalVelocity.magnitude > minHorizontalVelocity)
        {
            velocity = new Vector3(horizontalVelocity.x, 0, horizontalVelocity.z);
        }
        else
        {
            velocity = new Vector3(0, 0, 0);
        }

        characterController.Move((velocity+verticalVelocity*transform.up) * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity * 1.5f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.Rotate(Vector3.up * mouseX);
        cameraLocation.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseX, 0);

        //Sprinting
        if (wasSprinting != isSprinting)
        {
            timeSinceSprintChange = 0;
        }
        else
        {
            timeSinceSprintChange += Time.deltaTime;
        }
        wasSprinting = isSprinting;
        if (isSprinting)
        {
            cameraComponent.fieldOfView = Mathf.Lerp(defaultFov, sprintFov, timeSinceSprintChange * 5);
        }
        else
        {
            cameraComponent.fieldOfView = Mathf.Lerp(sprintFov, defaultFov, timeSinceSprintChange * 5);
        }

        //Crouching
        if (wasCrouching != isCrouching)
        {
            timeSinceCrouchChange = 0;
        }
        else
        {
            timeSinceCrouchChange += Time.deltaTime;
        }
        wasCrouching = isCrouching;
        if (isCrouching)
        {
            cameraLocation.transform.position = transform.position + new Vector3(0, Mathf.Lerp(cameraHeight, cameraHeightCrouch, timeSinceCrouchChange * crouchSpeed), 0);
        }
        else
        {
            cameraLocation.transform.position = transform.position + new Vector3(0, Mathf.Lerp(cameraHeightCrouch, cameraHeight, timeSinceCrouchChange * crouchSpeed), 0);
        }

        //Atacking
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("attack");
            StartCoroutine(attackSword());
        }
    }

    IEnumerator attackSword()
    {
        if (!isAttacking)
        {
            float rotationSpeed = 8;
            Quaternion initialRotation = Quaternion.Euler(Vector3.zero);
            Quaternion targetRotation = Quaternion.Euler(new Vector3(40f, -15f, 0f));
            isAttacking = true;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * rotationSpeed;
                hand.transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, t);
                yield return null;
            }

            yield return new WaitForSeconds(1 / rotationSpeed);
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * rotationSpeed;
                hand.transform.localRotation = Quaternion.Lerp(targetRotation, initialRotation, t);
                yield return null;
            }

            isAttacking = false;
        }
    }

    //private void SpeedControl()
    //{
    //    Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    //    if(flatVel.magnitude > maxSpeed)
    //    {
    //        Vector3 limitedVel = flatVel.normalized * maxSpeed;
    //        rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
    //    }
    //}
}
