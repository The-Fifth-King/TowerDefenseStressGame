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
    
    [SerializeField] private List<Enemy> possibleEnemies;
    //wird in der ersten Wave schon erhöht
    [SerializeField] private int currentWaveDifficulty = 2;
    [SerializeField] private int difficultyScale;
    private int _enemiesCurrentlyOnTrack;
    private List<int> wave = new List<int>();

    private void Awake()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        _lineRenderer = GetComponent<LineRenderer>();
        var wayPoints = ConvertCellsToWayPoints();
        wayPoints.ForEach(x => _gameController.PlaceGameFieldBlockage(x));
        var wayPointsArray = wayPoints.ToArray();
        _lineRenderer.positionCount = wayPointsArray.Length;
        _lineRenderer.SetPositions(wayPointsArray);
        _lineRenderer.SetPosition(0, _gameController.WorldToGrid(transform.position));
        _gameController.PlaceGameFieldBlockage(_gameController.WorldToGrid(transform.position));
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, 
            _gameController.WorldToGrid(GameObject.FindWithTag("Finish").transform.position));
        _gameController.PlaceGameFieldBlockage(
            _gameController.WorldToGrid(GameObject.FindWithTag("Finish").transform.position));

        _enemyParent = GameObject.FindWithTag("EntityController").transform.Find("Enemies");
    }

    public IEnumerator SpawnWave()
    {
        currentWaveDifficulty += difficultyScale;
        _enemiesCurrentlyOnTrack = 0;
        
        GenerateNewWave();
        foreach (var i in wave)
        {
            yield return new WaitForSeconds(possibleEnemies[i].spawnTime);
            SpawnEnemy(possibleEnemies[i]);
        }

    }
    
    private void GenerateNewWave()
    {
        //bestimmt random einen enemy type, kann dieser "gekauft" werden wird er in die Wave hinzugefügt
        wave.Clear();
        
        int tmpWaveDiff = currentWaveDifficulty;
        
        while (tmpWaveDiff > 0)
        {
            int i = Random.Range(0, possibleEnemies.Count);
            int cost = possibleEnemies[i].spawnCost;
            //Debug.Log("i "+ i +" cost and tmpWaveDiff "+ cost + "" + tmpWaveDiff);
            if (cost > tmpWaveDiff)
            {
                continue;
            }

            wave.Add(i);
            tmpWaveDiff -= cost;
        }

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

    private void SpawnEnemy(Enemy enemy)
    {
        _enemiesCurrentlyOnTrack++;

        var instance = Instantiate(enemy, transform.position, Quaternion.identity, _enemyParent);
        instance.GetComponent<Enemy>().init(this, ConvertCellsToWayPoints());
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
