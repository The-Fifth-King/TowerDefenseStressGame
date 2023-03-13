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
    private List<Battery> _batteries;

    public Circuit()
    {
        _generators = new List<Generator>();
        _consumers = new List<Consumer>();
        _wires = new List<CircuitComponent>();
        _batteries = new List<Battery>();
    }
    
    public void CalculatePower()
    {
        var power = 0f;
        power += _generators.Sum(x => x.powerGridImpact);
        power += _consumers.Sum(x => x.powerGridImpact);
        var isOn = power >= 0;

        if (isOn && _batteries.Count > 0)
        {
            var powerEach = power / _batteries.Count;
            _batteries.ForEach(x => x.Charge(powerEach));
        }
        else if (!isOn)
        {
            var powerEach = Mathf.Abs(power / _batteries.Count);
            _batteries.ForEach(x => x.Drain(powerEach));
        }
        
        _consumers.ForEach(x => x.isOn = isOn);
    }

    public void AddComponent(CircuitComponent component)
    {
        switch (component)
        {
            case Generator g:
                _generators.Add(g);
                break;
            case Consumer c:
                _consumers.Add(c);
                break;
            case Battery b:
                var totalCharge = _batteries.Sum(x => x.powerGridImpact);
                _batteries.Add(b);
                _batteries.ForEach(x => x.powerGridImpact = totalCharge/_batteries.Count);
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
        _batteries.AddRange(toAbsorb._batteries);
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
            case Battery b:
                _batteries.Remove(b);
                break;
            default:
                _wires.Remove(component);
                break;
        }
    }

    //todo for itch
     public void Split(Circuit toSplit)
    {
        //todo
    }

     public List<CircuitComponent> GetAllComponents()
     {
         var toReturn = new List<CircuitComponent>();
         toReturn.AddRange(_generators);
         toReturn.AddRange(_consumers);
         toReturn.AddRange(_batteries);
         toReturn.AddRange(_wires);
         return toReturn;
     }
}
