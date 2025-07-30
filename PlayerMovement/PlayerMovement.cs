using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float Walkspeed = 12f;
    public float Sprintspeed = 20f;
    public float gravity = -9.81f;
    public float JumpHight = 3f;

    public Transform groundCheck;
    public float GroundDistance = 0.4f; //the sphere radius of ground check
    public LayerMask GroundMask;



    Vector3 Velocity;
    public bool isGrounded;
    public bool isSprinting;


    // === Sprint FOV Variables ===
    public Camera playerCamera;
    public float walkFOV = 60f;
    public float sprintFOV = 75f;
    public float fovTransitionSpeed = 5f;

    // Update is called once per frame
    void LateUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;


        // === Sprinting logic === 
        bool hasInput = x != 0 || z != 0; //this is to detect that we are actually moving, so we don't consider "is sprinting = true" if we only press shift without actually moving

        if (Input.GetKey(KeyCode.LeftShift) && hasInput)
        {
            speed = Sprintspeed;
            isSprinting = true;
        }
        else
        {
            speed = Walkspeed;
            isSprinting = false;
        }

        controller.Move(move * speed * Time.deltaTime);

        // === Jumping logic === 
        if (Input.GetButton("Jump") && isGrounded) 
        {
            Velocity.y = Mathf.Sqrt(JumpHight * -2 * gravity);
        }


        // === Smooth FOV transition ===
        float targetFOV = isSprinting ? sprintFOV : walkFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, GroundDistance, GroundMask);
        
        if(isGrounded && Velocity.y < 0 )
        {
            Velocity.y = -2f;
        }

        


            Velocity.y += gravity * Time.deltaTime;

        controller.Move(Velocity * Time.deltaTime); //Time.deltatime again is necessary since the equation demands multiplying by time squared

    }
}
