using UnityEngine;

public class VasePlaceTrigger : MonoBehaviour
{

    [SerializeField] private GameObject vaseToPlace;
    [SerializeField] private Transform placePoint;
    [SerializeField] private Transform player;
    [SerializeField] private float activationRange = 3f;

    [Header("Boss Spawner")]
    [SerializeField] private MiniBossSpawner bossSpawner;


    private bool playerInRange;
    private bool placed;

    private void Start()
    {
        if (vaseToPlace != null)
            vaseToPlace.SetActive(false);

        if (placePoint == null)
            placePoint = transform;
     

    }
    // Update is called once per frame
    void Update()
    {
        if (placed) return;
        if (!playerInRange) return;
        if (!VaseInteract.HasVaseB) return;

        if (player == null) return;
        if (Vector3.Distance(player.position, transform.position) > activationRange) return;
      

        if (Input.GetKeyDown(KeyCode.E))
        {
            placed = true;

            if (vaseToPlace != null && placePoint != null)
            {
                vaseToPlace.SetActive(true);
                vaseToPlace.transform.SetPositionAndRotation(placePoint.position, placePoint.rotation);

            }

            VaseInteract.HasVaseB = false;

            if (bossSpawner != null)
                bossSpawner.StartLevel(1);
            

            gameObject.SetActive(false);
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
