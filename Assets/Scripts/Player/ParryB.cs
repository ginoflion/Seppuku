using System.Collections;
using UnityEngine;

public class ParryB : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider; // Referência ao BoxCollider2D
    [SerializeField] private PlayerAttack playerAttack; // Referência ao script PlayerAttack
    [SerializeField] private float parryDuration = 1.0f;
    [SerializeField] private float parryCooldown = 1f;// Tempo em que o parry fica ativado
    [SerializeField] private Animator anim;
    private PlayerMovement playerMovement;


    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        anim.SetBool("IsParrying", false);
        boxCollider.enabled = false;
    }
    private void Update()
    {
        if (playerAttack == null)
        {
            Debug.LogError("PlayerAttack não está atribuído! Verifique o Inspetor.");
            return;
        }

        if (playerAttack.hasParry && IsGrounded())
        {
            if (Input.GetButtonDown("ParryB"))
            {
                StartCoroutine(ActivateParryCollider());
            }
        }
    }

    
    private IEnumerator ActivateParryCollider()
    {
        playerMovement.isParrying = true;
        anim.SetBool("IsParrying",true);
        boxCollider.enabled = true;
        playerAttack.hasParry = false;
        Debug.Log("Parry Ativado!");

        yield return new WaitForSeconds(parryDuration);

        boxCollider.enabled = false;
        Debug.Log("Parry Desativado!");
        anim.SetBool("IsParrying", false);
        playerMovement.isParrying = false;
        StartCoroutine(ResetParry());

        
    }

    public void EndParry()
    {
        playerMovement.isParrying = false;
        anim.SetBool("IsParrying", false);
        Debug.Log("Parry ended. Player can move again.");
    }

    private IEnumerator ResetParry()
    {
        // Aguarda o tempo para o reset
        yield return new WaitForSeconds(parryCooldown);
        playerAttack.hasParry = true; // Ativa novamente a habilidade de parry
        Debug.Log("Parry Reativado!");
    }

    private bool IsGrounded()
    {
        // Acessa o script PlayerMovement para verificar o estado de "IsGrounded"
        PlayerMovement playerMovement = GetComponentInParent<PlayerMovement>();
        return playerMovement != null && playerMovement.IsGrounded();
    }

}
