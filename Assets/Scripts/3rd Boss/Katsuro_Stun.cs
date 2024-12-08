using System.Collections;
using UnityEngine;

public class KatsuroStun : MonoBehaviour
{
    [SerializeField] private BoxCollider2D stunCollider; // Collider para ser ativado durante o stun
    [SerializeField] private float stunDuration = 3f; // Dura��o do stun
    [SerializeField] private Animator animator; // Animator para controle de anima��es (se necess�rio)

    private KatsuroAttack katsuroAttack; // Refer�ncia para o script de KatsuroAttack
    private bool isStunned = false;

    private void Start()
    {
        katsuroAttack = GetComponent<KatsuroAttack>(); // Pegando a refer�ncia do KatsuroAttack
        stunCollider.enabled = false; // Certificando-se de que o collider come�a desativado
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile") && !isStunned)
        {
            StartCoroutine(ApplyStun());
        }
    }

    private IEnumerator ApplyStun()
    {
        // Inicia o stun
        isStunned = true;

        // Ativa o collider extra (BoxCollider2D) durante o stun
        stunCollider.enabled = true;

        // Desativa o ataque temporariamente
        if (katsuroAttack != null)
        {
            // Cancela o ataque se o boss for atingido por um proj�til
            katsuroAttack.CancelAttack();
        }

        // Coloca o boss em estado de stun (se necess�rio)
        if (animator != null)
        {
            animator.SetBool("IsStunned", true);
        }

        // Espera pelo tempo do stun
        yield return new WaitForSeconds(stunDuration);

        // Desativa o collider extra e o stun
        stunCollider.enabled = false;

        // Finaliza o estado de stun
        if (animator != null)
        {
            animator.SetBool("IsStunned", false);
        }

        isStunned = false;
    }

    // M�todo getter para acessar o estado de stun
    public bool IsStunned()
    {
        return isStunned;
    }
}
