using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Mine : NetworkBehaviour , ITrap
{

	[Range(0, 100)] [SerializeField] private float explosionDamage = 50f;

	[SerializeField] private GameObject explosionEffectPrefab = null;

	public override void OnStartClient()
	{
		if (!hasAuthority)
		{
			Invoke("WaitForSpawn", 0.3f);
		}
	}


	private void WaitForSpawn()
	{
		// If the mine is in the the enemy game then turn off the sprite renderer
		bool isInBlueTeam = MirrorPlayer.localPlayer.thisPlayerInGameSettings.roundIsInBlueTeam;
		if ((gameObject.tag == "BlueTeam" && !isInBlueTeam) || (gameObject.tag == "RedTeam" && isInBlueTeam))
		{
			GetComponent<SpriteRenderer>().enabled = false;
		}
		Destroy(this);
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
			if (other.tag == "BlueTeam" || other.tag == "RedTeam")
			{
				// If the other was in the other team and was a player (BlueTeam -- RedTeam) then explode the mine
				other.gameObject.GetComponent<IPlayer>().TakeDamage(explosionDamage);
				//PhotonNetwork.Instantiate(explosionEffectPrefab.name, new Vector3(transform.position.x, transform.position.y, explosionEffectPrefab.transform.position.z) , Quaternion.identity);
				MirrorSpawner.instance.SpawnGameObjectindex(explosionEffectPrefab, new Vector3(transform.position.x, transform.position.y, explosionEffectPrefab.transform.position.z), transform.rotation);
				DestroyGameObject();
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
