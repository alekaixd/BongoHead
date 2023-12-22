using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Movement : MonoBehaviour
{
    public float speed = 7.0f;
    public float sprintMultiplier;
    private float initialSpeed;
    public PlayerInput playerInput;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;

    public new Rigidbody2D rigidbody;
    public Animator animator;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        initialSpeed = speed;
    }

    private void Update()
    {
        if (playerInput.actions["Sprint"].IsPressed())
        {
            speed = initialSpeed * sprintMultiplier;
        }

        else
        {
            speed = initialSpeed;
        }
    }

    private void FixedUpdate()
    {
        smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, 0.1f);
        rigidbody.velocity = smoothedMovementInput * speed;
    }

    private void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }
}

