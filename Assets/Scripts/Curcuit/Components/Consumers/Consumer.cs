using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumer : ActiveCircuitComponent
{
    [SerializeField, Min(0)] private float cooldown;
    protected bool IsCoolingDown = false;
    
    protected virtual void Idle()
    {
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        IsCoolingDown = true;
        yield return new WaitForSeconds(cooldown);
        IsCoolingDown = false;
    }
}
