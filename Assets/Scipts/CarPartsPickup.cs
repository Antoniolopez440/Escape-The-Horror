using UnityEngine;

public class CarPartsPickup : MonoBehaviour
{
    public CarPart part;

    public void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();
        if(pickup != null)
        {
            pickup.GetCarPart(part);

            if(gameManager.instance != null)
            {
                gameManager.instance.OnCarPartPicked(part.partType);
            }
            Destroy(gameObject);
        }
    }

}
