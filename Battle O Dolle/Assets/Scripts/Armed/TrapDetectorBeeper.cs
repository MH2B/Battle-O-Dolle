using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TrapDetectorBeeper : NetworkBehaviour
{


	[Range(0, 10)] [SerializeField] private float radiousOfAction = 10f;
	[SerializeField] private LayerMask raycastableForInSightLayerMask;
	[SerializeField] private LayerMask detectableLayerMask;
	PlayerMatchData playerMatchData = null;

	private float timeCountDown = 1f;
	private float timer = 0f;

	private void Start()
	{
		playerMatchData = GetComponent<PlayerMatchData>();
		raycastableForInSightLayerMask = playerMatchData.raycastableForInSightLayerMask;
		detectableLayerMask = playerMatchData.detectableLayerMask;
	}

	private void Update()
	{
		//if (!photonView.IsMine)
		//{
		//	return;
		//}
		timer += Time.deltaTime;
		if(timer >= timeCountDown)
		{
			timer = 0f;
			Detect();
		}
	}


	private void Detect()
	{
		// Finds all players in the radious
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfAction, Vector2.up, 10 , detectableLayerMask);
		foreach (RaycastHit2D coll in hit)
		{
			// Finds which one of these players are in the sight
			Vector2 dir = coll.collider.gameObject.transform.position - transform.position;
			float distance = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
			RaycastHit2D hittest = Physics2D.Raycast(transform.position, dir.normalized, distance , raycastableForInSightLayerMask);
			if (hittest.collider == null)
			{
				if (coll.collider.gameObject.GetComponent<ITrap>() != null)
				{
					//PhotonNetwork.Destroy(coll.collider.gameObject);
					MirrorSpawner.instance.DestoyGameObject(coll.collider.gameObject);
					//photonView.RPC("RPCDestroyComponent", RpcTarget.AllBuffered);
					CmdDestroyComponent();
				}
			}
		}
	}

	[Command]
	private void CmdDestroyComponent()
	{
		RPCDestroyComponent();
	}

	[ClientRpc]
	private void RPCDestroyComponent()
	{
		Destroy(this);
	}


}
