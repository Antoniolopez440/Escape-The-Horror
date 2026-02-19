using UnityEngine;

public class NumberClueSpawner : MonoBehaviour
{
    
    [Header("What to spawn")]
    [SerializeField] GameObject cluePrefab;

    [Header("Where to spawn")]
    [SerializeField] Transform[] spawnPoints;

    [Header("How many")]
    [SerializeField] private int digitsToSpawn = 3;

    [Header("Spawn rules")]
    [SerializeField] private bool allowRepeats = false; // Whether the same digit can be

    private GameObject
        [] activeAtPoint; // Track the active clue at each spawn point

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cluePrefab == null)
        {
            Debug.LogWarning("[NumberClueSpawner] No clue prefab assigned.", this);
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[NumberClueSpawner] No spawn points assigned.", this);
            return;
        }

        if (!allowRepeats && digitsToSpawn > 10)
        {
            Debug.LogWarning("[NumberClueSpawner] Cannot spawn more than 10 unique digits without repeats.", this);
            return;
        }

        activeAtPoint = new GameObject[spawnPoints.Length];

        int[] digits = GenerateDigits(digitsToSpawn, allowRepeats);
    }

private int [] GenerateDigits(int count, bool allowRepeats)
    {
        int[] result = new int[count];

        if (allowRepeats)
        {
            for (int i = 0; i < count; i++)
                result[i] = Random.Range(0, 10);

                    return result;

        }

        int[] pool = new int[10];
        for (int i = 0; i < 10; i++) pool[i] = i;

        for (int i = 0; i < pool.Length; i++)
        {
         int swap = Random.Range(i, pool.Length);
            int temp = pool[i];
            pool[i] = pool[swap];
            pool[swap] = temp;
        }

        for (int i = 0; i < count; i++)
            result[i] = pool[i];

        return result;

    }
}
