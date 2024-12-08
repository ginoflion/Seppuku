using UnityEngine;

public class MeleeEnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    private bool facingRight = false;

    public void MoveTowards(Transform enemyTransform, Transform target, EnemyAttack enemyAttack)
    {
        if (target == null) return;

        float distanceToPlayer = Vector2.Distance(enemyTransform.position, target.position);

        if (distanceToPlayer > attackRange)
        {
            Vector2 direction = (target.position - enemyTransform.position).normalized;
            enemyTransform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

            // Flip the enemy to face the player
            FlipEnemyIfNeeded(enemyTransform, target.position.x);
        }
        else if (enemyAttack != null)
        {
            // Trigger attack if within range
            enemyAttack.TriggerAttack((target.position - enemyTransform.position).normalized);
        }
    }

    private void FlipEnemyIfNeeded(Transform enemyTransform, float targetXPosition)
    {
        bool shouldFaceRight = targetXPosition > enemyTransform.position.x;

        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            Vector3 localScale = enemyTransform.localScale;
            localScale.x *= -1; // Flip on the x-axis
            enemyTransform.localScale = localScale;
        }
    }
}
