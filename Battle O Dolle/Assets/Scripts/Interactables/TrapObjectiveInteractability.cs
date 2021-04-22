using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TrapObjectiveInteractability : NetworkBehaviour, IInteractable
{

	private InGame _inGame = null;

	[SerializeField] private string trapClassName = null;
	public string TrapClassName { get => trapClassName; set => CmdTrapClassName(value); }


	public override void OnStartClient()
	{
		_inGame = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InGame>();

		float lifeTimeRate = _inGame.CountDownTimerForTrapUsebality;
		if (TrapClassName == "TrapDetectorBeeper")
		{
			lifeTimeRate += 15f;
		}
		if (hasAuthority)
		{
			Invoke("DestroyGameObject", lifeTimeRate);
		}
	}


	void IInteractable.Interact(Transform parent)
	{
		IPlayer iplayer = parent.gameObject.GetComponent<IPlayer>();
		if(iplayer != null)
		{
			print("objective found the hero");
			if (!iplayer.HasTrapGetter())
			{
				iplayer.HasTrapSetter(true);

				iplayer.AddComponent(TrapClassName);
				//iplayer.InteractBtnTurnOnOrOff(false);
				print("is adding component");
				if (hasAuthority)
				{
					print("is destroying the objective");
					DestroyGameObject();
				}
			}
		}

	}

	private void DestroyGameObject()
	{
		MirrorSpawner.instance.DestoyGameObject(gameObject);
	}

	[Command]
	private void CmdTrapClassName(string _trapClassName)
	{
		RPCTrapClassName(_trapClassName);
	}

	[ClientRpc]
	private void RPCTrapClassName(string _trapClassName)
	{
		trapClassName = _trapClassName;
	}
}
