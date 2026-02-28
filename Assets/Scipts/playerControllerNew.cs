
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField] LayerMask aimMask;
    [SerializeField] GameObject gunModel;
    //[SerializeField] Transform gunOffset;
    //[SerializeField] Transform muzzlePoint;
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

    [Header("Extrta Health")]
    [SerializeField] private TMP_Text healPopupText;
    [SerializeField] private bool showPlusSign = true;

    private int overFlowHP = 0;
    private Coroutine healPopupRoutine;


    [SerializeField] public gunDisplayManagerUI gunUI;

    public List<ProjectileGun> GetGunList() => gunList;
    public int GetCurrentGunIndex() => gunListPos;

    [Header("----- Car Parts -----")]
    [SerializeField] List<CarPart> carParts = new List<CarPart>();

    public IReadOnlyList<CarPart> GetCarParts() => carParts;

    bool shooting;
    bool readyToShoot;
    bool reloading;

    [Header("----- Audio -----")]
    [Header("Footsteps by Zone")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audStepMansion;
    [SerializeField] AudioClip[] audStepGrass;

    [Range(0.4f, 10f)][SerializeField] float audStepVol;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip hurtSound;
    [Range(1f, 10f)]
    [SerializeField] float jumpSoundVol;
    [SerializeField] AudioClip shootSound;
    [Range(1f, 10f)]
    [SerializeField] float shootSoundVol = 0.8f;
    [SerializeField] AudioClip[] gunSwitchSounds;
    [Range(0.4f, 10f)][SerializeField] float gunSwitchVol;



    bool isSprinting;
    bool isPlayingSteps;
    


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

    public bool InMansion { get; private set; }
    public bool InFenceYard { get; private set; }

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

        TryInteract();

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

            if (moveDir.normalized.magnitude > 0.3f && !isPlayingSteps)
            {
                StartCoroutine(playStep());
            }
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
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator playStep()
    {
        isPlayingSteps = true;

        if (InMansion)
        {
            if (audStepMansion != null && audStepMansion.Length > 0)
                aud.PlayOneShot(audStepMansion[Random.Range(0, audStepMansion.Length)], audStepVol);
        } else
        {
            if (audStepGrass != null && audStepGrass.Length > 0)
                aud.PlayOneShot(audStepGrass[Random.Range(0, audStepGrass.Length)], audStepVol);
        }

        float delay = isSprinting ? 0.3f : 0.5f;
        yield return new WaitForSeconds(delay);

        isPlayingSteps = false;

    }


    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;

            if (jumpSound != null)
                aud.PlayOneShot(jumpSound, jumpSoundVol);
        }
    }

    public void takeDamage(float amount)
    {
        int dmg = (int)amount;

        // NEW: overflow absorbs damage first
        if (overFlowHP > 0 && dmg > 0)
        {
            int used = Mathf.Min(overFlowHP, dmg);
            overFlowHP -= used;
            dmg -= used;
            RefreshHealPopup();
        }


        if (dmg <= 0)
        {
            return;
        }

        HP -= dmg;

        if (hurtSound != null)
            aud.PlayOneShot(hurtSound, 1f);

        updateplayerUI();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public void Heal(int amount)
    {
        // NEW: fill real HP first up to HPOrig
        int need = HPOrig - HP;

        if (need > 0)
        {
            int toReal = Mathf.Min(need, amount);
            HP += toReal;
            amount -= toReal;
        }

        // NEW: anything leftover becomes overflow
        if (amount > 0)
        {
            overFlowHP += amount;
        }

        // Keep your existing UI update call(s)
        updateplayerUI();
        RefreshHealPopup();
    }

    private void RefreshHealPopup()
    {
        if (healPopupText == null) return;

        if (overFlowHP > 0)
        {
            healPopupText.text = "+" + overFlowHP.ToString();
            healPopupText.gameObject.SetActive(true);

            if (healPopupRoutine != null)
            
                StopCoroutine(healPopupRoutine);

            healPopupRoutine = StartCoroutine(HideHealPopup());

        }
        else
        {
            healPopupText.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideHealPopup()
    {
        yield return new WaitForSeconds(1.0f);
        if (overFlowHP <= 0 && healPopupText != null)
            healPopupText.gameObject.SetActive(false);
    }



    private void MyInput()
    {
        if (gameManager.instance != null && gameManager.instance.isPaused)
        {
            shooting = false;
            CancelInvoke("Shoot");
            return;
        }
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
        //muzzlePoint != null ? muzzlePoint :
        Transform spawn = attackPoint;

        ProjectileGun gun = gunList[gunListPos];

        if (currentAmmo <= 0)
            return;

        if (shootSound != null)
            aud.PlayOneShot(shootSound, shootSoundVol);



        readyToShoot = false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); ;
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, aimMask)) {
            targetPoint = hit.point;
        } else { 
            targetPoint = ray.GetPoint(75f);
        }

        Vector3 directionWithSpread = (targetPoint - spawn.position).normalized;

        float x = Random.Range(-gun.spread, gun.spread);
        float y = Random.Range(-gun.spread, gun.spread);
        Quaternion spreadRot = Quaternion.Euler(y, x, 0f);
        directionWithSpread = (spreadRot * directionWithSpread).normalized;
        

        Quaternion rot = Quaternion.LookRotation(directionWithSpread);
        GameObject currentBullet = Instantiate(gun.bullet, spawn.position, rot);

        // for normal bullet
        //currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //if (gunList[gunListPos].muzzleFlash != null)
        //   Instantiate(gunList[gunListPos].muzzleFlash, gunList[gunListPos].attackPoint.position, Quaternion.identity);

        Rigidbody rb = currentBullet.GetComponent<Rigidbody> ();
        if (rb != null)
        {
            rb.linearVelocity = (directionWithSpread * shootForce);

            if (upwardForce != 0f)
            {
                rb.linearVelocity += playerCamera.transform.up * upwardForce;
            }
        }
        currentAmmo--;
        gun.bulletsLeft = currentAmmo;
        bulletsShot++;
        gunUI.RefreshAmmo();


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

        gunUI.RefreshAmmo();
    }



    public void getGunStats(ProjectileGun gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

       

        changeGun();

        gunUI.BuildUI();
        gunUI.RefreshSelection();
        gunUI.RefreshAmmo();

    }

    void changeGun()
    { 
        if (!HasValidGun()) return;

        if (aud != null && gunSwitchSounds.Length > 0)
            aud.PlayOneShot(gunSwitchSounds[Random.Range(0, gunSwitchSounds.Length)], gunSwitchVol);

        ProjectileGun gun = gunList[gunListPos];
        currentAmmo = gun.bulletsLeft;
        if (currentAmmo > gun.magazineSize) currentAmmo = gun.bulletsLeft;
        if (currentAmmo < 0) currentAmmo = 0;

        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        timeBetweenShooting = gun.timeBetweenShooting; ;

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

        //gunOffset.localPosition = gun.viewPosOffset;
        //gunOffset.localEulerAngles = gun.viewRotOffset;

        //muzzlePoint.localPosition = gun.muzzleLocalPos;
        //muzzlePoint.localEulerAngles = gun.muzzleLocalRot;
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

        gunUI.RefreshSelection(); 
        gunUI.RefreshAmmo(); 
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

    public void GetCarPart(CarPart part)
    {
        carParts.Add(part);
        Debug.Log($"Picked up art part: {part.partType}");

        gameManager.instance.carPartsUI.Refresh(carParts);
    }

    public void TryInteract()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;


        if (carParts == null || carParts.Count == 0)
        {
            Debug.Log("TryInteract called with empty inventory");
            return;
        }

      

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (!Physics.Raycast(ray, out RaycastHit hit, 3f)) 
            return;

        CarRepair car = hit.collider.GetComponentInParent<CarRepair>();
        if (car == null)
            return;


        int index = carParts.Count - 1;
        CarPart part = carParts[index];

        if(!car.TryInstallPart(part))
        {
            Debug.Log($"Cannot install{part.partType} yet");
        }

        carParts.RemoveAt(index);

        gameManager.instance.carPartsUI.Refresh(carParts);

        Debug.Log($"Installed {part.partType}");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MansionZone"))
        {
            InMansion = true;
        }
        if (other.CompareTag("FenceYardZone"))
        {
            InFenceYard = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MansionZone"))
        {
            InMansion = true;
        }
        if (other.CompareTag("FenceYardZone"))
        {
            InFenceYard = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MansionZone"))
        {
            InMansion = false;
        }
        if (other.CompareTag("FenceYardZone"))
        {
            InFenceYard = false;
        }
    }
}
