using System;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEngine;


// This is a class that all of the matches we make should be made of so what ever we need to be saved in a match features can be filled here
[System.Serializable]
public class Match
{
    public string matchID;
    public bool isPublicMatch;
    public bool inMatch;
    public bool isMatchFull;
    public SyncListGameObject players = new SyncListGameObject();

    public Match(string matchID, GameObject player, bool publicMatch)
    {
        isMatchFull = false;
        inMatch = false;
        this.matchID = matchID;
        this.isPublicMatch = publicMatch;
        players.Add(player);
    }

    public Match() { }
}


// This will contain all of the gameobjects we want to be saved and synced over clients
[System.Serializable] public class SyncListGameObject : SyncList<GameObject> { }


// This will contain all of the matches that have made in the server and we want to be saved and synced over clients
[System.Serializable] public class SyncListMatch : SyncList<Match> { }



public class MatchMaker : NetworkBehaviour
{

    public static MatchMaker instance = null;

    public SyncListMatch matches = new SyncListMatch();
    public SyncList<string> matchIDs = new SyncList<string>();

    [SerializeField] int maxMatchPlayers = 12;

    private void Start()
	{
		if(instance == null)
		{
            instance = this;
		}
	}


    public bool HostARoom(bool _isPublic , GameObject _player, string _matchID ,out int _playerIndex)
	{
        _playerIndex = -1;

        // See if we have the room hosted by any other player or not then make the match
        if (!matchIDs.Contains(_matchID))
        {
            matchIDs.Add(_matchID);
            Match match = new Match(_matchID, _player, _isPublic);
            matches.Add(match);
            print($"Match generated");
            _player.GetComponent<MirrorPlayer>().currentMatch = match;
            _playerIndex = 1;
            return true;
        }
        else
        {
            print($"Match ID already exists");
            return false;
        }
    }


    public bool JoinAGame(string _matchID, GameObject _player, out int playerIndex)
    {
        playerIndex = -1;

        // See if we are joining a randome room then try to join any of the rooms that are possible to be joined
        if(_matchID == null)
		{
            _matchID = "";

            for (int i = 0; i < matches.Count; i++)
            {
                print($"Checking match {matches[i].matchID} | inMatch {matches[i].inMatch} | matchFull {matches[i].isMatchFull} | publicMatch {matches[i].isPublicMatch}");
                if (!matches[i].inMatch && !matches[i].isMatchFull && matches[i].isPublicMatch)
                {
                    if (JoinAGame(matches[i].matchID, _player, out playerIndex))
                    {
                        _matchID = matches[i].matchID;
                        return true;
                    }
                }
            }
        }
        // See if we have the room which we are searching for
        else if (matchIDs.Contains(_matchID))
        {

            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    if (!matches[i].inMatch && !matches[i].isMatchFull)
                    {
                        matches[i].players.Add(_player);
                        _player.GetComponent<MirrorPlayer>().currentMatch = matches[i];
                        playerIndex = matches[i].players.Count;

                        if (matches[i].players.Count == maxMatchPlayers)
                        {
                            matches[i].isMatchFull = true;
                        }

                        break;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            print($"Match joined");
            return true;
        }
        print($"Match ID does not exist");
        return false;
    }



}

public static class MatchExtensions
{

    // This will be needed when we want to use the match ID as NetworkMatchChecker component value 
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
