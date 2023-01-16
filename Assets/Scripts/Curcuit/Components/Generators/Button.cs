using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Generator, IInteractable
{
    [Header("Button")]
    [SerializeField, Min(0)] private float activeTime = 5;
    
    [SerializeField] private SpriteRenderer _buttonRenderer;
    [SerializeField] private Color onColor, offColor;

    public void Interact()
    {
        SetState(true);
        StartCoroutine(TurnOffRoutine());
    }

    private void SetState(bool state)
    {
        isOn = state;
        _buttonRenderer.color = state? onColor : offColor;
    }

    private IEnumerator TurnOffRoutine()
    {
        yield return new WaitForSeconds(activeTime);
        SetState(false);
    }
}
