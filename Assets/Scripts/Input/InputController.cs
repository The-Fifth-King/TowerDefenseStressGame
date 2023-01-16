using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class InputController : MonoBehaviour
{
    private GameController _gameController;

    private PlayerInput _input;
    private InputAction _interact, _pointer, _hotBarScroll, _startWave;
    
    private Vector3 _mousePos;
    private GameObject _silhouette;
    
    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _input = GetComponent<PlayerInput>();
        _interact = _input.currentActionMap.FindAction("Interact");
        _pointer = _input.currentActionMap.FindAction("Pointer");
        _hotBarScroll = _input.currentActionMap.FindAction("HotBarScroll");
        _startWave = _input.currentActionMap.FindAction("StartWave");
    }
    
    private void InteractHandler(InputAction.CallbackContext context)
    {
        var hit = Physics2D.Raycast(
            _mousePos, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Interactable"));
        if (hit)
        {
            hit.transform.GetComponent<IInteractable>()?.Interact();
        }
        else
        {
            _gameController.SetTower(_mousePos);
        }
    }
    private void PointerHandler(InputAction.CallbackContext context)
    {
        var mainCam = Camera.main;
        if (mainCam == null) return;
        
        _mousePos = mainCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
        _mousePos.z = 0;

        _gameController.UpdateSilhouette(_mousePos);
    }
    private void HotBarScrollHandler(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<float>();
        switch (input)
        {
            case > 0: _gameController.IncreaseCurrentHotBarIndex();
                break;
            case < 0: _gameController.DecreaseCurrentHotBarIndex();
                break;
        }
    }
    private void StartWaveHandler(InputAction.CallbackContext context) => _gameController.SpawnWave();

    private void SetInputActive(bool value)
    {
        if(value)
        {
            _interact.performed += InteractHandler;
            _pointer.performed += PointerHandler;
            _hotBarScroll.performed += HotBarScrollHandler;
            _startWave.performed += StartWaveHandler;
        }
        else
        {
            _interact.performed -= InteractHandler;
            _pointer.performed -= PointerHandler;
            _hotBarScroll.performed -= HotBarScrollHandler;
            _startWave.performed -= StartWaveHandler;
        }
    }
    private void OnEnable()
    {
        SetInputActive(true);
    }
    private void OnDisable()
    {
        SetInputActive(false);
    }
}
