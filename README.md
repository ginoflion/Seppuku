# Seppuku


---

**Grupo:**

Gonçalo Silva- a25970;

João Sousa - a25613

---

## IA

1. State Machine

2. Decision Scoring

---

## Análise das IAs

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
### Decision Scoring

O Decision Scoring é uma técnica que utiliza modelos matemáticos e algoritmos de inteligência artificial para avaliar e atribuir pontuações a diferentes opções ou cenários de decisão. 
Ou seja, é um sistema que analisa dados de forma estruturada, com o objetivo de ajudar a tomar decisões de maneira mais informada e eficiente.
Cada opção é analisada com base em critérios pré-definidos, e o sistema atribui uma pontuação que reflete a relevância, risco ou probabilidade de sucesso daquela escolha. 
Essa pontuação serve como uma medida que facilita a comparação e priorização das alternativas disponíveis, orientando tanto decisões humanas quanto processos automatizados.

### Implementação do Decision Scoring

O Decision Scoring é implementando na mecânica do 2º boss para decidir para onde o boss se deve teletransportar.

Neste Decision Scoring ele usa 3 critérios:
1. Distãncia do Boss ao Jogador
2. Linha de Visão
3. Previsão da localização futura do Jogador

Baseado nestes critérios cada ponto de teletransporte recebe uma pontuação , sendo escolhido o com maior pontuação.

  ```cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss_2 : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 0.5f;
    public float minProjectileSpeed = 5f;
    public float maxProjectileSpeed = 10f;
    public float minTeleportWait = 5.0f;
    public float maxTeleportWait = 10.0f;
    public Transform[] teleportPoints;
    public Target[] targets;
    [SerializeField] private Animator anim;
    private Transform player;
    private bool isGrounded = false;
    private bool isFalling = false;
    private BossHealth_2 bossHealth;
    private Rigidbody2D rb;

    // Variáveis para previsão de movimento
    public float predictionTime = 0.5f; // Tempo para prever onde o jogador estará

    void Start()
    {
        // Inicialmente desativa o comportamento até o boss ser ativado
        enabled = false;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossHealth = GetComponent<BossHealth_2>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void ActivateBoss()
    {
        enabled = true;
        StartCoroutine(TeleportAndShootRoutine());
        Debug.Log("Boss activated!");
    }

    IEnumerator TeleportAndShootRoutine()
    {
        while (!isGrounded)
        {
            // Escolher o melhor ponto de teletransporte
            Transform bestTeleportPoint = ChooseBestTeleportPoint();

            if (bestTeleportPoint != null)
            {
                TeleportToPoint(bestTeleportPoint);
            }
            else
            {
                FallToGround();
                yield break;
            }

            // Esperar e atacar
            float waitDuration = Random.Range(minTeleportWait, maxTeleportWait);

            float startTime = Time.time;
            while (Time.time < startTime + waitDuration)
            {
                ShootProjectile();
                yield return new WaitForSeconds(fireRate);
            }
        }
    }

    Transform ChooseBestTeleportPoint()
    {
        Transform bestPoint = null;
        float bestScore = float.MinValue;

        foreach (Transform point in teleportPoints)
        {
            if (!point.gameObject.activeSelf) continue;

            float score = EvaluateTeleportPoint(point);
            if (score > bestScore)
            {
                bestScore = score;
                bestPoint = point;
            }
        }

        return bestPoint;
    }

    float EvaluateTeleportPoint(Transform point)
    {
        if (player == null) return float.MinValue;

        // Direção e distância ao jogador
        Vector2 directionToPlayer = (player.position - point.position).normalized;
        float distanceToPlayer = Vector2.Distance(player.position, point.position);

        // Prever a posição futura do jogador
        Vector2 playerVelocity = player.GetComponent<Rigidbody2D>()?.velocity ?? Vector2.zero;
        Vector2 predictedPosition = (Vector2)player.position + (playerVelocity * predictionTime);
        Vector2 futureDirectionToPlayer = (predictedPosition - (Vector2)point.position).normalized;

        // 1. Avaliação da distância
        float distanceScore = Mathf.Clamp(1.0f / distanceToPlayer, 0.1f, 1.0f);

        // 2. Avaliação da linha de visão
        RaycastHit2D hit = Physics2D.Raycast(point.position, directionToPlayer, distanceToPlayer);
        float lineOfSightScore = (hit.collider == null || hit.collider.CompareTag("Player")) ? 1.0f : 0.0f;

        // 3. Alinhamento com o jogador 
        Vector2 bossForward = Vector2.right; // Direção "frontal" do boss, pode ser ajustada conforme necessário
        float alignmentScore = Vector2.Dot(futureDirectionToPlayer, bossForward);

        // Pesos ajustáveis para cada critério
        float totalScore = (distanceScore * 0.4f) + (lineOfSightScore * 0.4f) + (alignmentScore * 0.2f);

        return totalScore;
    }

    void TeleportToPoint(Transform point)
    {
        transform.position = point.position;
        ActivateTarget(System.Array.IndexOf(teleportPoints, point));
        Debug.Log("Teleported to point: " + point.name);
    }

    void ActivateTarget(int activeIndex)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                targets[i].gameObject.SetActive(i == activeIndex);
            }
        }
    }

    void FallToGround()
    {
        isGrounded = false;
        isFalling = true;
        anim.SetBool("isFalling", true);
        if (bossHealth != null)
        {
            bossHealth.isInvulnerable = false;
        }

        rb.isKinematic = false;
        rb.gravityScale = 1.5f;
        Debug.Log("Boss is falling to the ground and is now vulnerable!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isFalling)
        {
            isGrounded = true;
            isFalling = false;
            anim.SetBool("isFalling", false);
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            rb.gravityScale = 0;

            Debug.Log("Boss has landed and is vulnerable!");
        }
    }

    void ShootProjectile()
    {
        if (player == null || isGrounded) return;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;

        float randomProjectileSpeed = Random.Range(minProjectileSpeed, maxProjectileSpeed);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * randomProjectileSpeed;

        Collider2D enemyCollider = GetComponent<Collider2D>();
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(projectileCollider, enemyCollider);
    }
}

```
Um peso é atribuido a cada critério , podendo ser ajustado para obter um comportamento diferente do Boss

Este IA é usado sempre que o Boss dá teletransporte para um novo ponto.

***

