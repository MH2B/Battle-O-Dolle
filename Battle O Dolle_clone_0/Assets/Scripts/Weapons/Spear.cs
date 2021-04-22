using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spear : NetworkBehaviour
{

	private IPlayer playerInterface = null;
	public IPlayer PlayerInterface { get => playerInterface; set => playerInterface = value; }

	[Range(0, 5000)] [SerializeField] private float throwSpeed = 2500f;

	[SerializeField] private GameObject explosionEffectPrefab = null;


	[Range(0, 100)] [SerializeField] private float hitDamage = 50f;

	private Vector2 aimingDirection = Vector2.zero;
	public Vector2 AimingDirection { get => aimingDirection; set => aimingDirection = value; }

	private Rigidbody2D rb = null;

	private string team = null;
	private bool waitForSpawn = true;

	public override void OnStartClient()
	{
		if (!hasAuthority)
		{
			Destroy(this);
			return;
		}
		Invoke("WaitForSpawn", 0.2f);
		
	}

	private void WaitForSpawn()
	{
		Invoke("DestroyGameObject", 5f);
		team = playerInterface.TeamGetter();
		rb = GetComponent<Rigidbody2D>();
		waitForSpawn = false;
	}


	private void Update()
	{
		if(waitForSpawn)
		{
			return;
		}
		rb.AddForce(AimingDirection * throwSpeed * Time.deltaTime);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!hasAuthority || waitForSpawn)
		{
			return;
		}
		if (other.tag != team)
		{
			if(other.tag == "BlueTeam" || other.tag == "RedTeam")
			{
				other.gameObject.GetComponent<IPlayer>().TakeDamage(hitDamage);
				//PhotonNetwork.Instantiate(explosionEffectPrefab.name, new Vector3(transform.position.x, transform.position.y, explosionEffectPrefab.transform.position.z), Quaternion.identity);
				MirrorSpawner.instance.SpawnGameObjectindex(explosionEffectPrefab, new Vector3(transform.position.x, transform.position.y, explosionEffectPrefab.transform.position.z), transform.rotation);
				DestroyGameObject();
			}
			else if(other.tag == "Environment")
			{
				DestroyGameObject();
			}
		}
	}

	private void DestroyGameObject()
	{
		//PhotonNetwork.Destroy(gameObject);
		MirrorSpawner.instance.DestoyGameObject(gameObject);
	}

}
