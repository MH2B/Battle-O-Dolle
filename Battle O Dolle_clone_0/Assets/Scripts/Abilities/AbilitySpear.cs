using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilitySpear : AbilityAbstract
{

	[SerializeField] private GameObject spearPrefab = null;
	[SerializeField] private Transform spearPos = null;

	private GameObject newSpear = null;

	private IPlayer playerInterface = null;

	GameObject newAiming = null;

	private AimingDirection aiminingScaler = null;

	private void Awake()
	{
		spearPrefab = Resources.Load("Spear", typeof(GameObject)) as GameObject;
	}

	public override void OnStartClient()
	{
		playerInterface = GetComponent<IPlayer>();
	}


	public override void AbilityIsStarting(GameObject aimingPref)
	{
		newAiming = Instantiate(aimingPref, transform.position, Quaternion.identity);
		aiminingScaler = newAiming.GetComponent<AimingDirection>();
	}

	private void Update()
	{
		Vector2 aimingDirection = Vector2.zero;

		if (newAiming != null && aiminingScaler != null)
		{
			aimingDirection = aiminingScaler.AimDirection;
			if (aimingDirection.x <= 0.2f)
			{
				if (transform.localScale.x > 0)
				{
					Vector2 newScale = transform.localScale;
					newScale.x = -newScale.x;
					transform.localScale = newScale;
				}
			}
			//Flip ToLeft
			else if (aimingDirection.x >= -0.2f)
			{
				if (transform.localScale.x < 0)
				{
					Vector2 newScale = transform.localScale;
					newScale.x = -newScale.x;
					transform.localScale = newScale;
				}
			}
		}
	}


	private Quaternion LookTowards(Vector2 dir)
	{
		// Rotate towards the joystick direction
		var dirAngular2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if (dir.x < 0)
		{
			return Quaternion.AngleAxis(dirAngular2, Vector3.forward);
		}
		else
		{
			return Quaternion.AngleAxis(dirAngular2, Vector3.forward);
		}
	}

	public override void ExecuteAbility()
	{
		Vector2 aimingDirection = Vector2.zero;
		if (newAiming != null)
		{
			aimingDirection = newAiming.GetComponent<AimingDirection>().AimDirection;
			Destroy(newAiming);
			newAiming = null;
		}

		aimingDirection = -aimingDirection;

		int index = MirrorSpawner.instance.SpawnGameObjectindex(spearPrefab, spearPos.position, LookTowards(-aimingDirection));
		StartCoroutine(AfterSpawninSpear(index, InGame.instance.waitForSpawnedObjectsTimer , aimingDirection));
	}


	IEnumerator AfterSpawninSpear(int _index, float waitingTime , Vector2 _aimingDirection)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninSpear(_index, waitingTime , _aimingDirection));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		newSpear = MirrorSpawner.instance.spawnedFounders[_index];
		Spear spearClass = newSpear.GetComponent<Spear>();
		spearClass.PlayerInterface = playerInterface;
		_aimingDirection = _aimingDirection.normalized;
		spearClass.AimingDirection = _aimingDirection;

	}

	
}