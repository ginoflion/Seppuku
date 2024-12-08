using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeKill : MonoBehaviour
{
    [SerializeField] private Death death; // Refer�ncia ao script do jogador

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto colidido tem a tag "Player" e se o campo playerHealth foi atribu�do
        if (other.CompareTag("Player") && death != null)
        {
            death.hit();
            Debug.Log("Jogador morreu!");
        }
    }
}
