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
4. Dead

O Boss é ativado usando o GameManager, e enquanto que não se encontra ativo, o seu estado é o Idle. 
Quando o player finaliza a leitura do diálogo entre os dois, a boss fight começa, com a ativação do Boss. 
Assim sendo, o seu estado é automaticamente alterado para o Run, visto que se encontra fora do attack range do boss. O RunState é responsável por 
Quando o mesmo se aproxima do player, como a distância é menos que o seu attack range, ele atualiza o seu estado para o attack state que funciona em sincronia com o script Boss Attack. 
Os estados vão alterando entre si, até o player concluir a boss fight. 

```cs
public class Boss : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Run,
        Attack,
        Dead
    }

    private void Update()
    {
        if (!isActive)
        {
            ChangeState(BossState.Idle);
            return;
        }

        BossHealth bossHealth = GetComponent<BossHealth>();

        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            ChangeState(BossState.Idle);
            return;
        }

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
            case BossState.Dead:
                DeadState();
                break;
        }
    }

    private void IdleState()
    {
        rb.velocity = Vector2.zero;
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

    private void RunState()
    {
        if (player == null || currentState == BossState.Dead) return;

        float direction = player.position.x - transform.position.x;
        direction = Mathf.Sign(direction);
        BossHealth bossHealth = GetComponent<BossHealth>();
        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            Debug.Log("Boss is vulnerable and slowing down.");
            rb.velocity = Vector2.zero;
            return; 
        }

        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

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

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceToPlayer > attackRange)
        {
            Debug.Log("Player is too far. Switching to Run state.");
            animator.SetBool("isFarAway", true); 
            ChangeState(BossState.Run);
            return;
        }

        LookAtPlayer();
    }
    

    public void ActivateBoss()
    {
        if (currentState == BossState.Dead)
        {
            Debug.LogWarning("Cannot activate the boss: It is already dead.");
            return;
        }

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
        ....
        ChangeState(BossState.Idle);
        .... 
    }
}
```

---
### Boss Health
---

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

---
### Boss Attack
---

O outro script que afeta a State Machine é o Boss Attack. Este script é responsável por gerir o ataque do boss e o cooldown entre eles. 
Ao contrário do Boss_Health este script não altera o State do Boss, mas age quando está no atack state, devido ao range do ataque do boss e ao
método de ataque automatico, ou seja, estando dentro do range, o boss ataca automaticamente.

```cs
private IEnumerator AutoAttack()
    {
        while (true)
        {
            if (bossHealth.isDead) yield break;

            if (!bossHealth.isInvulnerable || Vector2.Distance(bossTransform.position, playerTransform.position) > attackRange)
            {
                yield return null;
                continue;
            }

            if (!isAttacking)
            {
                Attack();
            }

            yield return null; 
        }
    }
```

---

***

