using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameController _gameController;

    private List<Vector3> wayPoints;
    private int _wayPointIndex;

    private Spawn enemySpawn;
    
    //sind alles eigentlich nur CONST bis auf hp, aber serialize sonst nicht
    [SerializeField] private int goldDrop;
    [SerializeField] private float baseSpeed;
    [SerializeField] private int hp;
    public int spawnCost;
    public float spawnTime;
    public int damage;

    private float currentSpeed;

    public void init(Spawn enemySpawn, List<Vector3> wayPoints)
    {
        this.enemySpawn = enemySpawn;
        this.wayPoints = wayPoints;
    }

    private void Awake()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        currentSpeed = baseSpeed;
    }
    private void Update()
    {
        transform.up = wayPoints[_wayPointIndex] - transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position, wayPoints[_wayPointIndex], currentSpeed * Time.deltaTime);
        
        if(transform.position == wayPoints[_wayPointIndex]) _wayPointIndex++;
        if (_wayPointIndex >= wayPoints.Count)
        {
            _gameController.TakeHit(damage);
            OnDeath();
        }
    }

    public void Accelerate(float percentage)
    {
        currentSpeed += percentage * 0.01f * baseSpeed;
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
