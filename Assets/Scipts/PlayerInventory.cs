using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{

    public static PlayerInventory Instance { get; private set; }

    private HashSet<string> keys = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
    }

    public void AddKey(string keyId)
    {
        if (!string.IsNullOrEmpty(keyId))
            return;
        keys.Add(keyId);
    }


    public bool HasKey(string keyId)
    {
        if (string.IsNullOrEmpty(keyId)) return false;
        return keys.Contains(keyId);
    }
}
