using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class Action : NetworkBehaviour
{

	[SerializeField] private bool doGizmos = false;
	[Range(0, 5)] [SerializeField] private float radiousOfInteract = 0f;
	[SerializeField] private LayerMask interactableLayerMask = new LayerMask();
	[SerializeField] private Sprite lockDoorSprite = null;
	[SerializeField] private Sprite unLockDoorSprite = null;

	private GameObject canvas = null;

	private MovementAbstract movementClass = null;

	private Text lockBtnText = null;

	[SerializeField] private GameObject aimingPrefab = null;

	private GameObject newAiming = null;

	private IInteractable interactable = null;
	private IPlayer _iplayer = null;

	private DoorInteractable doorInteractable = null;

	private AbilityAbstract ability = null;

	private AnimatorController animatorController = null;

	private TrapAbstract trapClass = null;

	private UIBtns uiBtns = null;

	private WeaponManager weaponManager = null;

	private GameObject weaponFixedJoyStick = null;

	private GameObject weaponBtn = null;

	private GameObject shopPanel;


	private void OnDrawGizmos()
	{
		if (!doGizmos)
		{
			return;
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, radiousOfInteract);
	}

	public override void OnStartClient()
	{
		if (!hasAuthority)
		{
			return;
		}
		canvas = GameObject.FindGameObjectWithTag("UI").gameObject;
		weaponFixedJoyStick = UIBtns.instance.weaponJoystick;
		weaponBtn = UIBtns.instance.weaponBtn.gameObject;
		animatorController = GetComponent<AnimatorController>();
		movementClass = GetComponent<MovementAbstract>();
		_iplayer = GetComponent<IPlayer>();
		ability = GetComponent<AbilityAbstract>();
		trapClass = GetComponent<TrapAbstract>();
		shopPanel = UIBtns.instance.shopPanel;
		weaponManager = GetComponent<WeaponManager>();

		//Set the action button listener
		uiBtns = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>();
		AddDelegates();
	}


	public void AddDelegates()
	{
		uiBtns.onInteractBtnDelegate += OnInteractBtn;
		uiBtns.onAimingSelectDelegate += OnAimingSelect;
		uiBtns.onAimingDeSelectDelegate += OnAimingDeSelect;
		uiBtns.onShopBtnDelegate += OnShopBtn;
		uiBtns.onCloseShopBtnDelegate += OnCloseShopBtn;
		uiBtns.onWeaponPurchasedDelegate += OnWeaponPurchase;
		uiBtns.onLockBtnDelegate += OnLockBtn;
		uiBtns.onSetTrapBtnDelegate += OnSetTrapBtn;
		uiBtns.onWeaponBtnSelectDelegate += OnWeaponSelect;
		uiBtns.onWeaponBtnDeSelectDelegate += OnWeaponDeSelect;
	}

	public void RemoveDelegates()
	{
		uiBtns.onInteractBtnDelegate -= OnInteractBtn;
		uiBtns.onAimingSelectDelegate -= OnAimingSelect;
		uiBtns.onAimingDeSelectDelegate -= OnAimingDeSelect;
		uiBtns.onShopBtnDelegate -= OnShopBtn;
		uiBtns.onCloseShopBtnDelegate -= OnCloseShopBtn;
		uiBtns.onLockBtnDelegate -= OnLockBtn;
		uiBtns.onSetTrapBtnDelegate -= OnSetTrapBtn;
		uiBtns.onWeaponBtnSelectDelegate -= OnWeaponSelect;
		uiBtns.onWeaponBtnDeSelectDelegate -= OnWeaponDeSelect;
	}

	#region Weapons

	private void OnWeaponSelect()
	{
		if (weaponManager.currentWeapon != null)
		{
			if (weaponManager.currentWeapon.WeaponType == WeaponAbstract.WeaponTypes.Gun)
			{
				movementClass.enabled = false;
				weaponManager.currentWeapon.Aim();
			}
			else
			{
				if (weaponFixedJoyStick.activeInHierarchy)
				{
					weaponFixedJoyStick.SetActive(false);
				}
				weaponManager.currentWeapon.Attack();
			}
		}
	}

	private void OnWeaponDeSelect()
	{
		if (weaponManager.currentWeapon != null)
		{
			if (weaponManager.currentWeapon.WeaponType == WeaponAbstract.WeaponTypes.Gun)
			{
				weaponManager.currentWeapon.Attack();

				movementClass.enabled = true;
			}
		}
	}

	#endregion

	#region Ability

	private void OnAimingSelect()
	{
		CallAbilityIsStarting();
		//newAiming = Instantiate(aimingPrefab, transform.position, Quaternion.identity);
		movementClass.enabled = false;
	}

	private void OnAimingDeSelect()
	{
		//if(newAiming != null)
		//{
		//	Destroy(newAiming);
		//}
		CallExecuteAbility();
		movementClass.enabled = true;
	}

	private void CallAbilityIsStarting()
	{
		if (ability != null)
		{
			ability.AbilityIsStarting(aimingPrefab);
		}
		else
		{
			ability = GetComponent<AbilityAbstract>();
			if (ability != null)
			{
				ability.AbilityIsStarting(aimingPrefab);
			}
			else
			{
				print("No ability attached to the player");
			}
		}
	}

	private void CallExecuteAbility()
	{
		if (ability != null)
		{
			FactExecuteAbility();
		}
		else
		{
			ability = GetComponent<AbilityAbstract>();
			if (ability != null)
			{
				FactExecuteAbility();
			}
			else
			{
				print("No ability attached to the player");
			}
		}
	}

	private void FactExecuteAbility()
	{
		//Vector2 aimDirection = newAiming.GetComponent<AimingDirection>().AimDirection;
		//ability.ExecuteAbility(-aimDirection);
		animatorController.CanDoAbility();
		Invoke("AbilityAfterAnimation", 1f);
		_iplayer.AbilityUsed(ability.abilityCoolDown);
	}

	private void AbilityAfterAnimation()
	{
		ability.ExecuteAbility();
	}

	#endregion

	private void OnInteractBtn()
	{
		//CmdInteract();

		// See if there are interactable objects
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfInteract, Vector2.up, 10, interactableLayerMask);
		float dist = Mathf.Infinity;
		GameObject newInteractable = null;

		// Get the closest interactable from the founded ones
		foreach (RaycastHit2D coll in hit)
		{
			// To be sure that hited object is an interactable
			if (coll.collider.gameObject.GetComponent<IInteractable>() != null)
			{
				// Finding the closest interactable
				if (Vector2.Distance(coll.collider.gameObject.transform.position, transform.position) < dist)
				{
					dist = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
					newInteractable = coll.collider.gameObject;
				}
			}
		}
		if (newInteractable != null)
		{
			interactable = newInteractable.GetComponent<IInteractable>();
			interactable.Interact(transform);
			interactable = null;
		}
	}

	private void OnShopBtn()
	{
		shopPanel.SetActive(true);
	}

	private void OnWeaponPurchase(string weaponName)
	{
		GameObject newWeapon = null;
		switch (weaponName)
		{
			case "Knife":
				newWeapon = Resources.Load("Knife", typeof(GameObject)) as GameObject;
				weaponManager.AddWeapon(newWeapon);
				break;
			case "Sword":
				newWeapon = Resources.Load("Sword", typeof(GameObject)) as GameObject;
				weaponManager.AddWeapon(newWeapon);
				break;
			case "Mace":
				newWeapon = Resources.Load("Mace", typeof(GameObject)) as GameObject;
				weaponManager.AddWeapon(newWeapon);
				break;
			case "Club":
				newWeapon = Resources.Load("Club", typeof(GameObject)) as GameObject;
				weaponManager.AddWeapon(newWeapon);
				break;
			case "Vintage":
				newWeapon = Resources.Load("Vintage", typeof(GameObject)) as GameObject;
				weaponManager.AddWeapon(newWeapon);
				break;
		}
		if(newWeapon != null)
		{
			if (!weaponBtn.activeInHierarchy)
			{
				weaponBtn.SetActive(true);
			}
		}
	}

	private void OnCloseShopBtn()
	{
		shopPanel.SetActive(false);
	}

	private void OnLockBtn()
	{
		if (GetComponent<CollisionHandler>().HasLock == 0)
		{
			GetComponent<CollisionHandler>().UpdateLockBtn(false , null);
			return;
		}

		// Lock The Door
		//CmdLock();

		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfInteract, Vector2.up, 10, interactableLayerMask);
		float dist = Mathf.Infinity;
		GameObject newInteractable = null;
		foreach (RaycastHit2D coll in hit)
		{
			if (coll.collider.gameObject.GetComponent<DoorInteractable>() != null)
			{
				if (Vector2.Distance(coll.collider.gameObject.transform.position, transform.position) < dist)
				{
					dist = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
					newInteractable = coll.collider.gameObject;
				}
			}
		}
		if (newInteractable != null)
		{
			doorInteractable = newInteractable.GetComponent<DoorInteractable>();
			lockBtnText = UIBtns.instance.lockBtn.transform.GetChild(0).GetComponent<Text>();
			if (lockBtnText.text == "Lock")
			{
				if (doorInteractable.LockDoor(true))
				{
					if (hasAuthority)
					{
						GetComponent<CollisionHandler>().HasLock--;
					}
					lockBtnText.text = "UnLock";
					lockBtnText.transform.parent.GetComponent<Image>().sprite = unLockDoorSprite;
					//lockBtnText.transform.parent.gameObject.transform.GetChild(1).gameObject.SetActive(true);
					//lockBtnText.transform.parent.gameObject.transform.GetChild(2).gameObject.SetActive(false);
				}
			}
			else if (lockBtnText.text == "UnLock")
			{
				if (doorInteractable.LockDoor(false))
				{
					lockBtnText.text = "Lock";
					lockBtnText.transform.parent.GetComponent<Image>().sprite = lockDoorSprite;
					//lockBtnText.transform.parent.gameObject.transform.GetChild(2).gameObject.SetActive(true);
					//lockBtnText.transform.parent.gameObject.transform.GetChild(1).gameObject.SetActive(false);
				}
			}
			doorInteractable = null;
		}
	}

	private void OnSetTrapBtn()
	{
		// Set the mine
		if(trapClass == null)
		{
			trapClass = GetComponent<TrapAbstract>();
		}
		if (trapClass != null)
		{
			trapClass.SetTrap();
			UIBtns.instance.traperBtn.gameObject.SetActive(false);
			Destroy(trapClass);
		}
	}




	#region Remote Calls

	[Command]
	private void CmdInteract()
	{
		RPCInteract();
	}

	[Command]
	private void CmdLock()
	{
		RpcLock();
	}

	[ClientRpc]
	public void RPCInteract()
	{
		// See if there are interactable objects
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfInteract, Vector2.up, 10, interactableLayerMask);
		float dist = Mathf.Infinity;
		GameObject newInteractable = null;

		// Get the closest interactable from the founded ones
		foreach (RaycastHit2D coll in hit)
		{
			// To be sure that hited object is an interactable
			if (coll.collider.gameObject.GetComponent<IInteractable>() != null)
			{
				// Finding the closest interactable
				if (Vector2.Distance(coll.collider.gameObject.transform.position, transform.position) < dist)
				{
					dist = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
					newInteractable = coll.collider.gameObject;
				}
			}
		}
		if (newInteractable != null)
		{
			interactable = newInteractable.GetComponent<IInteractable>();
			interactable.Interact(transform);
			interactable = null;
		}
	}
	

	[ClientRpc]
	public void RpcLock()
	{
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfInteract, Vector2.up, 10, interactableLayerMask);
		float dist = Mathf.Infinity;
		GameObject newInteractable = null;
		foreach (RaycastHit2D coll in hit)
		{
			if (coll.collider.gameObject.GetComponent<DoorInteractable>() != null)
			{
				if (Vector2.Distance(coll.collider.gameObject.transform.position, transform.position) < dist)
				{
					dist = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
					newInteractable = coll.collider.gameObject;
				}
			}
		}
		if (newInteractable != null)
		{
			doorInteractable = newInteractable.GetComponent<DoorInteractable>();
			lockBtnText = UIBtns.instance.lockBtn.transform.GetChild(0).GetComponent<Text>();
			if (lockBtnText.text == "Lock")
			{
				if (doorInteractable.LockDoor(true))
				{
					if (hasAuthority)
					{
						GetComponent<CollisionHandler>().HasLock--;
					}
					lockBtnText.text = "UnLock";
					lockBtnText.transform.parent.GetComponent<Image>().sprite = unLockDoorSprite;
					//lockBtnText.transform.parent.gameObject.transform.GetChild(1).gameObject.SetActive(true);
					//lockBtnText.transform.parent.gameObject.transform.GetChild(2).gameObject.SetActive(false);
				}
			}
			else if (lockBtnText.text == "UnLock")
			{
				if (doorInteractable.LockDoor(false))
				{
					lockBtnText.text = "Lock";
					lockBtnText.transform.parent.GetComponent<Image>().sprite = lockDoorSprite;
					//lockBtnText.transform.parent.gameObject.transform.GetChild(2).gameObject.SetActive(true);
					//lockBtnText.transform.parent.gameObject.transform.GetChild(1).gameObject.SetActive(false);
				}
			}
			doorInteractable = null;
		}
	}

	#endregion

}
