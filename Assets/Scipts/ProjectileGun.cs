using UnityEngine;
using TMPro;


[CreateAssetMenu]


public class ProjectileGun : ScriptableObject
{
    [SerializeField] public GameObject gunModel;
    [SerializeField] public Sprite gunModelSprite;
    [SerializeField] public GameObject bullet;

    [Header("View Model Offset")]
    public Vector3 viewPosOffset;
    public Vector3 viewRotOffset;

    [Header("Muzzle Offset")]
    public Vector3 muzzleLocalPos;
    public Vector3 muzzleLocalRot;

    [Range(1, 10)] public int shootDamage;
    [Range(3, 1000)] public int shootDist;


    public float shootForce;
    public float upwardForce;

    public float timeBetweenShooting;
    public float spread;
    public float reloadTime;
    public float timeBetweenShots;

    public int magazineSize;
    public int bulletsLeft;
    public int bulletsPerTap;

    public bool allowButtonHold;

   
}
