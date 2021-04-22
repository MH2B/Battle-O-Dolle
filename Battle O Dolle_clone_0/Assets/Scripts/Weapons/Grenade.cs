using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Grenade : NetworkBehaviour
{

	[Range(0 , 10)] [SerializeField] private float explosionTimeCount = 5f;
	[Range(0, 100)] [SerializeField] private float explosionDamage = 50f;
	[Range(0, 10)] [SerializeField] private float radiousOfAction = 5f;
	[Range(0, 5000)] [SerializeField] private float throwSpeed = 1200f;

	[SerializeField] private AudioClip explosionSoundEffect = null;
	private AudioSource audioSource = null;

	[SerializeField] private GameObject explosionEffectPrefab = null;

	[SerializeField] private LayerMask raycastableForInSightLayerMask = new LayerMask();
	[SerializeField] private LayerMask damagableLayerMask = new LayerMask();


	private IPlayer playerInterface = null;
	public IPlayer PlayerInterface { get => playerInterface; set => playerInterface = value; }

	private Vector2 aimingDirection = Vector2.zero;
	public Vector2 AimingDirection { get => aimingDirection; set => aimingDirection = value; }

	private string team = null;

	private Rigidbody2D rb = null;


	private void Start()
	{
		if (!hasAuthority)
		{
			Destroy(this);
			return;
		}
		audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
		team = PlayerInterface.TeamGetter();
		Invoke("Explode", explosionTimeCount);
		rb = GetComponent<Rigidbody2D>();
		rb.AddForce(AimingDirection * throwSpeed, ForceMode2D.Force);
	}

	private void Explode()
	{
		// Finds all players in the radious
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfAction, Vector2.up, 10, damagableLayerMask);
		foreach (RaycastHit2D coll in hit)
		{
			// Finds which one of these players are in the sight
			Vector2 dir = coll.collider.gameObject.transform.position - transform.position;
			float distance = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
			RaycastHit2D hittest = Physics2D.Raycast(transform.position, dir.normalized, distance, raycastableForInSightLayerMask);
			if (hittest.collider == null)
			{
				if (coll.collider.gameObject.GetComponent<IPlayer>() != null && coll.collider.gameObject.tag != team)
				{
					playerInterface = coll.collider.gameObject.GetComponent<IPlayer>();
					playerInterface.TakeDamage(explosionDamage);
				}
			}
		}

		DestroyGameObject(gameObject);
	}

	private void DestroyGameObject(GameObject go)
	{
		//PhotonNetwork.Instantiate(explosionEffectPrefab.name, new Vector3(transform.position.x , transform.position.y ,explosionEffectPrefab.transform.position.z), Quaternion.identity);
		MirrorSpawner.instance.SpawnGameObjectindex(explosionEffectPrefab, new Vector3(transform.position.x, transform.position.y, explosionEffectPrefab.transform.position.z), transform.rotation);
		//PhotonNetwork.Destroy(go.gameObject);
		MirrorSpawner.instance.DestoyGameObject(go.gameObject);
	}

}
