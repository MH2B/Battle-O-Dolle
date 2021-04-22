using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{

	[SerializeField] private Transform meleePos = null;
	[SerializeField] private Transform gunPos = null;

	public WeaponAbstract currentWeapon = null;



	public void AddWeapon(GameObject requestedWeapon)
	{
		if(currentWeapon != null)
		{
			currentWeapon.DestroyWeapon();
			currentWeapon = null;
		}

		currentWeapon = requestedWeapon.GetComponent<WeaponAbstract>();
		Transform parentPos = null;
		if (currentWeapon.WeaponType == WeaponAbstract.WeaponTypes.Gun)
		{
			parentPos = gunPos;
		}
		else
		{
			parentPos = meleePos;
		}

		int index = MirrorSpawner.instance.SpawnGameObjectindex(requestedWeapon , parentPos.position , Quaternion.identity);
		StartCoroutine(AfterSpawninRequestedWeapon(index, InGame.instance.waitForSpawnedObjectsTimer));
	}

	IEnumerator AfterSpawninRequestedWeapon(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninRequestedWeapon(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		GameObject newWeapon = MirrorSpawner.instance.spawnedFounders[_index];
		currentWeapon = newWeapon.GetComponent<WeaponAbstract>();
		CmdSetParent(newWeapon);
	}


	[Command]
	private void CmdSetParent(GameObject _currentWeaponObject)
	{
		RPCSetParent(_currentWeaponObject);
	}

	[ClientRpc]
	private void RPCSetParent(GameObject _currentWeaponObject)
	{
		if (_currentWeaponObject != null)
		{
			if (_currentWeaponObject.GetComponent<WeaponAbstract>() != null)
			{
				// If player is to left
				if (transform.localScale.x > 0)
				{
					Vector2 scales = _currentWeaponObject.transform.localScale;
					scales.x = 1f;
					_currentWeaponObject.transform.localScale = scales;
				}
				else if (transform.localScale.x < 0)
				{
					Vector2 scales = _currentWeaponObject.transform.localScale;
					scales.x = -1f;
					_currentWeaponObject.transform.localScale = scales;
				}

				if (_currentWeaponObject.GetComponent<WeaponAbstract>().WeaponType == WeaponAbstract.WeaponTypes.Gun)
				{
					_currentWeaponObject.transform.parent = gunPos;
				}
				else
				{
					_currentWeaponObject.transform.parent = meleePos;
				}
			}
		}
	}

}
