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
    
    public GameObject[] _hotBar;
    private int _currentHotBarIndex;
    private GameObject _silhouette;
    private int CurrentHotBarIndex
    {
        get => _currentHotBarIndex;
        set
        {
            var clampedValue = Mathf.Clamp(value, 0, _hotBar.Length - 1);
            if (clampedValue == CurrentHotBarIndex || _silhouette == null) return;
            _currentHotBarIndex = clampedValue;
            var pos = _silhouette.transform.position;
            Destroy(_silhouette);
            CreateSilhouette(pos);
        }
    }
    
    public Transform projectileParent;
    public Transform enemyParent;

    private List<Spawn> _spawns;
    
    private void Awake()
    {
        if (projectileParent == null) projectileParent = transform;
        _spawns = FindObjectsOfType<Spawn>().ToList();
    }

    private bool HasTower(Vector3 pos)
    {
        return gameField.HasTile(WorldToCell(pos));
    }
    public void SetTower(Vector3 pos)
    {
        if (HasTower(pos)) return;
        Instantiate(_hotBar[_currentHotBarIndex], WorldToGrid(pos), Quaternion.identity);
        gameField.SetTile(gameField.WorldToCell(pos), gameFieldTile);
    }
    
    public void UpdateSilhouette(Vector3 pos)
    {
        if (HasTower(pos))
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
        _silhouette = Instantiate(_hotBar[_currentHotBarIndex], WorldToGrid(pos), Quaternion.identity);
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
        _spawns.ForEach(x => x.SpawnWave());
    }
    public void TakeHit() 
    { 
        //TODO
    }

    public void SetCurrentHotBarIndex(int value) => CurrentHotBarIndex = value;
    public void IncreaseCurrentHotBarIndex() => CurrentHotBarIndex++;
    public void DecreaseCurrentHotBarIndex() => CurrentHotBarIndex--;
}
