using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float horizontal;
    private float initialSpeed;
    private float speed = 5f;
    public float jumpingPower = 10f;
    private bool isFacingRight = true;
    public Rigidbody2D rb2d;
    public Transform groundCheck;
    public LayerMask groundlayer;
    public AnimationCurve movementCurve;
    public AnimationCurve decelerationCurve;
    public float decelerationTime;
    public float accelerationTime;
    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.075f;
    private float jumpBufferCounter;

    [SerializeField] private float maxHp;
    [SerializeField] private float hp;
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float rollVelocity;
    [SerializeField] private float rollCooldown;
    [SerializeField] private float rollDuration;
    [SerializeField] private float rollStaminaCost;
    [SerializeField] private float immuneTime;
    [SerializeField] private CameraMovement cameraMovement;
    public float damage;

    private bool isSprinting;
    private bool canRegenStamina;
    private bool k = true;
    private bool h = true;
    private float f;
    public float staminaRegenValue;
    public float staminaDecreaseValue;
    private bool canSprint;
    private bool canRoll;
    public bool isRolling;
    private bool canTakeDmg;
    public float normalAttackStaminaCost;
    private bool canNormalAttack;
    public bool canDealDmg;
    public float normalAttackSpeed;
    public float normalAttackDmg;

    // Look Direction Offset multiplier lol okay
    public float ldom;

    public Animator playerAnimator;
    public Slider staminaSlider;
    public Slider hpSlider;
    public float sprintMultiplier;
    private TrailRenderer tr;

    private void Awake()
    {
        tr = GetComponent<TrailRenderer>();
        //playerAnimator = GetComponent<Animator>();
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
        canTakeDmg = true;
        canNormalAttack = true;
        canDealDmg = false;
    }

    void Update()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpingPower);
            jumpBufferCounter = 0f;
        }

        if (Input.GetButtonUp("Jump") && rb2d.velocity.y > 0f)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }

        if (Input.GetButton("Horizontal"))
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            decelerationTime = 0;
            speed = movementCurve.Evaluate(accelerationTime);
            accelerationTime += Time.deltaTime;
            cameraMovement.offset.x = horizontal * ldom;
        }

        if (Input.GetButton("Horizontal") == false)
        {
            accelerationTime = 0;
            speed = decelerationCurve.Evaluate(decelerationTime);
            decelerationTime += Time.deltaTime;
        }

        Flip();

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0f && canSprint)
        {
            isSprinting = true;
            h = true;
            f = 0f;
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift) && stamina > 0f && canSprint)
        {
            StartCoroutine(SprintCooldown(0.33f));
        }

        else
        {
            isSprinting = false;
        }

        // Rolling
        if (Input.GetKeyDown(KeyCode.C) && canRoll && stamina >= rollStaminaCost)
        {
            stamina -= rollStaminaCost;
            staminaSlider.value = stamina;
            h = true;
            f = 0f;
            StartCoroutine(Roll(rollCooldown));
            StartCoroutine(SprintCooldown(rollDuration + 0.33f));
        }

        if (Input.GetMouseButtonDown(0) && canNormalAttack && stamina >= normalAttackStaminaCost)
        {
            damage = normalAttackDmg;
            stamina -= normalAttackStaminaCost;
            staminaSlider.value = stamina;
            h = true;
            f = 0f;
            StartCoroutine(RollCooldown(0.12f));
            StartCoroutine(NormalAttack(normalAttackSpeed));
            StartCoroutine(SprintCooldown(0.2f));
        }

        // Starts stamina regen cooldown
        if (h)
        {
            StaminaRegenCooldown(1.5f);
        }

        if (Input.GetKey("k"))
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
            rb2d.velocity = new Vector2((horizontal * speed) * rollVelocity, rb2d.velocity.y);
        }

        else if (isSprinting)
        {
            rb2d.velocity = new Vector2((horizontal * speed) * sprintMultiplier, rb2d.velocity.y);
        }

        else
        {
            rb2d.velocity = new Vector2(horizontal * speed, rb2d.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, LayerMask.GetMask("Ground"));
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
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
        StartCoroutine(ImmunityFrames(rollDuration + 0.20f));
        tr.emitting = true;
        canRoll = false;
        isRolling = true;
        yield return new WaitForSeconds(rollDuration);
        tr.emitting = false;
        isRolling = false;
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    private IEnumerator RollCooldown(float cooldownTime)
    {
        canRoll = false;
        yield return new WaitForSeconds(cooldownTime);
        canRoll = true;
    }

    private IEnumerator NormalAttack(float attackSpeed)
    {
        canNormalAttack = false;
        playerAnimator.SetTrigger("Normal Attack");
        yield return new WaitForSeconds(attackSpeed);
        canNormalAttack = true;
    }

    public void TakeDamage(float dmgTaken)
    {
        if (canTakeDmg)
        {
            hp -= dmgTaken;
            hpSlider.value = hp;
            StartCoroutine(ImmunityFrames(immuneTime));
        }
    }

    private IEnumerator ImmunityFrames(float timeImmune)
    {
        canTakeDmg = false;
        yield return new WaitForSeconds(timeImmune);
        canTakeDmg = true;
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

    public void CantDealDmg()
    {
        canDealDmg = false;
    }

    public void CanDealDmg()
    {
        canDealDmg = true;
    }
}
