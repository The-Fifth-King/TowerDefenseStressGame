using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Spawn : MonoBehaviour
{
    private GameController _gameController;
    private LineRenderer _lineRenderer;
    
    private Transform _enemyParent;

    [SerializeField] private List<Vector3Int> cellsToTraverse;
    
    [SerializeField] private List<EnemyType> possibleEnemies;
    //wird in der ersten Wave schon erhöht
    [SerializeField] private int currentWaveDifficulty = 2;
    [SerializeField] private int difficultyScale;
    private int _enemiesCurrentlyOnTrack;

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

    public IEnumerator SpawnWave()
    {
        currentWaveDifficulty += difficultyScale;
        _enemiesCurrentlyOnTrack = 0;
        
        List<int> wave = GenerateNewWave();
        foreach (var i in wave)
        {
            yield return new WaitForSeconds(possibleEnemies[i].spawnTime);
            SpawnEnemy(possibleEnemies[i]);
        }

    }
    
    private List<int> GenerateNewWave()
    {
        //bestimmt random einen enemy type, kann dieser "gekauft" werden wird er in die Wave hinzugefügt
        List<int> wave = new List<int>();
        int tmpWaveDiff = currentWaveDifficulty;
        
        while (tmpWaveDiff > 0)
        {
            int i = Random.Range(0, possibleEnemies.Count - 1);
            int cost = possibleEnemies[i].spawnCost;
            if (cost > tmpWaveDiff)
            {
                continue;
            }

            wave.Add(i);
            tmpWaveDiff -= cost;
        }

        return wave;
    }

    public void EnemyDied()
    {
        if (--_enemiesCurrentlyOnTrack == 0)
        {
            //wave over
            //TODO
            _gameController.isEnemyPhase = false;
        }
    }

    private void SpawnEnemy(EnemyType enemyType)
    {
        _enemiesCurrentlyOnTrack++;

        var instance = Instantiate(enemyType.enemyPrefab, transform.position, Quaternion.identity, _enemyParent);
        instance.GetComponent<Enemy>().init(this, enemyType, ConvertCellsToWayPoints());
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
