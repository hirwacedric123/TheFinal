using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Health and Damage")]
    public float enemyHealth = 120f;
    public float presentHealth;
    public float giveDamage = 5f;
    public float enemySpeed;

    [Header("Enemy Things")]
    public NavMeshAgent enemyAgent;
    public Transform LookPoint;
    public Transform playerBody;
    public LayerMask playerLayer;

    [Header("Enemy Shooting var")]
    public float timeBtwShoot;
    bool previouslyShoot;

    [Header("Enemy States")]
    public float visionRadius;
    public float shootingRadius;
    public bool playerInvisionRadius;
    public bool playerInshootingRadius;
    public bool isPlayer = false;
    public GameObject goreEffect;

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        presentHealth = enemyHealth;
    }

    private void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, playerLayer);
        playerInshootingRadius = Physics.CheckSphere(transform.position, shootingRadius, playerLayer);

        if (playerInvisionRadius && !playerInshootingRadius)
        {
            Pursueplayer();
        }
        else if (playerInvisionRadius && playerInshootingRadius)
        {
            ShootPlayer();
        }
    }

    private void Pursueplayer()
    {
        if (enemyAgent.destination != playerBody.position)
        {
            enemyAgent.SetDestination(playerBody.position);
        }
        else
        {
            enemyAgent.ResetPath();
        }
    }

    private void ShootPlayer()
    {
        enemyAgent.ResetPath();
        transform.LookAt(LookPoint);
        if (!previouslyShoot)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, LookPoint.forward, out hit, shootingRadius))
            {
                Debug.Log("Shooting " + hit.transform.name);
                PlayerScripts playerBody = hit.transform.GetComponent<PlayerScripts>();
                if (playerBody != null)
                {
                    playerBody.playerHitDamage(giveDamage);
                }
            }
        }
        previouslyShoot = true;
        Invoke(nameof(ActiveShooting), timeBtwShoot);
    }

    private void ActiveShooting()
    {
        previouslyShoot = false;
    }

    public void enemyHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;
        if (presentHealth <= 0)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        enemyAgent.SetDestination(transform.position);
        enemySpeed = 0f;
        shootingRadius = 0f;
        visionRadius = 0f;
        playerInvisionRadius = false;
        playerInshootingRadius = false;
    }
}