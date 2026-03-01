using UnityEngine;
using System.Collections.Generic;
using System.Text;


public class CodeManager : MonoBehaviour
{

    public string CurrentCode { get; private set; } = "000";

    public static CodeManager Instance { get; private set; }

    // Keeps "unique" tracking, and also preserves order for code building.
    private HashSet<int> foundSet = new HashSet<int>();
    private List<int> foundOrder = new List<int>();

    // 0-3 digits that have been found so far.
    [Header("Expected Code (optional)")]
    [Tooltip("If empty, the code is built from found numbers in the order they were first collected.")]
    [SerializeField] private string overrideCode = ""; // e.g. "752"

    
    private int[] foundByPosition = new int[3];
    private bool[] positionCollected = new bool[3];

    public bool AllNumbersFound => positionCollected[0] && positionCollected[1] && positionCollected[2];

 


    public string CurrentBuiltCode
    {
        get
        {
            if (!string.IsNullOrEmpty(overrideCode))
                return overrideCode;

            StringBuilder sb = new StringBuilder(3);
            for (int i = 0; i < 3; i++)
            {
                if (positionCollected[i])
                    sb.Append(foundByPosition[i]);
            }
              

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

    public string GetCurrentCode()
    {
        return !string.IsNullOrEmpty(CurrentCode) ? overrideCode : CurrentCode;
    }

    public void SetCurrentCode(string code)
    {
        overrideCode = code;
        CurrentCode = code;
        Debug.Log($"CodeManager: Current code set to {overrideCode}");
    }

    public void CollectNumber(int position, int number)
    {
        int index = position - 1;

        if (index < 0 || index >= foundByPosition.Length)
            return;

        if (!positionCollected[index])
        {
            positionCollected[index] = true;
            foundByPosition[index] = number;

            // Rebuild foundOrder in correct position order
            foundSet.Add(number);

            foundOrder.Clear();

            for (int i = 0; i < foundByPosition.Length; i++)
            {
                if (positionCollected[i])
                    foundOrder.Add(foundByPosition[i]);
            }
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



    public void ResetCode()
    {
        foundSet.Clear();
        foundOrder.Clear();

        for (int i = 0; i < positionCollected.Length; i++)
            positionCollected [i] = false;

        for (int i = 0; i < foundByPosition.Length; i++)
            foundByPosition[i] = 0;
   
    }

}
