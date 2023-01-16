using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircuitController : MonoBehaviour
{
    private List<PowerGridData> _powerGridData;
    private List<Circuit> _circuits;

    private class PowerGridData
    {
        public CircuitComponent component;
        public Vector3Int gridPos;
        public int circuitIndex;
    }

    private void Awake()
    {
        _powerGridData = new List<PowerGridData>();
        _circuits = new List<Circuit>();
    }
    private void Update()
    {
        _circuits.ForEach(x => x.CalculatePower());
    }

    public void AddComponent(CircuitComponent component, Vector3Int gridPos)
    {
        var powerGridData = new PowerGridData();
        powerGridData.component = component;
        powerGridData.gridPos = gridPos;
        
        var neighbours = FindNeighbours(gridPos);
        if (neighbours.Count == 0)
        {
            var circuitIndex = FindLowestFreeCircuitIndex();
            powerGridData.circuitIndex = circuitIndex;
            _circuits.Add(new Circuit());
            _circuits[circuitIndex].AddComponent(component);
        }
        else if (neighbours.TrueForAll(x => neighbours[0].circuitIndex == x.circuitIndex))
        {
            powerGridData.circuitIndex = neighbours[0].circuitIndex;
            _circuits[neighbours[0].circuitIndex].AddComponent(component);
        }
        else
        {
            var mergedCircuitIndex = neighbours.Min(x => x.circuitIndex);
            var circuitsIndicesToMerge = neighbours
                .Select(x => x.circuitIndex)
                .Where(x => x != mergedCircuitIndex)
                .Distinct().ToList();

            _powerGridData.ForEach(x =>
            {
                if (circuitsIndicesToMerge.Contains(x.circuitIndex)) x.circuitIndex = mergedCircuitIndex;
            });

            foreach (var toMerge in circuitsIndicesToMerge)
            {
                _circuits[mergedCircuitIndex].Merge(_circuits[toMerge]);
                _circuits.RemoveAt(toMerge);
            }

            powerGridData.circuitIndex = mergedCircuitIndex;
            _circuits[mergedCircuitIndex].AddComponent(component);
        }
        
        _powerGridData.Add(powerGridData);
    }
    
    private List<PowerGridData> FindNeighbours(Vector3Int pos)
    {
        var relativeNeighbourPos = new List<Vector3Int>
        {
            pos + Vector3Int.down,
            pos + Vector3Int.left,
            pos + Vector3Int.up,
            pos + Vector3Int.right,
            pos + (pos.y % 2 == 0? new Vector3Int(-1, -1) : new Vector3Int(1, -1)),
            pos + (pos.y % 2 == 0? new Vector3Int(-1, 1) : new Vector3Int(1, 1))
        };

        return _powerGridData.FindAll(x => relativeNeighbourPos.Contains(x.gridPos));
    }
    private int FindLowestFreeCircuitIndex()
    {
        var i = 0;
        foreach (var entry in _powerGridData.Where(entry => entry.circuitIndex == i))
        {
            i++;
        }

        return i;
    }
}
