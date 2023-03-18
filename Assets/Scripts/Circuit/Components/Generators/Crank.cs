using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crank : Generator, IInteractable
{
    [Header("Crank")] 
    [SerializeField, Min(0)] private float maxPowerOutput = 50;
    [SerializeField, Min(0)] private float leewayPercent = 10;
    [SerializeField, Min(0)] private float maxSpeed = 1000, acceleration = 1000, deceleration = 100;

    [SerializeField] private Transform crank;

    private bool _isTurned;
    private float _currentSpeed;
    
    private enum Direction
    {
        Clockwise,
        Counterclockwise
    }
    private Direction _turnDir;
    
    private new void Update()
    {
        powerGridImpact = Mathf.Lerp(0, maxPowerOutput, _currentSpeed * (1 + 0.01f * leewayPercent) / maxSpeed);
        
        if (!_isTurned)
        {
            Decelerate();
            Spin();
            return;
        }
        
        var mouseDir = Mouse.current.delta.ReadValue();
        if (mouseDir == Vector2.zero)
        {
            Decelerate();
            Spin();
            return;
        }
            
        if (_currentSpeed == 0)
        {
            var angle = Vector2.SignedAngle(crank.up, mouseDir);
            _turnDir = angle <= 0 ? Direction.Clockwise : Direction.Counterclockwise;
            Accelerate();
        }
        if (Vector2.Angle(_turnDir == Direction.Clockwise ? crank.right : -crank.right, mouseDir) < 90)
        {
            Accelerate();
        }

        Spin();
    }

    private void Accelerate() => _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, acceleration * Time.deltaTime);
    private void Decelerate() => _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, deceleration * Time.deltaTime);
    private void Spin()
    {
        if (_currentSpeed == 0) return;
        crank.Rotate(_turnDir == Direction.Clockwise ? -Vector3.forward : Vector3.forward, _currentSpeed * Time.deltaTime);
    }

    public void Interact()
    {
        _isTurned = true;
        GameObject.Find("GameController").GetComponent<InputController>().inCrank = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Release() 
    {
        _isTurned = false;
        GameObject.Find("GameController").GetComponent<InputController>().inCrank = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
