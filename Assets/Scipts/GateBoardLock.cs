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

    private bool isPlayerInRange = false;
    private int currentPlankIndex = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
