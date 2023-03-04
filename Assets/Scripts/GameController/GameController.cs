using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    [SerializeField] private Tilemap gameField;
    [SerializeField] private Tile gameFieldTile;
    
    public GameObject[] hotBar;
    private int _currentHotBarIndex;
    private GameObject _silhouette;
    private int CurrentHotBarIndex
    {
        get => _currentHotBarIndex;
        set
        {
            var clampedValue = Mathf.Clamp(value, 0, hotBar.Length - 1);
            if (clampedValue == CurrentHotBarIndex || _silhouette == null) return;
            _currentHotBarIndex = clampedValue;
            HotbarIndexChanged?.Invoke(_currentHotBarIndex);
            var pos = _silhouette.transform.position;
            Destroy(_silhouette);
            CreateSilhouette(pos);
        }
    }
    public event Action<int> HotbarIndexChanged;

    private Spawn _spawn;
    
    private CircuitController _circuitController;
    private Transform _circuitComponents;
    
    private void Awake()
    {
        _spawn = FindObjectOfType<Spawn>();
        _circuitController = GetComponent<CircuitController>();
        _circuitComponents = GameObject.FindWithTag("EntityController").transform.Find("CircuitComponents");
    }

    private bool IsSpaceFilled(Vector3 pos)
    {
        return gameField.HasTile(WorldToCell(pos));
    }
    public void PlaceCircuitComponent(Vector3 pos)
    {
        if (IsSpaceFilled(pos)) return;
        
        var cellPos = WorldToCell(pos);
        gameField.SetTile(cellPos, gameFieldTile);
        var circuitComponent = 
            Instantiate(hotBar[_currentHotBarIndex], WorldToGrid(pos), Quaternion.identity, _circuitComponents);

        var component = circuitComponent.GetComponent<CircuitComponent>();
        component.powerGridPos = cellPos;
        _circuitController.AddComponent(component, cellPos);
    }

    public void DeleteCircuitComponent(Vector3 pos)
    {
        if (!IsSpaceFilled(pos)) return;
        
        var cellPos = WorldToCell(pos);
        _circuitController.DeleteComponent(cellPos);
        gameField.SetTile(cellPos, null);
    }

    public void UpdateSilhouette(Vector3 pos)
    {
        if (IsSpaceFilled(pos))
        {
            DestroySilhouette();
            return;
        }
        
        if (_silhouette == null)
        {
            CreateSilhouette(pos);
        }
        else
        {
            MoveSilhouette(pos);
        }
    }
    private void MoveSilhouette(Vector3 pos) => _silhouette.transform.position = WorldToGrid(pos);
    private void CreateSilhouette(Vector3 pos)
    {
        _silhouette = Instantiate(hotBar[_currentHotBarIndex], WorldToGrid(pos), Quaternion.identity);
        foreach (var sr in _silhouette.GetComponentsInChildren<SpriteRenderer>())
        {
            var color = Color.black;
            color.a = 0.5f;
            sr.color = color;
        }
    }
    private void DestroySilhouette()
    {
        Destroy(_silhouette);
        _silhouette = null;
    }

    
    public Vector3Int WorldToCell(Vector3 pos) => gameField.WorldToCell(pos);
    public Vector3 CellToGrid(Vector3Int pos) => gameField.CellToWorld(pos);
    public Vector3 WorldToGrid(Vector3 pos) => gameField.CellToWorld(gameField.WorldToCell(pos));

    public void SpawnWave()
    {
        StartCoroutine(_spawn.SpawnWave());
    }
    public void TakeHit() 
    { 
        //TODO
    }

    public void SetCurrentHotBarIndex(int value) => CurrentHotBarIndex = value;
    public void IncreaseCurrentHotBarIndex() => CurrentHotBarIndex++;
    public void DecreaseCurrentHotBarIndex() => CurrentHotBarIndex--;
}