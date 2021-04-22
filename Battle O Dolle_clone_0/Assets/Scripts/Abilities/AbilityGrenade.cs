using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGrenade : AbilityAbstract
{
	
	[SerializeField] private GameObject grenadePrefab = null;
	
	private GameObject newGrenade = null;

	private IPlayer playerInterface = null;

	GameObject newAiming = null;

	private AimingDirection aiminingScaler = null;

	private void Awake()
	{
		grenadePrefab = Resources.Load("Grenade", typeof(GameObject)) as GameObject;
	}

	private void Start()
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

		//newGrenade = PhotonNetwork.Instantiate(grenadePrefab.name, transform.position, Quaternion.identity);
		int index = MirrorSpawner.instance.SpawnGameObjectindex(grenadePrefab, transform.position, transform.rotation);
		StartCoroutine(AfterSpawninGrenade(index, InGame.instance.waitForSpawnedObjectsTimer, aimingDirection));
	}


	IEnumerator AfterSpawninGrenade(int _index, float waitingTime , Vector2 _aimingDirection)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninGrenade(_index, waitingTime , _aimingDirection));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		newGrenade = MirrorSpawner.instance.spawnedFounders[_index];
		Grenade grenade = newGrenade.GetComponent<Grenade>();
		grenade.PlayerInterface = playerInterface;
		_aimingDirection = _aimingDirection.normalized;
		grenade.AimingDirection = _aimingDirection;
	}

}
