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
    private bool cursorLocked = true;
    public float pickUpDistance = 80f;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = Camera.main;
        // animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftControl))
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

        if (direction.magnitude >= 0.1f && cursorLocked == true)
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
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * pickUpDistance, Color.red);
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickUpDistance))
            {

                Debug.Log(hit.collider.gameObject.name);
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null)
                {
                    if (interactable.pickedUp == false)
                    {
                        InventoryManager.Instance.addItem(interactable);
                        interactable.gameObject.SetActive(false);
                    }
                    interactable.pickedUp = true;
                    Debug.Log("Interact");
                }

            }
        }
    }


}