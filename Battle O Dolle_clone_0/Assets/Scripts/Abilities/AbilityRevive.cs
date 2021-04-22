using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityRevive : AbilityAbstract
{
	[SerializeField] private GameObject revivePrefab = null;

	private GameObject newRevive = null;

	private IPlayer playerInterface = null;

	GameObject newAiming = null;

	private GameObject abilityFixedJoyStick = null;

	private void Awake()
	{
		revivePrefab = Resources.Load("Revive", typeof(GameObject)) as GameObject;
		abilityFixedJoyStick = UIBtns.instance.abilityJoystick;
	}

	private void Start()
	{
		playerInterface = GetComponent<IPlayer>();
	}


	public override void AbilityIsStarting(GameObject aimingPref)
	{
		if (abilityFixedJoyStick.activeInHierarchy)
		{
			abilityFixedJoyStick.SetActive(false);
		}
	}

	public override void ExecuteAbility()
	{
		if (newAiming != null)
		{
			Destroy(newAiming);
		}

		int index = MirrorSpawner.instance.SpawnGameObjectindex(revivePrefab, transform.position, transform.rotation);
		StartCoroutine(AfterSpawninRevive(index, InGame.instance.waitForSpawnedObjectsTimer));
	}

	IEnumerator AfterSpawninRevive(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninRevive(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		newRevive = MirrorSpawner.instance.spawnedFounders[_index];
		newRevive.GetComponent<Revive>().PlayerInterface = playerInterface;

	}
}
