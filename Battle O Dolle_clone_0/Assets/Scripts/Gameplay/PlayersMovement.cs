using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayersMovement : MovementAbstract
{


	public DroneMovement droneMovement = null;
	[SerializeField] private GameObject dronePrefab = null;

	private bool isStucked = false;
	public bool IsStucked { get => isStucked; set => CmdSetIsStucked(value); /*photonView.RPC("RPCSetisStucked", RpcTarget.AllBuffered, value);*/ }

	public override void Update()
	{
		if (IsStucked)
		{
			return;
		}
		base.Update();
	}

	[Command]
	private void CmdSetIsStucked(bool value)
	{
		RPCSetIsStucked(value);
	}

	public void SetToDrone()
	{
		// Create new drone
		int index = MirrorSpawner.instance.SpawnGameObjectindex(dronePrefab, transform.position, transform.rotation);
		StartCoroutine(AfterSpawninDrone(index, InGame.instance.waitForSpawnedObjectsTimer));
	}


	IEnumerator AfterSpawninDrone(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninDrone(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		GameObject newDrone = MirrorSpawner.instance.spawnedFounders[_index];
		droneMovement = null;
		droneMovement = newDrone.GetComponent<DroneMovement>();
		droneMovement.PlayerGameObject = gameObject;

		// Tell every one to set player game object off (change character)
		CmdChanegCharacter();

	}

	[Command]
	private void CmdChanegCharacter()
	{
		RPChangeCharacter();
	}

	private void SetActivity(GameObject go, bool activity)
	{
		go.SetActive(activity);
	}


	[ClientRpc]
	private void RPChangeCharacter()
	{

		SetActivity(gameObject, false);
	}


	[ClientRpc]
	private void RPCSetIsStucked(bool value)
	{
		isStucked = value;
	}

}
