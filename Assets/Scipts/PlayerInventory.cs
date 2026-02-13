using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
   private Hash<string> keys = new Hash<string>();

    public void AddKey(string keyId)
    {
        if (!string.IsNullOrEmpty(keyId))
            keys.Add(keyId);
    }


    public bool HasKey(string keyId)
    {
        return !string.IsNullOrEmpty(keyId) && keys.Contains(keyId);
    }
}
