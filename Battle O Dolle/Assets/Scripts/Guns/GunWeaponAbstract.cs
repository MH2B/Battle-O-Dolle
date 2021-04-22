using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(AimingDirection))]
[RequireComponent(typeof(GunPositioning))]
public class GunWeaponAbstract : WeaponAbstract
{
    private GunPositioning gunPositioning;
    private AimingDirection aimingDirection;
    
    [SerializeField] private GameObject shootingPoint;
    [SerializeField] private GameObject aimingLine;
    [SerializeField] private GameObject bulletPrefab;


    public bool isReloading = false;


    protected virtual void Awake()
    {
        gunPositioning = GetComponent<GunPositioning>();
        aimingDirection = GetComponent<AimingDirection>();
        
        WeaponType = WeaponTypes.Gun;
    }


    public override void Attack()
    {
        if (isReloading)
		{
            return;
        }

        int index = MirrorSpawner.instance.SpawnGameObjectindex(bulletPrefab, shootingPoint.transform.position, Quaternion.identity);
        StartCoroutine(AfterSpawninBullet(index, InGame.instance.waitForSpawnedObjectsTimer));
    }

    IEnumerator AfterSpawninBullet(int _index, float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
        {
            StartCoroutine(AfterSpawninBullet(_index, waitingTime));
        }
        if (MirrorSpawner.instance.spawnedFounders[_index] == null)
        {
            print("error");
        }
        else
        {
            print("found");
        }
        GameObject bullet = MirrorSpawner.instance.spawnedFounders[_index];
        bullet.GetComponent<Bullet>().Damage = (int)Damage;

        Vector2 aimDirection = aimingDirection.AimDirection;
        aimDirection *= -1;

        if (transform.parent.localScale.x < 0)
		{
            aimDirection *= -1;
        }

        aimDirection = aimDirection.normalized;

        var bulletMainScript = bullet.GetComponent<Bullet>();

        bulletMainScript.movementDirection = aimDirection;
        bulletMainScript.ShooterTag = transform.parent.tag;

        if (gameObject.CompareTag("RedTeam"))
		{
            bulletMainScript.TargetTag = "BlueTeam";
        }
        else if (gameObject.CompareTag("BlueTeam"))
		{
            bulletMainScript.TargetTag = "RedTeam";
        }

        aimingDirection.enabled = false;


        aimingLine.SetActive(false);
        isReloading = true;
        gunPositioning.currentState = GunPositioning.States.Reloading;
        StartCoroutine(ReloadingCounter());
    }

    public override void Aim()
    {
        if (isReloading)
		{
            return;
        }

        aimingDirection.enabled = true;
        aimingLine.SetActive(true);
    }


	private IEnumerator ReloadingCounter()
    {
        yield return new WaitForSeconds(6f);
        isReloading = false;
        gunPositioning.currentState = GunPositioning.States.Idle;
    }
    
}
