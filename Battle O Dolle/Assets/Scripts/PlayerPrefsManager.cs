using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{

    public const string playerName = "playerName";

    public static void SetThePlayerName(string _playerName)
	{
		PlayerPrefs.SetString(playerName, _playerName);
	}

	public static string GetThePlayerName()
	{
		return PlayerPrefs.GetString(playerName);
	}

}
