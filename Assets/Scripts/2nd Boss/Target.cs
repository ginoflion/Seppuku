using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject linkedTeleportPoint; 

    public void OnHit() 
    {
        if (linkedTeleportPoint != null)
        {
            linkedTeleportPoint.SetActive(false);
        }

        Destroy(gameObject);
    }
}
