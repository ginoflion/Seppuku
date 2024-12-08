using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    public Transform patrolPoint; // Ponto original dos espinhos (Transform 2D)
    public Transform attackPoint; // Ponto de ataque dos espinhos (Transform 2D)
    public GameObject spikes; // Referência ao objeto dos espinhos (GameObject)
    public float attackCooldown = 5f; // Cooldown após retorno ao ponto original
    public float delayBeforeAttack = 1f; // Tempo de espera antes de mover para o ponto de ataque
    public float waitAtAttackPoint = 2f; // Tempo de espera no ponto de ataque

    public float moveSpeed = 5f; // Velocidade de movimento dos espinhos entre os pontos

    private float activationTimer = 0f; // Temporizador para o cooldown

    void Update()
    {
        // Atualiza o cooldown de ativação apenas se os espinhos não estiverem em movimento
        if (activationTimer > 0)
        {
            activationTimer -= Time.deltaTime;
        }
    }

    private IEnumerator ActivateSpikes()
    {
        Debug.Log("Espinhos Movendo para o Ponto de Ataque");

        // Espera antes de iniciar o movimento para o ponto de ataque
        yield return new WaitForSeconds(delayBeforeAttack);

        // Move para o ponto de ataque
        while (Vector2.Distance(spikes.transform.position, attackPoint.position) > 0.01f)
        {
            spikes.transform.position = Vector2.MoveTowards((Vector2)spikes.transform.position, (Vector2)attackPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Espinhos chegaram ao ponto de ataque");

        // Espera no ponto de ataque
        yield return new WaitForSeconds(waitAtAttackPoint);

        Debug.Log("Espinhos voltando para o ponto original");

        // Move de volta para o ponto de patrulha original
        while (Vector2.Distance(spikes.transform.position, patrolPoint.position) > 0.01f)
        {
            spikes.transform.position = Vector2.MoveTowards((Vector2)spikes.transform.position, (Vector2)patrolPoint.position, moveSpeed * Time.deltaTime);
            Debug.Log($"Movendo Espinhos para o ponto original. Posição atual: {spikes.transform.position}");
            yield return null;
        }

        // Reinicia o cooldown de ativação após o retorno ao ponto original
        activationTimer = attackCooldown;
        Debug.Log("Espinhos retornaram ao ponto original e cooldown iniciado");
    }

    // Detecta o jogador entrando no trigger com OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ActivateSpikes());
            Debug.Log("Jogador entrou no trigger");
        }
        else
        {
            Debug.Log("Outro objeto entrou no trigger: " + other.gameObject.name);
        }
    }

    // Detecta o jogador saindo do trigger com OnTriggerExit2D
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador saiu do trigger");
        }
    }
}
