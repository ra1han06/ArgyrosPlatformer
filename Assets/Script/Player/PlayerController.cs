using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Unlock cursor agar bisa digunakan normal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Gerakan kiri kanan untuk platformer (Gunakan Global Right agar tidak terbalik saat rotasi)
        Vector3 right = Vector3.right;
        
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        // A untuk kiri, D untuk kanan
        float curSpeed = 0;
        if (canMove)
        {
            if (Input.GetKey(KeyCode.D))
            {
                curSpeed = isRunning ? runningSpeed : walkingSpeed;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                curSpeed = -(isRunning ? runningSpeed : walkingSpeed);
            }
        }
        
        float movementDirectionY = moveDirection.y;
        moveDirection = right * curSpeed;

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }
}