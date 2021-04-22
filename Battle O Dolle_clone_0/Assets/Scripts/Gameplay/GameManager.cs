using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{

	private MirrorGameManagerRemoter _mirrorGameManagerRemoter = null;

	[Range(0, 300)] [SerializeField] private float countDownTimerForRoundFinish = 90f;
	private float timeCounter = 0f;

	[SerializeField] private Text blueTeamScoreText = null;
	[SerializeField] private Text redTeamScoreText = null;
	[SerializeField] private Text roundText = null;
	[SerializeField] private Text roundTimerText = null;

	[SerializeField] private GameObject losePanel = null;
	[SerializeField] private GameObject winPanel = null;
	[SerializeField] private int winNeededScoreLimit = 2;
	private bool hasGameEnded = false;
	private bool roundTimeFinished = false;

	private int team = 0;
	private int group = 0;
	public int Group { get => group; set => group = value; }

	private int groupScore = 0;
	public int GroupScore
	{
		get => groupScore;
		set
		{
			groupScore = value;
			if (groupScore == winNeededScoreLimit)
			{
				if (Group == 1)
				{
					hasGameEnded = true;
					//photonView.RPC("SetTheWinLosePanel", RpcTarget.AllBuffered, 1);
					CmdSetTheWinLosePanel(1);
					//photonView.RPC("RPCGameHasFinished", RpcTarget.MasterClient, 1);
					CmdGameHasFinished(1);
					print("Group 1 won the game!!!");
				}
				else if (Group == 2)
				{
					hasGameEnded = true;
					//photonView.RPC("SetTheWinLosePanel", RpcTarget.AllBuffered, 2);
					CmdSetTheWinLosePanel(2);
					//photonView.RPC("RPCGameHasFinished", RpcTarget.MasterClient, 2);
					CmdGameHasFinished(2);
					print("Group 2 won the game!!!");
				}
			}
		}
	}


	

	private int group1Score = -1;
	private int group2Score = -1;

	private int teamGroup1RemainedPlayers = 0;
	public int TeamGroup1RemainedPlayers
	{
		get => teamGroup1RemainedPlayers;
		set
		{
			teamGroup1RemainedPlayers = value;
			if (teamGroup1RemainedPlayers == 0)
			{
				SetTheGroupScore(2);
				TheNextMatch(2);
				print("Group 2 won the match!!!");
			}
		}
	}


	private int teamGroup2RemainedPlayers = 0;
	public int TeamGroup2RemainedPlayers
	{
		get => teamGroup2RemainedPlayers;
		set
		{
			teamGroup2RemainedPlayers = value;
			if (teamGroup2RemainedPlayers == 0)
			{
				SetTheGroupScore(1);
				TheNextMatch(1);
				print("Group 1 won the match!!!");
			}
		}
	}

	private void LateGetMirrorManagerRemoter()
	{
		if (PlayerInGame.instance != null)
		{
			print("********************* found player");
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		else
		{
			Invoke("LateGetMirrorManagerRemoter", 0.5f);
			print("********************* no player");
			return;
		}
		// Set the group number
		Group = MirrorPlayer.localPlayer.thisPlayerInGameSettings.isInBlueTeam ? 1 : 2;

		// Get the team because we will use it in the loosing to say that the opponent teams score should increase and to see which group its in
		team = MirrorPlayer.localPlayer.thisPlayerInGameSettings.roundIsInBlueTeam ? 1 : 2;

		// Get the teams remained players count
		int allPlayersCount = MirrorPlayer.localPlayer.thisPlayerInGameSettings.matchPlayerSize;
		TeamGroup1RemainedPlayers = TeamGroup2RemainedPlayers = allPlayersCount / 2;

		// Get the teams scores
		if (team != 0)
		{
			GetTheGroupScore();
			//photonView.RPC("SendGroupScoreToMaster", RpcTarget.MasterClient, GroupScore, Group);
			CmdSendGroupScoreToMaster(GroupScore, Group);
		}
		if (MirrorPlayer.localPlayer.isHost)
		{
			Invoke("SetTheMatchScores", 2f);
		}
	}

	private void Start()
	{
		Invoke("LateGetMirrorManagerRemoter", 0.5f);
		if (countDownTimerForRoundFinish >= 60)
		{
			float iFloat = countDownTimerForRoundFinish / 60;
			int iInt = (int)iFloat;
			float secondFloat = (countDownTimerForRoundFinish - (iInt * 60));
			string secondString = secondFloat.ToString();
			if (secondFloat < 10)
			{
				secondString = "0" + secondString;
			}
			roundTimerText.text = "0" + iInt + " : " + secondString;
		}
		else if (countDownTimerForRoundFinish > 0)
		{
			float secondFloat = countDownTimerForRoundFinish;
			string secondString = secondFloat.ToString();
			if (secondFloat < 10)
			{
				secondString = "0" + secondString;
			}
			roundTimerText.text = "00" + " : " + secondString;
		}
	}


	private void Update()
	{
		if (MirrorPlayer.localPlayer.isHost)
		{
			if (roundTimeFinished)
			{
				return;
			}
			timeCounter += Time.deltaTime;
			if (timeCounter >= 1)
			{
				countDownTimerForRoundFinish--;
				timeCounter = 0f;
				//photonView.RPC("RPCChangeTheTimeOfRoundText", RpcTarget.AllBuffered, countDownTimerForRoundFinish);
				CmdChangeTheTimeOfRoundText(countDownTimerForRoundFinish);
				if (countDownTimerForRoundFinish <= 0)
				{
					if (!hasGameEnded)
					{

						roundTimeFinished = true;
						//photonView.RPC("RoundTimeFinished", RpcTarget.AllBuffered);
						CmdRoundTimeFinished();
					}
				}
			}
		}
	}


	private void SetTheMatchScores()
	{
		if (group1Score == -1 || group2Score == -1)
		{
			Invoke("SetTheMatchScores", 2f);
			return;
		}
		//photonView.RPC("RPCSetTheMatchScores", RpcTarget.AllBuffered, group1Score, group2Score);
		CmdSetTheMatchScores(group1Score, group2Score);
	}

	

	private void TheNextMatch(int winnerTeam)
	{
		ChangeTheTeamsRoleForTheNextMatch();
		if (MirrorPlayer.localPlayer.isHost)
		{
			float waitTime = 3f;
			Invoke("CallTheNextMatch", waitTime);
		}
	}


	private void CallTheNextMatch()
	{
		if (!hasGameEnded)
		{
			CmdLoadScene("NextMatch");
			//PhotonNetwork.LoadLevel("NextMatch");
		}
	}


	private void GetTheGroupScore()
	{
		GroupScore = MirrorPlayer.localPlayer.thisPlayerInGameSettings.roundWon;
	}

	private void SetTheGroupScore(int winnerGroup)
	{
		if (winnerGroup == Group)
		{
			GroupScore++;
			MirrorPlayer.localPlayer.thisPlayerInGameSettings.roundWon = GroupScore;
		}
	}

	private void ChangeTheTeamsRoleForTheNextMatch()
	{
		int newTeam = 0;
		if (team == 1)
		{
			newTeam = 2;
		}
		else if (team == 2)
		{
			newTeam = 1;
		}
		MirrorPlayer.localPlayer.thisPlayerInGameSettings.roundIsInBlueTeam = (newTeam == 1) ? true : false;
	}


	public void RPCChangeTheTimeOfRoundText(float remainedTime)
	{
		if (remainedTime >= 60)
		{
			float iFloat = remainedTime / 60;
			int iInt = (int)iFloat;
			float secondFloat = (remainedTime - (iInt * 60));
			string secondString = secondFloat.ToString();
			if (secondFloat < 10)
			{
				secondString = "0" + secondString;
			}
			roundTimerText.text = "0" + iInt + " : " + secondString;
		}
		else if (remainedTime > 0)
		{
			float secondFloat = remainedTime;
			string secondString = secondFloat.ToString();
			if (secondFloat < 10)
			{
				secondString = "0" + secondString;
			}
			roundTimerText.text = "00" + " : " + secondString;
		}
	}

	public void RPCRoundTimeFinished()
	{
		if (team == 2)
		{
			return;
		}
		if (Group == 1)
		{
			//photonView.RPC("RPCTeamGroup1RemainedPlayers", RpcTarget.AllBuffered);
			CmdTeamGroup1RemainedPlayers();
		}
		else if (Group == 2)
		{
			//photonView.RPC("RPCTeamGroup2RemainedPlayers", RpcTarget.AllBuffered);
			CmdTeamGroup2RemainedPlayers();
		}
	}

	

	public void RPCTeamGroup1RemainedPlayers()
	{
		TeamGroup1RemainedPlayers--;
	}

	public void RPCTeamGroup2RemainedPlayers()
	{
		TeamGroup2RemainedPlayers--;
	}

	public void RPCSetTheMatchScores(int group1Score, int group2Score)
	{
		roundText.text = (group1Score + group2Score + 1).ToString();
		if (Group == 1)
		{
			if (team == 1)
			{
				blueTeamScoreText.text = group1Score.ToString();
				redTeamScoreText.text = group2Score.ToString();
			}
			else if (team == 2)
			{
				blueTeamScoreText.text = group2Score.ToString();
				redTeamScoreText.text = group1Score.ToString();
			}
		}
		else if (Group == 2)
		{
			if (team == 1)
			{
				blueTeamScoreText.text = group2Score.ToString();
				redTeamScoreText.text = group1Score.ToString();
			}
			else if (team == 2)
			{
				blueTeamScoreText.text = group1Score.ToString();
				redTeamScoreText.text = group2Score.ToString();
			}
		}
	}

	public void RPCSendGroupScoreToMaster(int groupScore, int group)
	{
		if (MirrorPlayer.localPlayer.isHost)
		{
			if (group == 1)
			{
				if (group1Score == -1)
				{
					group1Score = groupScore;
				}
			}
			else if (group == 2)
			{
				if (group2Score == -1)
				{
					group2Score = groupScore;
				}
			}
		}
	}


	public void RPCSetTheWinLosePanel(int winnerGroup)
	{
		if (Group == winnerGroup)
		{
			winPanel.SetActive(true);
		}
		else
		{
			losePanel.SetActive(true);
		}
	}

	public void RPCGameHasFinished(int winnerGroup)
	{
		if (MirrorPlayer.localPlayer.isHost)
		{
			hasGameEnded = true;
			Invoke("FinishGame", 5f);
		}
	}

	private void FinishGame()
	{
		CmdLoadScene("Offline");
		//PhotonNetwork.LoadLevel("Pun");
	}

	public void RPcLoadScene(string sceneName)
	{
		if (sceneName == "Oflline")
		{
			Destroy(MirrorPlayer.localPlayer.gameObject);
		}
		SceneManager.LoadScene(sceneName);
	}


	#region Remoters

	private void CmdSendGroupScoreToMaster(int _groupScore, int _group)
	{
		if (_mirrorGameManagerRemoter == null)
		{
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		_mirrorGameManagerRemoter.SendGroupScoreToMaster(_groupScore, _group);
	}


	private void CmdSetTheWinLosePanel(int winnerGroup)
	{
		if (_mirrorGameManagerRemoter == null)
		{
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		_mirrorGameManagerRemoter.SetTheWinLosePanel(winnerGroup);
	}

	private void CmdGameHasFinished(int winnerGroup)
	{
		if (_mirrorGameManagerRemoter == null)
		{
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		_mirrorGameManagerRemoter.GameHasFinished(winnerGroup);
	}

	private void CmdRoundTimeFinished()
	{
		if (_mirrorGameManagerRemoter == null)
		{
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		_mirrorGameManagerRemoter.RoundTimeFinished();
	}

	private void CmdChangeTheTimeOfRoundText(float _countDownTimerForRoundFinish)
	{
		if (_mirrorGameManagerRemoter == null)
		{
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		_mirrorGameManagerRemoter.ChangeTheTimeOfRoundText(_countDownTimerForRoundFinish);
	}


	private void CmdSetTheMatchScores(int _group1Score, int group2Score)
	{
		if (_mirrorGameManagerRemoter == null)
		{
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		_mirrorGameManagerRemoter.SetTheMatchScores(_group1Score, group2Score);
	}

	public void CmdLoadScene(string sceneName)
	{
		if (_mirrorGameManagerRemoter == null)
		{
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		_mirrorGameManagerRemoter.LoadScene(sceneName);
	}


	private void CmdTeamGroup1RemainedPlayers()
	{
		if (_mirrorGameManagerRemoter == null)
		{
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		_mirrorGameManagerRemoter.TeamGroup1RemainedPlayers();
	}

	private void CmdTeamGroup2RemainedPlayers()
	{
		if (_mirrorGameManagerRemoter == null)
		{
			_mirrorGameManagerRemoter = PlayerInGame.instance.gameObject.GetComponent<MirrorGameManagerRemoter>();
		}
		_mirrorGameManagerRemoter.TeamGroup2RemainedPlayers();
	}


	#endregion



}
