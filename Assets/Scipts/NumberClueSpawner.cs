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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
