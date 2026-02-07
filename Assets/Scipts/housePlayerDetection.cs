using UnityEngine;

public class housePlayerDetection : MonoBehaviour
{
    [SerializeField] private spawner sp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            sp.SetPlayerInHouse(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            sp.SetPlayerInHouse(false);
    }
}
