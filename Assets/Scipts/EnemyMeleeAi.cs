using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

public class EnemyMeleeAi : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Animator animator;

    [Header("Melee")] 
    [SerializeField] Transform meleePos;
    [SerializeField] float attackRange;
    [SerializeField] float attackRate;
    [SerializeField] int attackDamage;
  //  [SerializeField] float hitRadius = 1.0f;

    [Header("Stats")]
    [SerializeField] int hp;

    Color colorOrig;
    float meleeTimer;
    bool isAttacking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;

        if (animator == null)
            animator = GetComponent<Animator>();

        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("TEST KEY PRESSED -> Calling DealMeleeDamage()");
            DealMeleeDamage();
        }
        meleeTimer += Time.deltaTime;

        agent.SetDestination(gameManager.instance.player.transform.position); // Set the destination of the NavMeshAgent to the player's position

        if (isAttacking)
            return;

        float dist = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);

        if (dist <= attackRange && meleeTimer >= attackRate)
        {
            Attack();
        }
    }

    void Attack()
    {
        meleeTimer = 0f;
        
        isAttacking = true;
        animator.SetTrigger("MeleeAttack");
        DealMeleeDamage();
    }
public void DealMeleeDamage()
    {
        float hitRadius = 2.0f;
        Collider[] hits = Physics.OverlapSphere(meleePos.position, hitRadius);
        Debug.Log("DealMeleeDamage connected!" + hits.Length);

        

        for (int i = 0; i < hits.Length; i++)
        {
            //if (hits[i].gameObject == gameObject) continue;
            if (hits[i].CompareTag("Player")) continue;

            Debug.Log("Hit: " + hits[i].name);

            IDamage damage = hits[i].GetComponentInParent<IDamage>();
            if (damage != null)
            {
                Debug.Log("Apply damage to: " + hits[i].name + " for " + attackDamage);

                damage.takeDamage(attackDamage);
                return;
            }
        }
        Debug.Log("No IDamage found in Hit Collider");
    }

    public void EndAttack()
    {
        isAttacking = false;
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