using System.Collections;


using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public Animator animator;
    public CharacterController controller;
    public Transform cam;

    public float speed = 6;
    private float runSpeed = 1.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 3;
    Vector3 velocity;
    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool("Walk");
        bool isRunning = animator.GetBool("Run");
        bool runPressed = Input.GetKey("right shift");
        //jump
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        //gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        //walk
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (runPressed)
            {
                isRunning = true;
                isWalking = false;
                controller.Move(moveDir.normalized * speed * runSpeed * Time.deltaTime);
            }
            else
            {
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
                isRunning = false;
                isWalking = true;
            }
        }
        else
        {
            isRunning = false;
            isWalking = false;
        }
        animator.SetBool("Walk", isWalking);
        animator.SetBool("Run", isRunning);
    }
}