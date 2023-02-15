using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    [SerializeField] private List<GameObject> content;
    private int _focusedSlot = -1;

    [SerializeField] private float padding;

    private void Awake()
    {
        content = new List<GameObject>();
    }
    
    public void SetFocus(int slotIndex)
    {
        if (_focusedSlot == slotIndex) return;
        if(_focusedSlot >= 0) transform.GetChild(_focusedSlot).GetComponent<Slot>().Unfocus();
        _focusedSlot = slotIndex;
        transform.GetChild(_focusedSlot).GetComponent<Slot>().Focus();
    }

    private void UpdateLayout()
    {
        foreach (Transform slot in transform)
        {
            slot.gameObject.SetActive(false);
        }
        
        var width = GetComponent<RectTransform>().rect.width - 2 * padding;
        var slotSpace = width / content.Count;
        for(var i = 0; i < content.Count; i++)
        {
            var slot = transform.GetChild(i);
            slot.gameObject.SetActive(true);
            slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(padding + i * slotSpace, 0);
            slot.GetComponent<Slot>().SetIndex((i+1)%10);
        }
    }

    private void OnValidate()
    {
        if (content.Count > 10) content.RemoveRange(10, content.Count - 10);
        UpdateLayout();
    }
}
