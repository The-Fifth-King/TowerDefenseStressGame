using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : ActiveCircuitComponent
{
    [Header("Battery")] 
    [SerializeField, Min(1)] private float capacity;
    
    [SerializeField] private Transform filling;

    private void Update()
    {
        SetFill(100f * powerGridImpact / capacity);
    }

    private void SetFill(float percent)
    {
        var fillScale = filling.localScale;
        fillScale.y = percent / 100f;
        filling.localScale = fillScale;
    }

    public void Charge(float amount)
    {
        powerGridImpact = Mathf.MoveTowards(powerGridImpact, capacity, amount);
    }

    public void Drain(float amount)
    {
        powerGridImpact = Mathf.MoveTowards(powerGridImpact, 0, amount);
    }
}
