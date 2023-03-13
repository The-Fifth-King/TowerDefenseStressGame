using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField, Min(0)] private float speed;
    [SerializeField, Min(0)] private int damage;
    [HideInInspector] public Vector3 targetPosition;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, targetPosition, speed * Time.deltaTime);
        
        if(transform.position == targetPosition) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var enemy = col.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ReceiveDamage(damage);
        }
        else
        {
            Destroy(col.gameObject);
        }
        Destroy(gameObject);
    }
}
