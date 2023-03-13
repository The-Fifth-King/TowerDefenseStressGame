using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameController _gameController;
    
    [SerializeField, Min(0)] private float speed;
    
    private List<Vector3> wayPoints;
    private int _wayPointIndex;

    private Spawn enemySpawn;
    private EnemyType enemyType;

    public void init(Spawn enemySpawn, EnemyType enemyType, List<Vector3> wayPoints)
    {
        this.enemySpawn = enemySpawn;
        this.enemyType = enemyType;
        this.wayPoints = wayPoints;
    }

    private void Awake()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, wayPoints[_wayPointIndex], speed * Time.deltaTime);
        
        if(transform.position == wayPoints[_wayPointIndex]) _wayPointIndex++;
        if (_wayPointIndex >= wayPoints.Count)
        {
            _gameController.TakeHit();
            OnDeath();
        }
    }

    public void gotKilled()
    {
        _gameController.increaseMoney(enemyType.goldDrop);
        OnDeath();
    }

    private void OnDeath()
    {
        enemySpawn.EnemyDied();
        Destroy(gameObject);
    }
}
