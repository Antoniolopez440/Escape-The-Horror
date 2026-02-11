using UnityEngine;

public class DoorCodeLock : MonoBehaviour
{
    [Header("Door Parts")]
    [SerializeField] private Transform doorHinge;     // rotate THIS
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 180f;

    [Header("State")]
    [SerializeField] private bool unlocked = false;

    private bool playerInRange = false;
    private bool opening = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    private void Start()
    {
        if (doorHinge == null) doorHinge = transform;

        closedRot = doorHinge.localRotation;
        openRot = Quaternion.Euler(0f, openAngle, 0f) * closedRot;
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // If already unlocked, just open
            if (unlocked)
            {
                Open();
                return;
            }

            // Not unlocked yet
            if (CodeManager.Instance != null && CodeManager.Instance.AllNumbersFound)
            {
                // 🔑 THIS IS WHAT YOU WERE MISSING
                UIManager.Instance.OpenCodePanel(this, "Enter the code:");
            }
            else
            {
                UIManager.Instance.ShowMessage("Door is locked. Find the numbers.");
            }
        }
    }


    private void TryInteract()
    {
        if (unlocked)
        {
            Open();
            return;
        }

        // Locked
        if (CodeManager.Instance != null && CodeManager.Instance.AllNumbersFound)
        {
            UIManager.Instance.ShowHint("Press E to enter code");
            UIManager.Instance.OpenCodePanel(this, "Enter Code:");
        }
        else
        {
            UIManager.Instance.ShowMessage("Door is locked. Find the numbers.");
        }
    }

    public void UnlockAndOpen()
    {
        unlocked = true;
        Open();
    }

    private void Open()
    {
        opening = true;
        UIManager.Instance.ShowMessage("Door opened.");
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log($"[Door] Trigger ENTER by: {other.name} tage={other.name}");
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (unlocked)
            UIManager.Instance.ShowHint("Press E to open");
        else if (CodeManager.Instance != null && CodeManager.Instance.AllNumbersFound)
            UIManager.Instance.ShowHint("Press E to enter code");
        else
            UIManager.Instance.ShowHint("Find the numbers");
    }

    private void OnTriggerExit(Collider other)
    {

        Debug.Log($"[Door] Trigger EXIT by: {other.name} tage={other.name}");
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        UIManager.Instance.HideHint();
    }
}
