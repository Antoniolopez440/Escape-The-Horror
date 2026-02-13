using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string keyId = "DoubleDoorKey"; // Unique ID for this key
    [SerializeField] private string pickupMessage = "You picked up a key!"; // Message to show on pickup


    private bool OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        
          var inventory = other.GetComponent<IInventory>();
            if (inventory != null)
        {
            Debug.Log($"[KeyPickup] Player entered trigger, inventory found: {inventory}");
            return;
        }

            inventory.AddKey(keyId);
            UIManager.Instance.ShowMessage(pickupMessage);

            Destroy(gameObject); // Destroy the pickup object

    }

}
