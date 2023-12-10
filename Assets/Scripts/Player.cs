using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public GameObject cameraLocation;
    public GameObject mesh;
    public float sensitivity = 1.0f;
    public float moveSpeed = 70;
    public float sprintSpeed = 20;

    public bool isGrounded = true;
    public float groundDrag = 5f;
    public float airDrag = 1f;
    public float jumpForce = 100;
    public float maxSpeed = 7;
    public float playerHeight = 1;
    public float gravityForce = 10;
    public float verticalVelocity = 0;

    float xRotation = 0;

    private CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;



        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isGrounded = characterController.isGrounded;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = jumpForce;
        }

        float verticalInput = Input.GetAxis("Vertical");
        float horizonalInput = Input.GetAxis("Horizontal"); 

        if (!isGrounded)
        {
            verticalVelocity -= gravityForce * Time.deltaTime;
        }
        else
        {
            //verticalVelocity = 0;
        }
        Vector3 moveDirection = verticalInput * forward + horizonalInput * right;
        characterController.Move((moveDirection * (isSprinting?sprintSpeed:moveSpeed) + transform.up*verticalVelocity)* Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity; 
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.Rotate(Vector3.up * mouseX);
        cameraLocation.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //cameraLocation.transform.Rotate(Vector3.up * mouseX);
        transform.rotation *= Quaternion.Euler(0,mouseX, 0);
        Debug.Log("movement"+verticalInput.ToString()+isSprinting+(transform.up*verticalVelocity).ToString());
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
