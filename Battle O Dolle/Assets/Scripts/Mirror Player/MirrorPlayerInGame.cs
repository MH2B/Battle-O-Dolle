using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorPlayerInGame : NetworkBehaviour
{

    public static MirrorPlayerInGame localPlayerInGame = null;

    [SerializeField] private List<GameObject> playersPrefabs = new List<GameObject>();


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

    public void StartSpawning()
	{
        playerInGameSettings = GetComponent<MirrorPlayer>().thisPlayerInGameSettings;
        InGame.instance.playerInGameSettings = playerInGameSettings;
        print("index :" + playerInGameSettings.playerIndex + " --- " + "blueteam :" + playerInGameSettings.isInBlueTeam + " --- " + "heroname :" + playerInGameSettings.heroName);
        Transform spawnPos = null;
        if (playerInGameSettings.isInBlueTeam)
        {
            spawnPos = InGame.instance.blueTeamStartPoss[playerInGameSettings.playerIndex];
        }
		else
		{
            spawnPos = InGame.instance.redTeamStartPoss[playerInGameSettings.playerIndex];
        }

        // Finding the selected hero prefab to spawn
        int selectedHeroIndex = 0;
        for(int i = 0; i < playersPrefabs.Count; i++)
		{
            if(playersPrefabs[i].name == playerInGameSettings.heroName)
			{
                selectedHeroIndex = i;
                break;
			}
		}
        print(spawnPos.localPosition);
        CmdSpawn(selectedHeroIndex, spawnPos.localPosition , gameObject , playerInGameSettings);
	}

    [Command(ignoreAuthority = true)]
    void CmdSpawn(int selectedHeroIndex, Vector3 _spawnPos, GameObject owner, PlayersInGameSettings _playerInGameSettings , NetworkConnectionToClient conn = null)
    {
        print("******************** Spawning in game player ****************************" + conn);
        GameObject player = Instantiate(playersPrefabs[selectedHeroIndex], _spawnPos, Quaternion.identity);

        if (_playerInGameSettings.roundIsInBlueTeam)
        {
            player.GetComponent<IPlayer>().TeamSetter("BlueTeam");
        }
        else
        {
            player.GetComponent<IPlayer>().TeamSetter("RedTeam");
        }

        player.GetComponent<NetworkMatchChecker>().matchId = _playerInGameSettings.matchID.ToGuid();
        player.GetComponent<PlayerInGame>().playerInGameSettings = _playerInGameSettings;
        
        NetworkServer.ReplacePlayerForConnection(conn, player);
    }

}
