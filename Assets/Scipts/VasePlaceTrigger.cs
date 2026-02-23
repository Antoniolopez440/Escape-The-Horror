using UnityEngine;

public class VasePlaceTrigger : MonoBehaviour
{

    [SerializeField] private GameObject vaseToPlace;
    [SerializeField] private Transform placePoint;
    [SerializeField] private GameObject placePromptText;

    [Header("Mini-Boss Spawner")]
    [SerializeField] spawner bossSpawner;
    [SerializeField] int bossSpawnAmount = 1;


    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip  placeSound;
    [SerializeField][Range(0f, 1f)] private float placeVolume;

    private bool playerInRange;
    private bool used;

    private void Start()
    {
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

            if (PlayerInventory.Instance != null)
                PlayerInventory.Instance.RemoveItems("VaseB");
            

            if (bossSpawner != null)
            {
            //    Debug.Log("Spawning Boss");
                bossSpawner.StartLevel(bossSpawnAmount);
            }
            else
            {
               // Debug.LogError("BossSpawner is Null");
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
