using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private Animator _animator;
    private TMP_Text _text;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Focus()
    {
        _animator.ResetTrigger("Unfocus");
        _animator.enabled = true;
    }
    public void Unfocus()
    {
        _animator.SetTrigger("Unfocus");
    }

    public void SetIndex(int index)
    {
        _text = GetComponentInChildren<TMP_Text>();
        _text.text = index.ToString();
    }
    private void DisableAnimator()
    {
        _animator.enabled = false;
    }
}
