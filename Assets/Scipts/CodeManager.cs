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


    public string CurrentBuiltCode
    {
        get
        { if (!string.IsNullOrEmpty(overrideCode))
                return overrideCode;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < foundOrder.Count; i++)
                sb.Append(foundOrder[i]);

            return sb.ToString();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
    }

    public void CollectNumber(int number)
    { // HashSet prevents duplicates
        if (foundSet.Add(number))
        {
            foundOrder.Add(number); 
            //Debug.Log($"Collected NEW number: {number}. Total unique: {foundSet.Count}. Code now: {CurrentBuiltCode}");
        }
        else
        {
         //   Debug.Log($"Number {number} already collected before. Code stays: {CurrentBuiltCode}");
        }
    }


    public bool CheckCode(string input)
    {
        if (input == null) input = "";
        input = input.Trim();

        // Only allow checking once all numbers are found
        if (!AllNumbersFound) return false;

        // Compare to built/override code
        return input == CurrentBuiltCode;
    }

 }
