using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuyAvailibility : NetworkBehaviour
{
	private bool canBuy = true;
	public bool CanBuy { get => canBuy; set { canBuy = value; ChangeBuyBtn(value); } }

	private GameObject buyBtn = null;

	private void Start()
	{
		if (!hasAuthority)
		{
			Destroy(this);
			return;
		}
		buyBtn = UIBtns.instance.shopBtn.gameObject;
	}

	private void ChangeBuyBtn(bool activity)
	{
		if (buyBtn.activeInHierarchy)
		{
			buyBtn.SetActive(activity);
		}
	}
}
