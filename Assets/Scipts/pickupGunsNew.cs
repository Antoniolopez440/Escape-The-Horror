
using UnityEngine;

public class pickupGunsNew : MonoBehaviour
{
    [SerializeField] ProjectileGun gun;
    [Header("Objective Step")]
    [SerializeField] private bool CountsForFindGunObjective = false;
    [SerializeField] private int questRequired = 1;

    private bool consumed;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if (pik == null) return;

        consumed = true;

        gun.bulletsLeft = gun.magazineSize;
        pik.getGunStats(gun);

        if (CountsForFindGunObjective && gameManager.instance != null && gameManager.instance.CurrentQuest == questRequired)
        {
            gameManager.instance.OnQuest1GunFound(); ;
        }

        Destroy(gameObject);
        
    }
}
