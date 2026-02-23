using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string KeyId = "DoubleDoorKey"; // Unique ID for this key
    [SerializeField] private string pickupMessage = "You picked up a key!"; // Message to show on pickup
    [SerializeField] private bool destroyOnPickup = true; // Whether to destroy the pickup object after picking up

    [Header("Objective Step")]
    [SerializeField] private bool completesObjectStep = false;
    [SerializeField] private int questRequired = 1;

    private bool pickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (pickedUp) return;

        pickedUp = true;

        PlayerInventory.Instance.AddItems(KeyId);
        UIManager.Instance.ShowMessage(pickupMessage);

        if (completesObjectStep && gameManager.instance != null && gameManager.instance.CurrentQuest == questRequired)
        {
            gameManager.instance.CompleteSubObjective();
        }

        //if (destroyOnPickup)
        //    Destroy(gameObject); // Destroy the pickup object
        //}
    }
}
