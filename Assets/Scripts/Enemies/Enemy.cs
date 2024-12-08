using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool defeated;

    public bool IsDefeated()
    {
        return defeated;
    }

    public void Kill()
    {
        defeated = true;
        gameObject.SetActive(false);
    }


}
