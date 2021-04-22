using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public struct PlayersInGameSettings
{
    public string playerName;
    public string heroName;
    public string matchID;
    public bool isInBlueTeam;
    public int playerIndex;
    public int roundWon;
    public bool roundIsInBlueTeam;
    public int matchPlayerSize;

    public void SetTheSetting(string _playerName , string _heroName , string _matchID , bool _isInBlueTeam, int _playerIndex , int _roundWon, bool _roundIsInBlueTeam , int _matchPlayerSize)
	{
        playerName = _playerName;
        heroName = _heroName;
        matchID = _matchID;
        isInBlueTeam = _isInBlueTeam;
        playerIndex = _playerIndex;
        roundWon = _roundWon;
        roundIsInBlueTeam = _roundIsInBlueTeam;
        matchPlayerSize = _matchPlayerSize;
    }
}

public class MirrorPlayer : NetworkBehaviour
{
    public static MirrorPlayer localPlayer = null;
    private NetworkMatchChecker matchChecker = null;

    [Header("Scene Settings")]
    [Scene] [SerializeField] private string gameLevelScene = null;

    // Match settings
    [Header("Match Settings")]
    [SyncVar] public string matchID;
    [SyncVar] public int matchPlayerSize;
    [SyncVar] public int playerIndex;
    [SyncVar] public bool isInBlueTeam = false;
    [SyncVar(hook = nameof(HandleIsHostHook))] public bool isHost = false;
    [SyncVar] public Match currentMatch = null;
    [SyncVar] public string playerName = null;
    public PlayersInGameSettings thisPlayerInGameSettings;

    [Header("Spawned Player Containers")]
    [SerializeField] public GameObject clientSpawnedPlayerContainer = null;
    [SerializeField] public PlayerContainer localSpawnedPlayerContainer = null;
    [SyncVar(hook = nameof(HandleIsHeroSelectedHook))] public string selectedHero = null;

    private bool isInMatch = false;
    private string currentHero = null;

    // Delegates
    public delegate void OnSuccesfullyMatchMaking(bool success);
    public OnSuccesfullyMatchMaking onSuccesfullyHosted;
    public OnSuccesfullyMatchMaking onSuccesfullyJoined;


    private void Awake()
	{
        matchChecker = GetComponent<NetworkMatchChecker>();
    }


    #region Call Backs & Their Methods

    public override void OnStartClient()
    {
        // Setting the singleton but this will be setted just for the local player because we will acces thsi class only from the client so we dont want to bother scripts
        if (isLocalPlayer)
        {
            if (localPlayer == null)
            {
                localPlayer = this;
                DontDestroyOnLoad(gameObject);
            }
            // Set the player's name
            CmdChangePlayerName(PlayerPrefsManager.GetThePlayerName());
            print($"Local Mirror Player");
            print("L : " + isInBlueTeam);
        }
        else
        {
            Invoke("InvokeNotLocalClientStartded", 1);
        }
    }


    private void InvokeNotLocalClientStartded()
	{
        print("C : " + isInBlueTeam + " & Compare : " + MatchMakerUI.instance.CompareTeamsForTheNewConnectedClient());
        // The script is not for the local player so its is a client in the match and we are going to spawn a player container for that and set its name
        if (isInBlueTeam == MatchMakerUI.instance.CompareTeamsForTheNewConnectedClient())
        {
            print("This is a client of our team so trying to spawn the not local player's playercontainer");
            clientSpawnedPlayerContainer = MatchMakerUI.instance.SpawnPlayerContainer(this);
            clientSpawnedPlayerContainer.GetComponent<PlayerContainer>().SetPlayerName(playerName);
        }
        else
        {
            print("This is a client of opponent team so do nothing");
        }
    }

    [Command]
    void CmdChangePlayerName(string _playerName)
	{
        playerName = _playerName;
    }

    public override void OnStopClient()
    {
		if (!isInMatch)
		{
            print("Client Stoped");
            ClientDisconnected();
        }
    }

    public override void OnStopServer()
    {
		if (!isInMatch)
		{
            print($"Client Stopped on Server");
            ServerDisconnect();
        }
    }

    private void ClientDisconnected()
    {
        // This means that if this is a client from the lcoal player in the match now we have to destroy the player cintainer we had spawned for it
        if (clientSpawnedPlayerContainer != null)
        {
            Destroy(clientSpawnedPlayerContainer);
        }
    }


    public void DisconnectMatch()
    {
        CmdDisconnectMatch();
    }

    [Command]
    void CmdDisconnectMatch()
    {
        ServerDisconnect();
    }

    private void ServerDisconnect()
    {
        MatchMaker.instance.PlayerDisconnected(this, matchID);
        RpcDisconnectGame();
        matchChecker.matchId = string.Empty.ToGuid();
    }

    [ClientRpc]
    void RpcDisconnectGame()
    {
        ClientDisconnected();
    }

    
    private void HandleIsHostHook(bool oldValue , bool newValue)
	{
		if (isLocalPlayer)
		{
            isHost = newValue;
            MatchMakerUI.instance.SetThePlayerAsHost(newValue);
		}
	}

    #endregion


    #region Hosting

    public void HostARoom(bool _isPublic , int roomMaxPlayerSize)
    {
        CmdHostARoom(_isPublic , roomMaxPlayerSize);
    }

    [Command]
    void CmdHostARoom(bool _isPublic , int roomMaxPlayerSize)
    {
        bool isSuccessed = false;
        matchID = GetRandomMatchID();
        isSuccessed = MatchMaker.instance.HostARoom(_isPublic, gameObject, matchID, out playerIndex , roomMaxPlayerSize);
        if (isSuccessed)
        {
            matchChecker.matchId = matchID.ToGuid();
        }
        TRpcSuccesfullyHosted(isSuccessed , playerIndex);
    }

    [TargetRpc]
    void TRpcSuccesfullyHosted(bool success , int _playerIndex)
    {
        playerIndex = _playerIndex;
        StartCoroutine(LateTRpcSuccesfullyHosted(1f, success));
    }

    IEnumerator LateTRpcSuccesfullyHosted(float seconds, bool success)
    {
        yield return new WaitForSeconds(seconds);
        // Send the action to UI and show the new added player
        if (onSuccesfullyHosted != null)
        {
            onSuccesfullyHosted.Invoke(success);
        }
    }

    #endregion


    #region Joining

    public void JoinARoom(string _roomName)
    {

        CmdJoinARoom(_roomName , gameObject);
    }

    [Command]
    void CmdJoinARoom(string _roomName , GameObject _player)
    {
        bool isSuccessed = MatchMaker.instance.JoinAGame(_roomName, _player, out playerIndex);
        
        if (isSuccessed)
        {
            matchChecker.matchId = matchID.ToGuid();
        }
        TRpcSuccesfullyJoined(isSuccessed , playerIndex);
    }

    [TargetRpc]
    void TRpcSuccesfullyJoined(bool success , int _playerIndex)
    {
        playerIndex = _playerIndex;
        StartCoroutine(LateTRpcSuccesfullyJoined(1f , success));
    }


    IEnumerator LateTRpcSuccesfullyJoined(float seconds , bool success)
	{
        yield return new WaitForSeconds(seconds);
        // Send the action to UI and show the new added player
        if (onSuccesfullyJoined != null)
        {
            onSuccesfullyJoined.Invoke(success);
        }
    }


    #endregion


    #region HeroSelecting

    public void HandleIsHeroSelectedHook(string oldValue , string newValue)
	{
        // We do this just for late joining players to know what has happened for hero selection before they join had joined
        StartCoroutine(LateHandleIsHeroSelectedHook(1f, newValue));
    }


    IEnumerator LateHandleIsHeroSelectedHook(float second , string heroName)
	{
        // The local player will send this hook and the client will recieve it and then try to update the client spawned player container with passed informations
        yield return new WaitForSeconds(second);
        if (heroName != null)
        {
            if (!isLocalPlayer)
            {
                if (clientSpawnedPlayerContainer != null)
                {
                    clientSpawnedPlayerContainer.GetComponent<PlayerContainer>().SetPlayerHero(heroName);
                    CmdUpdateTheHeroForInMatchSelectedHeros(heroName , null , false);
                }
            }
        }
    }

    public void ClearAllOfTheSelectedHerosListAfterDisconnect()
    {
        CmdClearAllOfTheSelectedHerosListAfterDisconnect();
    }

    [Command]
    void CmdClearAllOfTheSelectedHerosListAfterDisconnect()
    {
        TRpcClearAllOfTheSelectedHerosListAfterDisconnect();
    }

    [TargetRpc]
    void TRpcClearAllOfTheSelectedHerosListAfterDisconnect()
    {
        ClearAllOfTheSelectedHerosListAfterDisconnectForTarget();
    }

    private void ClearAllOfTheSelectedHerosListAfterDisconnectForTarget()
    {
        MatchMakerUI.instance.inMatchChoosedHeros.Clear();
    }

    public void UpdateTheHeroForInMatchSelectedHeros(string _selectedHero, string _previouslySelectedHeroName)
    {
        currentHero = _selectedHero;
        CmdUpdateTheHeroForInMatchSelectedHeros(_selectedHero, _previouslySelectedHeroName , false);
    }

   
    [Command(ignoreAuthority = true)]
    public void CmdUpdateTheHeroForInMatchSelectedHeros(string _selectedHero, string _previouslySelectedHeroName, bool lateJoin)
    {
        // we will se if the call is from a late join or not if it is from a late join then we dont need to call Rpc for everyone just for itself is enough
        print("Syncing hero in server");
		if (lateJoin)
		{
            TRpcUpdateTheHeroForInMatchSelectedHeros(_selectedHero, _previouslySelectedHeroName);
        }
        else
		{
            RpcUpdateTheHeroForInMatchSelectedHeros(_selectedHero, _previouslySelectedHeroName);
        }
    }


    [ClientRpc]
    void RpcUpdateTheHeroForInMatchSelectedHeros(string _selectedHero, string _previouslySelectedHeroName)
    {
        SyncingTheHerosSelectionInformations(_selectedHero, _previouslySelectedHeroName);
    }

    [TargetRpc]
    void TRpcUpdateTheHeroForInMatchSelectedHeros(string _selectedHero, string _previouslySelectedHeroName)
    {
        SyncingTheHerosSelectionInformations(_selectedHero, _previouslySelectedHeroName);
    }

    private void SyncingTheHerosSelectionInformations(string _selectedHero, string _previouslySelectedHeroName)
	{
        // Check if the client is in our team then update the hero selection
        if(clientSpawnedPlayerContainer != null || isLocalPlayer)
		{
            // Remove the previous selected hero from the selected heros name
            if (_previouslySelectedHeroName != null)
            {
                if (MatchMakerUI.instance.inMatchChoosedHeros.Contains(_previouslySelectedHeroName))
                {
                    print("Removing the hero : " + _previouslySelectedHeroName);
                    MatchMakerUI.instance.inMatchChoosedHeros.Remove(_previouslySelectedHeroName);
                }

            }
            // Add the new selected hero to the selected heros name
            if (_selectedHero != null)
            {
                if (!MatchMakerUI.instance.inMatchChoosedHeros.Contains(_selectedHero))
                {
                    print("Adding the hero : " + _selectedHero);
                    MatchMakerUI.instance.inMatchChoosedHeros.Add(_selectedHero);
                }
            }
        }
    }


    public void ChooseHero(string _heroType)
	{
        // Update the selected heros image for all of the RPcs and change the sync var to notify the late joiners
        CmdChooseHero(_heroType);
    }


    [Command]
    void CmdChooseHero(string _heroType)
	{
        // Try to make a hook
        selectedHero = _heroType;

        // Update what ever has happened
        RpcChooseHero(_heroType);
	}

    [ClientRpc]
    void RpcChooseHero(string _heroType)
	{
        // If this is the local client then we have to change the local spawned player contaienr otherwise we have to change the client spawned player container
		if (isLocalPlayer)
		{
            if(localSpawnedPlayerContainer != null)
			{
                localSpawnedPlayerContainer.GetComponent<PlayerContainer>().SetPlayerHero(_heroType);
            }
        }
		else
		{
            if (clientSpawnedPlayerContainer != null)
            {
                clientSpawnedPlayerContainer.GetComponent<PlayerContainer>().SetPlayerHero(_heroType);
            }
        }
	}

    #endregion


    #region Starting The Game

    public void StartGame()
    {
        TRpcBeginGame();
    }

    [TargetRpc]
    void TRpcBeginGame()
    {
        print($"MatchID: {matchID} | Beginning");
        thisPlayerInGameSettings = new PlayersInGameSettings {};
        thisPlayerInGameSettings.SetTheSetting(playerName, currentHero , matchID , isInBlueTeam , GetTheIndex() , 0 , isInBlueTeam , matchPlayerSize);
        AutoHostClient.instance.playerInGameSettings = thisPlayerInGameSettings;
        isInMatch = true;
        SceneManager.LoadScene(gameLevelScene);
    }
    
    public void BeginTheGame()
    {
        CmdBeginTheGame();
    }

    [Command]
    void CmdBeginTheGame()
    {
        MatchMaker.instance.BeginTheGame(matchID);
    }

    #endregion



    #region Methods

    private int GetTheIndex()
    {
        int index = 0;
        Transform containerParetnt = GameObject.FindGameObjectWithTag("UI").transform.GetChild(1).GetChild(3).GetChild(0).transform;
        foreach (Transform container in containerParetnt)
        {
            if (container.gameObject == localSpawnedPlayerContainer.gameObject)
            {
                return index;
            }
            index++;
        }
        return 0;
    }


    private string GetRandomMatchID()
    {
        string _id = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            int random = UnityEngine.Random.Range(0, 36);
            if (random < 26)
            {
                _id += (char)(random + 65);
            }
            else
            {
                _id += (random - 26).ToString();
            }
        }
        print($"Random Match ID: {_id}");
        return _id;
    }

    #endregion



}
