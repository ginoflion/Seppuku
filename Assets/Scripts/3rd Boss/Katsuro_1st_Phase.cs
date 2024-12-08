using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katsuro_1st_Phase : MonoBehaviour
{
    public float health = 100f;
    public bool isInvulnerable = true;
    public int parryCount = 0;
    public int maxParryCount = 5;

    [SerializeField] private Animator animator;
    [SerializeField] private float animationTime;
    [SerializeField] private GameObject armor;
    [SerializeField] private Boss_Enemy_Spawner boss_Enemy_Spawner;
    private Coroutine vulnerabilityCoroutine;

    public void TakeDamage(int damage)
    {

        // Verifica se o boss está invulnerável
        if (isInvulnerable)
        {
            // Aciona a animação de parry ao ser atacado enquanto invulnerável
            animator.SetTrigger("Parry");
            Debug.Log("Boss parried the attack due to invulnerability!");
            return; // Retorna sem aplicar dano ou contar parry
        }

        // Caso o boss esteja vulnerável, aplica o dano normalmente
        health -= damage;

        if (health <= 0)
        {
            EnableSecondPhase();
        }
    }

    public void RegisterParry()
    {
        if (isInvulnerable)
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

        if (vulnerabilityCoroutine != null)
        {
            StopCoroutine(vulnerabilityCoroutine);
        }
        vulnerabilityCoroutine = StartCoroutine(VulnerabilityTimer(10f));
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
    }

    private void EnableSecondPhase()
    {
        Debug.Log("Boss 1st Phase Done!");
        animator.Play("Katsuro second phase");
        StartCoroutine(WaitForAnimation(animator, "Katsuro second phase", () =>
        {
            boss_Enemy_Spawner.enabled = true;
            armor.SetActive(false);
        }));
    }

    private IEnumerator WaitForAnimation(Animator animator, string animationName, System.Action onComplete)
    {
        // Aguarda até que a animação termine
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null; // Espera até o próximo frame
        }

        // Executa a ação passada
        onComplete?.Invoke();
    }
}
