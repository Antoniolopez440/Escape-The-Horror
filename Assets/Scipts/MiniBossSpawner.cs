using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MiniBossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private float spawnRate;
    [SerializeField] private int spawnDist;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int startAmount = 1;



    private int spawnAmount;
    private int spawnCount;
    private float spawnTimer;

    private bool startSpawning = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startSpawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startSpawning) return;
        spawnTimer += Time.deltaTime;

        if (spawnCount < spawnAmount && spawnTimer >= spawnRate)
        {
            spawnOne();
        }
       

        if (spawnCount >= spawnAmount)
        {
            startSpawning = false;
        }
    }

    public void StartLevel(int amount)
    {
        spawnAmount = amount;
        spawnCount = 0;
        spawnTimer = 0f;

        startSpawning = true;

    }
    void spawnOne()
    {
        spawnTimer = 0f;
        spawnCount++;

        if (objectToSpawn == null) return;

        Vector3 spawnPos;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        else
        {
            Vector3 ranPos = Random.insideUnitSphere * spawnDist + transform.position;
          
          if (NavMesh.SamplePosition(ranPos, out NavMeshHit hit, spawnDist, NavMesh.AllAreas))
            spawnPos = hit.position;
            else
                spawnPos = transform.position;
        }

        Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
    }
}
