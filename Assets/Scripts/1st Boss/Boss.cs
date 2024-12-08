using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Run,
        Attack,
        Parry,
        Dead
    }

    [Header("Boss Settings")]
    public BossState currentState = BossState.Idle;
    public float speed = 5f;
    public float attackRange = 3f;
    public bool isFlipped = false;
    public bool isActive = false;

    [Header("References")]
    public Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Store the initial position of the boss as a Vector2
        startPosition = transform.position;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        // Return if the boss is not active
        if (!isActive)
        {
            ChangeState(BossState.Idle); // Ensure Idle when inactive
            return;
        }

        BossHealth bossHealth = GetComponent<BossHealth>();

        // Ensure Idle when the boss is vulnerable
        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            ChangeState(BossState.Idle);
            return;
        }

        // Handle state behavior
        switch (currentState)
        {
            case BossState.Idle:
                IdleState();
                break;
            case BossState.Run:
                RunState();
                break;
            case BossState.Attack:
                AttackState();
                break;
            case BossState.Parry:
                ParryState();
                break;
            case BossState.Dead:
                DeadState();
                break;
        }
    }

    private void IdleState()
    {
        // Ensure the boss stays still
        rb.velocity = Vector2.zero;

        // Play Idle animation
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Idle") == false)
        {
            animator.Play("Boss_Idle");
        }

        Debug.Log("Boss is in Idle state.");
    }



    public void ChangeState(BossState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        // Update Animator parameter
        animator.SetBool("isIdle", newState == BossState.Idle);

        Debug.Log($"Boss state changed to: {currentState}");
        OnStateEnter(newState);
    }


    private void OnStateEnter(BossState state)
    {
        switch (state)
        {
            case BossState.Idle:
                animator.Play("Boss_Idle");
                rb.velocity = Vector2.zero;
                break;

            case BossState.Run:
                animator.Play("Boss_Run");
                break;

            case BossState.Attack:
                break;

            case BossState.Parry:
                animator.Play("Rei_Parry");
                break;

            case BossState.Dead:
                animator.Play("Boss_Death");
                break;
        }
    }


    private void DeadState()
    {
        Debug.Log("Boss is dead!");
        isActive = false;
        rb.velocity = Vector2.zero;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void LookAtPlayer()
    {
        if (player == null || currentState == BossState.Dead) return;

        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }


   

    private void RunState()
    {
        if (player == null || currentState == BossState.Dead) return;

        float direction = player.position.x - transform.position.x;
        direction = Mathf.Sign(direction);
        BossHealth bossHealth = GetComponent<BossHealth>();
        // If the boss is vulnerable, it stops but doesn't switch states immediately
        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            Debug.Log("Boss is vulnerable and slowing down.");
            rb.velocity = Vector2.zero;
            return; // Pause movement but don't switch to Idle
        }

        // Chase the player
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        // If in attack range, stop and switch to Attack
        if (Vector2.Distance(player.position, transform.position) <= attackRange)
        {
            rb.velocity = Vector2.zero;
            ChangeState(BossState.Attack);
        }

        LookAtPlayer();
    }


    private void AttackState()
    {
        if (player == null || currentState == BossState.Dead) return;

        BossHealth bossHealth = GetComponent<BossHealth>();

        // Stop attacking if the boss is vulnerable
        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            Debug.Log("Boss is vulnerable and pausing attack.");
            return;
        }

        // Check the distance to the player
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        // If the player is out of attack range, transition to Run state
        if (distanceToPlayer > attackRange)
        {
            Debug.Log("Player is too far. Switching to Run state.");
            animator.SetBool("isFarAway", true); // Set the condition for the animator
            ChangeState(BossState.Run);
            return;
        }

        // Keep looking at the player during the attack
        LookAtPlayer();
    }



    private void ParryState()
    {
        Debug.Log("Boss is parrying");
    }

    

    public void ActivateBoss()
    {
        if (currentState == BossState.Dead)
        {
            Debug.LogWarning("Cannot activate the boss: It is already dead.");
            return;
        }

        // Check if the boss is currently vulnerable
        BossHealth bossHealth = GetComponent<BossHealth>();
        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            Debug.Log("Boss is vulnerable and remains idle.");
            ChangeState(BossState.Idle);
            return;
        }

        isActive = true;
        ChangeState(BossState.Run);
    }
    public void ResetBoss()
    {
        // Reset health
        BossHealth bossHealth = GetComponent<BossHealth>();
        if (bossHealth != null)
        {
            bossHealth.ResetHealth(); // Ensure this method sets health to max
        }

        // Teleport the boss to its starting position
        transform.position = startPosition;

        // Reset state to Idle
        ChangeState(BossState.Idle);

        // Reactivate the boss
        isActive = true;
        rb.velocity = Vector2.zero; // Optionally reset velocity
        Debug.Log("Boss has been reset to initial state and teleported to starting position.");
    }


}