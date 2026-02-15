using UnityEngine;

public class VaseInteract : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private GameObject vaseRoot;

    private bool playerInRange;
    private bool used;

    private void Awake()
    {
        if (vaseRoot == null) vaseRoot = gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (used) return;

        if (!NoteInteract.NoteRead) return;
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;

            vaseRoot.SetActive(false);

            Debug.Log("Vase picked up after note read");
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
