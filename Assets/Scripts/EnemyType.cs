using System;
using UnityEngine;

[Serializable]
public class EnemyType
{
    public GameObject enemyPrefab;
    public int spawnCost;
    public float spawnTime;
    public int goldDrop;
}