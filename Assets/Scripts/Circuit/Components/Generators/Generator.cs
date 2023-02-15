using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : ActiveCircuitComponent
{
    [SerializeField] private float offPowerGridImpact = 0;
    [SerializeField] private float onPowerGridImpact = 10;
    
    protected void Update()
    {
        powerGridImpact = isOn ? onPowerGridImpact : offPowerGridImpact;
    }
}
