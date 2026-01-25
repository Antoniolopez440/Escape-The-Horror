using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class playerControllerNew : MonoBehaviour , IDamage , IPickup
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats -----")]
    [Range(0, 10)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(8, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;

    [Header("----- Phyisics -----")]
    [Range(15, 40)][SerializeField] int gravity;

    [Header("----- Guns -----")]
    [SerializeField] List<ProjectileGun> gunList = new List<ProjectileGun>();

    [SerializeField] GameObject gunModel;

    [SerializeField] public float shootForce;
    [SerializeField] public float upwardForce;

    [SerializeField] public float timeBetweenShooting;
    [SerializeField] public float spread;
    [SerializeField] public float reloadTime;
    [SerializeField] public float timeBetweenShots;

    [SerializeField] public int magazineSize;
    [SerializeField] public int bulletsPerTap;

    [SerializeField] public bool allowButtonHold;

    [SerializeField] int bulletsLeft;
    [SerializeField] int bulletsShot;

    bool shooting;
    bool readyToShoot;
    bool reloading;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    //[SerializeField] float shootRate;



    int jumpCount;
    int HPOrig;
    int gunListPos;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    //bool HasValidGun()
    //{
    //    return gunList.Count > 0 && gunListPos >= 0 && gunListPos < gunList.Count;
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;

    }

    // Update is called once per frame
    void Update()
    {
        
       movement();
       Sprint();

        MyInput();

      //  if (ammunitionDisplay != null)
      //      ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + "/" + magazineSize / bulletsPerTap);
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        shootTimer += Time.deltaTime;

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump(); 
        controller.Move(playerVel * Time.deltaTime);

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        //if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= timeBetweenShooting)
        //{
        //    Shoot();
        //}

        selectGun();

    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    public void takeDamage(int amount)
    {
        
    }

    //void shoot()
    //{
    //   // Debug.Log(Camera.main);
    //    shootTimer = 0;

    //    gunList[gunListPos].ammoCur--;

    //    RaycastHit hit;
    //    if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, shootDist, ~ignoreLayer))
    //    {
    //        Debug.Log(hit.collider.name);

    //      //  Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

    //        IDamage dmg = hit.collider.GetComponent<IDamage>();
    //        if (dmg != null)
    //        {
    //            dmg.takeDamage(shootDamage);
    //        }
    //    }
    //}



    private void MyInput()
    {
        if (allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = gunList[gunListPos].fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - gunList[gunListPos].attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(gunList[gunListPos].bullet, gunList[gunListPos].attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        // for normal bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        // for grenade bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(gunList[gunListPos].fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //if (gunList[gunListPos].muzzleFlash != null)
         //   Instantiate(gunList[gunListPos].muzzleFlash, gunList[gunListPos].attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        if (gunList[gunListPos].allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            gunList[gunListPos].allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
        gunList[gunListPos].allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }



    //void reload()
    //{
    //    if (Input.GetButtonDown("Reload") && gunList.Count > 0)
    //    {
    //        gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMaz;
    //    }
    //}

    public void getGunStats(ProjectileGun gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        changeGun();

    }

    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        timeBetweenShots = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
                gunListPos++;
                changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel")< 0 && gunListPos> 0) 
            {
            gunListPos--;
            changeGun();
            }
    }

}
