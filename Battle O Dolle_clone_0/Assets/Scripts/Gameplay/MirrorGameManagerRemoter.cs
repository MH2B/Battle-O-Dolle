using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MirrorGameManagerRemoter : NetworkBehaviour
{

	private GameManager _gameManager = null;

	private void Start()
	{
		if (!hasAuthority)
		{
			return;
		}
		_gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
	}

	#region Functions

	public void SetTheWinLosePanel(int winnerGroup)
	{
		CmdSetTheWinLosePanel(winnerGroup);
	}

	public void GameHasFinished(int winnerGroup)
	{
		CmdGameHasFinished(winnerGroup);
	}

	public void SendGroupScoreToMaster(int _groupScore, int _group)
	{
		CmdSendGroupScoreToMaster(_groupScore, _group);
	}

	public void RoundTimeFinished()
	{
		CmdRoundTimeFinished();
	}

	public void ChangeTheTimeOfRoundText(float _countDownTimerForRoundFinish)
	{
		CmdChangeTheTimeOfRoundText(_countDownTimerForRoundFinish);
	}

	public void SetTheMatchScores(int _group1Score, int group2Score)
	{
		CmdSetTheMatchScores(_group1Score, group2Score);
	}

	public void TeamGroup1RemainedPlayers()
	{
		CmdTeamGroup1RemainedPlayers();
	}

	public void TeamGroup2RemainedPlayers()
	{
		CmdTeamGroup2RemainedPlayers();
	}

	public void LoadScene(string sceneName)
	{
		CmdLoadScene(sceneName);
	}

	#endregion


	#region Command

	[Command(ignoreAuthority = true)]
	private void CmdSetTheWinLosePanel(int winnerGroup)
	{
		RPCSetTheWinLosePanel(winnerGroup);
	}

	[Command(ignoreAuthority = true)]
	private void CmdGameHasFinished(int winnerGroup)
	{
		RPCGameHasFinished(winnerGroup);
	}

	[Command(ignoreAuthority = true)]
	private void CmdSendGroupScoreToMaster(int _groupScore, int _group)
	{
		RPCSendGroupScoreToMaster(_groupScore, _group);
	}

	[Command(ignoreAuthority = true)]
	private void CmdRoundTimeFinished()
	{
		RPCRoundTimeFinished();
	}

	[Command(ignoreAuthority = true)]
	private void CmdChangeTheTimeOfRoundText(float _countDownTimerForRoundFinish)
	{
		RPCChangeTheTimeOfRoundText(_countDownTimerForRoundFinish);
	}

	[Command(ignoreAuthority = true)]
	private void CmdSetTheMatchScores(int _group1Score, int group2Score)
	{
		RPCSetTheMatchScores(_group1Score, group2Score);
	}

	[Command(ignoreAuthority = true)]
	private void CmdTeamGroup1RemainedPlayers()
	{
		RPCTeamGroup1RemainedPlayers();
	}

	[Command(ignoreAuthority = true)]
	private void CmdTeamGroup2RemainedPlayers()
	{
		RPCTeamGroup2RemainedPlayers();
	}

	[Command(ignoreAuthority = true)]
	private void CmdLoadScene(string sceneName)
	{
		RPcLoadScene(sceneName);
	}

	#endregion


	#region RPC

	[ClientRpc]
	private void RPCChangeTheTimeOfRoundText(float remainedTime)
	{
		_gameManager.RPCChangeTheTimeOfRoundText(remainedTime);
	}

	[ClientRpc]
	private void RPCRoundTimeFinished()
	{
		_gameManager.RPCRoundTimeFinished();
	}



	[ClientRpc]
	private void RPCTeamGroup1RemainedPlayers()
	{
		_gameManager.RPCTeamGroup1RemainedPlayers();
	}

	[ClientRpc]
	private void RPCTeamGroup2RemainedPlayers()
	{

		_gameManager.RPCTeamGroup2RemainedPlayers();
	}

	[ClientRpc]
	private void RPCSetTheMatchScores(int group1Score, int group2Score)
	{
		_gameManager.RPCSetTheMatchScores(group1Score, group2Score);
	}

	[ClientRpc]
	private void RPCSendGroupScoreToMaster(int groupScore, int group)
	{

		_gameManager.RPCSendGroupScoreToMaster(groupScore, group);
	}


	[ClientRpc]
	private void RPCSetTheWinLosePanel(int winnerGroup)
	{
		_gameManager.RPCSetTheWinLosePanel(winnerGroup);
	}

	[ClientRpc]
	private void RPCGameHasFinished(int winnerGroup)
	{
		_gameManager.RPCGameHasFinished(winnerGroup);
	}




	[ClientRpc]
	private void RPcLoadScene(string sceneName)
	{
		_gameManager.RPcLoadScene(sceneName);
	}


	#endregion





}
