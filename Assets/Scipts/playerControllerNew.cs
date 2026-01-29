using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class playerControllerNew : MonoBehaviour , IDamage , IPickup
{
    [SerializeField] int animTranSpeed;
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Animator anim;

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
    [SerializeField] int bulletsShot;


    bool shooting;
    bool readyToShoot;
    bool reloading;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    //[SerializeField] float shootRate;

//stuff 

    int jumpCount;
    int HPOrig;
    int gunListPos;
    int remainingShots;
    int currentAmmo;

    bool allowInvoke = true;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    void Start()
    {
        HPOrig = HP;
        readyToShoot = true;
        updateplayerUI();
    }

    void Update()
    {
        movement();
        Sprint();

        MyInput();
        locoAnim();

        //  if (ammunitionDisplay != null)
        //      ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + "/" + magazineSize / bulletsPerTap);
    }
    void locoAnim()
    {

        Vector3 Velo = controller.velocity;
        Velo.y = 0f;
        float SpeedCur = Mathf.Clamp01(Velo.magnitude / Mathf.Max(1, speed));
        float SpeedAnim = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.MoveTowards(SpeedAnim, SpeedCur, Time.deltaTime * animTranSpeed));
    }

    bool HasValidGun()
    {
        return gunList.Count > 0 && gunListPos >= 0 && gunListPos < gunList.Count;
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        shootTimer += Time.deltaTime;

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        jump();

        Vector3 finalMove = (moveDir * speed) + playerVel;
        controller.Move(finalMove * Time.deltaTime);

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

    public void takeDamage(float amount)
    {
        HP -= (int)amount;
        updateplayerUI();
        StartCoroutine(flashRed());

        //Check if the player is dead
        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public void Heal(int amount)
    {
        HP += amount;
        if (HP < HPOrig) HP = HPOrig;
        Debug.Log($"[Player] Healed {amount}. HP now {HP}");

        updateplayerUI();
    }



    private void MyInput()
    {
        if (!HasValidGun()) return;

        ProjectileGun gun = gunList[gunListPos];

        if (allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Manual reload only if: not reloading, mag not full, and we have reserve ammo
        if (Input.GetKeyDown(KeyCode.R) && !reloading && currentAmmo < magazineSize && remainingShots > 0)
        {
            Reload();
            return;
        }

        // Auto reload only if: trying to shoot, mag empty, and we have reserve ammo
        if (shooting && !reloading && currentAmmo <= 0 && remainingShots > 0)
        {
            Reload();
            return;
        }

        // Shoot only if we have bullets in the mag
        if (readyToShoot && shooting && !reloading && currentAmmo > 0)
        {
            bulletsShot = 0;
            Shoot();
            anim.SetTrigger("Shoot");

        }
    }

    public void RefillCurrentMagazine()
    {
        if (!HasValidGun()) return;

        ProjectileGun gun = gunList[gunListPos];

        // Fill mag
        currentAmmo = magazineSize;
        gun.bulletsLeft = currentAmmo;

        // Fill reserve to max mag size too (simple rule: reserve = another full mag)
        remainingShots = 0;

        Debug.Log($"Refilled {gun.name} to {currentAmmo}/{magazineSize} + reserve {remainingShots}");
    }


    private void Shoot()
    {
        if (!HasValidGun()) return;

        ProjectileGun gun = gunList[gunListPos];

        if (currentAmmo <= 0)
            return;
        

        Debug.Log("Shoot() called");

        readyToShoot = false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); ;
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayer))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
        Debug.Log($"[Shoot] Gun={gunList[gunListPos].name} bulletPrefab={(gunList[gunListPos].bullet ? gunList[gunListPos].bullet.name : "NULL")}");
        
        GameObject currentBullet = Instantiate(gunList[gunListPos].bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        // for normal bullet
        //currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //if (gunList[gunListPos].muzzleFlash != null)
        //   Instantiate(gunList[gunListPos].muzzleFlash, gunList[gunListPos].attackPoint.position, Quaternion.identity);

        Rigidbody rb = currentBullet.GetComponent<Rigidbody> ();
        if (rb != null)
        {
            rb.AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

            if (upwardForce != 0)
            {
                rb.AddForce(transform.up * upwardForce, ForceMode.Impulse);
            }
        }
        currentAmmo--;
        gun.bulletsLeft = currentAmmo;
        bulletsShot++;


        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && currentAmmo > 0)
            Invoke("Shoot", timeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        if (!HasValidGun()) return;

        ProjectileGun gun = gunList[gunListPos];

        int need = magazineSize - currentAmmo;
        if (need <= 0)
        {
            reloading = false;
            return;
        }

        // Take from reserve
        int take = Mathf.Min(need, remainingShots);

        // If no reserve ammo, can't reload
        if (take <= 0)
        {
            reloading = false;
            return;
        }

        currentAmmo += take;
        remainingShots -= take;

        gun.bulletsLeft = currentAmmo;

        reloading = false;
    }



    public void getGunStats(ProjectileGun gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

       

        changeGun();

    }

    void changeGun()
    { 
        if (!HasValidGun()) return;

        ProjectileGun gun = gunList[gunListPos];
        currentAmmo = gun.bulletsLeft;
        if (currentAmmo > gun.magazineSize) currentAmmo = gun.bulletsLeft;
        if (currentAmmo < 0) currentAmmo = 0;

        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        timeBetweenShooting = gun.shootRate;

        magazineSize = gun.magazineSize;
        bulletsPerTap = gun.bulletsPerTap;
        allowButtonHold = gun.allowButtonHold;

        shootForce = gun.shootForce;
        upwardForce = gun.upwardForce;
        spread = gun.spread;
        reloadTime = gun.reloadTime;
        timeBetweenShots = gun.timeBetweenShots;
       
        bulletsShot = 0;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (!HasValidGun()) return;
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunList[gunListPos].bulletsLeft = currentAmmo;
            gunListPos++;
                changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel")< 0 && gunListPos> 0) 
            {
            gunList[gunListPos].bulletsLeft = currentAmmo;
            gunListPos--;
            changeGun();
            }
    }

    public void updateplayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashRed()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

}
