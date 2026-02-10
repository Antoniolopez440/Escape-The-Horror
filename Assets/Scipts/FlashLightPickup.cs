using UnityEngine;

public class FlashLightPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private string playerTag = "Player"; // Tag to identify the player
    [SerializeField] private KeyCode pickupKey = KeyCode.E; // Key to pick up the flashlight

    [Header("Flashlight Settings")]
    [SerializeField] private GameObject playerFlashlightHolder;

    private bool isPlayerInRange = false;


    private void Awake()
    {
        if (playerFlashlightHolder == null)
        playerFlashlightHolder.SetActive(false); // Ensure the flashlight is initially inactive
    }


    // Update is called once per frame
    void Update()
    {
        if (!isPlayerInRange)
            return;

        if (Input.GetKeyDown(pickupKey))
        {
            if (playerFlashlightHolder == null)
                playerFlashlightHolder.SetActive(false);

            Destroy(gameObject); // Destroy the pickup object
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
        }
    }
}
