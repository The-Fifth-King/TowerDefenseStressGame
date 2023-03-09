using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitComponent : MonoBehaviour
{
    [HideInInspector] public Vector3Int powerGridPos;
    [HideInInspector] public bool placedByPlayer = false;
}
