using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBtns : MonoBehaviour
{

	public static UIBtns instance = null;

	[Header("UI Accessible Panels")]
	public GameObject movementPanel = null;
	public GameObject buttonsPanel = null;
	public GameObject matchStatsPanel = null;
	public GameObject matchResultsPanel = null;
	public GameObject shopPanel = null;

	[Header("UI Accessible Btns")]
	public GameObject movementJoystick = null;
	public Button interactBtn = null;
	public Button abilityBtn = null;
	public GameObject abilityJoystick = null;
	public Button lockBtn = null;
	public Button shopBtn = null;
	public Button traperBtn = null;
	public Button weaponBtn = null;
	public GameObject weaponJoystick = null;
	public Button settingsBtn = null;

	[Header("Game Stats")]
	public GameObject bars = null;
	public Image healthBar = null;
	public Image abilityBar = null;
	public Image heroFace = null;
	public GameObject disconnection = null;
	public GameObject roundStats = null;
	public Text roundDinamicTxt = null;
	public Text timeDinamicTxt = null;
	public Text blueScoreTxt = null;
	public Text redScoreTxt = null;
	public GameObject blueTeamMembers = null;
	public GameObject redTeamMembers = null;

	[Header("Match Stats")]
	public GameObject winPage = null;
	public GameObject LosePage = null;

	public delegate void OnDelegateChanged();
	public OnDelegateChanged onInteractBtnDelegate;
	public OnDelegateChanged onAimingSelectDelegate;
	public OnDelegateChanged onAimingDeSelectDelegate;
	public OnDelegateChanged onShopBtnDelegate;
	public OnDelegateChanged onCloseShopBtnDelegate;
	public OnDelegateChanged onLockBtnDelegate;
	public OnDelegateChanged onSetTrapBtnDelegate;
	public OnDelegateChanged onWeaponBtnSelectDelegate;
	public OnDelegateChanged onWeaponBtnDeSelectDelegate;

	public delegate void OnWeaponPurchaseDelegateChanged(string weaponName);
	public OnWeaponPurchaseDelegateChanged onWeaponPurchasedDelegate;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}


	#region Weapons & Guns

	public void OnWeaponPurchased(string weaponName)
	{
		onWeaponPurchasedDelegate.Invoke(weaponName);
	}

	#endregion

	public void OnInteractBtn()
	{
		if(onInteractBtnDelegate != null)
		{
			onInteractBtnDelegate.Invoke();
		}
	}

	public void OnWeaponSelect()
	{
		if (onWeaponBtnSelectDelegate != null)
		{
			onWeaponBtnSelectDelegate.Invoke();
		}
	}
	
	public void OnWeaponDeSelect()
	{
		if (onWeaponBtnDeSelectDelegate != null)
		{
			onWeaponBtnDeSelectDelegate.Invoke();
		}
	}
	

	public void OmAimingSelect()
	{
		if (onAimingSelectDelegate != null)
		{
			onAimingSelectDelegate.Invoke();
		}
	}

	public void OmAimingDeSelect()
	{
		if (onAimingDeSelectDelegate != null)
		{
			onAimingDeSelectDelegate.Invoke();
		}
	}


	public void OnShopBtn()
	{
		if (onShopBtnDelegate != null)
		{
			onShopBtnDelegate.Invoke();
		}
	}

	public void OnCloseShopBtn()
	{
		if (onCloseShopBtnDelegate != null)
		{
			onCloseShopBtnDelegate.Invoke();
		}
	}

	public void OnLockBtn()
	{
		if (onLockBtnDelegate != null)
		{
			onLockBtnDelegate.Invoke();
		}
	}


	public void OnSetTrapBtn()
	{
		if (onSetTrapBtnDelegate != null)
		{
			onSetTrapBtnDelegate.Invoke();
		}
	}

	public void ChangeWeaponInteractability(float chanegTime)
	{
		weaponBtn.interactable = !weaponBtn.interactable;
		Invoke("InvokeChangeWeaponInteractability", chanegTime);
	}

	

	private void InvokeChangeWeaponInteractability()
	{
		weaponBtn.interactable = !weaponBtn.interactable;
	}


}
