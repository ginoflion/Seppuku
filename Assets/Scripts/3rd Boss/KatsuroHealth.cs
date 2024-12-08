using UnityEngine;

public class KatsuroHealth : MonoBehaviour
{
    public float health = 100f;
    public bool isDead = false;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameManager gameManager; // Referência para o GameManager
    [SerializeField] private Collider2D damageCollider; // Collider que receberá os acertos


    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        // Aplica o dano diretamente
        health -= damage;
        Debug.Log("Katsuro recebeu dano do jogador!");
        Debug.Log("Vida do Boss"+ health);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        rb.isKinematic = true;
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        Debug.Log("Boss defeated!");

        // Notifica o GameManager que o boss foi derrotado
        if (gameManager != null)
        {
            gameManager.OnBossDefeated();
        }
    }

}
