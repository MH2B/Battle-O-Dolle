using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BearTrap : NetworkBehaviour , ITrap
{
	[Range(0, 100)] [SerializeField] private float bearTrapDamage = 30f;
	[Range(0, 100)] [SerializeField] private float bearTrapLifeTimeRate = 5f;
	[Range(0, 100)] [SerializeField] private float bearTrapStuckTimeRate = 5f;

	[SerializeField] private GameObject bearTrapEffectPrefab = null;

	private bool isExecuted = false;


	private void Start()
	{
		if (!hasAuthority)
		{

			// If the mine is in the the enemy game then turn off the sprite renderer
			bool isInBlueTeam = MirrorPlayer.localPlayer.thisPlayerInGameSettings.roundIsInBlueTeam;
			if ((gameObject.tag == "BlueTeam" && !isInBlueTeam) || (gameObject.tag == "RedTeam" && isInBlueTeam))
			{
				GetComponent<SpriteRenderer>().enabled = false;
			}
			Destroy(this);
			return;
		}
	}



	public void SetTheTag(string team)
	{
		//photonView.RPC("RPCSetTheTag", RpcTarget.AllBuffered, team);
		CmdSetTheTag(team);
	}

	[Command]
	private void CmdSetTheTag(string _team)
	{
		RPCSetTheTag(_team);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!hasAuthority)
		{
			return;
		}

		if (other.tag != gameObject.tag)
		{
			if (other.tag == "BlueTeam" || other.tag == "RedTeam" && !isExecuted)
			{
				// If the other was in the other team and was a player (BlueTeam -- RedTeam) then explode the mine
				other.gameObject.GetComponent<IPlayer>().TakeDamage(bearTrapDamage);
				other.gameObject.GetComponent<IPlayer>().StuckPlayer(bearTrapStuckTimeRate);
				//PhotonNetwork.Instantiate(bearTrapEffectPrefab.name, new Vector3(transform.position.x, transform.position.y, bearTrapEffectPrefab.transform.position.z), Quaternion.identity);
				MirrorSpawner.instance.SpawnGameObjectindex(bearTrapEffectPrefab, new Vector3(transform.position.x, transform.position.y, bearTrapEffectPrefab.transform.position.z), transform.rotation);
				isExecuted = true;
				Invoke("DestroyGameObject", bearTrapLifeTimeRate);
			}
		}
	}

	private void DestroyGameObject()
	{
		//PhotonNetwork.Destroy(gameObject);
		MirrorSpawner.instance.DestoyGameObject(gameObject);
	}


	[ClientRpc]
	private void RPCSetTheTag(string team)
	{
		gameObject.tag = team;
	}
}
