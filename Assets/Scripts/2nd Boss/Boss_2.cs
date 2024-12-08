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

        // 3. Alinhamento com o jogador (ângulo atual)
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
