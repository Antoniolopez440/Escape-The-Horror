using UnityEngine;

public class VaseInteract : MonoBehaviour
{

 
    [SerializeField] private GameObject vaseRoot;
    [SerializeField] private GameObject FlowerInteractText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField][Range(0f, 1f)] private float pickupVolume;

    private bool playerInRange;
    private bool used;

    public static bool HasVaseB = false;

    private void Awake()
    {
        if (vaseRoot == null) vaseRoot = gameObject;
        
    }

    private void Start()
    {
        if (vaseRoot) vaseRoot.SetActive(true);

        if (FlowerInteractText) FlowerInteractText.SetActive(false);

        used = false;
        HasVaseB = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (used) return;

        // Always hide by default, then only show when valid
        if (FlowerInteractText) FlowerInteractText.SetActive(false);

        // Locked until note is read
        if (!NoteInteract.NoteRead) return;

        // Must be inside trigger
        if (!playerInRange) return;

        // Now we can show the prompt
        if (FlowerInteractText) FlowerInteractText.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;
           
            HasVaseB = true;

            if (audioSource != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);

            if (PlayerInventory. Instance != null)
                PlayerInventory.Instance.AddItems("VaseB");

            // Hide first so it never flashes after pickup
            if (FlowerInteractText) FlowerInteractText.SetActive(false);
            if (vaseRoot) vaseRoot.SetActive(false);


            // Disable this script so it can't re-enable UI next frame
            enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
            {
                if (other.CompareTag("Player"))
                {
                    playerInRange = true;


                }
            }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
