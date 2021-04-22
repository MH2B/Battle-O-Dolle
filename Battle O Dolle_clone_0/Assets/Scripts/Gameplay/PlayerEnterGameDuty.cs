using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerEnterGameDuty : NetworkBehaviour
{

	private int team = 0;
	public int Team { get => team; set => team = value; }


	private GameObject shootingBtn = null;


	private DroneMovement droneMovement = null;


	private UIBtns _uIBtns = null;


	private void Start()
	{
		_uIBtns = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>();
	}

	public void PlayerEntered()
	{
		//photonView.RPC("SetLAyerAndTag", RpcTarget.AllBuffered, Team);
		CmdSetTheLayerAndTag(Team);

		if (Team == 1)
		{
			// Blue Team ===> the player will change to a drone to spectacle the map
			BlueTeamFirstCondition();
		}
		else if(Team == 2)
		{
			// Blue Team ===> the player will have time to set their positions
			RedTeamFirstCondition();
		}
	}

	[Command]
	private void CmdSetTheLayerAndTag(int _team)
	{
		RPCSetTheLayerAndTag(_team);
	}


	private void RedTeamFirstCondition()
	{
		SetAbilityBtnActivity(false);
		SetMineBtnActivity(false);
	}

	private void BlueTeamFirstCondition()
	{
		GetComponent<PlayersMovement>().SetToDrone();
		StartCoroutine(AfterSpawninDrone(1f));

	}

	IEnumerator AfterSpawninDrone(float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		droneMovement = GetComponent<PlayersMovement>().droneMovement;
		if (droneMovement == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		SetAbilityBtnActivity(false);
		SetShopBtnActivity(false);
		SetMineBtnActivity(false);
		SetWeaponBtnActivity(false);
	}

	public void PlayerEnteredFinished()
	{
		if (Team == 1)
		{
			// Blue Team ===> the player will change to a character
			BlueTeamSecondCondition();
		}
		else if (Team == 2)
		{
			// Blue Team ===> the player will have time to set their positions
			RedTeamSecondCondition();
		}
	}

	private void RedTeamSecondCondition()
	{
		SetAbilityBtnActivity(true);
	}

	private void BlueTeamSecondCondition()
	{
		droneMovement.SetToCharacter();
		SetAbilityBtnActivity(true);
		SetShopBtnActivity(true);
		SetWeaponBtnActivity(true);
	}


	private void SetAbilityBtnActivity(bool activity)
	{
		_uIBtns.abilityBtn.gameObject.SetActive(activity);
	}

	private void SetShopBtnActivity(bool activity)
	{
		_uIBtns.shopBtn.gameObject.SetActive(activity);
	}
	
	private void SetWeaponBtnActivity(bool activity)
	{
		_uIBtns.weaponBtn.gameObject.SetActive(activity);
	}


	private void SetMineBtnActivity(bool activity)
	{
		_uIBtns.traperBtn.gameObject.SetActive(activity);
	}


	[ClientRpc]
	private void RPCSetTheLayerAndTag(int team)
	{
		string tagName = null;
		if (team == 1)
		{
			tagName = "BlueTeam";
			gameObject.name = "Blue Player";
		}
		else if (team == 2)
		{
			tagName = "RedTeam";
			gameObject.name = "Red Player";
		}
		gameObject.tag = tagName;
	}
}
