using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string keyId = "DoubleDoorKey"; // Unique ID for this key
    [SerializeField] private string pickupMessage = "You picked up a key!"; // Message to show on pickup


    private bool OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

    if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddKey(keyId);
            if (UIManager.Instance != null)
                UIManager.Instance.ShowMessage(pickupMessage);

            if (destroyOnPickup)
                Destroy(gameObject); // Destroy the pickup object
        }
        else
        {
            Debug.LogWarning("PlayerInventory.Instance is null. Add PlayerInventory to the scene.");
        }
      return true;

    }

}
