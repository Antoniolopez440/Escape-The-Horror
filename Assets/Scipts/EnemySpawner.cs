using System.Threading;
using UnityEngine;


//Handles enemy spawning at random spawn points with a maximum limit and delay.
//
public class EnemySpawner : MonoBehaviour
{

    [Header("References")]
    //Enemy prefab to spawn
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]

    //Possible delay between spawns
    [SerializeField] private float spwanDelay = 2.0f;
    [SerializeField] private int maxEnemies = 10;

    private float timer;

    // Reset is called when the script is first attached or reset in the inspector
    private void Reset()
    {
        spwanDelay = 2.0f;
        maxEnemies = 10;
    }


    //It controls the spawn timing and checks the game state to pause spawning when necessary.
    private void Update()
    {
        if (gameManager.instance != null && gameManager.instance.isPaused)
            return;

        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= spwanDelay)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }


    //It ensures that the number of enemies does not exceed the maximum limit before spawning a new enemy at a random spawn point.
    private void SpawnEnemy()
    {
        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemyCount >= maxEnemies)
            return;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

}
