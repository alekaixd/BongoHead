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
    [SerializeField] private float maxHp;
    [SerializeField] private float hp;
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float rollVelocity;
    [SerializeField] private float rollCooldown;
    [SerializeField] private float rollDuration;
    [SerializeField] private float rollStaminaCost;
    private bool isSprinting;
    private bool canRegenStamina;
    private bool k = true;
    private bool h = true;
    private float f;
    public float staminaRegenValue;
    public float staminaDecreaseValue;
    private bool canSprint;
    private bool canRoll;
    private bool isRolling;
    

    private Rigidbody2D rb;
    private Animator playerAnimator;
    public Slider staminaSlider;
    public Slider hpSlider;
    public float speed = 7.0f;
    public float sprintMultiplier;
    private TrailRenderer tr;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        this.rb = this.GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        playerAnimator = GetComponent<Animator>();
        initialSpeed = speed;
        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;
        hp = maxHp;
        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;

        canRegenStamina = true;
        canSprint = true;
        canRoll = true;
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
        }

        // Rolling
        if (playerInput.actions["Roll"].IsPressed() && canRoll)
        {
            stamina -= rollStaminaCost;
            staminaSlider.value = stamina;
            h = true;
            f = 0f;
            StartCoroutine(Roll(rollCooldown));
            StartCoroutine(SprintCooldown(rollDuration + 0.33f));
        }

        // Starts stamina regen cooldown
        if (h)
        {
            StaminaRegenCooldown(1.5f);
        }

        if(Input.GetKey("k"))
        {
            TakeDamage(10f);
        }

        // Stamina
        staminaSlider.maxValue = maxStamina;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        if (stamina <= 0 && k)
        {
            StartCoroutine(SprintCooldown(3f - staminaRegenValue / 100));
        }

        if (!isSprinting && canRegenStamina)
        {
            ChangeStamina(staminaRegenValue);
        }

        else if (isSprinting)
        {
            ChangeStamina(staminaDecreaseValue);
        }

        // Hp 
        hpSlider.maxValue = maxHp;
        hp = Mathf.Clamp(hp, 0, maxHp);
    }

    private void FixedUpdate()
    {
        if (isRolling)
        {
            smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, 0.25f);
            rb.velocity = smoothedMovementInput * rollVelocity;
        }

        else
        {
            smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, 0.1f);
            rb.velocity = smoothedMovementInput * speed;
        }
    }

    private void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }

    private void ChangeStamina(float i)
    {
        stamina += i * Time.deltaTime;
        staminaSlider.value = stamina;
    }

    private IEnumerator SprintCooldown(float cooldownTime)
    {
        k = false;
        canSprint = false;
        yield return new WaitForSeconds(cooldownTime);
        canSprint = true;
        k = true;
    }

    private IEnumerator Roll(float cooldownTime)
    {
        tr.emitting = true;
        canRoll = false;
        isRolling = true;
        yield return new WaitForSeconds(rollDuration);
        tr.emitting = false;
        isRolling = false;
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    public void TakeDamage(float dmgTaken)
    {
        hp -= dmgTaken;
        hpSlider.value = hp;
    }

    private void StaminaRegenCooldown(float time)
    {
        canRegenStamina = false;

        if (f < time)
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

