using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class InGame : NetworkBehaviour
{

    public static InGame instance = null;

	[Header("Spawn Settings")]
	[SerializeField] public Transform[] blueTeamStartPoss = null;
	[SerializeField] public Transform[] redTeamStartPoss = null;
	[SerializeField] public Transform[] trapSpawnPoss = null;
	public float waitForSpawnedObjectsTimer = 0.1f;

	public PlayersInGameSettings playerInGameSettings;


	[Header("CountDown Conditions")]
	[Range(0, 60)] [SerializeField] private float countDownTimer = 10f;


	[Range(0, 60)] [SerializeField] private float countDownTimerForTrapUsebality = 50f;
	public float CountDownTimerForTrapUsebality { get => countDownTimerForTrapUsebality; set => countDownTimerForTrapUsebality = value; }

	private float countDownTimerTemp = 0f;
	public float CountDownTimer { get => countDownTimer; set => countDownTimer = value; }

	[SerializeField] private Image enterGameWaitLoadBar = null;
	[SerializeField] private GameObject timeCounterPanel = null;


	[Header("Prefabs")]
	[SerializeField] private GameObject treasurePrefab = null;
	[SerializeField] private GameObject trapObjectivePrefab = null;
	[Range(0, 5)] [SerializeField] public int trapsSpawnCount = 3;

	[Header("Objectives Spawn Points")]
	[SerializeField] private Transform objectivesSpawnPoint = null;
	[SerializeField] private Transform trapDetectorObjectivesSpawnPoint = null;
	[SerializeField] private Transform[] trapObjectivesSpawnPoints = null;
	private bool hasSpawnedLocker = false;


	private bool timerStarted = false;
	private float counter = 0f;
	private PlayerEnterGameDuty playerEnterGameDuty = null;



	float timmmmmm = 0f;


	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}


	private void Start()
	{
		MirrorPlayerInGame.localPlayerInGame.StartSpawning();
		countDownTimerTemp = countDownTimer;
	}

	private void Update()
	{
		if (MirrorPlayer.localPlayer.isHost)
		{
			if (timerStarted)
			{
				counter += Time.deltaTime;
				if (counter >= 1)
				{
					counter = 0;
					CountDownTimer--;
					//photonView.RPC("PlayerEnteryWaiting", RpcTarget.AllBuffered, countDownTimer);
					//PlayerInGame.instance.PlayerEnteringWaiting(countDownTimer);
				}
			}
		}
		timmmmmm += Time.deltaTime;
	}

	public void AfterPlayerSpawn(int spawnedTeam)
	{
		int team = playerInGameSettings.roundIsInBlueTeam ? 1 : 2;
		if(team == spawnedTeam)
		{
			PlayerInGame.instance.gameObject.layer = LayerMask.NameToLayer("Player");
		}
		PlayerInGame.instance.gameObject.GetComponent<CollisionHandler>().Team = team;
		playerEnterGameDuty = PlayerInGame.instance.gameObject.GetComponent<PlayerEnterGameDuty>();
		playerEnterGameDuty.Team = team;
		playerEnterGameDuty.PlayerEntered();
		StartTimerCountDown();
	}

	public void StartTimerCountDown()
	{
		timeCounterPanel.SetActive(true);
		timerStarted = true;
	}

	

	public void SpawnObjectives()
	{
		if(MirrorSpawner.instance == null)
		{
			print("error");
			return;
		}
		//PhotonNetwork.Instantiate(objectivePrefab.name, objectivesSpawnPoint.position, Quaternion.identity);
		MirrorSpawner.instance.SpawnGameObjectindex(treasurePrefab, objectivesSpawnPoint.position, transform.rotation);
		int index = MirrorSpawner.instance.SpawnGameObjectindex(trapObjectivePrefab, trapDetectorObjectivesSpawnPoint.position, transform.rotation);
		//GameObject newTrapDetector = MirrorSpawner.instance.SpawnGameObject(trapObjectivePrefab, trapDetectorObjectivesSpawnPoint.position);
		StartCoroutine(AfterSpawninTreasure(index , waitForSpawnedObjectsTimer));
	}

	IEnumerator AfterSpawninTreasure(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			print("Coroutine couldnt finddddd");
			StartCoroutine(AfterSpawninTreasure(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		GameObject newTrapDetector = MirrorSpawner.instance.spawnedFounders[_index];
		
		newTrapDetector.GetComponent<TrapObjectiveInteractability>().TrapClassName = "TrapDetectorBeeper";

		
		newTrapDetector.GetComponent<TrapObjectiveInteractability>().TrapClassName = "TrapDetectorBeeper";

		hasSpawnedLocker = false;
		for (int i = 0; i < trapsSpawnCount; i++)
		{
			int index = MirrorSpawner.instance.SpawnGameObjectindex(trapObjectivePrefab, trapObjectivesSpawnPoints[i].position, transform.rotation);
			StartCoroutine(AfterSpawninTrapObjective(index, waitForSpawnedObjectsTimer));
		}
	}

	IEnumerator AfterSpawninTrapObjective(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninTrapObjective(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		GameObject newTrap = MirrorSpawner.instance.spawnedFounders[_index];
		string trapClassName = null;
		int rand = Random.Range(1, 5);
		switch (rand)
		{
			case 1:
				trapClassName = "Miner";
				break;
			case 2:
				trapClassName = "Poisoner";
				break;
			case 3:
				trapClassName = "BearTraper";
				break;
			case 4:
				if (hasSpawnedLocker)
				{
					trapClassName = "Miner";
				}
				else
				{
					trapClassName = "Locker";
					hasSpawnedLocker = true;
				}
				break;
			default:
				trapClassName = "Miner";
				break;
		}
		newTrap.GetComponent<TrapObjectiveInteractability>().TrapClassName = trapClassName;
	}

	public void RPCPlayerEnteryWaiting(float CountDownTimer)
	{
		//timerCounter.text = CountDownTimer.ToString();
		enterGameWaitLoadBar.fillAmount = (1 - CountDownTimer / countDownTimerTemp);
		if (CountDownTimer == 0)
		{
			playerEnterGameDuty.PlayerEnteredFinished();
			timeCounterPanel.SetActive(false);
			timerStarted = false;
		}
	}

	private void Disconnected()
	{
		SceneManager.LoadScene("Offline");
	}

	//public void GetInitializeSettings(ref int _trapsSpawnCount, ref float _countDownTimer , ref GameObject _objectivePrefab, ref GameObject _trapObjectivePrefab, ref Transform _objectivesSpawnPoint, ref Transform _trapDetectorObjectivesSpawnPoint, ref Transform[] _trapObjectivesSpawnPoints)
	//{
	//	_trapsSpawnCount = trapsSpawnCount;
	//	_countDownTimer = CountDownTimer;
	//	_objectivePrefab = treasurePrefab;
	//	_trapObjectivePrefab = trapObjectivePrefab;
	//	_objectivesSpawnPoint = objectivesSpawnPoint;
	//	_trapDetectorObjectivesSpawnPoint = trapDetectorObjectivesSpawnPoint;
	//	_trapObjectivesSpawnPoints = trapObjectivesSpawnPoints;
	//}

	//[Command]
	//private void CmdPlayerEnteringWaiting(float _countdownTimer)
	//{
	//	RPCPlayerEnteryWaiting(_countdownTimer);
	//}

	//private void CheckIfAllThePlayersHaveJoined()
	//{
	//	int eachTeamPlayerSize = MirrorPlayer.localPlayer.thisPlayerInGameSettings.matchPlayerSize / 2;
	//	if ((spawnedPlayers.Count - 1) != (eachTeamPlayerSize * 2))
	//	{
	//		int team1Players = 0;
	//		int team2Players = 0;
	//		foreach (int spawnedPlayersteamNumber in spawnedPlayers)
	//		{
	//			if (spawnedPlayersteamNumber == 1)
	//			{
	//				team1Players++;
	//			}
	//			else
	//			{
	//				team2Players++;
	//			}
	//		}

	//		int team1Remained = eachTeamPlayerSize - team1Players;
	//		for (int i = 1; i <= team1Remained; i++)
	//		{
	//			//photonView.RPC("DisconnectedPlayerNotSpawnedSenedFromMaster", RpcTarget.AllBuffered, 1);
	//			CmdDisconnectedPlayerNotSpawnedSenedFromMaster(1);
	//		}

	//		int team2Remained = eachTeamPlayerSize - team2Players;
	//		for (int i = 1; i <= team2Remained; i++)
	//		{
	//			//photonView.RPC("DisconnectedPlayerNotSpawnedSenedFromMaster", RpcTarget.AllBuffered, 2);
	//			CmdDisconnectedPlayerNotSpawnedSenedFromMaster(2);
	//		}
	//	}
	//}

	//[Command]
	//private void CmdDisconnectedPlayerNotSpawnedSenedFromMaster(int value)
	//{
	//	RPCDisconnectedPlayerNotSpawnedSenedFromMaster(value);
	//}


	//private void SpawnHeros()
	//{
	//	// Get the team and spawn related to that
	//	team = MirrorPlayer.localPlayer.thisPlayerInGameSettings.roundIsInBlueTeam ? 1 : 2;
	//	//photonView.RPC("RPCSendTheMasterWhenSpawnedThePlayer", RpcTarget.MasterClient, (int)PhotonNetwork.LocalPlayer.CustomProperties["Group"]);
	//	CmdSendTheMasterWhenSpawnedThePlayer(playerGroup);
	//	className = MirrorPlayer.localPlayer.thisPlayerInGameSettings.heroName;
	//	//photonView.RPC("AddPlayerToTeamListUI", RpcTarget.AllBuffered, className, team);
	//	CmdAddPlayerToTimListUI(className , team);
	//	GameObject spawnablePlayerPrefab = playersPrefab[0];
	//	foreach (GameObject go in playersPrefab)
	//	{
	//		if (go.name == className)
	//		{
	//			spawnablePlayerPrefab = go;
	//			break;
	//		}
	//	}


	//	// Team Blue
	//	if (team == 1)
	//	{
	//		//player = PhotonNetwork.Instantiate(spawnablePlayerPrefab.name, blueTeamSpawnPoint.position, Quaternion.identity);
	//		player = MirrorSpawner.instance.SpawnGameObject(spawnablePlayerPrefab, blueTeamSpawnPoint.position);
	//		// Set Player Statues
	//		playerInterface = player.GetComponent<IPlayer>();
	//		playerInterface.TeamSetter("BlueTeam");
	//	}
	//	// Team Red
	//	else if (team == 2)
	//	{
	//		//player = PhotonNetwork.Instantiate(spawnablePlayerPrefab.name, redTeamSpawnPoint.position, Quaternion.identity);
	//		player = MirrorSpawner.instance.SpawnGameObject(spawnablePlayerPrefab, redTeamSpawnPoint.position);
	//		// Set Player Statues
	//		playerInterface = player.GetComponent<IPlayer>();
	//		playerInterface.TeamSetter("RedTeam");
	//	}

	//	// Change the layer to Player so that we wont be our own enemy because at the first all of the layers are at Enemy
	//	player.gameObject.layer = LayerMask.NameToLayer("Player");

	//	if (MirrorPlayer.localPlayer.isHost)
	//	{
	//		SpawnObjectives();
	//	}
	//}



	//[Command]
	//private void CmdAddPlayerToTimListUI(string _className , int _team)
	//{
	//	RPCAddPlayerToTeamListUI(_className, _team);
	//}


	//[Command]
	//private void CmdSendTheMasterWhenSpawnedThePlayer(int _group)
	//{
	//	RPCSendTheMasterWhenSpawnedThePlayer(_group);
	//}





	//[ClientRpc]
	//private void RPCAddPlayerToTeamListUI(string className, int teamNumber)
	//{
	//	Transform parentContainer = null;
	//	if (teamNumber == 1)
	//	{
	//		parentContainer = blueTeamPlayersContainer.transform;
	//	}
	//	else if (teamNumber == 2)
	//	{
	//		parentContainer = redTeamPlayersContainer.transform;
	//	}
	//	GameObject newPlayer = Instantiate(playersInGame, parentContainer);
	//	newPlayer.name = className;
	//	switch (className)
	//	{
	//		case "Healer":
	//			newPlayer.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = healerCharacter;
	//			break;
	//		case "Reviver":
	//			newPlayer.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = rammalCharacter;
	//			break;
	//		case "Spearer":
	//			newPlayer.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = sarbazCharacter;
	//			break;
	//		case "Grenader":
	//			newPlayer.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = jalladCharacter;
	//			break;
	//	}

	//}



	//[ClientRpc]
	//private void RPCDisconnectedPlayerNotSpawnedSenedFromMaster(int lostTeamNumber)
	//{
	//	if (lostTeamNumber == 1)
	//	{
	//		gameManager.TeamGroup1RemainedPlayers--;

	//	}
	//	else if (lostTeamNumber == 2)
	//	{
	//		gameManager.TeamGroup2RemainedPlayers--;
	//	}
	//}

	//[ClientRpc]
	//private void RPCSendTheMasterWhenSpawnedThePlayer(int spawnedTeam)
	//{
	//	if (MirrorPlayer.localPlayer.isHost)
	//	{
	//		spawnedPlayers.Add(spawnedTeam);
	//	}
	//}



	//public override void OnPlayerLeftRoom(Player otherPlayer)
	//{
	//	if (otherPlayer != PhotonNetwork.LocalPlayer)
	//	{
	//		int disconnectedPlayerGroup = (int)otherPlayer.CustomProperties["Group"];
	//		if (disconnectedPlayerGroup == 0)
	//		{
	//			return;
	//		}
	//		otherPlayer.CustomProperties["Group"] = 0;
	//		if (disconnectedPlayerGroup == 1)
	//		{
	//			gameManager.TeamGroup1RemainedPlayers--;
	//		}
	//		else if (disconnectedPlayerGroup == 2)
	//		{
	//			gameManager.TeamGroup2RemainedPlayers--;
	//		}

	//		// Player Container
	//		int disconnectedPlayerTeam = (int)otherPlayer.CustomProperties["Team"];
	//		string disconnectedPlayerClass = (string)otherPlayer.CustomProperties["Class"];
	//		if (disconnectedPlayerTeam == 1)
	//		{
	//			foreach (Transform trans in blueTeamPlayersContainer.transform)
	//			{
	//				if (trans.name == disconnectedPlayerClass)
	//				{
	//					Destroy(trans.gameObject);
	//					return;
	//				}
	//			}
	//		}
	//		else if (disconnectedPlayerTeam == 2)
	//		{
	//			foreach (Transform trans in redTeamPlayersContainer.transform)
	//			{
	//				if (trans.name == disconnectedPlayerClass)
	//				{
	//					Destroy(trans.gameObject);
	//					return;
	//				}
	//			}
	//		}
	//	}
	//}


}
