using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    private float initialSpeed;
    private PlayerInput playerInput;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool canRegenStamina;
    private bool k = true;
    private bool h = true;
    private float f;
    public float staminaRegenValue;
    public float staminaDecreaseValue;
    private bool canSprint;

    public new Rigidbody2D rigidbody;
    public Animator animator;
    public Slider staminaSlider;
    public GameObject FillArea;
    public float speed = 7.0f;
    public float sprintMultiplier;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody2D>();
        initialSpeed = speed;
        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;

        canRegenStamina = true;
        canSprint = true;
    }

    private void Update()
    {
        // Sprinting
        if (playerInput.actions["Sprint"].IsPressed() && stamina > 0f && canSprint)
        {
            speed = initialSpeed * sprintMultiplier;
            isSprinting = true;
            h = true;
            f = 0f;
        }

        else if (playerInput.actions["Sprint"].WasReleasedThisFrame() && stamina > 0f && canSprint)
        {
            StartCoroutine(SprintCooldown(0.33f));
        }

        else
        {
            speed = initialSpeed;
            isSprinting = false;

            if (h)
            {
                canRegenStamina = false;

                if (f < 1.5f)
                {
                    f += Time.deltaTime;
                }

                else
                {
                    canRegenStamina = true;
                    f = 0f;
                    h = false;
                }
            }
        }

        // Stamina
        if (staminaSlider.value == 0f)
        {
            FillArea.SetActive(false);
        }

        else
        {
            FillArea.SetActive(true);
        }

        if (stamina <= 0 && k)
        {
            StartCoroutine(SprintCooldown(4 - staminaRegenValue * 10));
        }

        if (!isSprinting && canRegenStamina)
        {
            ChangeStamina(staminaRegenValue);
        }

        else if (isSprinting)
        {
            ChangeStamina(staminaDecreaseValue);
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

    private void ChangeStamina(float i)
    {
        stamina += i;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        staminaSlider.value = stamina;
    }

    private IEnumerator RegenStaminaCooldown(float speed)
    {
        yield return new WaitForSeconds(speed);
        canRegenStamina = true;
    }

    private IEnumerator SprintCooldown(float cooldownTime)
    {
        k = false;
        canSprint = false;
        yield return new WaitForSeconds(cooldownTime);
        canSprint = true;
        k = true;
    }
}

