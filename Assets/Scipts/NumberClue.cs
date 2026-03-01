using UnityEngine;

public class NumberClue : MonoBehaviour
{

    [Header("Clue Settings")]
    public int numberValue; // The number that represents the clue

    [Header("Clue Display")]
    public string interactMessagePrefix = "number: "; // The message shown when the player can interact with the clue

    [Header("Clue Order")]
    [Range(1, 3)] public int codePosition = 1;

    [Tooltip("The message shown when the player interacts with the clue")]
    public bool requireRendererVisible = true; // Whether the clue can only be interacted with when its renderer is visible


    [Header("Audio")]
    [SerializeField] private AudioSource pulseSource;
    [SerializeField] private AudioClip pulseClip;
    [SerializeField] private bool loopPulse = true;

    private bool collectedOnce = false; // Whether the clue has been collected at least once
    private bool playerInRange = false; // Whether the player is currently in range to interact with the clue

    private Renderer cachedRenderer; // Cached reference to the Renderer component



    // Start is called before the first frame update
    private void Awake()
    {
        cachedRenderer = GetComponentInChildren<Renderer>();

        if (pulseSource == null)
            pulseSource = GetComponent<AudioSource>();

        if (pulseSource != null)
        {
            pulseSource.loop = loopPulse;
            pulseSource.playOnAwake = false; // Don't play immediately
        }

    }

    private void Start()
    {
        if (CodeManager.Instance != null)
            return;

        string code = CodeManager.Instance.GetCurrentCode();

        if (string.IsNullOrEmpty(code)) return;

      
    }


    // Update is called once per frame
    void Update()
    {
        if (!playerInRange)
            return;
        if (requireRendererVisible && cachedRenderer != null && !cachedRenderer.enabled)
        {
            UIManager.Instance.ShowHint("Clue Nearby");
            return; 
        }

        UIManager.Instance.ShowHint("Press E to read");

        if (Input.GetKeyDown(KeyCode.E))
        {
            Reveal();
        }
    }

    private void Reveal()
    {
        if (requireRendererVisible && cachedRenderer != null && !cachedRenderer.enabled)
        {
            UIManager.Instance.ShowMessage("It’s too dark to read...");
            return;
        }

          // Always show the message when interacted (even after collected)
            UIManager.Instance.ShowMessage($"Position {codePosition}: {numberValue}");

        // Only count it once toward the code

        if (!collectedOnce)
        {
            collectedOnce = true;
            if (CodeManager.Instance != null)
                CodeManager.Instance.CollectNumber(codePosition, numberValue); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;

       
            UIManager.Instance.ShowHint("Clue Nearby");

        if (pulseSource != null && pulseClip != null)
        {
           if (pulseSource.clip != pulseClip)
                pulseSource.clip = pulseClip;
            if (!pulseSource.isPlaying)
                pulseSource.Play();
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (!other.CompareTag("Player")) return;
            playerInRange = false;

            UIManager.Instance.HideHint();

        if (pulseSource != null && pulseSource.isPlaying)
            pulseSource.Stop();

    }
}
