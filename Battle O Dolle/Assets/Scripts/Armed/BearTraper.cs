using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BearTraper : TrapAbstract
{

	
	[SerializeField] private GameObject bearTrapPrefab = null;
	private GameObject newBearTrap = null;
	private BearTrap BearTrapClas = null;
	private GameObject trapSetBtn = null;
	private float lifeTimeRate = 60f;


	private void Start()
	{
		trapSetBtn = UIBtns.instance.traperBtn.gameObject;

		if (hasAuthority)
		{
			Invoke("TrapBtnTurnOff", lifeTimeRate);
			Destroy(this, lifeTimeRate);
		}
		bearTrapPrefab = Resources.Load("BearTrap", typeof(GameObject)) as GameObject;
	}

	private void TrapBtnTurnOff()
	{
		trapSetBtn.SetActive(false);
	}

	public override void SetTrap()
	{
		//newBearTrap = PhotonNetwork.Instantiate(bearTrapPrefab.name, transform.position, Quaternion.identity);
		int index = MirrorSpawner.instance.SpawnGameObjectindex(bearTrapPrefab, transform.position, transform.rotation);
		StartCoroutine(AfterSpawninBearTrap(index, InGame.instance.waitForSpawnedObjectsTimer));
		
	}

	IEnumerator AfterSpawninBearTrap(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninBearTrap(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		newBearTrap = MirrorSpawner.instance.spawnedFounders[_index];
		BearTrapClas = newBearTrap.GetComponent<BearTrap>();
		BearTrapClas.SetTheTag(gameObject.tag);

	}
}
