using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class DoorInteractable : NetworkBehaviour, IInteractable
{

	private GameObject doorLock = null;

	public bool isLocked = false;

	private bool isOpen = false;

	private GameObject door = null;
	private GameObject doorFrame = null;

	private void Start()
	{
		door = transform.GetChild(0).gameObject;
		doorFrame = transform.GetChild(1).gameObject;
		doorLock = door.transform.GetChild(0).gameObject;
	}

	void IInteractable.Interact(Transform viewID)
	{
		if (isLocked)
		{
			return;
		}

		CmdOpenAndClose();
	}


	[Command]
	private void CmdOpenAndClose()
	{
		RPCOpenAndClose();
	}


	[ClientRpc]
	private void RPCOpenAndClose()
	{
		if (isOpen)
		{
			OpenAndCloseDoor(true);
		}
		else
		{
			OpenAndCloseDoor(false);
		}
		isOpen = !isOpen;
	}

	public bool LockDoor(bool activity)
	{
		if (isLocked == activity)
		{
			return false;
		}
		else
		{
			CmdLockDoor(activity);
			return true;
		}
	}

	[Command]
	private void CmdLockDoor(bool activity)
	{
		RPCLockDoor(activity);
	}

	[ClientRpc]
	private void RPCLockDoor(bool _activity)
	{
		isLocked = _activity;
		doorLock.SetActive(_activity);
		isOpen = false;
		OpenAndCloseDoor(true);
	}

	private void OpenAndCloseDoor(bool activity)
	{
		door.SetActive(activity);
		doorFrame.SetActive(!activity);
	}
	
}
