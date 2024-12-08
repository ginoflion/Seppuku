using UnityEngine;

public class MeleeEnemyDetection : MonoBehaviour
{
    public Transform enemyTransform;
    public float detectionRadius = 5f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    private GameObject player;
    private Transform playerTransform;
    private Vector2 initialPosition;
    private bool playerDetected = false;

    // References to other components
    private Death death;
    private EnemyAttack enemyAttack;
    private MeleeEnemyMovement enemyMovement;
    private EnemyDetection enemyDetection;

    void Start()
    {
        initialPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            death = player.GetComponent<Death>();
        }

        enemyAttack = GetComponent<EnemyAttack>();
        enemyMovement = GetComponent<MeleeEnemyMovement>();
        enemyDetection = new EnemyDetection(detectionRadius, playerLayer, obstacleLayer); // Instantiate with parameters
    }

    void Update()
    {
        if (death != null && death.isDead)
        {
            playerDetected = false;
            playerTransform = null;

            ReturnToInitialPosition();
        }
        else
        {
            playerTransform = enemyDetection.DetectPlayer(enemyTransform);
            playerDetected = playerTransform != null;

            if (playerDetected)
            {
                enemyMovement.MoveTowards(enemyTransform, playerTransform, enemyAttack);
            }
        }
    }

    void ReturnToInitialPosition()
    {
        transform.position = initialPosition;
    }

    void OnDrawGizmosSelected()
    {
        if (enemyDetection != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }

    // Inner class for detection
    private class EnemyDetection
    {
        private float detectionRadius;
        private LayerMask playerLayer;
        private LayerMask obstacleLayer;

        public EnemyDetection(float radius, LayerMask playerLayer, LayerMask obstacleLayer)
        {
            this.detectionRadius = radius;
            this.playerLayer = playerLayer;
            this.obstacleLayer = obstacleLayer;
        }

        public Transform DetectPlayer(Transform enemyTransform)
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(enemyTransform.position, detectionRadius, playerLayer);

            if (playerCollider != null && playerCollider.CompareTag("Player"))
            {
                if (HasLineOfSight(playerCollider, enemyTransform))
                {
                    return playerCollider.transform; // Return player's transform if detected and in line of sight
                }
            }
            return null;
        }

        private bool HasLineOfSight(Collider2D playerCollider, Transform enemyTransform)
        {
            Vector2 directionToPlayer = (playerCollider.transform.position - enemyTransform.position).normalized;
            float distanceToPlayer = Vector2.Distance(enemyTransform.position, playerCollider.transform.position);

            RaycastHit2D hit = Physics2D.Raycast(enemyTransform.position, directionToPlayer, distanceToPlayer, obstacleLayer);
            Debug.DrawRay(enemyTransform.position, directionToPlayer * distanceToPlayer, Color.red, 0.1f);

            return hit.collider == null;
        }
    }
}
