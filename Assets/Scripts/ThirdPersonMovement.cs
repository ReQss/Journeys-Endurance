using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using Cinemachine;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public Interactable focus;
    public Animator animator;
    public CharacterController controller;
    public Transform cam;
    public Camera cinemachineCamera;
    Camera camera;


    public float speed = 6;
    public float runSpeed = 1.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 3;
    public AnimationCurve jumpCurve; // Add this curve for jump
    private float jumpTime; // Track time for jump curve
    private bool isJumping = false;
    Vector3 velocity;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;
    public static bool cursorLocked = true;
    public float pickUpDistance = 80f;
    private float playerVelocity = 0.0f;
    public float acceleration = 0.1f;
    public float decceleration = 0.5f;
    int velocityHash;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = Camera.main;
        velocityHash = Animator.StringToHash("Velocity");
    }

    void Update()
    {
        if (GameManager.Instance.isPlayerInteracting == true)
        {
            animator.SetFloat("Velocity", 0);
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(parameter.name, false);
                }
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (cursorLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                camera.GetComponent<CinemachineBrain>().enabled = false;
            }
            else
            {
                camera.GetComponent<CinemachineBrain>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
            cursorLocked = !cursorLocked;
        }

        bool runPressed = Input.GetKey("right shift");

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && !isJumping)
        {
            velocity.y = -2f;
            animator.SetBool("Jump", false);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpTime = 0f;
            isJumping = true;
            animator.SetBool("Jump", true);
        }

        if (isJumping)
        {
            jumpTime += Time.deltaTime;
            velocity.y = jumpCurve.Evaluate(jumpTime) * jumpHeight;

            if (jumpTime >= jumpCurve.keys[jumpCurve.length - 1].time)
            {
                isJumping = false;
                velocity.y = -2f;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f && cursorLocked)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            playerVelocity = Mathf.Clamp(playerVelocity + Time.deltaTime * acceleration, 0f, 1f);
            float moveSpeed = speed * (runPressed ? runSpeed : 1f);
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime * playerVelocity * 3f);
        }
        else if (playerVelocity > 0)
        {
            playerVelocity = Mathf.Clamp(playerVelocity - Time.deltaTime * decceleration, 0f, 1f);
        }

        animator.SetFloat(velocityHash, playerVelocity);
    }
}
