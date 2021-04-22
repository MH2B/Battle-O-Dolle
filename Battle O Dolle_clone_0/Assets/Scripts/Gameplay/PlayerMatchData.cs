using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerMatchData : NetworkBehaviour , IPlayer
{

	private bool hasTrap = false;

	public LayerMask raycastableForInSightLayerMask = new LayerMask();
	public LayerMask detectableLayerMask = new LayerMask();

	private Image healthBarMain = null;
	private Image abilityCoolDownBarMain = null;
	private GameObject canvas = null;
	private GameObject blueTeamMembers = null;
	private GameObject redTeamMembers = null;
	private GameObject healthBar = null;
	private GameObject headers = null;
	private SpriteRenderer healthBarSprite = null;
	private AnimatorController animatorController = null;

	[SerializeField] private GameObject deathVfx = null;
	[SerializeField] private GameObject bodyHandler = null;
	[SerializeField] private GameObject spectDeathDrone = null;

	[SerializeField] private float health = 100;

	public float Health { get => health; set => health = value; }
	
	[SyncVar] private string team = null;

	private string className = null;


	[HideInInspector]
	public GameManager gameManager = null;

	private PlayersMovement playersMovement = null;

	public int playerGroup = 0;

	private Button abilityBtn = null;
	private Button interactBtn = null;

	private float abilityCoolDownTimer = 10f;
	private float abilitytimer = 0f;
	private bool abilityUsed = false;

	private bool canTakeDamage = true;
	public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }

	public override void OnStartClient()
	{
		Invoke("SetTheTagWaiter", 1f);
		if (!hasAuthority)
		{
			return;
		}
		animatorController = GetComponent<AnimatorController>();
		headers = transform.GetChild(0).gameObject;
		healthBar = headers.transform.GetChild(0).gameObject;
		healthBarSprite = healthBar.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

		playersMovement = GetComponent<PlayersMovement>();
		canvas = GameObject.FindGameObjectWithTag("UI").gameObject;

		blueTeamMembers = UIBtns.instance.blueTeamMembers;
		redTeamMembers = UIBtns.instance.redTeamMembers;

		interactBtn = UIBtns.instance.interactBtn;
		abilityBtn = UIBtns.instance.abilityBtn;

		healthBarMain = UIBtns.instance.healthBar;
		abilityCoolDownBarMain = UIBtns.instance.abilityBar;

		gameManager = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<GameManager>();

		healthBar.SetActive(false);
		headers.transform.GetChild(1).gameObject.SetActive(false);
		SetThePlayersGroup();
	}

	private void SetTheTagWaiter()
	{
		gameObject.tag = TeamGetter();
	}

	private void Update()
	{
		if (!hasAuthority)
		{
			return;
		}
		if (abilityUsed)
		{
			abilitytimer += Time.deltaTime;
			abilityCoolDownBarMain.fillAmount = (abilitytimer / abilityCoolDownTimer);
			if(abilitytimer >= abilityCoolDownTimer)
			{
				abilitytimer = 0f;
				abilityUsed = false;
				abilityBtn.interactable = true;
			}
		}
	}
	
	private void SetThePlayersGroup()
	{
		playerGroup = MirrorPlayer.localPlayer.thisPlayerInGameSettings.isInBlueTeam ? 1 : 2;
		//photonView.RPC("RPCSetThePlayerGroup", RpcTarget.OthersBuffered, playerGroup);
		CmdSetThePlayerGroup(playerGroup);
	}

	public void Revived()
	{
		if (hasAuthority)
		{
			//photonView.RPC("RPCSetDeathInPlayerContainerDead", RpcTarget.AllBuffered, TeamGetter(), className, true);
			CmdSetDeathInPlayerContainerDead(TeamGetter(), className, true);
		}
	}

	public void AddAbility()
	{
		//className = (string)PhotonNetwork.LocalPlayer.CustomProperties["Class"];
		className = MirrorPlayer.localPlayer.thisPlayerInGameSettings.heroName;

		switch (className)
		{
			case "Healer":
				gameObject.AddComponent<AbilityHealer>();
				break;
			case "Spearer":
				gameObject.AddComponent<AbilitySpear>();
				break;
			case "Grenader":
				gameObject.AddComponent<AbilityGrenade>();
				break;
			case "Teleporter":
				gameObject.AddComponent<AbilityTeleport>();
				break;
			case "Reviver":
				gameObject.AddComponent<AbilityRevive>();
				break;
			case "Thief":
				gameObject.AddComponent<AbilityThief>();
				break;
			case "Guard":
				gameObject.AddComponent<AbilityDoubleDamage>();
				break;

			// If no ability was choosed we will give the player grenade ability automaticlly
			default:
				gameObject.AddComponent<AbilityGrenade>();
				break;
		}
	}

	public string TeamGetter()
	{
		return team;
	}

	public void TeamSetter(string team)
	{
		this.team = team;
	}

	public void TakeDamage(float damageAmount)
	{
		if (CanTakeDamage)
		{
			//photonView.RPC("RPCTakeDamage", RpcTarget.AllBuffered, damageAmount);
			CmdTakeDamage(damageAmount);
		}
	}

	

	IEnumerator AfterSpawninBodyHandler(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninBodyHandler(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		GameObject newBodyHandler = MirrorSpawner.instance.spawnedFounders[_index];
		newBodyHandler.GetComponent<DeadBodyHandler>().AtatchTheDeadBody(gameObject);

		// Set the spect drone active
		//GameObject newSpectDeathDrone = PhotonNetwork.Instantiate(spectDeathDrone.name, transform.position, Quaternion.identity);
		int index = MirrorSpawner.instance.SpawnGameObjectindex(spectDeathDrone, transform.position, transform.rotation);
		StartCoroutine(AfterSpawninSpectDeathDrone(index, InGame.instance.waitForSpawnedObjectsTimer, newBodyHandler));
	}

	IEnumerator AfterSpawninSpectDeathDrone(int _index, float waitingTime , GameObject newBodyHandler)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninSpectDeathDrone(_index, waitingTime , newBodyHandler));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		GameObject newSpectDeathDrone = MirrorSpawner.instance.spawnedFounders[_index];
		newBodyHandler.GetComponent<DeadBodyHandler>().SpectDeathDrone = newSpectDeathDrone;

		// Set the canvas to not active
		newBodyHandler.GetComponent<DeadBodyHandler>().Canavs = canvas;
		UIBtns.instance.buttonsPanel.SetActive(false);
		//for (int i = 1; i < canvas.transform.childCount; i++)
		//{
		//	Button btn = canvas.transform.GetChild(i).gameObject.GetComponent<Button>();
		//	if (btn != null)
		//	{
		//		btn.interactable = false;
		//	}
		//}
		//photonView.RPC("RPCSetDeathInPlayerContainerDead", RpcTarget.AllBuffered, TeamGetter(), className , false);
		CmdSetDeathInPlayerContainerDead(TeamGetter(), className, false);

	}

	public void Die()
	{
		if (hasAuthority)
		{
			MirrorSpawner.instance.SpawnGameObjectindex(deathVfx, new Vector3(transform.position.x, transform.position.y, deathVfx.transform.position.z), transform.rotation);
			int index = MirrorSpawner.instance.SpawnGameObjectindex(bodyHandler, transform.position, transform.rotation);
			StartCoroutine(AfterSpawninBodyHandler(index, InGame.instance.waitForSpawnedObjectsTimer));
		}

		// Update the all players of teams in the game manager
		if (playerGroup == 1)
		{
			gameManager.TeamGroup1RemainedPlayers--;
		}
		else if(playerGroup == 2)
		{
			gameManager.TeamGroup2RemainedPlayers--;
		}
		gameObject.SetActive(false);
	}

	
	public void Heal(float healAmount)
	{
		//photonView.RPC("RPCHeal", RpcTarget.AllBuffered, healAmount);
		CmdHeal(healAmount);
	}


	public void ObjectiveReached()
	{
		//photonView.RPC("RPCObjectiveReached", RpcTarget.AllBuffered);
		CmdObjectiveReached();
	}


	public void AddComponent(string component)
	{
		// UI
		if (hasAuthority)
		{
			//if (!MirrorPlayer.localPlayer.thisPlayerInGameSettings.roundIsInBlueTeam)
			//{
			//	UIBtns.instance.buttonsPanel.SetActive(true);
			//}
			UIBtns.instance.traperBtn.gameObject.SetActive(true);
		}
		
		switch (component)
		{
			case "Miner":
				gameObject.AddComponent<Miner>();
				break;
			case "Poisoner":
				gameObject.AddComponent<Poisoner>();
				break;
			case "BearTraper":
				gameObject.AddComponent<BearTraper>();
				break;
			case "TrapDetectorBeeper":
				gameObject.AddComponent<TrapDetectorBeeper>();
				break;
			case "Locker":
				if (hasAuthority)
				{
					GetComponent<CollisionHandler>().HasLock = 2;
					//UIBtns.instance.buttonsPanel.SetActive(false);
				}
				break;
		}
	}

	public bool HasTrapGetter()
	{
		return hasTrap;
	}

	public void HasTrapSetter(bool activity)
	{
		hasTrap = activity;
	}

	public void StuckPlayer(float timeRate)
	{
		playersMovement.IsStucked = true;
		Invoke("ResetStucked", timeRate);
	}

	public void AbilityUsed(float abilityCoolDownValue)
	{
		abilityCoolDownTimer = abilityCoolDownValue;
		abilityUsed = true;
		abilityCoolDownBarMain.fillAmount = 0f;
		abilityBtn.interactable = false;
	}

	public void ActionBtnTurnOnOrOff(bool availibility)
	{
		interactBtn.gameObject.SetActive(availibility);
	}

	private void ResetStucked()
	{
		playersMovement.IsStucked = false;
	}


	[Command]
	private void CmdSetThePlayerGroup(int _playerGroup)
	{
		RPCSetThePlayerGroup(_playerGroup);
	}

	[Command]
	private void CmdTakeDamage(float damageAmount)
	{
		RPCTakeDamage(damageAmount);
	}

	[Command]
	private void CmdSetDeathInPlayerContainerDead(string teamNumber, string className, bool isAlive)
	{
		RPCSetDeathInPlayerContainerDead(TeamGetter(), className, false);
	}

	[Command]
	private void CmdObjectiveReached()
	{
		RPCObjectiveReached();
	}

	[Command]
	private void CmdHeal(float healAmount)
	{
		RPCHeal(healAmount);
	}


	[ClientRpc]
	private void RPCSetDeathInPlayerContainerDead(string teamNumber , string className , bool isAlive)
	{
		if (teamNumber == "BlueTeam")
		{
			foreach (Transform trans in blueTeamMembers.transform)
			{
				if (trans.name == className)
				{
					trans.GetChild(1).gameObject.SetActive(!isAlive);
					return;
				}
			}
		}
		else if (teamNumber == "RedTeam")
		{
			foreach (Transform trans in redTeamMembers.transform)
			{
				if (trans.name == className)
				{
					trans.GetChild(1).gameObject.SetActive(!isAlive);
					return;
				}
			}
		}
	}

	[ClientRpc]
	private void RPCSetThePlayerGroup(int _playerGroup)
	{
		// Call this only in other clients not the local one
		if (!hasAuthority)
		{
			playerGroup = _playerGroup;
		}
	}

	[ClientRpc]
	private void RPCObjectiveReached()
	{
		// Update the all players of teams in the game manager
		// Update the all players of teams in the game manager
		if (playerGroup == 1)
		{
			gameManager.TeamGroup2RemainedPlayers = 0;
		}
		else if (playerGroup == 2)
		{
			gameManager.TeamGroup1RemainedPlayers = 0;
		}

		gameObject.SetActive(false);
	}


	[ClientRpc]
	public void RPCTakeDamage(float damageAmount)
	{
		if (!canTakeDamage)
			return;
		
		Health -= damageAmount;
		// Animation
		animatorController.CanTakeHit();
		healthBar.transform.localScale = new Vector3(Health / 100f , healthBar.transform.localScale.y, healthBar.transform.localScale.z);
		healthBarSprite.color = Color.Lerp(Color.red, Color.green, healthBar.transform.localScale.x);

		if (hasAuthority)
		{
			healthBarMain.fillAmount = (Health / 100f);
		}

		if (Health <= 0)
		{
			Die();
		}
	}

	[ClientRpc]
	public void RPCHeal(float damageAmount)
	{
		Health += damageAmount;
		Health = Mathf.Clamp(Health , 0 , 100);
		healthBar.transform.localScale = new Vector3(Health / 100f, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
		healthBarSprite.color = Color.Lerp(Color.red, Color.green, healthBar.transform.localScale.x);
		if (hasAuthority)
		{
			healthBarMain.fillAmount = (Health / 100f);
		}
	}

	public void InteractBtnTurnOnOrOff(bool availibility)
	{
		interactBtn.gameObject.SetActive(availibility);
	}
}
