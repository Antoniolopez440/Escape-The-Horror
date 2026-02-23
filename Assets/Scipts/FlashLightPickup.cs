using UnityEngine;

public class FlashLightPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private string playerTag = "Player"; // Tag to identify the player
    [SerializeField] private KeyCode pickupKey = KeyCode.E; // Key to pick up the flashlight

    [Header("Flashlight Settings")]
    [SerializeField] private GameObject playerFlashlightHolder;

    private bool isPlayerInRange = false;
    public static bool HasFlashlightt = false;


    private void Awake()
    {

        HasFlashlightt = false;

        if (playerFlashlightHolder != null)
        playerFlashlightHolder.SetActive(false); // Ensure the flashlight is initially inactive
    }


    // Update is called once per frame
    void Update()
    {
        if (!isPlayerInRange)
            return;

        if (Input.GetKeyDown(pickupKey))
        {
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddItems("Flashlight");
                gameManager.instance.CompleteSubObjective();
                PlayerInventory.Instance.SelectIndex(0); // Automatically select the flashlight after picking it up
            }

        
            if (playerFlashlightHolder != null)
                playerFlashlightHolder.SetActive(true);

               HasFlashlightt = true;

            if (UIManager.Instance != null)
                UIManager.Instance.HideMessage();

            Destroy(gameObject); // Destroy the pickup object
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;

            if (!HasFlashlightt && UIManager.Instance != null)
                UIManager.Instance.ShowMessage("Press E to Pickup");
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;

            if (UIManager.Instance != null)
                UIManager.Instance.HideMessage();
        }
    }
}
