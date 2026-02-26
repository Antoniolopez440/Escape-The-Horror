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

    [SerializeField] Animator animator;
    [SerializeField] float hitStunTime = 0.35f;
    [SerializeField] float attackLockTime = 0.80f;
    [SerializeField] float attackCooldown = 1.00f;

    Color colorOrig;

    [SerializeField] bool hasEmerged = false;
    [SerializeField] float emergetime = 4.0F;
    [SerializeField] float screamTime = 1.833f;

    [SerializeField] private bool canTakeDamage = false;


    bool isDead;
    bool isBusy;
    float nextAttackTime;
    Coroutine busyRoutine;

    [SerializeField] bool dropsItem = true;
    [SerializeField] GameObject dropObject;
    [SerializeField] Transform dropPoint;
    [SerializeField] private GameObject[] attackHitboxes;
    private Collider[] attackColliders;

    Vector3 deathPos;
    Quaternion deathRot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (model == null)
        {
            model = GetComponentInChildren<Renderer>(true);
        }
        else
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

        attackColliders = new Collider[attackHitboxes.Length];
        for (int i = 0; i < attackHitboxes.Length; i++)
        {
            if (attackHitboxes[i] != null)
            {
                attackColliders[i] = attackHitboxes[i].GetComponent<Collider>();
            }
            StartCoroutine(EmergeThenEnable());
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        if (!hasEmerged)
        {
            return;
        }
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (agent == null) agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (agent == null || player == null) return;

        if (!agent.enabled || !agent.isOnNavMesh) return;

        if (isBusy)
        {
            agent.SetDestination(transform.position);
            return;
        }

        if (playerInsight)
        {
            if (playerInAttackRange)
            {
                TryAttack();
            }
            else
            {
                agent.SetDestination(player.transform.position);
            }
        }
    }

    void TryAttack()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;

        if (busyRoutine != null) StopCoroutine(busyRoutine);
        busyRoutine = StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isBusy = true;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        animator.ResetTrigger("Hit");
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackLockTime);

        if (isDead || agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            isBusy = false;
            yield break;
        }

        agent.isStopped = false;
        isBusy = false;
    }


    public int GetV(float amount)
    {
        return (int)(hp -= amount);
    }

    //can be used for all game objects that take damage
    public void takeDamage(float amount)
    {
        if (isDead) return;
        if (!canTakeDamage) return;
        hp -= amount;

        if (hp <= 0)
        {
            isDead = true;
            DisableAttackColliders();
            deathPos = (dropPoint != null) ? dropPoint.position : transform.position;

            if (animator != null)
                animator.applyRootMotion = false;

            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Attack");

            int deathIndex = Random.Range(0, 4);
            animator.SetInteger("DieIndex", deathIndex);
            animator.SetTrigger("Die");


            agent.isStopped = true;
            agent.ResetPath();
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.enabled = false;

            enabled = false;

            StartCoroutine(DieRoutine());
            return;
        }
        if (animator != null)
        {
            int hitIndex = Random.Range(0, 2);
            animator.SetInteger("Hitindex", hitIndex);
            animator.SetTrigger("Hit");
        }
        if (busyRoutine != null)
        {
            StopCoroutine(busyRoutine);
            busyRoutine = null;
        }
        isBusy = true;
        busyRoutine = StartCoroutine(HitStunRoutine());
        // Start the flashRed coroutine
        StartCoroutine(flashRed());
    }

    IEnumerator HitStunRoutine()
    {
        isBusy = true;
        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        yield return new WaitForSeconds(hitStunTime);

        if (isDead || agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            isBusy = false;
            yield break;
        }

        agent.isStopped = false;
        isBusy = false;
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red; // change color to red
        yield return new WaitForSeconds(0.1f); // wait for 0.1 seconds
        model.material.color = colorOrig; // change color back to original
    }

    IEnumerator EmergeThenEnable()
    {
        yield return new WaitForSeconds(emergetime + screamTime);
        OnEmergeFinished();
    }

    IEnumerator DieRoutine()
    {
        Debug.Log($"[DieRoutine start] enemy pos = {transform.position}");
        yield return new WaitForSeconds(2.5f);
        Debug.Log($"[before drop] enemy pos = {transform.position}");
        dropItem();

        Destroy(gameObject);
    }

    public void OnEmergeFinished()
    {
        if (agent != null)
        {
            agent.enabled = true;
        }

        hasEmerged = true;
        canTakeDamage = true;
    }

    void dropItem()
    {
        if (!dropsItem) return;
        if (!dropObject) return;

        Vector3 pos = deathPos;

        Vector3 rayStart = pos + Vector3.up * 1f;
        Debug.Log($"[DropItem] base pos = {pos} rayStart={rayStart}");
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 50f, ~0, QueryTriggerInteraction.Ignore))
        {
            Debug.Log($"[DropItem] ray git = {hit.collider.name} at {hit.point}");
            pos = hit.point + Vector3.up * 1f;
        }
        Instantiate(dropObject, pos, Quaternion.identity);
    }

    void DisableAttackColliders()
    {
        if (attackColliders == null) return;

        foreach (Collider col in attackColliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }
}