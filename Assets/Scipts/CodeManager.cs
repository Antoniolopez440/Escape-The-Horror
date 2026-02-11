using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class CodeManager : MonoBehaviour
{

    public static CodeManager Instance { get; private set; }

    // Keeps "unique" tracking, and also preserves order for code building.
    private HashSet<int> foundSet = new HashSet<int>();
    private List<int> foundOrder = new List<int>();

    // 0-3 digits that have been found so far.
    [Header("Expected Code (optional)")]
    [Tooltip("If empty, the code is built from found numbers in the order they were first collected.")]
    [SerializeField] private string overrideCode = ""; // e.g. "752"

    public bool AllNumbersFound => foundSet.Count >= 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
