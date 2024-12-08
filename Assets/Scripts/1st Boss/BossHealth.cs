using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    public float health = 30f;
    public bool isInvulnerable = true;
    public int parryCount = 0;
    public int maxParryCount = 5;
    public bool isDead = false;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float animationTime;
    [SerializeField] private GameManager gameManager; 

    private Coroutine vulnerabilityCoroutine;

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        if (isInvulnerable)
        {
            animator.SetTrigger("Parry");
            Debug.Log("Boss parried the attack due to invulnerability!");
            return;
        }

        health -= damage;

        animator.SetTrigger("Hit");

        if (health <= 0)
        {
            Die();
        }
    }


    public void RegisterParry()
    {
        if (isInvulnerable && !isDead)
        {
            parryCount++;
            Debug.Log("Parry registered! Current parry count: " + parryCount);

            if (parryCount >= maxParryCount)
            {
                MakeVulnerable();
            }
        }
    }

    private void MakeVulnerable()
    {
        isInvulnerable = false;
        parryCount = 0;
        animator.SetBool("IsInvulnerable", false);
        Debug.Log("Boss is now vulnerable!");

        // Make the boss idle
        Boss boss = GetComponent<Boss>();
        if (boss != null)
        {
            boss.ChangeState(Boss.BossState.Idle);
        }

        // Start the vulnerability timer
        if (vulnerabilityCoroutine != null)
        {
            StopCoroutine(vulnerabilityCoroutine);
        }
        vulnerabilityCoroutine = StartCoroutine(VulnerabilityTimer(5f));
    }


    private IEnumerator VulnerabilityTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ResetInvulnerability();
    }

    private void ResetInvulnerability()
    {
        isInvulnerable = true;
        animator.SetBool("IsInvulnerable", true);
        Debug.Log("Boss is now invulnerable again!");

        // Change back to active state
        Boss boss = GetComponent<Boss>();
        if (boss != null && !isDead)
        {
            boss.ChangeState(Boss.BossState.Run); // Or another appropriate state
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("isDead", true);
        rb.isKinematic = true; // Disable physics interactions
        rb.simulated = false;
        rb.velocity = Vector2.zero;

        // Notify the Boss script to change state to Dead
        Boss boss = GetComponent<Boss>();
        boss.ChangeState(Boss.BossState.Dead);
        

        

        Debug.Log("Boss defeated!");
        gameManager.OnBossDefeated();
        
    }
    public void ResetHealth()
    {
        health = 30f; // Reset to full health or the desired initial value
        isInvulnerable = true; // Reset invulnerability if needed
        parryCount = 0; // Reset the parry count
        isDead = false; // Ensure the boss is alive
        animator.SetBool("IsInvulnerable", true);

        Debug.Log("Boss health reset to full.");
    }



}
