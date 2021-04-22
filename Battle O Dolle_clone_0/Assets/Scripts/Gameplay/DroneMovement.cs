using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class DroneMovement : MovementAbstract
{

	private GameObject playerGameObject = null;

	public GameObject PlayerGameObject { get => playerGameObject; set => playerGameObject = value; }

	public override void Start()
	{
		base.Start();
	}

	public override void Update()
	{
		base.Update();
	}
	
	public void SetToCharacter()
	{
		//photonView.RPC("ChangeCharacter", RpcTarget.AllBuffered, playerGameObject.GetComponent<PlayerMovement>().photonView.ViewID);
		CmdChangeCharacter(PlayerGameObject);
		//PhotonNetwork.Destroy(gameObject);
		MirrorSpawner.instance.DestoyGameObject(gameObject);
	}

	[Command]
	private void CmdChangeCharacter(GameObject player)
	{
		RPCChangeCharacter(player);
	}

	[ClientRpc]
	private void RPCChangeCharacter(GameObject player)
	{
		SetActivity(player, true);
	}

	private void SetActivity(GameObject go, bool activity)
	{
		go.SetActive(activity);
	}

}
