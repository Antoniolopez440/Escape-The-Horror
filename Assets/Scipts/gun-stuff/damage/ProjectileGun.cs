using UnityEngine;
using TMPro;

public class ProjectileGun : MonoBehaviour
{
    public GunProfile profile;

    [SerializeField] public GameObject bullet;

   

    int bulletsLeft;
    int bulletsShot;

    bool shooting;
    bool readyToShoot;
    bool reloading;

    public Camera fpsCam;
    public Transform attackPoint;

    public TextMeshProUGUI ammunitionDisplay;

    bool allowInvoke = true;

    private void Awake()
    {
        bulletsLeft = profile.magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();

        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / profile.bulletsPerTap + "/" + profile.magazineSize / profile.bulletsPerTap);
    }

    private void MyInput()
    {
        if(profile.allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < profile.magazineSize && !reloading)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
            Reload();

        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-profile.spread, profile.spread);
        float y = Random.Range(-profile.spread, profile.spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        // for normal bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * profile.shootForce, ForceMode.Impulse);

        // for grenade bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * profile.upwardForce, ForceMode.Impulse);

        if (profile.muzzleFlash != null)
            Instantiate(profile.muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        if(allowInvoke)
        {
            Invoke("ResetShot", profile.timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < profile.bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", profile.timeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", profile.reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = profile.magazineSize;
        reloading = false;
    }
}
