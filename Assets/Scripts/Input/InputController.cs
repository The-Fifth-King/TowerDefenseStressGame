#nullable enable
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
    private InputAction _interact, _delete, _pointer, _hotBarScroll, _hotBarChoose, _startWave;
    private RaycastHit2D _hit;
    
    private Vector3 _mousePos;
    private GameObject _silhouette;

    private bool _pressed;
    public bool inCrank;
    
    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _input = GetComponent<PlayerInput>();
        _interact = _input.currentActionMap.FindAction("Interact");
        _delete = _input.currentActionMap.FindAction("Delete");
        _pointer = _input.currentActionMap.FindAction("Pointer");
        _hotBarScroll = _input.currentActionMap.FindAction("HotBarScroll");
        _hotBarChoose = _input.currentActionMap.FindAction("HotBarChoose");
        _startWave = _input.currentActionMap.FindAction("StartWave");
        inCrank = false;
    }
    
    private void InteractHandler(InputAction.CallbackContext context)
    {
        _pressed = true;
    }

    private void Update()
    {
        if (_pressed && !inCrank) ClickHelper(true);
    }

    private void ReleaseHandler(InputAction.CallbackContext context)
    {
        _pressed = false;
        if (!_hit) return;
        _hit.transform.GetComponentInParent<IInteractable>()?.Release();
    }
    
    private void DeleteHandler(InputAction.CallbackContext context)
    {
        ClickHelper(false);
    }

    private void ClickHelper(bool interacted)
    {
        _hit = Physics2D.Raycast(
            _mousePos, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Interactable"));
        if (_hit && interacted)
        {
            _hit.transform.GetComponentInParent<IInteractable>()?.Interact();
        }
        else if (interacted)
        {
            _gameController.PlaceCircuitComponent(_mousePos);
        }
        else
        {
            _gameController.DeleteCircuitComponent(_mousePos);
        }
    }
    private void PointerHandler(InputAction.CallbackContext context)
    {
        var mainCam = Camera.main;
        if (mainCam == null) return;
        
        _mousePos = mainCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
        _mousePos.z = 0;

        if (Cursor.lockState != CursorLockMode.Locked)
            _gameController.UpdateSilhouette(_mousePos);
        else
            _gameController.DestroySilhouette();
        
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

    private void HotBarChooseHandler(InputAction.CallbackContext context) => _gameController.SetCurrentHotBarIndex((int) context.ReadValue<float>());
    
    private void StartWaveHandler(InputAction.CallbackContext context) => _gameController.SpawnWave();

    private void SetInputActive(bool value)
    {
        if(value)
        {
            _interact.performed += InteractHandler;
            _interact.canceled += ReleaseHandler;
            _delete.performed += DeleteHandler;
            _pointer.performed += PointerHandler;
            _hotBarScroll.performed += HotBarScrollHandler;
            _hotBarChoose.performed += HotBarChooseHandler;
            _startWave.performed += StartWaveHandler;
        }
        else
        {
            _interact.performed -= InteractHandler;
            _interact.canceled -= ReleaseHandler;
            _delete.performed -= DeleteHandler;
            _pointer.performed -= PointerHandler;
            _hotBarScroll.performed -= HotBarScrollHandler;
            _hotBarChoose.performed -= HotBarChooseHandler;
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
