# Seppuku


---

**Grupo:**

Gonçalo Silva- a25970;

João Sousa - a25613

Ricardo Almeida - a26344

---

## IA

1. State Machine

2. Decision Scoring 

3. A* Pathfinding

---

## Análise das IAs

## State Machine:

Uma State Machine é um modelo matemático utilizado para representar os diversos comportamentos e as respetivas transições entre estes em um programa.
Ou seja, é composta por estados e transições. Nela cada estado representa o sistema em um determinado momento no tempo e
não é possível uma máquina de estados estar em dois estados ao mesmo tempo.

### Implementação da State Machine

A State Machine é implementada no primeiro Boss do jogo. Ela é utilizada para gerir o comportamento do primeiro boss do jogo. 
A State Machine presente no nosso jogo é composta pelos seguintes estados:

1. Idle
2. Run
3. Attack
4. Dead

O Boss é ativado usando o GameManager, e enquanto que não se encontra ativo, o seu estado é o Idle. 
Quando o player finaliza a leitura do diálogo entre os dois, a boss fight começa, com a ativação do Boss. 
Assim sendo, o seu estado é automaticamente alterado para o Run, visto que se encontra fora do attack range do boss. O RunState é responsável por 
Quando o mesmo se aproxima do player, como a distância é menos que o seu attack range, ele atualiza o seu estado para o attack state que funciona em sincronia com o script Boss Attack. 
Os estados vão alterando entre si, até o player concluir a boss fight. 

```cs
public class Boss : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Run,
        Attack,
        Dead
    }

    private void Update()
    {
        if (!isActive)
        {
            ChangeState(BossState.Idle);
            return;
        }

        BossHealth bossHealth = GetComponent<BossHealth>();

        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            ChangeState(BossState.Idle);
            return;
        }

        switch (currentState)
        {
            case BossState.Idle:
                IdleState();
                break;
            case BossState.Run:
                RunState();
                break;
            case BossState.Attack:
                AttackState();
                break;
            case BossState.Dead:
                DeadState();
                break;
        }
    }

    private void IdleState()
    {
        rb.velocity = Vector2.zero;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Idle") == false)
        {
            animator.Play("Boss_Idle");
        }

        Debug.Log("Boss is in Idle state.");
    }



    public void ChangeState(BossState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        animator.SetBool("isIdle", newState == BossState.Idle);

        Debug.Log($"Boss state changed to: {currentState}");
        OnStateEnter(newState);
    }


    private void OnStateEnter(BossState state)
    {
        switch (state)
        {
            case BossState.Idle:
                animator.Play("Boss_Idle");
                rb.velocity = Vector2.zero;
                break;

            case BossState.Run:
                animator.Play("Boss_Run");
                break;

            case BossState.Attack:
                break;
            case BossState.Dead:
                animator.Play("Boss_Death");
                break;
        }
    }


    private void DeadState()
    {
        Debug.Log("Boss is dead!");
        isActive = false;
        rb.velocity = Vector2.zero;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void RunState()
    {
        if (player == null || currentState == BossState.Dead) return;

        float direction = player.position.x - transform.position.x;
        direction = Mathf.Sign(direction);
        BossHealth bossHealth = GetComponent<BossHealth>();
        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            Debug.Log("Boss is vulnerable and slowing down.");
            rb.velocity = Vector2.zero;
            return; 
        }

        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        if (Vector2.Distance(player.position, transform.position) <= attackRange)
        {
            rb.velocity = Vector2.zero;
            ChangeState(BossState.Attack);
        }

        LookAtPlayer();
    }


    private void AttackState()
    {
        if (player == null || currentState == BossState.Dead) return;

        BossHealth bossHealth = GetComponent<BossHealth>();

        // Stop attacking if the boss is vulnerable
        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            Debug.Log("Boss is vulnerable and pausing attack.");
            return;
        }

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceToPlayer > attackRange)
        {
            Debug.Log("Player is too far. Switching to Run state.");
            animator.SetBool("isFarAway", true); 
            ChangeState(BossState.Run);
            return;
        }

        LookAtPlayer();
    }
    

    public void ActivateBoss()
    {
        if (currentState == BossState.Dead)
        {
            Debug.LogWarning("Cannot activate the boss: It is already dead.");
            return;
        }

        BossHealth bossHealth = GetComponent<BossHealth>();
        if (bossHealth != null && !bossHealth.isInvulnerable)
        {
            Debug.Log("Boss is vulnerable and remains idle.");
            ChangeState(BossState.Idle);
            return;
        }

        isActive = true;
        ChangeState(BossState.Run);
    }

    public void ResetBoss()
    {
        ....
        ChangeState(BossState.Idle);
        .... 
    }
}
```

---
### Boss Health
---

Esta State Machine funciona em conjunto com os outros dois scripts que dão manage no Boss, o Boss_Health e o Boss_Attack.
O Boss_Health é responsável pela vida do Boss e a sua vulnerabilidade.
Neste caso quando o Boss fica vulneravel, o seu estado é mudado para o Idle, uma vez que ele fica stun locked. 
Quando esse período acaba, o estado leva update para o Run, para o Boss continuar a trocar de estados sem o envolvimento deste script.
Quando a vida do Boss é igual ou inferior a 0, o seu estado leva update para o Die e desativamos as suas físicas e mecânicas

```cs 
    private void MakeVulnerable()
    {
       .....
        Boss boss = GetComponent<Boss>();
        if (boss != null)
        {
            boss.ChangeState(Boss.BossState.Idle);
        }

    }



    private void ResetInvulnerability()
    {
        ....
        Boss boss = GetComponent<Boss>();
        if (boss != null && !isDead)
        {
            boss.ChangeState(Boss.BossState.Run); 
        }
    }

    private void Die()
    {
        ...
        Boss boss = GetComponent<Boss>();
        boss.ChangeState(Boss.BossState.Dead);

        ....
    }
```

---
### Boss Attack
---

O outro script que afeta a State Machine é o Boss Attack. Este script é responsável por gerir o ataque do boss e o cooldown entre eles. 
Ao contrário do Boss_Health este script não altera o State do Boss, mas age quando está no atack state, devido ao range do ataque do boss e ao
método de ataque automatico, ou seja, estando dentro do range, o boss ataca automaticamente.

```cs
private IEnumerator AutoAttack()
    {
        while (true)
        {
            if (bossHealth.isDead) yield break;

            if (!bossHealth.isInvulnerable || Vector2.Distance(bossTransform.position, playerTransform.position) > attackRange)
            {
                yield return null;
                continue;
            }

            if (!isAttacking)
            {
                Attack();
            }

            yield return null; 
        }
    }
```

---
## Decision Scoring

O Decision Scoring é uma técnica que utiliza modelos matemáticos e algoritmos de inteligência artificial para avaliar e atribuir pontuações a diferentes opções ou cenários de decisão. 
Ou seja, é um sistema que analisa dados de forma estruturada, com o objetivo de ajudar a tomar decisões de maneira mais informada e eficiente.
Cada opção é analisada com base em critérios pré-definidos, e o sistema atribui uma pontuação que reflete a relevância, risco ou probabilidade de sucesso daquela escolha. 
Essa pontuação serve como uma medida que facilita a comparação e priorização das alternativas disponíveis, orientando tanto decisões humanas quanto processos automatizados.

### Implementação do Decision Scoring

O Decision Scoring é implementando na mecânica do 2º boss para decidir para onde o boss se deve teletransportar.

Neste Decision Scoring ele usa 3 critérios:
1. Distãncia do ponto de teletransporte ao Jogador
2. Linha de Visão
3. Previsão da localização futura do Jogador

Baseado nestes critérios cada ponto de teletransporte recebe uma pontuação , sendo escolhido o com maior pontuação.

  ```cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss_2 : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 0.5f;
    public float minProjectileSpeed = 5f;
    public float maxProjectileSpeed = 10f;
    public float minTeleportWait = 5.0f;
    public float maxTeleportWait = 10.0f;
    public Transform[] teleportPoints;
    public Target[] targets;
    [SerializeField] private Animator anim;
    private Transform player;
    private bool isGrounded = false;
    private bool isFalling = false;
    private BossHealth_2 bossHealth;
    private Rigidbody2D rb;

    // Variáveis para previsão de movimento
    public float predictionTime = 0.5f; // Tempo para prever onde o jogador estará

    void Start()
    {
        // Inicialmente desativa o comportamento até o boss ser ativado
        enabled = false;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossHealth = GetComponent<BossHealth_2>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void ActivateBoss()
    {
        enabled = true;
        StartCoroutine(TeleportAndShootRoutine());
        Debug.Log("Boss activated!");
    }

    IEnumerator TeleportAndShootRoutine()
    {
        while (!isGrounded)
        {
            // Escolher o melhor ponto de teletransporte
            Transform bestTeleportPoint = ChooseBestTeleportPoint();

            if (bestTeleportPoint != null)
            {
                TeleportToPoint(bestTeleportPoint);
            }
            else
            {
                FallToGround();
                yield break;
            }

            // Esperar e atacar
            float waitDuration = Random.Range(minTeleportWait, maxTeleportWait);

            float startTime = Time.time;
            while (Time.time < startTime + waitDuration)
            {
                ShootProjectile();
                yield return new WaitForSeconds(fireRate);
            }
        }
    }

    Transform ChooseBestTeleportPoint()
    {
        Transform bestPoint = null;
        float bestScore = float.MinValue;

        foreach (Transform point in teleportPoints)
        {
            if (!point.gameObject.activeSelf) continue;

            float score = EvaluateTeleportPoint(point);
            if (score > bestScore)
            {
                bestScore = score;
                bestPoint = point;
            }
        }

        return bestPoint;
    }

    float EvaluateTeleportPoint(Transform point)
    {
        if (player == null) return float.MinValue;

        // Direção e distância ao jogador
        Vector2 directionToPlayer = (player.position - point.position).normalized;
        float distanceToPlayer = Vector2.Distance(player.position, point.position);

        // Prever a posição futura do jogador
        Vector2 playerVelocity = player.GetComponent<Rigidbody2D>()?.velocity ?? Vector2.zero;
        Vector2 predictedPosition = (Vector2)player.position + (playerVelocity * predictionTime);
        Vector2 futureDirectionToPlayer = (predictedPosition - (Vector2)point.position).normalized;

        // 1. Avaliação da distância
        float distanceScore = Mathf.Clamp(1.0f / distanceToPlayer, 0.1f, 1.0f);

        // 2. Avaliação da linha de visão
        RaycastHit2D hit = Physics2D.Raycast(point.position, directionToPlayer, distanceToPlayer);
        float lineOfSightScore = (hit.collider == null || hit.collider.CompareTag("Player")) ? 1.0f : 0.0f;

        // 3. Alinhamento com o jogador 
        Vector2 bossForward = Vector2.right; // Direção "frontal" do boss, pode ser ajustada conforme necessário
        float alignmentScore = Vector2.Dot(futureDirectionToPlayer, bossForward);

        // Pesos ajustáveis para cada critério
        float totalScore = (distanceScore * 0.4f) + (lineOfSightScore * 0.4f) + (alignmentScore * 0.2f);

        return totalScore;
    }

    void TeleportToPoint(Transform point)
    {
        transform.position = point.position;
        ActivateTarget(System.Array.IndexOf(teleportPoints, point));
        Debug.Log("Teleported to point: " + point.name);
    }

    void ActivateTarget(int activeIndex)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                targets[i].gameObject.SetActive(i == activeIndex);
            }
        }
    }

    void FallToGround()
    {
        isGrounded = false;
        isFalling = true;
        anim.SetBool("isFalling", true);
        if (bossHealth != null)
        {
            bossHealth.isInvulnerable = false;
        }

        rb.isKinematic = false;
        rb.gravityScale = 1.5f;
        Debug.Log("Boss is falling to the ground and is now vulnerable!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isFalling)
        {
            isGrounded = true;
            isFalling = false;
            anim.SetBool("isFalling", false);
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            rb.gravityScale = 0;

            Debug.Log("Boss has landed and is vulnerable!");
        }
    }

    void ShootProjectile()
    {
        if (player == null || isGrounded) return;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;

        float randomProjectileSpeed = Random.Range(minProjectileSpeed, maxProjectileSpeed);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * randomProjectileSpeed;

        Collider2D enemyCollider = GetComponent<Collider2D>();
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(projectileCollider, enemyCollider);
    }
}

```
Um peso é atribuido a cada critério , podendo ser ajustado para obter um comportamento diferente do Boss

Este IA é usado sempre que o Boss dá teletransporte para um novo ponto.

***
AI do Sliding Tile Puzzle- Ricardo Salema de Almeida n26344
=============================================
![ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/f7e5c6be-62e8-476b-a45e-8d2d265abd7f)

Introdução
=

Neste projeto, apresento a Inteligência Artificial utilizada no Puzzle Generator (apenas no formato 3x3), que estou a desenvolver no âmbito de outra Unidade Curricular(algumas modificações foram realizadas com o objetivo de testar os resultados de diferentes métodos).

O objetivo principal é resolver o puzzle utilizando algoritmos de Pathfinding. Para este projeto, foram implementados o A* e o BFS, cujos resultados serão comparados. Além disso, serão abordados os algoritmos Dijkstra e DFS, explicando o motivo de não terem sido incluídos no projeto final.


Explicação
=

Breadth First Search(BFS)
=

O BFS é um algoritmo de busca que explora todos os nós de um nível de profundidade antes de avançar para o próximo. Ele garante encontrar o caminho mais curto em grafos sem pesos diferentes (onde o custo entre nós é constante, como neste caso, com custo igual a 1).

Agora, abordando o exemplo prático que tenho, a função bfsSolve recebe como argumentos o estado inicial (atual) do puzzle, o estado final (o puzzle resolvido) e o tamanho do puzzle (que, neste caso, é sempre 3x3, pois, se aumentar, o custo de memória torna a solução impossível devido ao elevado número de estados). A função retorna um array com todos os estados até à solução ou null, caso não encontre uma solução.
```ts
export const bfsSolve = (
  initialState: PuzzleState,
  targetState: PuzzleState,
  gridRow: number,
  gridCol: number
): PuzzleState[] | null => {
```


Começamos por verificar se o estado inicial/atual tem solução(vai ter devido a verificações anteriores mas convém ter sempre) e retorna null se não tiver
```ts
if (!isSolvable(initialState, gridCol)) {
    console.log("Puzzle is not solvable");
    return null;
  }
```

Inicializamos uma queue, que é uma estrutura de dados responsável por armazenar os estados do puzzle que ainda precisam ser explorados. Começamos a fila com o estado inicial do puzzle e um caminho vazio, pois ainda não houve movimentos. Cada elemento da fila contém o estado atual do puzzle e o caminho percorrido até esse estado.
Em seguida, inicializamos também uma lista chamada visited para armazenar os estados já visitados. Isso previne que estados repetidos sejam explorados novamente, evitando loops infinitos e perda de eficiência. Por fim, adicionamos o estado inicial à lista de visitados.
```ts
const queue: { state: PuzzleState; path: PuzzleState[] }[] = [{ state: initialState, path: [] }];
  const visited = new Set<string>();
  visited.add(JSON.stringify(initialState));
```

Começamos um loop que continua enquanto a queue não estiver vazia, o que significaria que não há mais estados para explorar. Usando a abordagem FIFO (First In, First Out), removemos o primeiro estado da fila e obtemos o caminho percorrido até agora e o estado atual desse elemento.
```ts
 while (queue.length > 0) {
    const { state, path } = queue.shift()!;
```

Se o estado atual for igual ao estado alvo (ou solução), retornamos o caminho até esse ponto.
```ts
if (isSolved(state, targetState)) {
      return path;
    }
```
Caso contrário, obtemos os estados possíveis a partir dos movimentos válidos do estado atual e, para cada novo estado, verificamos se ele já foi visitado. Se ainda não foi visitado, marcamos como visitado e adicionamos o novo estado à fila, junto com o caminho atualizado. Este processo é repetido até que uma solução seja encontrada ou até que a fila fique vazia, o que indicaria que não há solução.
```ts
const validMoves = getValidMoves(state, gridRow, gridCol);
    for (const move of validMoves) {
      const moveKey = JSON.stringify(move);
      if (!visited.has(moveKey)) {
        visited.add(moveKey);
        queue.push({
          state: move,
          path: [...path, move],
        });
      }
    }
  }

  console.log("No solution found");
  return null;
};
```
A*
=
O A*, ao contrário do BFS, é um algoritmo de Pathfinding que usa heurísticas (no meu caso, a distância de Manhattan) para alcançar a solução ótima e é definido pela fórmula f=g+h, melhorando significativamente a rapidez e a eficiência da busca. A distância de Manhattan é uma heurística ideal para grids, pois reflete exatamente o tipo de movimentação permitida (movimentos ortogonais: cima, baixo, esquerda, direita). Em vez de calcular a distância reta (euclidiana), calcula a soma das diferenças absolutas das coordenadas nos eixos x e y (no nosso caso, col e row, respectivamente).

No caso do meu projeto, primeiro faço o cálculo da heurística (distância de Manhattan) para determinar o quão longe está cada peça da sua posição alvo. Para isso, inicializo a distância total como 0 e percorro cada peça do estado atual fornecido à função. Durante a iteração, identifico a posição atual de cada peça (linha e coluna) no tabuleiro e, em seguida, localizo a posição alvo correspondente dessa peça no estado final.
Para calcular a distância, utilizo a soma das diferenças absolutas entre as coordenadas da posição atual e da posição alvo, considerando os eixos row (linha) e col (coluna).
Por fim, somo as distâncias individuais de todas as peças, obtendo a distância total, que será usada como heurística.
```ts
const manhattanDistance = (state: PuzzleState, gridRow: number, gridCol: number, targetState: PuzzleState): number => {
  let distance = 0;
  for (let i = 0; i < state.length; i++) {
    if (state[i] !== null) {
      const currentRow = Math.floor(i / gridCol);
      const currentCol = i % gridCol;
      const targetIndex = targetState.indexOf(state[i])!;
      const targetRow = Math.floor(targetIndex / gridCol);
      const targetCol = targetIndex % gridCol;
      distance += Math.abs(currentRow - targetRow) + Math.abs(currentCol - targetCol);
    }
  }
  return distance;
};
```

A função que usa o algoritmo, aStarSolve, recebe como argumentos e retorna exatamente a mesma coisa que a bfsSolve que já expliquei anteriormente
```ts
export const aStarSolve = (
  initialState: PuzzleState,
  targetState: PuzzleState,
  gridRow: number,
  gridCol: number
): PuzzleState[] | null => {
```
De seguida, inicializamos uma lista aberta e uma lista fechada. A lista aberta é semelhante à queue usada no bfsSolve, armazenando os estados que ainda precisam de ser explorados. No entanto, ao contrário da queue do BFS, a lista aberta armazena, além do estado atual e do caminho até lá, os valores de g, h e f. O g representa o custo acumulado para alcançar o estado atual a partir do estado inicial, enquanto o h é a heurística. O valor de f é a soma de g e h. A lista fechada, por sua vez, tem um papel semelhante à lista de visitados no bfsSolve, sendo responsável por armazenar os estados já explorados, evitando a exploração repetida dos mesmos e, assim, prevenindo loops infinitos e a perda de eficiência. Depois, colocamos o estado inicial na lista aberta, com o caminho vazio, g igual a 0, h calculado pela função auxiliar da distância de Manhattan e f usando a mesma função para h, já que g é 0 no início.
```ts
const openList: { state: PuzzleState; path: PuzzleState[]; g: number; h: number; f: number }[] = [];
  const closedList = new Set<string>();

  openList.push({
    state: initialState,
    path: [],
    g: 0,
    h: manhattanDistance(initialState, gridRow, gridCol, targetState),
    f: manhattanDistance(initialState, gridRow, gridCol, targetState),
  });
```
Começamos um loop que continua enquanto a lista aberta não estiver vazia. Dentro do loop, transformamos a lista aberta numa priority queue, ordenando os elementos com base no valor de f(os com menor valor de f têm prioridade). Em seguida, verificamos se o estado atual é a solução. Caso seja, retornamos o caminho de estados até esse aqui.
```ts
while (openList.length > 0) {
    openList.sort((a, b) => a.f - b.f);
    const current = openList.shift()!;

    if (isSolved(current.state, targetState)) {
      return current.path;
    }
```
Adicionamos o estado atual à lista fechada, obtemos os estados possíveis a partir dos movimentos válidos do estado atual e, para cada novo estado, verificamos se ele já foi visitado. Se não foi, calculamos g, h e adicionamos ambos para obter o f, adicionando o novo estado com esses parâmetros à lista aberta. Repetimos este processo até encontrarmos a solução ou a lista aberta ficar vazia
```ts
closedList.add(JSON.stringify(current.state));
    const validMoves = getValidMoves(current.state, gridRow, gridCol);
    for (const move of validMoves) {
      const moveKey = JSON.stringify(move);

      if (!closedList.has(moveKey)) {
        const g = current.g + 1;
        const h = manhattanDistance(move, gridRow, gridCol, targetState);
        const f = g + h;

        openList.push({
          state: move,
          path: [...current.path, move],
          g,
          h,
          f,
        });
      }
    }
  }
  return null;
```

Comparações
=

No programa em si adicionei um contador que conta os movimentos até chegar à solução e o tempo que o algoritmo demora a chegar à solução. Comparando ambos os números, no que toca a tempo de execução o A* é bastante mais rápido que o bfs a chegar à solução e no que toca a movimentos também, em média, tem menos do que o bfs mas a diferença não é significativa

![image](https://github.com/user-attachments/assets/65573e6d-cf0d-4716-b38e-2ecf62eb2b98)
![image](https://github.com/user-attachments/assets/23f04555-a2ab-4830-817e-f3b035f655af)

Dois exemplos do A*

![image](https://github.com/user-attachments/assets/5a1ec00b-94e3-459e-ab7d-0a663cd7d6ca)
![image](https://github.com/user-attachments/assets/5ea02ac1-6184-4930-b70a-caf2baed835d)

Dois exemplos do BFS


Também tentei aplicar o DFS para fazer comparações, mas não conseguiu resolver o problema, ficando infinitamente à procura da solução. Não tentei aplicar o Dijkstra, porque os pesos são todos iguais, o que torna o BFS automaticamente mais eficiente para este tipo de problema.

Conclusão
=
Concluindo, vejo que o A* é um algoritmo mais eficiente, especialmente em termos de tempo de execução, pois utiliza heurísticas para a busca, escolhendo sempre o caminho com o menor valor de f. Isso evita a exploração de estados desnecessários, tornando o processo mais rápido e otimizado comparando com o BFS. Embora o BFS seja uma solução garantida para encontrar o caminho mais curto, ele acaba a explorar mais estados do que o necessário, resultando num maior tempo de execução.

O A* destaca-se pela sua inteligência na escolha de caminhos, combinando a exploração de estados com uma função heurística (distância de Manhattan) para priorizar movimentos que levam à solução de forma mais eficiente. No entanto, em termos de número de movimentos, a diferença entre A* e BFS não é assim tão significativa, embora o A* ainda consiga, em alguns casos, reduzir o número de movimentos.

