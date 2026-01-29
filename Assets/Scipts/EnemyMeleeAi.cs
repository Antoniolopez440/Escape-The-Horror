using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class EnemyMeleeAI : MonoBehaviour, IDamage
{
    GameObject player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask groundLayer, playerLayer;

    [SerializeField] float sightRange, attackRange;
    [SerializeField] bool playerInsight, playerInAttackRange;

    [SerializeField] Renderer model;
    [SerializeField] float meleeDamage;

    [SerializeField] float hp;
    [SerializeField] Transform meleePos;
    [SerializeField] Animator animator;
    [SerializeField] int attackDamage;
    [SerializeField] float hitRadius = 1.0f;

    [Header("Stats")]
    [SerializeField] int hp;

    [SerializeField] Animator animator;

    Color colorOrig;

    [SerializeField] bool hasEmerged = false;
 
        if (model == null)
        {
            model = GetComponentInChildren<Renderer>(true);
        } else
        {
            colorOrig = model.sharedMaterial.color;
        }
            agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        {
            colorOrig = model.sharedMaterial.color;
        }
            agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");

        animator = GetComponent<Animator>();
        hasEmerged = false;
        
        if (agent != null)
        {
            agent.enabled = false;
        }

        StartCoroutine(EmergeThenEnable());
    }



    // Update is called once per frame
    void Update()
    {
        if (!hasEmerged)
        {
            return;
        }
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (agent == null) agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (agent == null || player == null) return;

        agent.SetDestination(player.transform.position);

        if (playerInsight && playerInAttackRange)
        {
            meleeAttack();
        }
    }

    void meleeAttack()
    {
        animator.SetTrigger("Attack");
        agent.SetDestination(transform.position);
    }

    public int GetV(float amount)
    {
        return (int)(hp -= amount);
    }

    //can be used for all game objects that take damage
    public void takeDamage(float amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            // Can instantiate a scriptable game object for dropping item after death
            if (dropItem != null)
                Instantiate(dropItem, transform.position, transform.rotation);

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

    IEnumerator EmergeThenEnable()
    {
        yield return new WaitForSeconds(emergetime);
        OnEmergeFinished();
    }

    public void OnEmergeFinished()
    {
        if (agent != null)
        {
            agent.enabled = true;
        }

        hasEmerged = true;
    }
}
