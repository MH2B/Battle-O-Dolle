using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class GhostMovement : MovementAbstract
{
	private GameObject playerGameObject = null;

	public GameObject PlayerGameObject { get => playerGameObject; set => playerGameObject = value; }


	public override void Start()
	{
		if (!hasAuthority)
		{
			Destroy(gameObject);
			return;
		}
		base.Start();
	}

	public override void Update()
	{
		base.Update();
	}

	public void SetToCharacter()
	{
		//photonView.RPC("ChangeCharacter", RpcTarget.AllBuffered, playerGameObject.GetComponent<PlayerMovement>().photonView.ViewID);
		CmdChangeCharacter(playerGameObject);
		//PhotonNetwork.Destroy(gameObject);
		MirrorSpawner.instance.DestoyGameObject(gameObject);
	}


	[Command]
	private void CmdChangeCharacter(GameObject _playerGameObject)
	{
		RPCChangeCharacter(_playerGameObject);
	}

	[ClientRpc]
	private void RPCChangeCharacter(GameObject _playerGameObject)
	{
		SetActivity(_playerGameObject, true);
	}

	private void SetActivity(GameObject go, bool activity)
	{
		go.SetActive(activity);
	}
}
