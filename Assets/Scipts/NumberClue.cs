using UnityEngine;

public class NumberClue : MonoBehaviour
{

    [Header("Clue Settings")]
    public int clueValue; // The number that represents the clue

    [Header("Clue Display")]
    public string interactMessagePrefix = "number: "; // The message shown when the player can interact with the clue

    [Tooltip("The message shown when the player interacts with the clue")]
    public bool requireRendererVisible = true; // Whether the clue can only be interacted with when its renderer is visible

    private bool collectedOnce = false; // Whether the clue has been collected at least once
    private bool playerInRange = false; // Whether the player is currently in range to interact with the clue

    private Renderer cachedRenderer; // Cached reference to the Renderer component



    // Start is called before the first frame update
    private void Awake()
    {
        cachedRenderer = GetComponentInChildren<Renderer>();
        
    }
   

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange)
            return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            Reveal();
        }
    }

    private void Reveal()
    {
        if (requireRendererVisible && cachedRenderer != null && !cachedRenderer.enabled)
        {
            UIManager.Instance.ShowMessage("It’s too dark to read..."); return;
        }

          // Always show the message when interacted (even after collected)
            UIManager.Instance.ShowMessage(interactMessagePrefix + numberValue);

        // Only count it once toward the code
        
        if (!collectedOnce)
        {
            collectedOnce = true;
            if (CodeManager.Instance != null)
                CodeManager.Instance.CollectNumber(numberValue); }
    }
}
