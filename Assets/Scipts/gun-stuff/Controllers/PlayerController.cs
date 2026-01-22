using System.Threading;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    //[SerializeField] Renderer model;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats -----")]
    [Range(1,10)] [SerializeField] int HP;
    [Range(1,10)] [SerializeField] int speed;
    [Range(2, 5)] [SerializeField] int sprintMod;
    [Range(1,3)] [SerializeField] int jumpMax;
    [Range(8,20)] [SerializeField] int jumpSpeed;

    [Header("----- Physics -----")]
    [Range(15,40)] [SerializeField] int gravity;

    [Header("----- Guns -----")]
   // [SerializeField] GameObject currentGun;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
   // [SerializeField] int magazineSize;

    [SerializeField] ProjectileGun equippedGun;

    int jumpCount;
    int HPOrig;

    float shootTimer;
  // int remaningShots;

    Vector3 moveDir;
    Vector3 playerVel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
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

        //if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        //{
        //    shoot();
        //}
        //if (Input.GetButton("Fire2"))
        //{
        //    reload();
        //}
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    void sprint()
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

    void shoot()
    {

        //shootTimer = 0;

        //RaycastHit hit;

        //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        //{
        //    Debug.Log(hit.collider.name);

        //    IDamage dmg = hit.collider.GetComponent<IDamage>();


       // if (remaningShots <= 0)
      //  {
      //      return;
      //  }
        //    if(dmg != null)
        //    {
        //        dmg.takeDamage(shootDamage);
        //    }
        //}


    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }
    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP/HPOrig;
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    //public void reload()
    //{

    //    remaningShots = magazineSize;

    //}
}
