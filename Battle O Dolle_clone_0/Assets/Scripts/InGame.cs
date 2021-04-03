using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InGame : NetworkBehaviour
{

    public static InGame instance = null;


	[Header("Spawn Settings")]
	[SerializeField] private Transform startPos = null;


	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}


	private void Start()
	{
		MirrorPlayerInGame.localPlayerInGame.StartSpawning(startPos);
	}


}
