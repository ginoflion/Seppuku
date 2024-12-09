# Seppuku


---

**Grupo:**

Gonçalo Silva- a25970;

João Sousa - a25613

---

## AI

1. State Machine

2. 

---

## Análise das AIs

### State Machine:

Uma State Machine é um modelo matemático utilizado para representar os diversos comportamentos e as respetivas transições entre estes em um programa.
Ou seja, é composta por estados e transições. Nela cada estado representa o sistema em um determinado momento no tempo e
não é possível uma máquina de estados estar em dois estados ao mesmo tempo.

### Implementação da State Machine

A State Machine é implementada no primeiro Boss do jogo. Ela é utilizada para gerir o comportamento do primeiro boss do jogo. 
A State Machine presente no nosso jogo é composta pelos seguintes estados:

1. Idle
2. Run
3. Attack
4. Parry
5. Dead


```cs
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
```
Esta State Machine funciona em conjunto com os outros dois scripts que dão manage no Boss, o Boss_Health e o Boss_Attack.
O Boss_Health é responsável pela vida do Boss e a sua vulnerabilidade.
Neste caso quando o Boss fica vulneravel, o seu estado é mudado para o Idle, uma vez que ele fica stun locked. 
Quando esse período acaba, o estado leva update para o Run, para o Boss continuar a trocar de estados sem o envolvimento deste script.
Quando a vida do Boss é igual ou inferior a 0, o seu estado leva update para o Die e desativamos as suas físicas e mecânicas
```cs 
    private void MakeVulnerable()
    {
       .....
        Boss boss = GetComponent<Boss>();
        if (boss != null)
        {
            boss.ChangeState(Boss.BossState.Idle);
        }

    }



    private void ResetInvulnerability()
    {
        ....
        Boss boss = GetComponent<Boss>();
        if (boss != null && !isDead)
        {
            boss.ChangeState(Boss.BossState.Run); 
        }
    }

    private void Die()
    {
        ...
        Boss boss = GetComponent<Boss>();
        boss.ChangeState(Boss.BossState.Dead);

        ....
    }
```
O outro script que afeta a State Machine é o Boss Attack. Este script é responsável por gerir o ataque do boss e o cooldown entre eles. 
Ao contrário do Boss_Health este script não altera o State do Boss, mas age quando está no atack state, devido ao range do ataque do boss e ao método de ataque automatico, ou seja, estando dentro do range, o boss ataca automaticamente
```cs
private IEnumerator AutoAttack()
    {
        while (true)
        {
            if (bossHealth.isDead) yield break;

            // Skip attacking if the boss is vulnerable or too far from the player
            if (!bossHealth.isInvulnerable || Vector2.Distance(bossTransform.position, playerTransform.position) > attackRange)
            {
                yield return null;
                continue;
            }

            // If not already attacking, initiate an attack
            if (!isAttacking)
            {
                Attack();
            }

            yield return null; // Check conditions every frame
        }
    }
```
***

