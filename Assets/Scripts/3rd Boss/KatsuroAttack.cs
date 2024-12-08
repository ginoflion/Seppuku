using System.Collections;
using UnityEngine;

public class KatsuroAttack : MonoBehaviour
{
    [SerializeField] private Transform bossTransform;
    [SerializeField] private KatsuroHealth katsuroHealth; // Referência ao script de KatsuroHealth
    [SerializeField] private Transform playerTransform; // Referência ao Transform do jogador
    [SerializeField] private float delay = 0.4f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Katsuro_1st_Phase katsuro_1StPhase;
    [SerializeField] private Animator animp;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask parryLayer;

    private LayerMask targetLayer;
    private bool isAttacking = false;

    private void Start()
    {
        // Combina as layers do Player e do Parry
        targetLayer = playerLayer | parryLayer;
        StartCoroutine(AutoAttack());
    }

    private IEnumerator AutoAttack()
    {
        while (true)
        {
            // Para de atacar se o boss estiver morto
            if (katsuroHealth.isDead)
            {
                yield break;
            }

            // Verifica se o jogador está dentro do alcance de ataque
            float distanceToPlayer = Vector2.Distance(bossTransform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                Attack();
            }

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private void Attack()
    {
        isAttacking = true;

        Vector2 attackDirection = (playerTransform.position - bossTransform.position).normalized; // Direção para o jogador
        RaycastHit2D hit = Physics2D.Raycast(bossTransform.position, attackDirection, attackRange, targetLayer);

        if (hit)
        {
            Debug.Log("Katsuro acertou: " + hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerHealth>().TakeDamage(2);
            }
            else if (hit.collider.CompareTag("Parry"))
            {
                animp.SetTrigger("Parry");
                katsuro_1StPhase.RegisterParry();
                animp.SetBool("IsParrying", false);
                Debug.Log("Ataque de Katsuro parried!");

                // Cancela o ataque atual
                CancelAttack();
            }
        }
        else
        {
            Debug.Log("Nada atingido por Katsuro");
        }

        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }

    public void CancelAttack()
    {
        // Cancela o ataque atual
        isAttacking = false;
    }
}
