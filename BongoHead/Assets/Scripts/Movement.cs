using System.Collections;
using System.Collections.Generic;
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
    private float stamina;
    private float maxStamina;
    private bool isSprinting;
    private bool canRegenStamina;

    public new Rigidbody2D rigidbody;
    public Animator animator;
    public Slider staminaSlider;
    public float speed = 7.0f;
    public float sprintMultiplier;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody2D>();
        initialSpeed = speed;
        maxStamina = 100;
        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;
    }

    private void Update()
    {
        if (playerInput.actions["Sprint"].IsPressed() && stamina > 0)
        {
            speed = initialSpeed * sprintMultiplier;
            isSprinting = true;
        }

        else
        {
            speed = initialSpeed;
            isSprinting = false;
            StartCoroutine(SprintCooldown(.25f));
        }
        if (stamina <= 0) //jostain syystä kun stamina menee nollaan niin se silti regeneroi mutta jos lopetat sprinttaamisen niin siinä kestää hetki ennen kuin se regenaa uudestaan :)
        {
            StartCoroutine(SprintCooldown(2f));
        }
    }

    private void FixedUpdate()
    {
        if (isSprinting)
        {
            ChangeStamina(-1);
        }
        else if (!isSprinting && stamina < 100 && canRegenStamina)
        {
            ChangeStamina(1);
        }
        smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, 0.1f);
        rigidbody.velocity = smoothedMovementInput * speed;
        
    }

    private void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }

    private void ChangeStamina(float decreaseAmount)
    {
        stamina += decreaseAmount;
        if(stamina < 0)
        {
            stamina = 0;
        }
        staminaSlider.value = stamina;
    }

    private IEnumerator SprintCooldown(float cooldownTime)
    {
        canRegenStamina = false;
        yield return new WaitForSeconds(cooldownTime);
        canRegenStamina = true;
    }
}

