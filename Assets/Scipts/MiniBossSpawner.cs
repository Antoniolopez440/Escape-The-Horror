using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MiniBossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Transform[] spawnPoints;


    private GameObject bossInstance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SpawnBoss()
    {
        // If a boss already exists, don't spawn another
        if (bossInstance != null) return;

        if (objectToSpawn == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
        bossInstance = Instantiate(objectToSpawn, p.position, p.rotation);
    }
}
