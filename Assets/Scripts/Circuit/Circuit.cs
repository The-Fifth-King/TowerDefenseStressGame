using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Circuit
{
    private List<Generator> _generators;
    private List<Consumer> _consumers;
    private List<CircuitComponent> _wires;
   // private List<CircuitComponent> _allCircuitsComponents;
    
    public Circuit()
    {
        _generators = new List<Generator>();
        _consumers = new List<Consumer>();
        _wires = new List<CircuitComponent>();
    }
    
    public void CalculatePower()
    {
        var power = 0f;
        power += _generators.Sum(x => x.powerGridImpact);
        power += _consumers.Sum(x => x.powerGridImpact);
        var isOn = power >= 0;
        _consumers.ForEach(x => x.isOn = isOn);
    }

    public void AddComponent(CircuitComponent component)
    {
       // _allCircuitsComponents.Add(component);
        switch (component)
        {
            case Generator g:
                _generators.Add(g);
                break;
            case Consumer c:
                _consumers.Add(c);
                break;
            default:
                _wires.Add(component);
                break;
        }
    }
    
    public void Merge(Circuit toAbsorb)
    {
        _generators.AddRange(toAbsorb._generators);
        _consumers.AddRange(toAbsorb._consumers);
        _wires.AddRange(toAbsorb._wires);
    }

    public void RemoveComponent(CircuitComponent component)
    {
        switch (component)
        {
            case Generator g:
                _generators.Remove(g);
                break;
            case Consumer c:
                _consumers.Remove(c);
                break;
            default:
                _wires.Remove(component);
                break;
        }
    }

    public void Split(Circuit toSplit)
    {
        //todo
    }
}
