using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolingField : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Accelerate(-30f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Accelerate(30f);
        }
    }
}
