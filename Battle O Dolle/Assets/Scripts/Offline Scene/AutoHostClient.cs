using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class AutoHostClient : NetworkManager
{
    public static AutoHostClient instance = null;

    private PlayersInGameSettings _playerInGameSettings;
    public PlayersInGameSettings playerInGameSettings { get => _playerInGameSettings; set => _playerInGameSettings = value; }

    #region Methods


    public override void Awake()
	{
		if(instance == null)
		{
            instance = this;
		}
	}

    public void StartTheHostingOrClienting()
    {
        // Connect the player to server
        if (!Application.isBatchMode)
        { //Headless build
            print($"=== Client Build ===");
            StartClient();
        }
        else
        {
            print($"=== Server Build ===");
        }
    }

    public void BeginTheGame(string sceneName)
	{
        ServerChangeScene(sceneName);
	}



	#endregion


	#region Call Backs

	public override void OnClientDisconnect(NetworkConnection conn)
    {
        // If there wasnt any host in the server we have to make the player host but it will be have some changes in matchmaking system
        base.OnClientDisconnect(conn);
        print("We have not found a host so we are going to be the one");
        StartHost();
    }

    #endregion



}