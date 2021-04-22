using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vintage : WeaponAbstract
{

	[SerializeField] private GameObject bulletPrefab = null;
	[SerializeField] private Transform shootingPoint = null;

	public override void Attack()
	{
		if (isReloading)
		{
			return;
		}

		int index = MirrorSpawner.instance.SpawnGameObjectindex(bulletPrefab, shootingPoint.position, Quaternion.identity);
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

        GameObject newBullet = MirrorSpawner.instance.spawnedFounders[_index];
        Bullet _bullet = newBullet.GetComponent<Bullet>();
        _bullet.Damage = Damage;

        Vector2 aimDirection = _aimingDirection.AimDirection;
        aimDirection *= -1;

        if (transform.parent.parent.localScale.x < 0)
        {
            aimDirection *= -1;
        }

        aimDirection = aimDirection.normalized;
        _bullet.movementDirection = aimDirection;
        _bullet.ShooterTag = transform.parent.parent.tag;

        if (transform.parent.parent.CompareTag("RedTeam"))
        {
            _bullet.TargetTag = "BlueTeam";
        }
        else if (transform.parent.parent.CompareTag("BlueTeam"))
        {
            _bullet.TargetTag = "RedTeam";
        }

        _aimingDirection.enabled = false;
        aimingLine.SetActive(false);
        isReloading = true;
        UIBtns.instance.ChangeWeaponInteractability(ReloadTime);
        WeaponState = WeaponStates.Reloading;
        Invoke("ReloadingCounter", ReloadTime);
    }

    private void ReloadingCounter()
    {
        isReloading = false;
        WeaponState = WeaponStates.Idle;
    }


}
