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

    [Header("Hand")]
    public int itemSelected = 0;
    public GameObject[] handGameObjects;
    public Inventory inventory;
    public MainMenu mainMenu;

    public GameObject[] buildables;

    public bool isBuilding = false;
    private Coroutine blueprintCoroutine;
    private GameObject blueprint;

    // Vector3 startPosition = new Vector3(0,300,0);



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainMenu.currentScreen != 0)
        {
            return;
        }
        //TODO: check proper velocity/accel time.deltaTime usecase
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.C);
        isGrounded = characterController.isGrounded;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity = jumpForce;
        }

        float verticalInput = Input.GetAxis("Vertical");
        float horizonalInput = Input.GetAxis("Horizontal");
        Vector3 horizontalMovement = verticalInput * transform.forward + horizonalInput * transform.right;


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

        characterController.Move((velocity + verticalVelocity * transform.up) * Time.deltaTime);

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity * 50;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity * 75;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90);

            transform.Rotate(Vector3.up * mouseX);
            cameraLocation.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mouseX, 0);
        }

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

        //Hand
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            handGameObjects[itemSelected].SetActive(false);
            if (scrollInput > 0)
            {
                itemSelected = (itemSelected + 1) % handGameObjects.Length;
            }
            else
            {
                itemSelected = (itemSelected - 1) % handGameObjects.Length;
            }
            if (itemSelected < 0)
            {
                itemSelected = handGameObjects.Length - 1;
            }
            handGameObjects[itemSelected].SetActive(true);
            // hand.GetComponent<MeshFilter>().mesh = handGameObjects[itemSelected];
        }

        //Atacking
        if (Input.GetMouseButtonDown(0) && !isBuilding)
        {
            StartCoroutine(attack());
            Ray ray = new Ray(cameraLocation.transform.position, cameraLocation.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5f, 1 << 6))
            {
                hit.collider.GetComponent<Destroyable>().Damage(10, (Tool)itemSelected, inventory);
            }
        }



        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StopBuilding();
            blueprintCoroutine = StartCoroutine(createBlueprint(0, 1, 0));
            // createBlueprintHelper(0, 1,0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopBuilding();
            blueprintCoroutine = StartCoroutine(createBlueprint(1, 1.5f, 0));
        }
        else if (isBuilding && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
        {
            StopBuilding();
        }

    }

    public void StopBuilding()
    {
        try
        {
            isBuilding = false;
            hand.SetActive(true);
            StopCoroutine(blueprintCoroutine);
            Destroy(blueprint);
        }
        catch { }
    }

    IEnumerator createBlueprint(int objectIndex, float gridSize, int yOffest)
    {
        blueprint = Instantiate(buildables[objectIndex]);
        Destroyable destroyable = blueprint.GetComponent<Destroyable>();
        if (destroyable.CheckCanBuild(inventory))
        {
            isBuilding = true;
            hand.SetActive(false);
            BoxCollider boxCollider = blueprint.GetComponent<BoxCollider>();
            boxCollider.enabled = false;
            Quaternion rotationOffset = Quaternion.Euler(Vector3.zero);


            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<BoxCollider>().enabled = false;
            while (!Input.GetMouseButton(0))
            {
                float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                if (scrollInput != 0)
                {
                    rotationOffset *= Quaternion.Euler(0, 90 * Mathf.Sign(scrollInput), 0);
                    blueprint.transform.rotation = rotationOffset;
                }
                Ray ray = new Ray(cameraLocation.transform.position, cameraLocation.transform.forward);
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, 100))
                {
                    Vector3 translatedSize = Math.multiplyVector3(boxCollider.size, blueprint.transform.localScale);
                    Vector3 startPos = new Vector3(Mathf.Round(raycastHit.point.x / gridSize) * gridSize, raycastHit.point.y + yOffest, Mathf.Round(raycastHit.point.z / gridSize) * gridSize);
                    Vector3 buildingPosition = FindLowestSafePosition(startPos, rotationOffset, translatedSize, 200f);
                    blueprint.transform.position = buildingPosition;

                    blueprint.SetActive(true);
                    cube.transform.position = buildingPosition;
                    cube.transform.rotation = rotationOffset;
                    cube.transform.localScale = translatedSize;
                }
                else
                {
                    blueprint.SetActive(false);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
            if (destroyable.CheckCanBuild(inventory))
            {
                destroyable.RemoveItems(inventory);
                isBuilding = false;
                hand.SetActive(true);
                boxCollider.enabled = true;
                blueprint = null;
            }
            else
            {
                Destroy(blueprint);
                blueprint = null;
            }
        }
        else
        {
            Destroy(blueprint);
            blueprint = null;
        }
    }

    public Vector3 FindLowestSafePosition(Vector3 startPosition, Quaternion startRotation, Vector3 boxSize, float maxCheckDistance)
    {
        float maxY = startPosition.y + maxCheckDistance;
        Vector3 currentPosition = startPosition;

        while (currentPosition.y < maxY)
        {
            if (!Physics.CheckBox(currentPosition, boxSize / 2, startRotation))
            {
                return currentPosition;
            }
            else
            {
                currentPosition.y += 0.01f;
            }

        }

        Debug.LogError("Not found Lowest safe position");
        return startPosition;
    }
    IEnumerator attack()
    {
        if (!isAttacking)
        {
            float rotationSpeed = 8;
            Quaternion initialRotation = Quaternion.Euler(Vector3.zero);
            Quaternion targetRotation = Quaternion.Euler(new Vector3(179f, -15f, 0f));
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

public enum Tool
{
    SWORD,
    AXE,
    PICKAXE,
    TOOLS_AMOUNT
}
