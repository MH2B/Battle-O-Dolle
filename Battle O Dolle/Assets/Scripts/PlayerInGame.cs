using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInGame : NetworkBehaviour
{
    public static PlayerInGame instance = null;
	[SyncVar] public PlayersInGameSettings playerInGameSettings;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}



}
