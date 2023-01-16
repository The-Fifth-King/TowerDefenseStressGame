using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCircuitComponent : CircuitComponent
{
    public float powerGridImpact;
    [HideInInspector] public bool isOn = false;
}
