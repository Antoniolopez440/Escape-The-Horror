using UnityEngine;

public class FlashLightPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private string playerTag = "Player"; // Tag to identify the player
    [SerializeField] private KeyCode pickupKey = KeyCode.E; // Key to pick up the flashlight

    [Header("Flashlight Settings")]
    [SerializeField] private GameObject playerFlashlightHolder;

    private bool isPlayerInRange = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
