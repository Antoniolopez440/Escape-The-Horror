
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int spawnRate;
    [SerializeField] int spawnDist;
    [SerializeField] private Transform[] spawnPoints;

    
    [Header("Boss Warning Shake")]
    [SerializeField] bool shakeBeforeBoss = true;
    [SerializeField] float shakeDuration = 1.0f;
    [SerializeField] float shakeMagnitude = 0.15f;
    [SerializeField] float shakeFrequency = 25f;
    [SerializeField] float bossSpawnDelay = 2.0f;

    bool isBossSpawning;
    private bool playerInHouse = true;

    private int spawnAmount;
    int spawnCount;
    float spawnTimer;

    bool startSpawning;

    private bool levelStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startSpawning = true;
    } 
    public void SetPlayerInHouse(bool value)
    {
        playerInHouse = value;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startSpawning) return;
        spawnTimer += Time.deltaTime;

        if (spawnCount < spawnAmount && spawnTimer >= spawnRate)
            spawn();

        if(spawnCount >= spawnAmount)
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
    void spawn()
    {
        spawnTimer = 0;
        spawnCount++;

        Vector3 spawnPos;
        if (playerInHouse && spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        else
        {
            Vector3 ranPos = Random.insideUnitSphere * spawnDist;
            ranPos += transform.position;

            NavMeshHit hit;
            NavMesh.SamplePosition(ranPos, out hit, spawnDist, 1);
            spawnPos = hit.position;
        }
        Quaternion spawnRot;

        if (objectToSpawn.CompareTag("Boss"))
        {
            if (isBossSpawning) return;
            isBossSpawning = true;
            StartCoroutine(SpawnBossWithWarning(spawnPos));
            return;
        } else
        {
            spawnRot = Quaternion.Euler(0f, Random.Range(0f, 300f), 0f);
            Instantiate(objectToSpawn, spawnPos, spawnRot);
        }

    }

    IEnumerator SpawnBossWithWarning(Vector3 spawnPos)
    {
        if (shakeBeforeBoss)
        {
            Camera cam = null;

            if(gameManager.instance != null &&  gameManager.instance.player != null) 
                cam = gameManager.instance.player.GetComponentInChildren<Camera>();

            if (cam != null)
                yield return StartCoroutine(CameraShake(cam.transform, shakeDuration, shakeMagnitude, shakeFrequency));
            else
                yield return new WaitForSeconds(shakeDuration);
        }
        Quaternion spawnRot = Quaternion.identity;
        if (bossSpawnDelay > 0f)
            yield return new WaitForSeconds(bossSpawnDelay);
        Transform player = gameManager.instance.player.transform;
        Vector3 direction = player.transform.position - spawnPos;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.0001f)
            spawnRot = Quaternion.LookRotation(direction);

        Instantiate(objectToSpawn, spawnPos, spawnRot);
        isBossSpawning = false;
    }
    IEnumerator CameraShake(Transform cam, float duration, float magnitude, float frequency)
    {
        Vector3 startLocalPos = cam.localPosition;
        float t = 0f;

        while(t < duration)
        {
            float x = (Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f) * 2f;

            cam.localPosition = startLocalPos + new Vector3(x, y, 0f) * magnitude;

            t += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = startLocalPos;
    }
}
