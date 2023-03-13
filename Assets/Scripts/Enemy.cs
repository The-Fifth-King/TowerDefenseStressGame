using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Dictionary<string, EnemyType> EnemyTypes;

    private GameController _gameController;

    private List<Vector3> wayPoints;
    private int _wayPointIndex;

    private Spawn enemySpawn;
    
    [SerializeField] private int goldDrop;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int hp;
    [SerializeField] public int spawnCost;
    [SerializeField] public float spawnTime;

    public void init(Spawn enemySpawn, List<Vector3> wayPoints)
    {
        this.enemySpawn = enemySpawn;
        this.wayPoints = wayPoints;
    }

    private void Awake()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, wayPoints[_wayPointIndex], moveSpeed * Time.deltaTime);
        
        if(transform.position == wayPoints[_wayPointIndex]) _wayPointIndex++;
        if (_wayPointIndex >= wayPoints.Count)
        {
            _gameController.TakeHit();
            OnDeath();
        }
    }

    public void ReceiveDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            _gameController.increaseMoney(goldDrop);
            _gameController.increaseScore(goldDrop);
            OnDeath();
        }
    }

    private void OnDeath()
    {
        enemySpawn.EnemyDied();
        Destroy(gameObject);
    }
}
