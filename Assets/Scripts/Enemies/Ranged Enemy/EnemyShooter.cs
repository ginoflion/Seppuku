using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 1.5f;
    public float projectileSpeed = 5f;
    public float detectionRange = 5f;
    public LayerMask obstacleMask;

    private Transform player;
    private float lastShotTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Check if the game is paused
        if (MenuPausa.GamePaused)
            return;

        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            if (HasLineOfSight())
            {
                if (Time.time > lastShotTime + fireRate)
                {
                    ShootProjectile();
                    lastShotTime = Time.time;
                }
            }
        }
    }

    bool HasLineOfSight()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);

        return hit.collider == null;
    }

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Vector2 direction = (player.position - transform.position).normalized;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        rb.velocity = direction * projectileSpeed;

        Collider2D enemyCollider = GetComponent<Collider2D>();
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(projectileCollider, enemyCollider);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
