using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BirdFlyingMovement : MonoBehaviour
{
    [Header("Flying Settings")]
    public float flySpeed = 6.0f;

    public float ascendSpeed = 4.0f;
    public float descendSpeed = 4.0f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    [Header("Height Settings")]
    public float minHeight = 5.0f;


    private CharacterController controller;
    private Vector3 velocity;
    private Transform cam;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        HandleMovement();
        ApplyGravity();
        MoveCharacter();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * flySpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            velocity.y = ascendSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            velocity.y = -descendSpeed;
        }
    }

    private void ApplyGravity()
    {
        if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftControl))
        {
            velocity.y += gravity * Time.deltaTime;
        }

        if (transform.position.y <= minHeight)
        {
            velocity.y = Mathf.Max(velocity.y, 0);
            Vector3 correctedPosition = transform.position;
            correctedPosition.y = minHeight;
            transform.position = correctedPosition;
        }
    }

    private void MoveCharacter()
    {
        controller.Move(velocity * Time.deltaTime);
    }
}
