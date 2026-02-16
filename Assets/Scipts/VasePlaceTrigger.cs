using UnityEngine;

public class VasePlaceTrigger : MonoBehaviour
{

    [SerializeField] private GameObject vaseToPlace;
    [SerializeField] private Transform placePoint;
    [SerializeField] private GameObject placePromptText;

    [Header("Boss Spawner")]
    [SerializeField] private MiniBossSpawner bossSpawner;


    private bool playerInRange;
    private bool used;

    private void Start()
    {
        if (bossSpawner == null) bossSpawner = FindObjectOfType<MiniBossSpawner>();

      if (vaseToPlace) vaseToPlace.SetActive(false);
      if (placePromptText) placePromptText.SetActive(false);
      used = false;
    }
    // Update is called once per frame
   private void Update()
    {
        if (used) return;
        if (placePromptText) placePromptText.SetActive(false);
        if (!playerInRange) return;
        if (!VaseInteract.HasVaseB) return;
        if (placePromptText) placePromptText.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            used = true;

            if (vaseToPlace && placePoint)
            {
                vaseToPlace.SetActive(true);
                vaseToPlace.transform.SetPositionAndRotation(placePoint.position, placePoint.rotation);

            }

            VaseInteract.HasVaseB = false;

            if (bossSpawner != null)
            {
                Debug.Log("Spawning Boss");
                bossSpawner.SpawnBoss();
            }
            else
            {
                Debug.LogError("BossSpawner is Null");
            }

            if (placePromptText) placePromptText.SetActive(false);

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            enabled = false;
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
