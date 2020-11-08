using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelManager : MonoBehaviour
{
    [Serializable]
    public struct Player
    {
        public GameObject playerPrefab;
        public Transform spawnPosition;
    }

    [Serializable]
    public class Enemy
    {
        public GameObject enemyPrefab;
        public Transform spawnPosition;
        public GameObject[] patrolPoints;
    }

    public Player player;
    public List<Enemy> enemies;

    void Start()
    {
        Instantiate(player.playerPrefab, player.spawnPosition);

        foreach(Enemy enemy in enemies)
        {
            GameObject newEnemy = Instantiate(enemy.enemyPrefab, enemy.spawnPosition);
            if(newEnemy)
            {
                EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
                enemyController.patrolPoints = enemy.patrolPoints;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(player.spawnPosition.position, "Player Spawn");

        foreach (Enemy enemy in enemies)
        {
            Handles.Label(enemy.spawnPosition.position, "Enemy Spawn");
        }
    }
#endif
}
