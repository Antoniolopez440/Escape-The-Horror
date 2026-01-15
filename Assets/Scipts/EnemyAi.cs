using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour, IDamage
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] float meleeDamage;
    [SerializeField] float attackRange;
    [SerializeField] bool playerInAttackRange;


    [SerializeField] int hp;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Animator animator;

    Color colorOrig;
    float shootTimer;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;

        gameManager.instance.updateGameGoal(1);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);


        agent.SetDestination(gameManager.instance.player.transform.position); // Set the destination of the NavMeshAgent to the player's position

        if (shootTimer >= shootRate)
        {
            shoot();
        }
        if (playerInAttackRange)
        {
            meleeAttack();
        }
    }

    void meleeAttack()
    {
        animator.SetTrigger("Attack");
        agent.SetDestination(transform.position);
    }
    void shoot()
    {
        shootTimer = 0;

        Instantiate(bullet, shootPos.position, transform.rotation); // Spawn bullet at shootPos with enemy rotation
    }
    //can be used for all game objects that take damage
    public void takeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed()); // Start the flashRed coroutine
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red; // change color to red
        yield return new WaitForSeconds(0.1f); // wait for 0.1 seconds
        model.material.color = colorOrig; // change color back to original

    }
}
