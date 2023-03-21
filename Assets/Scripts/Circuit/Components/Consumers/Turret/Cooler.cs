using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooler : Consumer
{
    [SerializeField] private Transform coolingField;
    [SerializeField] private float growthSpeed, maximumRadius;

    [SerializeField] private Transform towerHead;
    [SerializeField] private float spinSpeed;
    private void Update()
    {
        if (!isOn)
        {
            Reset();
            return;
        }
        Grow();
    }

    private void Grow()
    {
        towerHead.Rotate(Vector3.forward, -spinSpeed * Time.deltaTime);
        coolingField.localScale = Vector3.MoveTowards(coolingField.localScale, Vector3.one * (maximumRadius * 2),
            growthSpeed * Time.deltaTime);
    }
    private void Reset()
    {
        coolingField.localScale = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maximumRadius);
    }
}
