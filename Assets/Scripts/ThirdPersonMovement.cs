using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using Cinemachine;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public AudioSource footstepsSound;
    public AudioSource healingSound;

    public GameObject healingUI;
    public Item focus;
    public Animator animator;
    public CharacterController controller;
    public Transform cam;
    public Camera cinemachineCamera;
    public GameObject birdCamera;
    public GameObject playerCamera;
    public Rigidbody birdRigidbody;
    public Camera camera;


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
    private float playerVelocity = 0.0f;
    public float acceleration = 0.1f;
    public float decceleration = 0.5f;
    int velocityHash;
    public LayerMask baseLayer;
    public bool inBase = false;
    private bool isTakingDamage = false;
    public HealthBar healthBar;
    private bool isHealed = false;
    public bool birdMode;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // camera = Camera.main;
        velocityHash = Animator.StringToHash("Velocity");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            // Debug.Log(cursorLocked);

            GameManager.cursorLocked = !GameManager.cursorLocked;
        }
        if (!GameManager.cursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            camera.GetComponent<CinemachineBrain>().enabled = false;
        }
        else
        {
            camera.GetComponent<CinemachineBrain>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

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

        BirdMovement();
        if (birdMode == false)
            Movement();
        HealingAndDamage();
    }
    public void BirdMovement()
    {
        if (birdRigidbody == null) return;

        if (Input.GetKeyDown(KeyCode.O))
        {
            birdMode = !birdMode;
        }
        if (birdMode)
        {
            playerCamera.SetActive(false);

            birdCamera.SetActive(true);
            return;
        }
        else
        {
            birdRigidbody.useGravity = false;
            playerCamera.SetActive(true);
            birdCamera.SetActive(false);
        }
    }
    public void Movement()
    {

        bool runPressed = Input.GetKey("right shift");
        inBase = Physics.CheckSphere(groundCheck.position, 5f, baseLayer);
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

        if (direction.magnitude >= 0.1f && GameManager.cursorLocked)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            playerVelocity = Mathf.Clamp(playerVelocity + Time.deltaTime * acceleration, 0f, 1f);
            float moveSpeed = speed * (runPressed ? runSpeed : 1f);
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime * playerVelocity * 3f);
            footstepsSound.enabled = true;
        }
        else if (playerVelocity > 0)
        {
            playerVelocity = Mathf.Clamp(playerVelocity - Time.deltaTime * decceleration, 0f, 1f);
            footstepsSound.enabled = false;
        }

        animator.SetFloat(velocityHash, playerVelocity);
    }
    public void HealingAndDamage()
    {
        //   NIGHT TIME DAMAGE
        if (!inBase && GameManager.Instance.isDay == false && isTakingDamage == false)
        {
            StartCoroutine(NightDamage());
        }
        // IN BASE HEAL 
        if (inBase)
        {
            healingSound.enabled = true;
            StartCoroutine(BaseHeal());
            healingUI.SetActive(true);
        }
        else
        {
            healingSound.enabled = false;
            healingUI.SetActive(false);
        }
    }
    public void TakeDamage()
    {
        GameManager.Instance.playerHealth -= 1;
        healthBar.TakeDamage();
    }
    public void HealPlayer()
    {
        GameManager.Instance.playerHealth += 1;
        healthBar.AddHealth();
    }
    IEnumerator NightDamage()
    {

        isTakingDamage = true;
        Debug.Log("Player damaged ");
        TakeDamage();
        yield return new WaitForSeconds(GameManager.Instance.timeForNightDamage);
        isTakingDamage = false;

    }
    IEnumerator BaseHeal()
    {
        if (isHealed == false)
        {
            isHealed = true;
            Debug.Log("Player healed");
            HealPlayer();
            yield return new WaitForSeconds(GameManager.Instance.timeForNightDamage);
            isHealed = false;
        }
    }
}
