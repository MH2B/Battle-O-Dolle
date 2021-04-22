using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MatchMakerUI : NetworkBehaviour
{

	public static MatchMakerUI instance = null;

	[Header("Spawnables Settings")]
	[SerializeField] private GameObject playerContainer = null;
	[SerializeField] private Transform playerLobby = null;
	private GameObject localSpawnedPlayerContainer = null;

	[Header("Canvas Elements")]
	[SerializeField] private GameObject beforeRoomPanel = null;
	[SerializeField] private GameObject afterRoomPanel = null;
	[SerializeField] private GameObject blueTeamMatchRoomPanel = null;
	[SerializeField] private GameObject redTeamMatchRoomPanel = null;
	[SerializeField] private InputField roomName = null;
	[SerializeField] private Text roomNameText = null;
	[SerializeField] private Text roomMaxPlayerSizeText = null;
	[SerializeField] private GameObject beginTheGameButton = null;
	[HideInInspector] public NetworkMatchChecker matchChecker = null;

	[Header("Heros Info Containers")]
	[SerializeField] private List<Button> herosContainersChooseButtons = new List<Button>();
	[SerializeField] private List<GameObject> herosInfoContainers = new List<GameObject>();
	private GameObject visitedHeroInfoContainer = null;
	private bool canSelectHero = true;

	private int roomMaxPlayerSize = 2;

	// Contract heros selection with other players in the match to avoid selecting a selected hero
	public string previouslySelectedHero = null;
	public SyncList<string> inMatchChoosedHeros = new SyncList<string>();


	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		matchChecker = GetComponent<NetworkMatchChecker>();
	}



	#region Hosting

	public void HostPublicBtn()
	{
		MirrorPlayer.localPlayer.HostARoom(true , roomMaxPlayerSize);
		MirrorPlayer.localPlayer.onSuccesfullyHosted += OnSuccesfullyHosted;
	}


	public void HostPrivateBtn()
	{
		MirrorPlayer.localPlayer.HostARoom(false , roomMaxPlayerSize);
		MirrorPlayer.localPlayer.onSuccesfullyHosted += OnSuccesfullyHosted;
	}

	private void OnSuccesfullyHosted(bool success)
	{
		MirrorPlayer.localPlayer.onSuccesfullyHosted -= OnSuccesfullyHosted;
		if (success)
		{
			// Show the new player in player container
			localSpawnedPlayerContainer = SpawnPlayerContainer(MirrorPlayer.localPlayer);
			MirrorPlayer.localPlayer.localSpawnedPlayerContainer = localSpawnedPlayerContainer.GetComponent<PlayerContainer>();
			localSpawnedPlayerContainer.GetComponent<PlayerContainer>().SetPlayerName(MirrorPlayer.localPlayer.playerName);

			// Change this scripts macth checker to be same as the players match checker
			print(MirrorPlayer.localPlayer.matchID);
			matchChecker.matchId = MirrorPlayer.localPlayer.matchID.ToGuid();

			// Canavas
			roomNameText.text = MirrorPlayer.localPlayer.matchID;
			beforeRoomPanel.SetActive(false);
			afterRoomPanel.SetActive(true);
			blueTeamMatchRoomPanel.SetActive(true);
			beginTheGameButton.SetActive(true);
		}
		else
		{
			print("Sorry we couldnt make the room for you plz try again");
		}
	}



	#endregion


	#region Joining

	public void JoinGivenRoomNameBtn()
	{
		if (string.IsNullOrEmpty(roomName.text))
		{
			print("Enter room name");
			return;
		}
		else
		{
			string roomNameText = roomName.text.ToUpper();
			MirrorPlayer.localPlayer.JoinARoom(roomNameText);
			MirrorPlayer.localPlayer.onSuccesfullyJoined += OnSuccesfullyJoined;
		}
	}

	public void SearchRandomeRoomBtn()
	{
		MirrorPlayer.localPlayer.JoinARoom(null);
		MirrorPlayer.localPlayer.onSuccesfullyJoined += OnSuccesfullyJoined;
	}

	private void OnSuccesfullyJoined(bool success)
	{
		MirrorPlayer.localPlayer.onSuccesfullyJoined -= OnSuccesfullyJoined;
		if (success)
		{
			// Show the new player in player container
			localSpawnedPlayerContainer = SpawnPlayerContainer(MirrorPlayer.localPlayer);
			MirrorPlayer.localPlayer.localSpawnedPlayerContainer = localSpawnedPlayerContainer.GetComponent<PlayerContainer>();
			localSpawnedPlayerContainer.GetComponent<PlayerContainer>().SetPlayerName(MirrorPlayer.localPlayer.playerName);

			// Change this scripts macth checker to be same as the players match checker
			matchChecker.matchId = MirrorPlayer.localPlayer.matchID.ToGuid();

			// Canavas
			roomNameText.text = MirrorPlayer.localPlayer.matchID;
			beforeRoomPanel.SetActive(false);
			afterRoomPanel.SetActive(true);
			if (MirrorPlayer.localPlayer.isInBlueTeam)
			{
				blueTeamMatchRoomPanel.SetActive(true);
			}
			else
			{
				redTeamMatchRoomPanel.SetActive(true);
			}
		}
		else
		{
			print("Sorry we couldnt join the room for you plz try again");
		}
	}

	#endregion


	#region Methods

	public void SetThePlayerAsHost(bool value)
	{
		beginTheGameButton.SetActive(value);
	}


	public bool CompareTeamsForTheNewConnectedClient()
	{
		return MirrorPlayer.localPlayer.isInBlueTeam;
	}

	public GameObject SpawnPlayerContainer(MirrorPlayer spawnerMirrorPlayer)
	{
		// Instantiate a player container then add it to the local players MirrorPlayer and then set the player name and at the last set the child by index order
		GameObject newPlayerContainer = Instantiate(playerContainer, playerLobby);
		//spawnerMirrorPlayer.clientSpawnedPlayerContainer = newPlayerContainer;
		newPlayerContainer.transform.SetSiblingIndex(spawnerMirrorPlayer.playerIndex - 1);
		//spawnerMirrorPlayer.clientSpawnedPlayerContainer.GetComponent<PlayerContainer>().SetPlayerName(spawnerMirrorPlayer.playerName);
		return newPlayerContainer;
	}

	public void ClearTheDisconnectPlayersPlayerContainer(GameObject _playerContainer)
	{
		if(_playerContainer != null)
		{
			Destroy(_playerContainer);
		}
	}

	IEnumerator ChangeHerosChooseButtonActivity(float second , List<Button> _heroSelectButtons)
	{
		canSelectHero = false;
		_heroSelectButtons.ForEach(x => x.interactable = false);

		yield return new WaitForSeconds(second);

		_heroSelectButtons.ForEach(x => x.interactable = true);
		canSelectHero = true;
	}

	private void ClearAllPlayerContainers()
	{
		foreach (Transform playerContainerChild in playerLobby)
		{
			ClearTheDisconnectPlayersPlayerContainer(playerContainerChild.gameObject);
		}
		beforeRoomPanel.SetActive(true);
	}


	#endregion


	#region Other Btns

	public void ChangeRoomMaxPlayerSizeTextBtn(int value)
	{
		roomMaxPlayerSize += value;
		roomMaxPlayerSize = Mathf.Clamp(roomMaxPlayerSize, 2, 10);
		roomMaxPlayerSizeText.text = roomMaxPlayerSize.ToString();
	}

	public void BeginTheGameBtn()
	{
		MirrorPlayer.localPlayer.BeginTheGame();
	}

	public void DisconnectMatchBtn()
	{
		MirrorPlayer.localPlayer.UpdateTheHeroForInMatchSelectedHeros(null , previouslySelectedHero);
		previouslySelectedHero = null;
		MirrorPlayer.localPlayer.ChooseHero(null);
		MirrorPlayer.localPlayer.DisconnectMatch();
		matchChecker.matchId = string.Empty.ToGuid();
		MirrorPlayer.localPlayer.ClearAllOfTheSelectedHerosListAfterDisconnect();

		// Canvas
		Invoke("ClearAllPlayerContainers", 1f);
		afterRoomPanel.SetActive(false);
		blueTeamMatchRoomPanel.SetActive(false);
		redTeamMatchRoomPanel.SetActive(false);
		beginTheGameButton.SetActive(false);
		herosInfoContainers.ForEach(x => x.SetActive(false));
	}

	public void ChooseHero(HeroType _heroType)
	{
		string currentSeletcingHeroName = _heroType.ToString();
		if (canSelectHero && !inMatchChoosedHeros.Contains(currentSeletcingHeroName))
		{
			// Send the Mirror Player to change hero updates in all of the clients and sync it from RPC then turn off the hero selection button for 2 seconds to avoid conflict
			MirrorPlayer.localPlayer.UpdateTheHeroForInMatchSelectedHeros(currentSeletcingHeroName, previouslySelectedHero);
			previouslySelectedHero = currentSeletcingHeroName;
			MirrorPlayer.localPlayer.ChooseHero(currentSeletcingHeroName);
			StartCoroutine(ChangeHerosChooseButtonActivity(2f , herosContainersChooseButtons));
		}
		else
		{
			print("Could not choose hero try again");
		}
	}

	public void HeoInfo(HeroType _heroType)
	{
		// Find which hero's info the player wants to see then show it from the canvas
		print("info for : " + _heroType);
		foreach(GameObject heroInfoContainer in herosInfoContainers)
		{
			// Set all of the infos off because player can open the second info before closing the first one
			herosInfoContainers.ForEach(x => x.SetActive(false));
			if(heroInfoContainer.name == _heroType.ToString())
			{
				visitedHeroInfoContainer = heroInfoContainer;

				visitedHeroInfoContainer.gameObject.SetActive(true);
				return;
			}
		}
	}

	public void CloseHeroInfoBtn()
	{
		visitedHeroInfoContainer.gameObject.SetActive(false);
		visitedHeroInfoContainer = null;
	}

	#endregion





}
