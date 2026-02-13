using UnityEngine;
using System.Collections;

public class DoorCodeLock : MonoBehaviour
{
    public enum LockType { Key, Code }

    [Header("Door Parts")]
    [SerializeField] private Transform doorHinge;     // rotate THIS
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 180f;

    [Header("Lock")]
    [SerializeField] private bool unlocked = false;
    [SerializeField] private LockType lockType = LockType.Code;

    [Header("Key Lock")]
    [SerializeField] private string requiredKeyId = "FrontDoorKey";

    private bool playerInRange = false;
    private bool opening = false;
    private bool isMoving = false;
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

        if (UIManager.Instance != null && UIManager.Instance.IsCodePanelOpen)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            if (unlocked)
            {
                Toggle();
                return;
            }

            if (lockType == LockType.Key)
            {
                bool hasKey = PlayerInventory.Instance != null && PlayerInventory.Instance.HasKey(requiredKeyId);

                if (!hasKey)
                {
                    UIManager.Instance?.ShowMessage("Door is locked. You need a key.");
                    return;
                }

                unlocked = true;
                UIManager.Instance?.ShowMessage("Unlocked!");
                StartCoroutine(RotateTo(openRot));
                return;
            }

            // Code lock path
            if (CodeManager.Instance != null && CodeManager.Instance.AllNumbersFound)
            {
                UIManager.Instance?.OpenCodePanel(UnlockAndOpen, "Enter the code:");
            }
            else
            {
                UIManager.Instance?.ShowMessage("Door is locked. Find the numbers.");
            }
        }
    }

    void Toggle()
    {
        float toOpen = Quaternion.Angle(doorHinge.localRotation, openRot);
        float toClosed = Quaternion.Angle(doorHinge.localRotation, closedRot);

        Quaternion target = (toOpen < toClosed) ? closedRot : openRot;
        StartCoroutine(RotateTo(target));
    }

    IEnumerator RotateTo(Quaternion target)
    {
        isMoving = true;

        while (Quaternion.Angle(doorHinge.localRotation, target) > 0.1f)
        {
            doorHinge.localRotation = Quaternion.RotateTowards(
                doorHinge.localRotation,
                target,
                openSpeed * Time.deltaTime
            );
            yield return null;
        }

        doorHinge.localRotation = target;
        isMoving = false;
    }


    public void UnlockAndOpen()
    {
        unlocked = true;
        StartCoroutine(RotateTo(openRot));
    }



    private void OnTriggerEnter(Collider other)
    {

        Debug.Log($"[Door] Trigger ENTER by: {other.name} tage={other.name}");
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (UIManager.Instance != null)
        {
            if (unlocked)
                UIManager.Instance.ShowHint("Press E to open");
            else if (lockType == LockType.Key)
                UIManager.Instance.ShowHint("Find the key");
            else if (CodeManager.Instance != null && CodeManager.Instance.AllNumbersFound)
                UIManager.Instance.ShowHint("Press E to enter code");
            else
                UIManager.Instance.ShowHint("Find the numbers");
        }
    }

    private void OnTriggerExit(Collider other)
    {

        Debug.Log($"[Door] Trigger EXIT by: {other.name} tage={other.name}");
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        UIManager.Instance.HideHint();
    }
}
