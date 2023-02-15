using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    private GameController _gameController;
    private LineRenderer _lineRenderer;
    
    private Transform _enemyParent;

    [SerializeField] private List<Vector3Int> cellsToTraverse;
    [SerializeField] private int countOfEnemiesToSpawn;
    [SerializeField] private List<GameObject> spawnLoop;
    [SerializeField] private List<DistinctSpawn> distinctSpawn;
    [Serializable] private class DistinctSpawn
    {
        [Min(0)] public int spawnIndex;
        public GameObject enemy;
    }
    
    private void Awake()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        _lineRenderer = GetComponent<LineRenderer>();
        var wayPoints = ConvertCellsToWayPoints().ToArray();
        _lineRenderer.positionCount = wayPoints.Length;
        _lineRenderer.SetPositions(wayPoints);
        _lineRenderer.SetPosition(0, _gameController.WorldToGrid(transform.position));
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, 
            _gameController.WorldToGrid(GameObject.FindWithTag("Finish").transform.position));

        _enemyParent = GameObject.FindWithTag("EntityController").transform.Find("Enemies");
    }

    public void SpawnWave()
    {
        var j = 0;
        for (var i = 0; i < countOfEnemiesToSpawn; i++)
        {
            if (j < distinctSpawn.Count && distinctSpawn[j].spawnIndex == i)
            {
                SpawnEnemy(distinctSpawn[j].enemy);
                j++;
                continue;
            }
            
            SpawnEnemy(spawnLoop[countOfEnemiesToSpawn % spawnLoop.Count]);
        }
    }
    private void SpawnEnemy(GameObject enemy)
    {
        var instance = Instantiate(enemy, transform.position, Quaternion.identity, _enemyParent);
        instance.GetComponent<Enemy>().wayPoints = ConvertCellsToWayPoints();
    }

    private List<Vector3> ConvertCellsToWayPoints()
    {
        var wayPoints = new List<Vector3>();
        
        var cell = _gameController.WorldToCell(transform.position);
        foreach (var relativeCell in cellsToTraverse)
        {
            cell += relativeCell;
            wayPoints.Add(_gameController.CellToGrid(cell));
        }

        return wayPoints;
    }

    private void OnDrawGizmos()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();

        Gizmos.color = Color.red;
        var start = _gameController.WorldToGrid(transform.position);
        foreach (var wayPoint in ConvertCellsToWayPoints())
        {
            Gizmos.DrawLine(start, wayPoint);
            start = wayPoint;
        }
    }
}
