using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityHealer : AbilityAbstract
{
	[SerializeField] private GameObject healPrefab = null;

	private GameObject newHealer = null;

	private IPlayer playerInterface = null;

	GameObject newAiming = null;

	private GameObject fixedJoyStick = null;

	private void Awake()
	{
		healPrefab = Resources.Load("Heal", typeof(GameObject)) as GameObject;
		fixedJoyStick = UIBtns.instance.abilityJoystick;
	}

	private void Start()
	{
		playerInterface = GetComponent<IPlayer>();
	}

	public override void AbilityIsStarting(GameObject aimingPref)
	{
		if (fixedJoyStick.activeInHierarchy)
		{
			fixedJoyStick.SetActive(false);
		}
	}


	public override void ExecuteAbility()
	{
		if (newAiming != null)
		{
			Destroy(newAiming);
		}
		int index = MirrorSpawner.instance.SpawnGameObjectindex(healPrefab, transform.position , transform.rotation);
		StartCoroutine(AfterSpawninHeal(index, InGame.instance.waitForSpawnedObjectsTimer));
	}

	IEnumerator AfterSpawninHeal(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninHeal(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		newHealer = MirrorSpawner.instance.spawnedFounders[_index];
		newHealer.GetComponent<Heal>().PlayerInterface = playerInterface;
	}

}
