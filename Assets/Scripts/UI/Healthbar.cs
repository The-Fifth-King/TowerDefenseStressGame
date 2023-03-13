using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Slider bar;
    public Gradient gradient;
    public Image fillBar;
    public void setMaxHealth(int value)
    {
        bar.maxValue = value;
    }

    public void setHealth(int i)
    {
        bar.value = i;

        fillBar.color = gradient.Evaluate(bar.normalizedValue);
    }
    
}
