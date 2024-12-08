using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bossTransform;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animp;
    [SerializeField] private Animator animator;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 10f;  // Distance at which the attack will reach
    [SerializeField] private float attackCooldown = 1f;  // Cooldown after each attack
    [SerializeField] private float attackDelay = 0.3f;  // Delay before raycast after attack animation trigger

    [Header("Layer Masks")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask parryLayer;

    private bool isAttacking = false;  // Tracks if an attack is in progress
    private LayerMask targetLayer;

    private void Start()
    {
        // Combine player and parry layers into one LayerMask
        targetLayer = playerLayer | parryLayer;

        // Start the auto-attack coroutine
        StartCoroutine(AutoAttack());
    }

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

    private void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;

        // Trigger attack animation
        animator.SetBool("isFarAway", false);
        animator.SetTrigger("Attack");

        // Delay the raycast to sync with the animation
        StartCoroutine(PerformAttackWithDelay());
    }

    private IEnumerator PerformAttackWithDelay()
    {
        yield return new WaitForSeconds(attackDelay);

        // Perform the raycast logic
        PerformRaycast();

        // Wait for cooldown before allowing another attack
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    private void PerformRaycast()
    {
        // Calculate direction from boss to player
        Vector2 attackDirection = (playerTransform.position - bossTransform.position).normalized;

        // Fire a raycast in the attack direction
        RaycastHit2D hit = Physics2D.Raycast(bossTransform.position, attackDirection, attackRange, targetLayer);

        Debug.DrawLine(bossTransform.position, bossTransform.position + (Vector3)(attackDirection * attackRange), hit ? Color.green : Color.red, 1f);

        if (hit)
        {
            HandleRaycastHit(hit.collider);
        }
    }
    private void OnDrawGizmos()
    {
        // Ensure we only draw the Gizmo when the boss is attacking
        if (isAttacking)
        {
            // Set Gizmo color for the attack range
            Gizmos.color = Color.red;

            // Draw a line representing the attack direction
            if (playerTransform != null && bossTransform != null)
            {
                Vector2 attackDirection = (playerTransform.position - bossTransform.position).normalized;
                Gizmos.DrawLine(bossTransform.position, bossTransform.position + (Vector3)(attackDirection * attackRange));
            }

            // Draw a sphere to represent the attack range
            if (bossTransform != null)
            {
                Gizmos.color = new Color(1, 0, 0, 0.2f); // Semi-transparent red
                Gizmos.DrawWireSphere(bossTransform.position, attackRange); // Outline of the range
                Gizmos.DrawSphere(bossTransform.position, 0.2f); // A small marker for the boss position
            }
        }
    }


    private void HandleRaycastHit(Collider2D hitCollider)
    {
        if (hitCollider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(2); // Deal damage to the player
            }
        }
        else if (hitCollider.CompareTag("Parry"))
        {
            animp.SetTrigger("Parry");
            bossHealth.RegisterParry();
            Debug.Log("Boss's attack parried!");
        }
    }
}
