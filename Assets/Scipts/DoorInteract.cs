using UnityEngine;
using System.Collections;


public class DoorInteract : MonoBehaviour
{

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
    [SerializeField] bool startsLocked = true;


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

        unlocked = !startsLocked;
    }



    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;
        if (isMoving) return;

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }


    void TryInteract()
    {
        // If locked, don't open. Instead: show message / open code panel.
        if (!unlocked)
        {
            // If they haven't found numbers yet
            if (CodeManager.Instance == null || !CodeManager.Instance.AllNumbersFound)
            {
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowMessage("Door is locked. Find the numbers.");
                return;
            }

            // They found all numbers -> open code panel
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenCodePanel(OnCorrectCodeEntered, "Enter Code:");
            }
            else
            {
                Debug.LogWarning("UIManager.Instance is null. Add UIManager to the scene.");
            }

            return;
        }

        // Unlocked -> normal door behavior
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (UIManager.Instance != null)
            {
                if (unlocked)
                    UIManager.Instance.ShowHint("Press E to open");
                else if (CodeManager.Instance != null && CodeManager.Instance.AllNumbersFound)
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
}

