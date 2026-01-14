using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spwanDelay = 2.0f;
    [SerializeField] private int maxEnemies = 10;

    private float timer;

    private void Reset()
    {
        spwanDelay = 2.0f;
        maxEnemies = 10;
    }
}
