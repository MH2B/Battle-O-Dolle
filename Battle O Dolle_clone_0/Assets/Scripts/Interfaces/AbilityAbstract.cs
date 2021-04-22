using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class AbilityAbstract : NetworkBehaviour
{

	public float abilityCoolDown = 10f;

	public virtual void AbilityIsStarting(GameObject aimingPref) { }

	public virtual void ExecuteAbility() { }



}
