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

    private void Awake()
    {
        cachedRenderer = GetComponentInChildren<Renderer>();
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
