using UnityEngine;

public class GateBoardLock : MonoBehaviour
{

    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private string requiredKeyId = "Crowbar";

    [Header("Planks to rremove (IN Order)")]
    [SerializeField] private GameObject[] planks;

    [Header("Gate Door Interaction")]
    [SerializeField] private DoorInteract LeftGateDoor;
    [SerializeField] private DoorInteract RightGateDoor;

    [Header("Message")]
    [SerializeField] private string noToolMessage = "Planks are in the way. Crowbar Needed";
    [SerializeField] private string removePlanksMessage = "Press E to remove the planks.";
    [SerializeField] private string gateOpenMessage = "The gate is now open.";

    private bool playerInRange = false;
    private int plankIndex = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGateLocked(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactionKey))
        {
            bool hasTool = PlayerInventory.Instance != null &&
                PlayerInventory.Instance.HasKey(requiredKeyId);

            if (!hasTool)
            {
                UIManager.Instance.ShowMessage(noToolMessage);
                return;
            }

            if (planks != null && plankIndex < planks.Length)
            {
                if (planks[plankIndex] != null)
                {
                    planks[plankIndex].SetActive(false);
                    plankIndex++;
                    if (plankIndex < planks.Length)
                    {
                        UIManager.Instance.ShowMessage(removePlanksMessage);
                        return;

                    }

                    UIManager.Instance.ShowMessage("Planks Removed");
                    SetGateLocked(false);
                    return;


                }

                UIManager.Instance.ShowMessage(gateOpenMessage);
                if (LeftGateDoor != null)
                    LeftGateDoor.ToggleFromController();

                if (RightGateDoor != null)
                    RightGateDoor.ToggleFromController();
            }
        }
    }

    private void SetGateLocked(bool locked)
    {
        if (LeftGateDoor != null)
            LeftGateDoor.SetLocked(locked);
        if (RightGateDoor != null)
            RightGateDoor.SetLocked(locked);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
            playerInRange = true;

        if (UIManager.Instance != null)
        {
            if (planks != null && plankIndex < planks.Length)
                UIManager.Instance.ShowHint("Press E");
            else
                UIManager.Instance.ShowHint("Press E to Open");
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        playerInRange = false;
        if (UIManager.Instance != null)
            UIManager.Instance.HideHint();
    }
}
