using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarController : MonoBehaviour
{
    private GameController _gameController;
    private Hotbar _hotbar;

    private void Awake()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        _hotbar = GetComponent<Hotbar>();
        _hotbar.SetFocus(0);
    }

    private void OnEnable()
    {
        _gameController.HotbarIndexChanged += _hotbar.SetFocus;
    }

    private void OnDisable()
    {
        _gameController.HotbarIndexChanged -= _hotbar.SetFocus;
    }
}
