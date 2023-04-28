using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [Header("Rifle")]
    public Camera cam;
    public float giveDamage = 10f;
    public float shootRange = 100f;
    public float fireCharge = 15f;
    public PlayerScripts player;
    public Animator animator;

    [Header("Rifle Ammunition and Shooting")]
    private float nextTimeToShoot = 0f;
    public int maxAmmunition = 20;
    public int mag = 15;
    public int presentAmmunition;
    public float reloadingTime = 1.3f;
    private bool setReloading = false;
    [Header("Rifle Effects")]
    public ParticleSystem muzzleSpark;
    public GameObject woodedEffect;
    public GameObject goreEffect;
    private void Awake()
    {
        presentAmmunition = maxAmmunition;
    }

    private void Update()
    {
        if (setReloading)
            return;
        if (presentAmmunition <= 0)
        {
            StartCoroutine(Reload());
            return;

        }


        if (Input.GetButton("Fire1")&&Time.time>=nextTimeToShoot)
        {
            animator.SetBool("Fire", true);
            animator.SetBool("Idle", false);

            nextTimeToShoot = Time.time + 1f / fireCharge;
            Shoot();
            Reload();
        }
        else if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            animator.SetBool("Idle", false);
           
            animator.SetBool("FireWalk", true);
          

        }
        else if (Input.GetButton("Fire1")&& Input.GetButton("Fire2"))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("IdleAim", true);
            animator.SetBool("FireWalk", true);
            animator.SetBool("Walk", true);
            animator.SetBool("Reloading", false);

        }
        else
        {
            animator.SetBool("Fire", false);
            animator.SetBool("Idle", true);
            animator.SetBool("FireWalk", false);
          
        }
    }
    void Shoot()
    {
        if (mag == 0)
        {
            return;
        }
        presentAmmunition--;
        if (presentAmmunition == 0)
        {
            mag--;
        }
        muzzleSpark.Play();
        RaycastHit hitInfo;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootRange))
        {
            Debug.Log(hitInfo.transform.name);
            Objects objects = hitInfo.transform.GetComponent<Objects>();
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();

            if (objects != null)
            {
                objects.ObjectHitDamage(giveDamage);
                GameObject WoodGo = Instantiate(woodedEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(WoodGo, 1f);
            }
            else if (enemy != null)
            {
                enemy.enemyHitDamage(giveDamage);
                GameObject goreGo = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreGo, 1f);
            }
        }
    }
    IEnumerator Reload()
    {
        player.playerSpeed = 0;
        player.playerSprint = 0;
        setReloading = true;
        Debug.Log("Reloading...");
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadingTime);
        animator.SetBool("Reloading", false);
        presentAmmunition = maxAmmunition;
        player.playerSpeed = 1.9f;
        player.playerSprint = 3f;
        setReloading = false;
    }

 
}
