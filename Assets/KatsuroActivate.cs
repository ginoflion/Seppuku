using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatsuroActivate : MonoBehaviour
{
        public Transform player;
        public bool isFlipped = false;
        public bool isActive = false; // New flag to control activation

        public void LookAtPlayer()
        {
            Vector3 flipped = transform.localScale;
            flipped.z *= -1f;

            if (transform.position.x > player.position.x && isFlipped)
            {
                transform.localScale = flipped;
                transform.Rotate(0f, 180f, 0f);
                isFlipped = false;
            }
            else if (transform.position.x < player.position.x && !isFlipped)
            {
                transform.localScale = flipped;
                transform.Rotate(0f, 180f, 0f);
                isFlipped = true;
            }
        }

        // Method to activate boss movement
        public void ActivateBoss()
        {
            isActive = true;
            GetComponent<Animator>().SetBool("isActive", true);
        }
    }