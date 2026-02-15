using UnityEngine;

public class VasePlaceTrigger : MonoBehaviour
{

    [SerializeField] private GameObject vaseToPlace;
    [SerializeField] private Transform placePoint;
    [SerializeField] private MiniBossSpawner bossSpawner;

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (other.CompareTag("Player")) return;

        if (!VaseInteract.VasePickedUp) return;

        used = true;

        if (vaseToPlace != null && placePoint != null)
        {
            vaseToPlace.SetActive(true);
            vaseToPlace.transform.position = placePoint.position;
            vaseToPlace.transform.rotation = placePoint.rotation;
        }

        if (bossSpawner != null)
        {
            bossSpawner.StartLevel(1);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
