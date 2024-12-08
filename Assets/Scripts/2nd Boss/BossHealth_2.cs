using UnityEngine;
using System.Collections;

public class BossHealth_2 : MonoBehaviour
{
    public float health = 100f;
    public bool isInvulnerable = true;
    public bool isDead = false;
    public Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private GameManager gameManager; // Reference to GameManager


    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isDead)
            return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        Debug.Log("Boss defeated!");

        if (gameManager != null)
        {
            gameManager.OnBossDefeated();
        }
    }
}
