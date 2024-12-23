using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class scr_Enemy : MonoBehaviour {

    public NavMeshAgent agent;

    private Rigidbody[] ragdollRigidBodies;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public GameObject projectile;

    public float sightRange, attackRange, hearingRange;
    public bool playerInSightRange, playerInAttackRange, playerInHearingRange;

    public float health = 100f;

    public Animator enemyAnimator;

    public void Awake() {
        StartCoroutine(WaitForPlayer());
        agent = GetComponent<NavMeshAgent>();
        ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }

    private void Update() {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInHearingRange = Physics.CheckSphere(transform.position, hearingRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange && !playerInHearingRange) Patroling();
        if ((playerInSightRange || playerInHearingRange) && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling() {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && agent != null && agent.enabled) {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) {
            walkPointSet = false;
        }

        if (agent != null && agent.enabled) {
            agent.speed = 3.5f;
        }
    }

    private void SearchWalkPoint() {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) {
            walkPointSet = true;
        }
    }

    private void ChasePlayer() {
        if (agent != null && agent.enabled) {
            agent.SetDestination(player.position);
            agent.speed = 5.5f;

            if (enemyAnimator != null) {
                enemyAnimator.SetTrigger("Walk");
            }
        }
    }

    private void AttackPlayer() {
        if (agent != null && agent.enabled) {
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            if (!alreadyAttacked) {


                if (projectile != null) {
                    Rigidbody rb = Instantiate(projectile, transform.position + new Vector3(0, 1, 0), Quaternion.identity).GetComponent<Rigidbody>();
                    rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                }

                if (enemyAnimator != null) {
                    enemyAnimator.SetTrigger("Attack");
                }

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }

    private void ResetAttack() {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) {
            Die();
        }
    }

    private void Die() {
        scr_GameManager.Instance.EnemyKilled();

        if (agent != null) {
            agent.enabled = false;
        }

        if (enemyAnimator != null) {
            enemyAnimator.enabled = false;
        }

        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) {
            controller.enabled = false;
        }

        EnableRagdoll();
        Destroy(gameObject, 5f);
    }

    private void DisableRagdoll() {
        foreach (var rigidbody in ragdollRigidBodies) {
            rigidbody.isKinematic = true;
        }
    }

    private void EnableRagdoll() {
        foreach (var rigidbody in ragdollRigidBodies) {
            rigidbody.isKinematic = false;
        }
    }

    IEnumerator WaitForPlayer() {
        GameObject playerObject = null;
        while (playerObject == null) {
            playerObject = GameObject.Find("Player");
            yield return null;
        }
        player = playerObject.transform;
    }

}