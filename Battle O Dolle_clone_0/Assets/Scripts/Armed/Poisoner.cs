using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Poisoner : TrapAbstract
{

	[SerializeField] private GameObject poisonPrefab = null;
	private GameObject newPoison = null;
	private Poison poisonClas = null;
	private GameObject trapSetBtn = null;
	private float lifeTimeRate = 60f;


	private void Start()
	{
		trapSetBtn = UIBtns.instance.traperBtn.gameObject;
		poisonPrefab = Resources.Load("Poison", typeof(GameObject)) as GameObject;
		if (hasAuthority)
		{
			Invoke("TrapBtnTurnOff", lifeTimeRate);
			Destroy(this, lifeTimeRate);
		}
	}

	private void TrapBtnTurnOff()
	{
		trapSetBtn.SetActive(false);
	}


	public override void SetTrap()
	{
		int index = MirrorSpawner.instance.SpawnGameObjectindex(poisonPrefab, transform.position, transform.rotation);
		StartCoroutine(AfterSpawninPoison(index, InGame.instance.waitForSpawnedObjectsTimer));
		
	}

	IEnumerator AfterSpawninPoison(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninPoison(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		newPoison = MirrorSpawner.instance.spawnedFounders[_index];
		poisonClas = newPoison.GetComponent<Poison>();
		poisonClas.SetTheTag(gameObject.tag);

	}
}
