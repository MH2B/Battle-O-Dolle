using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class ObjectiveInteractability : NetworkBehaviour, IInteractable
{

	private bool isGrabbed = false;
	private SpriteRenderer spriteRenderer = null;
	private BoxCollider2D coll = null;


	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		coll = GetComponent<BoxCollider2D>();
	}

	void IInteractable.Interact(Transform parent)
	{
		CmdGrabbingAvailibility(parent);
	}


	[Command]
	private void CmdGrabbingAvailibility(Transform parent)
	{
		RPCGrabbingAvailibility(parent);
	}

	[ClientRpc]
	private void RPCGrabbingAvailibility(Transform parent)
	{
		GrabbingAvailibility(parent);
	}

	private void GrabbingAvailibility(Transform parent)
	{
		if (isGrabbed && transform.parent == parent)
		{
			isGrabbed = false;
			transform.parent = null;
			spriteRenderer.enabled = true;
			parent.GetChild(0).GetChild(2).gameObject.SetActive(false);
			coll.enabled = false;
		}
		else if (!isGrabbed)
		{
			isGrabbed = true;
			//transform.position = parent.position;
			transform.parent = parent;
			spriteRenderer.enabled = false;
			parent.GetChild(0).GetChild(2).gameObject.SetActive(true);
			coll.enabled = true;
		}
	}
}
