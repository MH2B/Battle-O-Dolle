using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorPlayerInGame : NetworkBehaviour
{

    public static MirrorPlayerInGame localPlayerInGame = null;

    [SerializeField] private GameObject playerPrefab = null;


    public PlayersInGameSettings playerInGameSettings;

    public override void OnStartClient()
    {
        // Setting the singleton but this will be setted just for the local player because we will acces thsi class only from the client so we dont want to bother scripts
        if (isLocalPlayer)
        {
            if (localPlayerInGame == null)
            {
                localPlayerInGame = this;
            }
        }
    }

    public void StartSpawning(Transform startPos)
	{
        playerInGameSettings = AutoHostClient.instance.playerInGameSettings;
        CmdSpawn(startPos , gameObject , playerInGameSettings);
	}

    [Command(ignoreAuthority = true)]
    void CmdSpawn(Transform startPos , GameObject owner , PlayersInGameSettings _playerInGameSettings)
	{
        print("Spawning in game player");
		GameObject player = startPos != null
			? Instantiate(playerPrefab, startPos.position, startPos.rotation)
			: Instantiate(playerPrefab);
        
        player.GetComponent<NetworkMatchChecker>().matchId = _playerInGameSettings.matchID.ToGuid();
        player.GetComponent<PlayerInGame>().playerInGameSettings = _playerInGameSettings;

        NetworkServer.Spawn(player , owner);
	}

}
