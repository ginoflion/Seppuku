using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement variables
    private float horizontal;
    [SerializeField] private float jumpingPower = 8f;
    [SerializeField] private float speed = 8f;
    private bool isFacingRight = true;
    private Vector2 horizontalvelocity;
    public SpriteRenderer spriteRenderer;
    public bool isParrying = false;


    // References
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private Camera mainCamera;
    [SerializeField] private Animator anim;
    [SerializeField] private Death death;
    [SerializeField] private PlayerAttack playerAttack;
    public bool isAttacking = false;

    // Audio
    [SerializeField] private AudioSource footstepAudioSource;
    private bool isFootstepsPlaying = false;
    public bool isDialogueActive = false;

    // Wall Slide variables
    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask oneWayPlatformLayer;

    // Wall Jump variables
    private bool isWallJumping;
    private float wallJumpDirection;
    [SerializeField] private float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    [SerializeField] private float wallJumpDuration = 0.4f;
    private Vector2 wallJumpPower = new Vector2(8f, 16f);

    // Dash variables
    [SerializeField] private float dashingVelocity = 10f;
    [SerializeField] private float dashingTime = 0.5f;
    private Vector2 dashingDirection;
    private bool isDashing = false;
    private bool canDash = true;
    private bool dashInput;
    private Quaternion originalRotation;

    // Other variables
    private float originalDrag;

    private enum MovementState { Idle, Correr, Saltar, Cair, Atacar, Dash, Parry, AtacarNoAr }

    // Start method
    private void Start()
    {
        originalDrag = rb.drag;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        death = GetComponent<Death>();
        playerAttack = GetComponentInChildren<PlayerAttack>(); // Assumindo que o script PlayerAttack está num filho
    }



    // Update method for inputs
    private void Update()
    {
        HandleMovementInput();
        HandleDashInput();
        HandleJumpInput();
        WallSlide();
        WallJump();
        if (!isWallJumping) FlipTowardsMouse();
        horizontalvelocity.x = rb.velocity.x;
        UpdateAnimationState();
        HandleFootsteps();
    }

    // FixedUpdate for physics calculations
    private void FixedUpdate()
    {
        ApplyMovement();
    }
    public void SetDialogueState(bool inDialogue)
    {
        isDialogueActive = inDialogue;

        // Stop footsteps immediately if dialogue starts
        if (inDialogue && isFootstepsPlaying)
        {
            footstepAudioSource.Stop();
            isFootstepsPlaying = false;
        }
    }

    private void HandleFootsteps()
    {
        // Only play footsteps if the player is on the ground, moving horizontally, not dashing, and not in dialogue
        if (IsGrounded() && horizontal != 0f && !isDashing && !isDialogueActive && !isParrying)
        {
            if (!isFootstepsPlaying)
            {
                footstepAudioSource.Play();
                isFootstepsPlaying = true;
            }
        }
        else
        {
            if (isFootstepsPlaying)
            {
                footstepAudioSource.Stop();
                isFootstepsPlaying = false;
            }
        }
    }
    private void UpdateAnimationState()
    {
        if (death != null && death.isDead)
        {
            anim.SetInteger("state", 0);
            return;
        }

        MovementState state;

        // Prioriza a animação de ataque se isAttacking for true
        if (isAttacking)
        {
            state = MovementState.Atacar;
        }
        else if (IsGrounded())
        {
            // Se o jogador está no chão, decide entre Idle e Correr
            if (horizontal != 0f)
            {
                state = MovementState.Correr;
            }
            else
            {
                state = MovementState.Idle;
            }
        }
        else
        {
            // Se está no ar, define entre Saltar e Cair, apenas se não estiver atacando
            if (rb.velocity.y > 0.1f)
            {
                state = MovementState.Saltar;
            }
            else
            {
                state = MovementState.Cair;
            }
        }

        anim.SetInteger("state", (int)state);
    }






    #region Movement
    private void HandleMovementInput()
    {
        if (!isDashing && !isParrying)
        {
            horizontal = Input.GetAxis("Horizontal");
        }
    }


    private void ApplyMovement()
    {
        if (!isDashing && !isWallJumping && !isParrying)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        else if (isParrying)
        {
            rb.velocity = new Vector2(0, 0); 
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer | oneWayPlatformLayer) != null;
    }

    private void FlipTowardsMouse()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (mousePosition.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    #endregion

    #region Dash
    private void HandleDashInput()
    {
        dashInput = Input.GetButtonDown("Dash");

        if (dashInput && canDash && !isDashing)
        {
            StartDash();
        }

        if (IsGrounded() && !isDashing)
        {
            canDash = true;
        }
    }


    private void StartDash()
    {
        if (anim != null)
        {
            anim.SetTrigger("isDashing");
        }
        isDashing = true;
        canDash = false;

        originalRotation = transform.rotation;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        dashingDirection = (mousePosition - (Vector2)transform.position).normalized;

        if (IsGrounded() && Mathf.Abs(dashingDirection.y) < 0.1f)
        {
            dashingDirection.x *= 1.5f;
        }

        float dashAngle = Mathf.Atan2(dashingDirection.y, dashingDirection.x) * Mathf.Rad2Deg; 
        transform.rotation = Quaternion.Euler(0, 0, dashAngle); 

        if ((dashAngle > 90 && dashAngle <= 180) || (dashAngle < -90 && dashAngle >= -180))
        {
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }

        rb.drag = 0f;
        StartCoroutine(PerformDash());
    }


    private IEnumerator PerformDash()
    {
        float dashTimeElapsed = 0f;

        while (dashTimeElapsed < dashingTime)
        {
            rb.velocity = dashingDirection * dashingVelocity;
            dashTimeElapsed += Time.deltaTime;

            // Verifica se o jogador ainda está no chão durante o dash
            if (!IsGrounded())
            {
                UpdateAnimationState();  // Atualiza o estado para "Cair" caso não esteja grounded
            }

            yield return null;
        }

        rb.drag = originalDrag;

        // Restaura a rotação original quando o dash termina
        transform.rotation = originalRotation;

        // Desativa o flip no X e Y do SpriteRenderer após o dash
        spriteRenderer.flipX = false;
        spriteRenderer.flipY = false;

        if (anim != null)
        {
            anim.SetTrigger("isIdle");
        }

        isDashing = false;

        // Após o dash, garante que o estado de animação é atualizado
        UpdateAnimationState();
    }


    #endregion

    #region Jump & Wall Mechanics
    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            UpdateAnimationState();
        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpCounter = wallJumpTime;

            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpCounter = 0f;

            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJump), wallJumpDuration);
        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }
    #endregion
    private void OnAnimatorMove()
    {
        // Verifique se está caindo no final da animação de salto
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Saltar") && rb.velocity.y < 0f)
        {
            anim.SetInteger("state", (int)MovementState.Cair);
        }
    }

}
