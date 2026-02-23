using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string KeyId = "DoubleDoorKey"; // Unique ID for this key
    [SerializeField] private string pickupMessage = "You picked up a key!"; // Message to show on pickup
    [SerializeField] private bool destroyOnPickup = true; // Whether to destroy the pickup object after picking up

    [Header("Objective Step")]
    [SerializeField] private bool completesObjectStep = false;
    [SerializeField] private int questRequired = 1;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField][Range(0f, 1f)] private float pickupVolume;

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
            if(questRequired == 1 && KeyId == "DoubleDoorKey")
                gameManager.instance.OnQuest1KeyFound();
            if (questRequired == 2 && KeyId == "ShedKey")
                gameManager.instance.OnQuest2ShedKeyFound();
            if (questRequired == 2 && KeyId == "CrowbarKey")
                gameManager.instance.OnQuest2CrowbarFound();
        }

        if (destroyOnPickup)
        {
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound, pickupVolume);
                
            }


            if (destroyOnPickup)
            {
                if (pickupSound != null)
                    Destroy(gameObject, pickupSound.length); // Destroy after sound finishes
                else
                    Destroy(gameObject); // Destroy immediately if no sound
            }
            
       
        }
    }
}
