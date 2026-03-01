using System.Collections;
using UnityEngine;

public class FlashLightPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private string playerTag = "Player"; // Tag to identify the player
    [SerializeField] private KeyCode pickupKey = KeyCode.E; // Key to pick up the flashlight

    [Header("Flashlight Settings")]
    [SerializeField] private GameObject playerFlashlightHolder;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] [Range(0f, 1f)]private float pickupVolume = 1f;

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
                gameManager.instance.OnQuest1FlashlightFound();
                PlayerInventory.Instance.SelectIndex(0); // Automatically select the flashlight after picking it up

                if (audioSource != null && pickupSound != null)
                
                    audioSource.PlayOneShot(pickupSound, pickupVolume);
                
            }

        
            if (playerFlashlightHolder != null)
                playerFlashlightHolder.SetActive(true);

               HasFlashlightt = true;

            StartCoroutine(ShowToggleHint());

            if (UIManager.Instance != null)
             

            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f); // Destroy the pickup object
        }
    }

    private IEnumerator ShowToggleHint()
    {
        if (UIManager.Instance == null) yield break;

        UIManager.Instance.ShowMessage("F : Toggle");
        yield return new WaitForSeconds(1f);
        UIManager.Instance.HideMessage();

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
