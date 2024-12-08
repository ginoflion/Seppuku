using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Enemy_Spawner : MonoBehaviour
{
    // Lista de posi��es espec�ficas onde os inimigos ser�o spawnados
    public Transform[] spawnPoints;

    // Prefab do inimigo que ser� spawnado
    public GameObject enemyPrefab;

    // Tempo entre cada tentativa de spawn
    public float spawnInterval = 5f;

    // Lista para guardar os inimigos spawnados
    private List<GameObject> spawnedEnemies;

    // Refer�ncia ao script KatsuroStun para acessar o estado de stun
    private KatsuroStun katsuroStun;

    // Refer�ncia � coroutine de spawn
    private Coroutine spawnCoroutine;

    private bool isSpawningEnabled = true;

    [SerializeField] private KatsuroHealth katsuroHealth;
    void Start()
    {

        spawnedEnemies = new List<GameObject>(new GameObject[spawnPoints.Length]);
        katsuroStun = GetComponent<KatsuroStun>();  // Obt�m a refer�ncia do KatsuroStun

        // Inicia o ciclo de spawn
        spawnCoroutine = StartCoroutine(SpawnEnemies());

    }

    void Update()
    {
        if (katsuroHealth.isDead)
        {
            KillEnemies();
            isSpawningEnabled = false;  // Desativa o spawn ao matar os inimigos
        }
        else if (isSpawningEnabled) // S� executa o spawn se estiver ativo
        {
            if (katsuroStun != null)
            {
                bool isStunned = katsuroStun.IsStunned();

                if (isStunned && spawnCoroutine != null)
                {
                    StopCoroutine(spawnCoroutine);
                    spawnCoroutine = null;
                    Debug.Log("Corrotina pausada devido ao atordoamento");
                }
                else if (!isStunned && spawnCoroutine == null)
                {
                    spawnCoroutine = StartCoroutine(SpawnEnemies());
                    Debug.Log("Corrotina retomada ap�s o atordoamento");
                }
            }
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Limpa os inimigos desativados que foram spawnados pelos pontos espec�ficos
            CleanUpInactiveEnemies();

            // Tenta spawnar inimigos em cada ponto especificado
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                // Verifica se existe um inimigo spawnado no �ndice `i`
                if (spawnedEnemies[i] != null)
                {
                    // Se o inimigo ainda est� ativo, passa para o pr�ximo
                    if (spawnedEnemies[i].activeSelf)
                        continue;
                }

                // Se n�o existe inimigo ou ele foi destru�do/desativado, faz o spawn
                spawnedEnemies[i] = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);
            }

            // Espera pelo intervalo de tempo antes de tentar spawnar novamente
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void KillEnemies()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
                Destroy(spawnedEnemies[i]);
                spawnedEnemies[i] = null;  // Remove a refer�ncia ao objeto destru�do
        }
    }
    private void CleanUpInactiveEnemies()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            // Verifica se o inimigo est� desativado ou nulo na posi��o espec�fica de spawn
            if (spawnedEnemies[i] != null && !spawnedEnemies[i].activeSelf)
            {
                Destroy(spawnedEnemies[i]);
                spawnedEnemies[i] = null;  // Remove a refer�ncia ao objeto destru�do
            }
        }
    }
}
