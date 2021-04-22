using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CollisionHandler : NetworkBehaviour
{

	private int hasLock = 0;
	public int HasLock { get => hasLock; set => hasLock = value; }

	private bool hasExitedBuyZone = false;
	private GameObject interactBtn = null;
	private GameObject lockBtn = null;
	private Text lockBtnText = null;
	private BuyAvailibility buyAvailibility = null;
	private int team = 0;
	public int Team { get => team; set => team = value; }

	public override void OnStartClient()
	{
		if (!hasAuthority)
		{
			Destroy(this);
			return;
		}

		interactBtn = UIBtns.instance.interactBtn.gameObject;
		lockBtn = UIBtns.instance.lockBtn.gameObject;
		lockBtnText = lockBtn.transform.GetChild(0).gameObject.GetComponent<Text>();
		buyAvailibility = GetComponent<BuyAvailibility>();
	}



	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!hasAuthority)
		{
			return;
		}
		// Check if we have triggered the objective reachpoint and we have objective with ourself
		if (other.tag == "ObjectiveReachPoint" && gameObject.tag == "BlueTeam" && transform.childCount != 5)
		{
			foreach (Transform trans in transform)
			{
				if (trans.gameObject.tag == "Interactable")
				{
					PlayerMatchData _playerMatchData = GetComponent<PlayerMatchData>();
					_playerMatchData.ObjectiveReached();
					break;
				}
			}

		}

		// If entered an interactable then update Interact btn
		if (other.tag == "Interactable")
		{
			UpdateInteractBtn(true);

			if (HasLock != 0)
			{
				// If the interactable is door
				DoorInteractable doorInteractable = other.gameObject.GetComponent<DoorInteractable>();
				if (doorInteractable != null && Team == 2)
				{
					UpdateLockBtn(true, doorInteractable);
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

		// On off lock btn in the UI
		if (HasLock != 0)
		{
			DoorInteractable doorInteractable = other.gameObject.GetComponent<DoorInteractable>();
			if (doorInteractable != null && Team == 2)
			{
				UpdateLockBtn(false, doorInteractable);
			}
		}


		// If we have exited an interactable object turn off the action btn but if the object is our child then dont because we want to redo the action later
		if (other.tag == "Interactable" && other.gameObject.transform.parent != transform && transform.childCount == 5)
		{
			UpdateInteractBtn(false);
		}

		//If we exited the buy zone we have to disable buy button
		if (other.tag == "BuyZone")
		{
			buyAvailibility.CanBuy = false;
			if (gameObject.tag == "BlueTeam" && !hasExitedBuyZone)
			{
				other.gameObject.tag = "ObjectiveReachPoint";
				hasExitedBuyZone = true;
			}
			else
			{
				Destroy(other.gameObject);
			}
		}
	}

	private void UpdateInteractBtn(bool activity)
	{
		if (hasAuthority)
		{
			if (interactBtn == null)
			{
				interactBtn = UIBtns.instance.interactBtn.gameObject;
			}
			interactBtn.SetActive(activity);
		}
	}

	public void UpdateLockBtn(bool activity, DoorInteractable doorInteractable)
	{
		lockBtn.SetActive(activity);

		if (doorInteractable != null)
		{
			if (doorInteractable.isLocked)
			{
				lockBtnText.text = "UnLock";
			}
			else
			{
				lockBtnText.text = "Lock";
			}
		}

	}

}
