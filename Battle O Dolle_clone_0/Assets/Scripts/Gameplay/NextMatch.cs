using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


public class NextMatch : NetworkBehaviour
{

	private void Start()
	{
		if (MirrorPlayer.localPlayer.isHost)
		{
			Invoke("CallNextLevel", 2f);
		}
	}

	private void CallNextLevel()
	{
		//PhotonNetwork.LoadLevel("MainScene");
		CmdGoToNextRound();
	}

	[Command]
	private void CmdGoToNextRound()
	{
		RpcGoToNextRound();
	}

	[ClientRpc]
	private void RpcGoToNextRound()
	{
		SceneManager.LoadScene("MainScene");
	}

}
