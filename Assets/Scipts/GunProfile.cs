using UnityEngine;


[CreateAssetMenu(menuName = "Weapons/Gun PRofile")]
public class GunProfile : ScriptableObject
{
    public string gunName;

    [Header("projectile")]
    public GameObject bullet;
    public float shootForce;
    public float upwardForce;

    [Header("Timing")]
    public float timeBewteenShooting;
    public float timeBetweenShoots;
    public float realoadTime;

    [Header("Spread & Fire")]
    public float spread;
    public int bulletsPerTap;
    public bool allowButtonHold;

    [Header("Ammo")]
    public int magazineSize;

    [Header("Visuals")]
    public GameObject muzzleFlash;

    //public float damage;
    //public float range;
    //public float fireRate;
   

    //public GameObject gunPrefab;

}
