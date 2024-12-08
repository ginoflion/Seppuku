using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator doorAnimator;
    private Collider2D doorCollider;

    private void Start()
    {
        doorCollider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null && playerInventory.hasKey)
            {
                doorAnimator.SetTrigger("Open"); 
                playerInventory.hasKey = false;
                doorCollider.enabled = false;
                Debug.Log("O jogador abriu a porta");
            }
        }
    }
    
    
}
