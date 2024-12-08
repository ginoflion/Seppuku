using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackCooldown = 2f;          // Tempo entre ataques
    public float attackRange = 1.5f;           // Alcance do ataque
    public float rectangleHeight = 2f;         // Altura do ret�ngulo de ataque
    public int rayCount = 5;                   // N�mero de raios (Raycasts) que ser�o disparados
    public float meleeDamage = 5f;             // Dano do ataque melee
    public LayerMask playerLayer;              // Layer do player para ataque
    public LayerMask obstacleLayer;            // Layer dos obst�culos
    public LayerMask parryLayer;
    public Transform enemyTransform;           // Refer�ncia � posi��o do inimigo
    private Death death;                       // Refer�ncia ao script Death do jogador
    private GameObject player;                 // Refer�ncia ao objeto do jogador
    private bool isAttacking = false;          // Estado de ataque
    private float lastAttackTime;              // Tempo do �ltimo ataque
    public float parryDelay = 1f;              // Tempo de delay ap�s um parry
    public float attackAnimationDelay = 0.5f;  // Tempo de espera ap�s iniciar anima��o para ataque
    [SerializeField] private Animator anim;
    [SerializeField] private Animator animp;

    void Start()
    {
       
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            death = player.GetComponent<Death>(); 
            if (death == null)
            {
                Debug.LogError("Componente Death n�o encontrado no jogador!");
            }
        }
        else
        {
            Debug.LogError("Jogador n�o encontrado!");
        }

    }

    void Update()
    {
        // Reseta o estado de ataque ap�s o cooldown
        if (isAttacking && Time.time > lastAttackTime + attackCooldown)
        {
            isAttacking = false;
            anim.SetBool("Idle", true);      
            anim.SetBool("Attack", false);
        }
    }

    
    public void TriggerAttack(Vector2 attackDirection)
    {
        if (isAttacking) return; 
        if (Time.time < lastAttackTime + attackCooldown)
        {
            return; 
        }
        anim.SetBool("Idle", false);     
        anim.SetBool("Attack", true);
        Debug.Log("anim��o triggered");

       
        StartCoroutine(DelayedAttack(attackDirection));
    }

    private IEnumerator DelayedAttack(Vector2 attackDirection)
    {
        
        yield return new WaitForSeconds(attackAnimationDelay);

      
        AttackPlayer(attackDirection);
        anim.SetBool("Idle", true);
        anim.SetBool("Attack", false);
    }

    public void AttackPlayer(Vector2 attackDirection)
    {
        // Calcula a dire��o perpendicular para formar o ret�ngulo de ataque
        Vector2 perpendicularDirection = Vector2.Perpendicular(attackDirection);
        float spacing = rectangleHeight / (rayCount - 1); // Espa�amento entre os raios

        RaycastHit2D closestHit = new RaycastHit2D();
        bool foundParry = false; // Para rastrear se um parry foi encontrado
        bool hitPlayer = false;  // Para rastrear se o jogador foi atingido

        for (int i = 0; i < rayCount; i++)
        {
            // Define a origem do Raycast ajustada pela perpendicular
            Vector2 rayOrigin = (Vector2)enemyTransform.position + perpendicularDirection * (i * spacing - rectangleHeight / 2);

            // Dispara um Raycast para o jogador
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, attackDirection, attackRange, playerLayer | obstacleLayer | parryLayer);
            Debug.DrawLine(rayOrigin, rayOrigin + attackDirection * attackRange, Color.red, 0.1f); // Visualiza o Raycast

            if (hit.collider != null)
            {
            
                if (hit.collider.CompareTag("Parry"))
                {
                    animp.SetTrigger("Parry");
                    animp.SetBool("IsParrying", false);
                    Debug.Log("Ataque parado pelo Parry!");
                    foundParry = true; 

                    // Guarda a dist�ncia do parry
                    if (closestHit.collider == null || hit.distance < closestHit.distance)
                    {
                        closestHit = hit; // Atualiza o hit mais pr�ximo
                    }

                    ParryB playerParry = hit.collider.GetComponent<ParryB>();
                    if (playerParry != null)
                    {
                        playerParry.EndParry();
                    }
                    lastAttackTime = Time.time + parryDelay;
                    return;
                }
                else if (hit.collider.CompareTag("Player"))
                {
                    // Verifica se o hit do jogador est� mais pr�ximo que um parry
                    if (!foundParry || (closestHit.collider != null && hit.distance < closestHit.distance))
                    {
                        Debug.Log("Atacou o Player com ataque melee retangular!");

                        
                        if (death != null)
                        {
                            death.hit(); 
                            Debug.Log("Morreu");
                        }

                        hitPlayer = true; 
                        closestHit = hit; 
                    }
                }
            }
        }

        // Processa o hit mais pr�ximo
        if (closestHit.collider != null)
        {
            if (foundParry)
            {
                lastAttackTime = Time.time + parryDelay; // Define o cooldown ap�s o parry
                return; // Para de processar o ataque
            }

            // Se atingiu o jogador
            if (hitPlayer)
            {
                Debug.Log("Atacou o Player com ataque melee retangular!");
                if (death != null)
                {
                    death.hit(); // Chama a fun��o hit para aplicar dano
                }
            }
        }

        if (hitPlayer)
        {
            isAttacking = true; // Muda para estado de ataque
            lastAttackTime = Time.time; // Atualiza o tempo do �ltimo ataque
        }
    }

    // Visualizar o ataque no Editor (opcional)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 attackDirection = (Vector2)(enemyTransform.right).normalized; // Utilize o vetor de dire��o correto
        Vector2 perpendicularDirection = Vector2.Perpendicular(attackDirection);
        float spacing = rectangleHeight / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = (Vector2)enemyTransform.position + perpendicularDirection * (i * spacing - rectangleHeight / 2);
            Gizmos.DrawLine(rayOrigin, rayOrigin + attackDirection * attackRange);
        }
    }
}
