using NUnit.Framework.Interfaces;
using UnityEngine;

public class pickupGunsNew : MonoBehaviour
{
    [SerializeField] gunStatsNew gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if (pik != null)
        {
            gun.ammoCur = gun.ammoMaz;
            pik.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
