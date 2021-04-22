using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerInGame : NetworkBehaviour
{
    public static PlayerInGame instance = null;

	[SyncVar] public PlayersInGameSettings playerInGameSettings;

	[Header("Player Member Settings")]
	[SerializeField] private GameObject playerMemeberPrefab = null;
	[SerializeField] private Sprite heroFaceSprite = null;

	private GameObject playerMember = null;


	private GameManager _gameManager = null;

	private Transform teamMembers = null;


	public override void OnStartClient()
	{
		if (isLocalPlayer)
		{
			if (instance == null)
			{
				instance = this;
			}
			if (MirrorPlayer.localPlayer.isHost)
			{
				Invoke("InvokeSpawnObjectives", 0.5f);
			}
			gameObject.layer = LayerMask.NameToLayer("Player");
		}
		int team = playerInGameSettings.roundIsInBlueTeam ? 1 : 2;
		//AfterPlayerSpawn(team);
		_gameManager = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<GameManager>();
		Invoke("PlayerMemberContainerSpawner", 1f);
	}

	private void InvokeSpawnObjectives()
	{
		InGame.instance.SpawnObjectives();
	}


	private void AfterPlayerSpawn(int spawnedTeam)
	{
		InGame.instance.AfterPlayerSpawn(spawnedTeam);
	}

	public void PlayerEnteringWaiting(float _countdownTimer)
	{
		CmdPlayerEnteringWaiting(_countdownTimer);
	}

	[Command]
	private void CmdPlayerEnteringWaiting(float _countdownTimer)
	{
		RPCPlayerEnteryWaiting(_countdownTimer);
	}

	[ClientRpc]
	private void RPCPlayerEnteryWaiting(float countDownTimer)
	{
		InGame.instance.RPCPlayerEnteryWaiting(countDownTimer);
	}


	private void OnDestroy()
	{
		if (!isClientOnly)
		{
			return;
		}
		/// Show that the player has died and then reduce the number of teams alive players
		if(playerMember != null)
		{
			playerMember.transform.GetChild(1).gameObject.SetActive(true);
		}
		print(playerInGameSettings.roundIsInBlueTeam);
		if (playerInGameSettings.roundIsInBlueTeam)
		{
			_gameManager.TeamGroup1RemainedPlayers--;
		}
		else
		{
			_gameManager.TeamGroup2RemainedPlayers--;
		}
	}

	private void PlayerMemberContainerSpawner()
	{
		/// Getting the team members container
		if (playerInGameSettings.roundIsInBlueTeam)
		{
			teamMembers = UIBtns.instance.blueTeamMembers.transform;
			_gameManager.TeamGroup1RemainedPlayers++;
		}
		else
		{
			teamMembers = UIBtns.instance.redTeamMembers.transform;
			_gameManager.TeamGroup2RemainedPlayers++;
		}
		playerMember = Instantiate(playerMemeberPrefab, teamMembers);
		playerMember.transform.GetChild(0).GetComponent<Image>().sprite = heroFaceSprite;
	}

}
