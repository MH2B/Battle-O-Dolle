using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Poison : NetworkBehaviour, ITrap
{
	[Range(0, 100)] [SerializeField] private float poisingDamage = 10f;
	[Range(0, 100)] [SerializeField] private float poisonEffectRate = 1f;
	[Range(0, 100)] [SerializeField] private float poisonLifeTimeRate = 3f;
	[SerializeField] private GameObject poisonEffectPrefab = null;

	private float timer = 0f;

	private List<GameObject> inRangeEnemies = new List<GameObject>();
	private bool isExecuted = false;


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


	private void Update()
	{
		if (isExecuted)
		{
			timer += Time.deltaTime;
			if (timer >= 1)
			{
				timer = 0f;
				foreach (GameObject go in inRangeEnemies)
				{
					go.GetComponent<IPlayer>().TakeDamage(poisingDamage);
				}
			}
		}
	}

	public void SetTheTag(string team)
	{
		CmdSetTheTag(team);
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
				// If the other game object was in the other team and was a player (BlueTeam -- RedTeam) then add it to the enemies
				if (!inRangeEnemies.Contains(other.gameObject))
				{
					inRangeEnemies.Add(other.gameObject);
				}
				if (!isExecuted)
				{
					ExecutePoisonDamage();
					isExecuted = true;
				}
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (!hasAuthority)
		{
			return;
		}
		if (other.tag != gameObject.tag)
		{
			if (other.tag == "BlueTeam" || other.tag == "RedTeam")
			{
				// If the other game object was in the other team and was a player (BlueTeam -- RedTeam) then if it was in the ranged enemies remove it
				if (inRangeEnemies.Contains(other.gameObject))
				{
					inRangeEnemies.Remove(other.gameObject);
				}

			}
		}
	}

	private void ExecutePoisonDamage()
	{
		MirrorSpawner.instance.SpawnGameObjectindex(poisonEffectPrefab, new Vector3(transform.position.x, transform.position.y, poisonEffectPrefab.transform.position.z), transform.rotation);
		Invoke("DestroyGameObject", poisonLifeTimeRate);
	}

	private void DestroyGameObject()
	{
		MirrorSpawner.instance.DestoyGameObject(gameObject);
	}


	[Command]
	private void CmdSetTheTag(string _team)
	{
		RPCSetTheTag(_team);
	}

	[ClientRpc]
	private void RPCSetTheTag(string team)
	{
		gameObject.tag = team;
	}
}
