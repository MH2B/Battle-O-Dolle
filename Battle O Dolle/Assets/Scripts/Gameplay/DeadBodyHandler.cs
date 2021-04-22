using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DeadBodyHandler : NetworkBehaviour
{


	private GameObject canvas = null;
	public GameObject Canavs { get => canvas; set => canvas = value; }

	private GameObject spectDeathDrone = null;
	public GameObject SpectDeathDrone { get => spectDeathDrone; set => spectDeathDrone = value; }


	public void AtatchTheDeadBody(GameObject deadPlayer)
	{
		CmdAttachTheDeadBody(deadPlayer);
		//photonView.RPC("RPCAttachTheDeadBody", RpcTarget.AllBuffered , photonID);
	}

	[Command]
	private void CmdAttachTheDeadBody(GameObject deadPlayer)
	{
		RPCAttachTheDeadBody(deadPlayer);
	}

	public void ResetPlayer()
	{
		//photonView.RPC("RPCResetPlayer", RpcTarget.AllBuffered);
		CmdResetPlayer();
	}

	[Command]
	private void CmdResetPlayer()
	{
		RPCResetPlayer();
	}

	[ClientRpc]
	private void RPCAttachTheDeadBody(GameObject deadPlayer)
	{
		deadPlayer.transform.parent = gameObject.transform;
	}

	[ClientRpc]
	private void RPCResetPlayer()
	{
		GameObject deadPlayer = gameObject.transform.GetChild(0).gameObject;
		deadPlayer.SetActive(true);
		IPlayer iplayer = deadPlayer.GetComponent<IPlayer>();
		iplayer.Heal(100f);
		iplayer.Revived();
		deadPlayer.transform.parent = null;

		if (SpectDeathDrone != null)
		{
			deadPlayer.GetComponent<MovementAbstract>().SetTheFollowTarget();
			Destroy(SpectDeathDrone);
			UIBtns.instance.buttonsPanel.SetActive(true);
			//for (int i = 1; i < canvas.transform.childCount; i++)
			//{
			//	Button btn = canvas.transform.GetChild(i).gameObject.GetComponent<Button>();
			//	if (btn != null)
			//	{
			//		btn.interactable = true;
			//	}
			//}
		}

		PlayerMatchData playerMatchData = deadPlayer.GetComponent<PlayerMatchData>();

		// Update the all players of teams in the game manager
		if (playerMatchData.playerGroup == 1)
		{
			playerMatchData.gameManager.TeamGroup1RemainedPlayers++;
		}
		else if (playerMatchData.playerGroup == 2)
		{
			playerMatchData.gameManager.TeamGroup2RemainedPlayers++;
		}

		Destroy(gameObject);
	}
}
