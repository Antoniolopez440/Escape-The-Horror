using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float spwanDelay = 2.0f;
    [SerializeField] private int maxEnemies = 10;

    private float timer;

    private void Reset()
    {
        spwanDelay = 2.0f;
        maxEnemies = 10;
    }

    private void Update()
    {
        if (gameManager.instance != null && gameManager.instance.isPaused)
            return;

        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;
    }
}
