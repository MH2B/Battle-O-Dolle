using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorPlayer : NetworkBehaviour
{
    public static MirrorPlayer localPlayer = null;

    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;
    [SyncVar] public Match currentMatch = null;

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            if(localPlayer == null)
			{
                localPlayer = this;
            }
        }
        else
        {
            print($"Spawning other player UI Prefab");
            //playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab(this);
        }
    }

    public bool HostARoom(bool _isPublic)
    {
        matchID = GetRandomMatchID();
        return MatchMaker.instance.HostARoom(_isPublic , gameObject , matchID, out playerIndex);
    }

    
    public bool JoinARoom(string _roomName)
	{
        return MatchMaker.instance.JoinAGame(_roomName , gameObject, out playerIndex);
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

}
