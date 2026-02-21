using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MiniBossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Drop onDeath")]
    [SerializeField] private GameObject dropItem;
    [SerializeField] private Vector3 dropOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private bool snapToGround = true;
    [SerializeField] private float groundCheckDistance = 10f;


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

        MiniBossLink link = bossInstance.AddComponent<MiniBossLink>();
        if (link == null) link = bossInstance.AddComponent<MiniBossLink>();
        link.Init(this);
    }

    public void OnBossRemoved(Vector3 cachedDeathposition)
    {
        // Drop item
        if (dropItem != null)
        {
            Vector3 spawnPos = cachedDeathposition + dropOffset;

            if (snapToGround)
            {
                if (Physics.Raycast(spawnPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, groundCheckDistance))
                {
                    spawnPos = hit.point + dropOffset;
                }
            }
            Instantiate(dropItem, spawnPos, Quaternion.identity);
        }

        bossInstance = null;
    }
}

public class MiniBossLink : MonoBehaviour
{
    private MiniBossSpawner spawner;
    private bool alreadyNotified;
    private bool quitting;

    private Vector3 lastWorldPos;

    public void Init(MiniBossSpawner s)
    {
        spawner = s;
        alreadyNotified = false;
        quitting = false;
        lastWorldPos = transform.position;
    }

    private void Update()
    {
            lastWorldPos = transform.position;
    }

    private void OnApplicationQuit() => quitting = true;

    //
    private void Notify()
    {
        if (alreadyNotified) return;
        if (quitting) return;
        if (!Application.isPlaying) return;

        alreadyNotified = true;

        if (spawner != null)
        {
            spawner.OnBossRemoved(lastWorldPos);
        }
    }
    private void OnDestroy() => Notify();


}
