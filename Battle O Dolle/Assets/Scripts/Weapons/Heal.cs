using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Heal : NetworkBehaviour
{
	[Range(0, 10)] [SerializeField] private float healEndTimeCounter = 5f;
	[Range(0, 10)] [SerializeField] private float timeEffectRate = 1f;
	[Range(0, 100)] [SerializeField] private float healEffectAmount = 50f;
	[Range(0, 50)] [SerializeField] private float radiousOfAction = 15f;
	[SerializeField] private GameObject healEffectPrefab = null;


	[SerializeField] private LayerMask raycastableForInSightLayerMask = new LayerMask();

	[SerializeField] private LayerMask healableLayerMask = new LayerMask();

	private float timeCounter = 0f;

	private IPlayer playerInterface = null;
	public IPlayer PlayerInterface { get => playerInterface; set => playerInterface = value; }


	private string team = null;

	private void Start()
	{
		if (!hasAuthority)
		{
			Destroy(this);
			return;
		}
		team = playerInterface.TeamGetter();
		Invoke("DestroyGameObject", healEndTimeCounter);
	}

	private void Update()
	{
		timeCounter += Time.deltaTime;
		if(timeCounter >= timeEffectRate)
		{
			timeCounter = 0f;
			Healing();
		}
	}

	private void Healing()
	{
		//PhotonNetwork.Instantiate(healEffectPrefab.name, new Vector3(transform.position.x, transform.position.y, healEffectPrefab.transform.position.z), Quaternion.identity);
		MirrorSpawner.instance.SpawnGameObjectindex(healEffectPrefab, new Vector3(transform.position.x, transform.position.y, healEffectPrefab.transform.position.z), transform.rotation);
		//audioSource.PlayOneShot(healSoundEffect);
		// Finds all players in the radious
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfAction, Vector2.up, 10, healableLayerMask);
		foreach (RaycastHit2D coll in hit)
		{
			// Finds which one of these players are in the sight
			Vector2 dir = coll.collider.gameObject.transform.position - transform.position;
			float distance = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
			RaycastHit2D hittest = Physics2D.Raycast(transform.position, dir.normalized, distance, raycastableForInSightLayerMask);
			if (hittest.collider == null)
			{
				if (coll.collider.gameObject.GetComponent<IPlayer>() != null && coll.collider.gameObject.tag == team)
				{
					playerInterface = coll.collider.gameObject.GetComponent<IPlayer>();
					playerInterface.Heal(healEffectAmount);
					print("Is Healing the " + coll.collider.gameObject.tag);
				}
			}
		}		
	}

	private void DestroyGameObject()
	{
		//PhotonNetwork.Destroy(gameObject);
		MirrorSpawner.instance.DestoyGameObject(gameObject);
	}
}
