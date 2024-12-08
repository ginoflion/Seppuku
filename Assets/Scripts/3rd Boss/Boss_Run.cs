using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float attackRange = 3f;

    Transform player;
    Rigidbody2D rb;
    Boss boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();

        if (player == null)
            Debug.LogWarning("Player not found! Make sure the Player GameObject has the correct tag.");

        if (rb == null)
            Debug.LogWarning("Rigidbody2D not found! Make sure Rigidbody2D is attached to the GameObject.");

        if (boss == null)
            Debug.LogWarning("Boss script not found! Make sure Boss script is attached to the GameObject.");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null)
        {
            Debug.LogWarning("Boss component is null! Make sure it is attached.");
            return;
        }

        if (!boss.isActive)
        {
            Debug.LogWarning("Boss isActive is false!");
            return;
        }

        if (boss.player == null)
        {
            Debug.LogWarning("Player reference in Boss script is null!");
            return;
        }

        // Debug position values
        Vector2 target = new Vector2(boss.player.position.x, rb.position.y);
        Debug.Log($"Boss Position: {rb.position}, Target: {target}");

        // Perform movement
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        Debug.Log($"Calculated new position: {newPos}");

        rb.MovePosition(newPos);
        Debug.Log("Boss moved.");

        // Check attack range
        if (Vector2.Distance(boss.player.position, rb.position) <= attackRange)
        {
            animator.SetTrigger("Attack");
            Debug.Log("Boss is in attack range. Triggering Attack.");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
        Debug.Log("Exiting state and resetting Attack trigger.");
    }
}
