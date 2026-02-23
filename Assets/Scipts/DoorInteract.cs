using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;


public class DoorInteract : MonoBehaviour
{
    public enum LockType { None, Code, Key }

    [Header("Door parts")]
    [SerializeField] Transform hinge;

    [Header("Open Settings")]
    [SerializeField] float openAngle = 90f;
    [SerializeField] float speed = 6f;

    [Header("Input")]
    [SerializeField] KeyCode interactKey = KeyCode.E;

    [Header("Auto Open For Zombies")]
    [SerializeField] string zombieTag = "Enemy";

    [Header("Lock Settings")]
    [SerializeField] LockType lockType = LockType.Code;
    [SerializeField] bool startsLocked = true;

    [Header("Key Lock Settings (if using Key lock)")]
    [SerializeField] string requiredKeyId = "DoorKey"; // The name of the

    [Header("Quest Update")]
    [SerializeField] private bool adevanceQuest = false;
    [SerializeField] private int questToSet = 2;
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private int onlyRunOnQuest = 1;

    private bool questTriggered;


    // State variables
    bool playerInRange;
    bool isOpen;
    bool isMoving;
    int zombiesInRange;

    bool unlocked;


    // Rotations
    // Store the closed and open rotations
    Quaternion closedRotation;
    Quaternion openRotation;


    // Awake is called when the script instance is being loaded
    // This happens before any Start functions and also just after a prefab is instantiated
    private void Awake()
    {
        if (hinge == null) hinge = transform;

        closedRotation = hinge.rotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);

        unlocked = !startsLocked || lockType == LockType.None;
    }



    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;
        if (isMoving) return;

        if (UIManager.Instance != null && UIManager.Instance.IsCodePanelOpen)
            return; // Don't allow interaction while code panel is open

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }


    void TryInteract()
    {
        // Always allow normal open/close once unlocked
        if (unlocked)
        {
            TryAdvanceObjectiveStep();
            StartCoroutine(ToggleDoor());
            return;
        }

        // LOCKED behavior by type
        if (lockType == LockType.Key)
        {
            bool hasKey = PlayerInventory.Instance != null && PlayerInventory.Instance.HasKey(requiredKeyId);

            if (!hasKey)
            {
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowMessage("Door is locked. You need a key.");
                UIManager.Instance?.ShowMessage("Quest: Find the key.");
                return;
            }

            // Key found -> unlock and open (then free open/close forever)
            unlocked = true;
            TryAdvanceObjectiveStep();
            if (UIManager.Instance != null)
                UIManager.Instance.ShowMessage("Unlocked!");
            UIManager.Instance?.ShowMessage("Quest Updated: Escape!");
            StartCoroutine(ToggleDoor());
            return;
        }

        if (lockType == LockType.Code)
        {
            // If they haven't found numbers yet
            if (CodeManager.Instance == null || !CodeManager.Instance.AllNumbersFound)
            {
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowMessage("Door is locked. Find the numbers.");
                UIManager.Instance?.ShowMessage("Quest: Find the numbers.");
                return;
            }

            UIManager.Instance.ShowMessage("Enter the code to unlock.");

            // They found all numbers -> open code panel
            if (UIManager.Instance != null)
               
            UIManager.Instance.OpenCodePanel(OnCorrectCodeEntered, "Enter the code:");
            else
                Debug.LogWarning("UIManager.Instance is null. Add UIManager to the scene.");

            return;
        }

        // LockType.None fallback
        if (gameManager.instance != null && gameManager.instance.CurrentQuest == 1)
        {
            TryAdvanceObjectiveStep();
        }
        StartCoroutine(ToggleDoor());
    }

    void OnCorrectCodeEntered()
    {
        unlocked = true;
        StartCoroutine(ToggleDoor());
    }


    IEnumerator ToggleDoor()
    {
        isMoving = true;

        Quaternion start = hinge.localRotation;
        Quaternion targetRotation = isOpen ? closedRotation : openRotation;

        float t = 0f;
        while (t < 1f)
        {

            t += Time.deltaTime * speed;
            hinge.rotation = Quaternion.Slerp(start, targetRotation, t);
            yield return null;
        }

        hinge.rotation = targetRotation;
        isOpen = !isOpen;
        isMoving = false;
    }

    public void ToggleFromController()
    {
        if (isMoving) return;
        StartCoroutine(ToggleDoor());
    }

    public void SetLocked(bool locked)
    {
        unlocked = !locked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (UIManager.Instance != null)
            {
                if (unlocked)
                    UIManager.Instance.ShowHint("Press E to open");
                else if (lockType == LockType.Key)
                    UIManager.Instance.ShowHint("Find the key");
                else if (lockType == LockType.Code && CodeManager.Instance != null && CodeManager.Instance.AllNumbersFound)
                    UIManager.Instance.ShowHint("Press E to enter code");
                else
                    UIManager.Instance.ShowHint("Find the numbers");
            }
        }

        if (other.CompareTag(zombieTag))
        {
            zombiesInRange++;

            // Zombies can only open if unlocked (keeps your door puzzle intact)
            if (unlocked && !isOpen && !isMoving)
            {
                StartCoroutine(ToggleDoor());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (UIManager.Instance != null)
                UIManager.Instance.HideHint();
        }

        if (other.CompareTag(zombieTag))
        {
            zombiesInRange--;
            if (zombiesInRange < 0) zombiesInRange = 0;
        }
    }

    private void TryAdvanceObjectiveStep()
    {
        if (!adevanceQuest)
        {
            return;
        }
        if (triggerOnce && questTriggered)
        {
            return;
        }
        if (gameManager.instance != null && gameManager.instance.CurrentQuest == onlyRunOnQuest)
        {
            gameManager.instance.CompleteSubObjective();
            questTriggered = true;
        }
    }
}

