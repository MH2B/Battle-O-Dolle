using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Miner : TrapAbstract
{

	[SerializeField] private GameObject minePrefab = null;
	private GameObject newMine = null;
	private Mine mineClas = null;
	private GameObject trapSetBtn = null;
	private float lifeTimeRate = 60f;


	private void Start()
	{
		trapSetBtn = UIBtns.instance.traperBtn.gameObject;
		if (hasAuthority)
		{
			Invoke("TrapBtnTurnOff", lifeTimeRate);
			Destroy(this , lifeTimeRate);
		}
		minePrefab = Resources.Load("Mine", typeof(GameObject)) as GameObject;
	}


	private void TrapBtnTurnOff()
	{
		trapSetBtn.SetActive(false);
	}

	public override void SetTrap()
	{
		//newMine = PhotonNetwork.Instantiate(minePrefab.name, transform.position, Quaternion.identity);
		int index = MirrorSpawner.instance.SpawnGameObjectindex(minePrefab, transform.position, transform.rotation);
		StartCoroutine(AfterSpawninMine(index, InGame.instance.waitForSpawnedObjectsTimer));
	}

	IEnumerator AfterSpawninMine(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninMine(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		newMine = MirrorSpawner.instance.spawnedFounders[_index];
		mineClas = newMine.GetComponent<Mine>();
		mineClas.SetTheTag(gameObject.tag);
	}
}
