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

    public bool isGrounded = true;
    public float groundDrag = 5f;
    public float airDrag = 1f;
    public float jumpForce = 100;
    public float maxSpeed = 7;
    public float playerHeight = 1;

    float xRotation = 0;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 1f + 0.3f);

        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity; 
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.Rotate(Vector3.up * mouseX);
        cameraLocation.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        cameraLocation.transform.Rotate(Vector3.up * mouseX);


        //camera.transform.Rotate(mouseY, mouseX, 0);
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        float verticalInput = Input.GetAxis("Vertical");
        float horizonalInput = Input.GetAxis("Horizontal"); 
        Debug.Log("movement"+verticalInput.ToString()+isGrounded);

        Vector3 moveDirection = verticalInput * transform.forward + horizonalInput * transform.right;

        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
        //SpeedControl();
    }


    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
