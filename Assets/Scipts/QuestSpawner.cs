
using UnityEngine;
using UnityEngine.AI;

public class QuestSpawner : MonoBehaviour
{
    public enum SpawnMode { MansionPoints, AroundPlayer }

    [Header("Regular Zombie Prefab")]
    [SerializeField] private GameObject regularZombiePrefab;

    [Header("Mansion Spawn points")]
    [SerializeField] private Transform[] mansionSpawnpoints;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private playerControllerNew playerController;
    private int currentQuest = 1;

    [Header("Spawn Rate Per Quest")]
    [Tooltip("QuestSpawner 1: inside mansion only")]
    [SerializeField] private float quest1SpawnRate = 4.0f;

    [Tooltip("Quest 2: outside the mansion around the player")]
    [SerializeField] private float quest2SpawnRate = 2.0f;

    [Tooltip("Quest 3: past the gate around the player")]
    [SerializeField] private float quest3SpawnRate = 1.0f;

    [Header("Spawn Around Player (Outside)")]
    [SerializeField] private BoxCollider fenceZone;
    [SerializeField] private float minDist = 12f;
    [SerializeField] private float maxDist = 25f;
    [SerializeField] private int tries = 20;
    [SerializeField] private float navmeshSampleRadius = 2f;

    private float spawnRateSeconds;
    private float timer;

    private void Awake()
    {
        SetQuest(1);
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log($"[QS] Quest={currentQuest} rate={spawnRateSeconds} plyer={(player?player.name : "NULL")} pc={(playerController?playerController.name:"NULL")} InFence={playerController?.InFenceYard} InMansion={playerController?.InMansion}");
        timer += Time.deltaTime;
        if (timer >= spawnRateSeconds)
        {
            timer = 0f;
            SpawnRegular();
        }
    }

    public void SetQuest(int quest)
    {
        currentQuest = quest;

        if (quest == 1)
        {
            spawnRateSeconds = quest1SpawnRate;
        }
        else if (quest == 2)
        {
            spawnRateSeconds = quest2SpawnRate;
        }
        else
        {
            spawnRateSeconds = quest3SpawnRate;
        }

        timer = 0f;
    }

    private void SpawnRegular()
    {
        if (!TryGetSpawnPos(out Vector3 pos)) return;
        Instantiate(regularZombiePrefab, pos, Quaternion.Euler(0f, Random.Range(0f, 360), 0f));
    }

    private bool TryGetSpawnPos(out Vector3 pos)
    {
        if (playerController != null && playerController.InMansion)
        {
            if (mansionSpawnpoints == null || mansionSpawnpoints.Length == 0)
            {
                pos = Vector3.zero;
                return false;
            }

            pos = mansionSpawnpoints[Random.Range(0, mansionSpawnpoints.Length)].position;
            return true;
        }
        if (currentQuest == 1)
        {
            pos = Vector3.zero;
            return false;
        }

        if (currentQuest == 2)
        {
            if (playerController == null || !playerController.InFenceYard)
            {
                pos = Vector3.zero;
                return false;
            }
        }

        return TrySpawnAroundPlayer(out pos);
    }

    private bool TrySpawnAroundPlayer(out Vector3 pos)
    {
        for (int i = 0; i < tries; i++)
        {
            Vector2 direct = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minDist, maxDist);
            Vector3 candidate = player.position + new Vector3(direct.x, 0f, direct.y) * dist;
            if (currentQuest == 2 && fenceZone != null)
            {
                Vector3 test = candidate;
                test.y = fenceZone.bounds.center.y;
                if (!fenceZone.bounds.Contains(test))
                {
                    continue;
                }
            }
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, navmeshSampleRadius, NavMesh.AllAreas))
            {
                if (currentQuest == 2 && fenceZone != null)
                {
                    Vector3 testHit = hit.position;
                    testHit.y = fenceZone.bounds.center.y;
                    if (!fenceZone.bounds.Contains(testHit))
                    {
                        continue;
                    }
                }
                pos = hit.position;
                return true;
            }
        }

        pos = Vector3.zero;
        return false;
    }
}
