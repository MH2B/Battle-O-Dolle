using System;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mirror;
using UnityEngine;


// This is a class that all of the matches we make should be made of so what ever we need to be saved in a match features can be filled here
[Serializable]
public class Match
{
    public string matchID;
    public bool isPublicMatch;
    public bool inMatch;
    public bool isMatchFull;
    public int maxPlayerSize;
    public SyncListGameObject players = new SyncListGameObject();
    public SyncListGameObject blueTeamPlayers = new SyncListGameObject();
    public SyncListGameObject redTeamPlayers = new SyncListGameObject();

    public Match(string matchID, GameObject player, bool isPublicMatch , int maxPlayerSize)
    {
        isMatchFull = false;
        inMatch = false;
        this.matchID = matchID;
        this.isPublicMatch = isPublicMatch;
        this.maxPlayerSize = maxPlayerSize;
        players.Add(player);
    }

    public Match() { }
}


// This will contain all of the gameobjects we want to be saved and synced over clients
[Serializable] public class SyncListGameObject : SyncList<GameObject> { }


// This will contain all of the matches that have made in the server and we want to be saved and synced over clients
[Serializable] public class SyncListMatch : SyncList<Match> { }



public class MatchMaker : NetworkBehaviour
{

    public static MatchMaker instance = null;

    public SyncListMatch matches = new SyncListMatch();
    public SyncList<string> matchIDs = new SyncList<string>();

    [SerializeField] int maxMatchPlayers = 10;



    private void Start()
	{
		if(instance == null)
		{
            instance = this;
		}
    }


    public bool HostARoom(bool _isPublic , GameObject _player, string _matchID ,out int _playerIndex , int maxPlayerSize)
	{
        _playerIndex = -1;

        // See if we have the room hosted by any other player or not then make the match
        if (!matchIDs.Contains(_matchID))
        {
            matchIDs.Add(_matchID);
            Match match = new Match(_matchID, _player, _isPublic , maxPlayerSize);
            match.blueTeamPlayers.Add(_player);
            matches.Add(match);
            print($"Match generated & isPublic is " + _isPublic + " & maxPlayerSize is : " + maxPlayerSize);
            MirrorPlayer instance = _player.GetComponent<MirrorPlayer>();
            instance.isHost = true;
            instance.currentMatch = match;
            instance.matchID = _matchID;
            instance.isInBlueTeam = true;
            _playerIndex = 1;
            instance.playerIndex = _playerIndex;
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
            print("Randome");
            for (int i = 0; i < matches.Count; i++)
            {
                print($"Checking match {matches[i].matchID} | inMatch {matches[i].inMatch} | matchPlayerCount {matches[i].players.Count} | matchPlayerCount {matches[i].maxPlayerSize} | matchFull {matches[i].isMatchFull} | publicMatch {matches[i].isPublicMatch}");
                if (!matches[i].inMatch && !matches[i].isMatchFull && matches[i].isPublicMatch)
                {
                    return JoinAGame(matches[i].matchID, _player, out playerIndex);
                }
            }
            return false;
        }
        // See if we have the room which we are searching for
        else if (matchIDs.Contains(_matchID))
        {
            print("By room name" + _matchID);
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    if (!matches[i].inMatch && !matches[i].isMatchFull)
                    {
                        matches[i].players.Add(_player);
                        MirrorPlayer instance = _player.GetComponent<MirrorPlayer>();
                        // If the blue team is not full add the player to blue team otherwise add him to the red team
                        if (matches[i].blueTeamPlayers.Count < (matches[i].maxPlayerSize / 2))
                        {
                            matches[i].blueTeamPlayers.Add(_player);
                            instance.isInBlueTeam = true;
                        }
                        else
                        {
                            matches[i].redTeamPlayers.Add(_player);
                            instance.isInBlueTeam = false;
                        }
                        instance.currentMatch = matches[i];
                        instance.matchID = _matchID;
                        playerIndex = matches[i].players.Count;
                        instance.playerIndex = playerIndex;

                        // Set the match si full
                        if (matches[i].players.Count == matches[i].maxPlayerSize)
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


    public void PlayerDisconnected(MirrorPlayer _mirrorPlayer, string _matchID)
    {
        // Will be called in the server
        // The player has disconnected so the server has to remove it from the match's player's list which it was in it
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _matchID)
            {
                // Remove from the main players list
                int playerIndex = matches[i].players.IndexOf(_mirrorPlayer.gameObject);
                matches[i].players.RemoveAt(playerIndex);

				// Remove from the blue players list
				if (matches[i].blueTeamPlayers.Contains(_mirrorPlayer.gameObject))
				{
                    int bluePlayerIndex = matches[i].blueTeamPlayers.IndexOf(_mirrorPlayer.gameObject);
                    matches[i].blueTeamPlayers.RemoveAt(bluePlayerIndex);

                    // The player was the host so change the host
                    if (_mirrorPlayer.isHost && bluePlayerIndex == 0)
					{
                        // Blue team is empty now
                        if(matches[i].blueTeamPlayers.Count != 0)
						{
                            matches[i].blueTeamPlayers[0].gameObject.GetComponent<MirrorPlayer>().isHost = true;
                        }
                        // Red team has player so we can host that player
						else if(matches[i].redTeamPlayers.Count != 0)
						{
                            matches[i].redTeamPlayers[0].gameObject.GetComponent<MirrorPlayer>().isHost = true;
                        }
                    }
                }

                // Remove from the red players list
                else if (matches[i].redTeamPlayers.Contains(_mirrorPlayer.gameObject))
                {
                    int redPlayerIndex = matches[i].redTeamPlayers.IndexOf(_mirrorPlayer.gameObject);
                    matches[i].redTeamPlayers.RemoveAt(redPlayerIndex);

                    // The player was the host so change the host
                    if (_mirrorPlayer.isHost && redPlayerIndex == 0)
                    {
                        // Blue team is empty now
                        if (matches[i].blueTeamPlayers.Count != 0)
                        {
                            matches[i].blueTeamPlayers[0].gameObject.GetComponent<MirrorPlayer>().isHost = true;
                        }
                        // Red team has player so we can host that player
                        else if (matches[i].redTeamPlayers.Count != 0)
                        {
                            matches[i].redTeamPlayers[0].gameObject.GetComponent<MirrorPlayer>().isHost = true;
                        }
                    }
                }

                // If match was full now one player has left and it is not full any more so check it
                if (matches[i].isMatchFull)
                {
                    matches[i].isMatchFull = false;
                }

                // Remove the hosting possibility
                _mirrorPlayer.isHost = false;

                print($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");

				

                // If the player was the only player in the macth then we can remove and end the match itself to free the memory of server
                if (matches[i].players.Count == 0)
                {
                    print($"No more players in Match. Terminating {_matchID}");
                    matches.RemoveAt(i);
                    matchIDs.Remove(_matchID);
                }
                break;
            }
        }
    }


    public void BeginTheGame(string _matchID)
    {
        // This is called in server
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _matchID)
            {
                matches[i].inMatch = true;
                foreach (var player in matches[i].players)
                {
                    MirrorPlayer _mirrorPlayer = player.GetComponent<MirrorPlayer>();
                    _mirrorPlayer.StartGame();
                }
                break;
            }
        }
    }


    public List<GameObject> GetTheMatchPlayers(string _matchID)
	{
        List<GameObject> currentMatchPlayers = new List<GameObject>();
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _matchID)
            {
                foreach(GameObject player in matches[i].players)
				{
                    currentMatchPlayers.Add(player);
                }
            }
        }
        return currentMatchPlayers;
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
