using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumer : CurcuitComponent
{
    [SerializeField, Min(0)] private float energyConsumption;
    [SerializeField, Min(0)] private float cooldown;

    private bool _isCoolingDown = false;
    
    
    protected virtual void Update()
    {
        CurrentEnergy = 10;
        if (_isCoolingDown || CurrentEnergy < energyConsumption) return;
        Consume();
    }

    protected virtual void Consume()
    {
        CurrentEnergy -= energyConsumption;
    }

    protected void StartCooldown()
    {
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        _isCoolingDown = true;
        yield return new WaitForSeconds(cooldown);
        _isCoolingDown = false;
    }
}
