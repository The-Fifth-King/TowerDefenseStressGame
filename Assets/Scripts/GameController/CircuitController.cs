using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircuitController : MonoBehaviour
{
    private List<PowerGridData> _powerGridData;
    private Dictionary<int, Circuit> _circuits;

    private class PowerGridData
    {
        public CircuitComponent component;
        public Vector3Int gridPos;
        public int circuitIndex;
        public bool visitedFlag = false;
    }

    private void Awake()
    {
        _powerGridData = new List<PowerGridData>();
        _circuits = new Dictionary<int, Circuit>();
    }
    private void FixedUpdate()
    {
        foreach (var circuit in _circuits.Values)
        {
            circuit.CalculatePower();
        }
    }

    public void AddComponent(CircuitComponent component, Vector3Int gridPos)
    {
        var powerGridData = new PowerGridData();
        powerGridData.component = component;
        powerGridData.gridPos = gridPos;

        var neighbours = FindNeighbours(gridPos);
        if (component is not Wire) neighbours.RemoveAll(x => x.component is not Wire);
        foreach (var neighbour in neighbours)
        {
            if(component is Wire wire) wire.AddConnection(GetNeighbourConnectionMaskBit(gridPos, neighbour.gridPos));
            if (neighbour.component is Wire neighbourWire) 
                neighbourWire.AddConnection(GetNeighbourConnectionMaskBit(neighbour.gridPos, gridPos));
        }
        
        if (neighbours.Count == 0)
        {
            var circuitIndex = FindLowestFreeCircuitIndex();
            powerGridData.circuitIndex = circuitIndex;
            _circuits.Add(circuitIndex, new Circuit());
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
            
            for(var i = 0; i < circuitsIndicesToMerge.Count; i++)
            {
                _circuits[mergedCircuitIndex].Merge(_circuits[circuitsIndicesToMerge[i]]);
                _circuits.Remove(circuitsIndicesToMerge[i]);
            }

            powerGridData.circuitIndex = mergedCircuitIndex;
            _circuits[mergedCircuitIndex].AddComponent(component);
        }
        
        _powerGridData.Add(powerGridData);
    }

    public void DeleteComponent(Vector3Int gridPos, Tilemap gameField)
    {
        PowerGridData current = _powerGridData.Find(x => x.gridPos.Equals(gridPos));
        CircuitComponent component = current.component;
        int index = current.circuitIndex;

        if (!component.placedByPlayer) return;

        var componentList = _circuits[index].GetAllComponents();
        foreach (var i in componentList)
        {
            if (!i.placedByPlayer)
            {
                //todo, when prebuilt components are implemented
                continue;
            }
            
            PowerGridData currentData = _powerGridData.Find(x => x.gridPos.Equals(i.powerGridPos));
            var currentNeighbours = FindNeighbours(currentData.gridPos);
            if (component is not Wire) currentNeighbours.RemoveAll(x => x.component is not Wire);
            foreach (var j in currentNeighbours)
                
            {
                if(i is Wire wire) wire.RemoveConnection(GetNeighbourConnectionMaskBit(currentData.gridPos, j.gridPos));
            }
            _powerGridData.Remove(currentData);
            gameField.SetTile(currentData.gridPos, null);
            Destroy(i.gameObject);
        }
        
        _circuits.Remove(index);
    }

    private bool ConnectionDFS(PowerGridData currentData, PowerGridData rootData, PowerGridData target)
    {
        currentData.visitedFlag = true;
        var currentNeighbours = FindNeighbours(currentData.gridPos);
        
        foreach (var newData in currentNeighbours)
        {
            if (newData == target)
                return true;
            if (newData != rootData && !newData.visitedFlag)
                ConnectionDFS(newData, currentData, target);
        }
        
        return false;
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
    public static int GetNeighbourConnectionMaskBit(Vector3Int pos, Vector3Int neighbour)
    {
        var relativeNeighbourPos = new List<Vector3Int>
        {
            pos + Vector3Int.right,
            pos + (pos.y % 2 == 0? Vector3Int.up : new Vector3Int(1, 1)),
            pos + (pos.y % 2 == 0? new Vector3Int(-1, 1) : Vector3Int.up),
            pos + Vector3Int.left,
            pos + (pos.y % 2 == 0? new Vector3Int(-1, -1) : Vector3Int.down),
            pos + (pos.y % 2 == 0? Vector3Int.down : new Vector3Int(1, -1)),
        };

        for (var i = 0; i < relativeNeighbourPos.Count; i++)
        {
            if (relativeNeighbourPos[i].Equals(neighbour))
            {
                return 1 << i;
            }
        }

        return -1;
    }
    private int FindLowestFreeCircuitIndex()
    {
        var result = 0;
        for (var i = 0; i < _powerGridData.Count; i++)
        {
            if (_powerGridData[i].circuitIndex != result) continue;
            result++;
            i = 0;
        }

        return result;
    }
}
