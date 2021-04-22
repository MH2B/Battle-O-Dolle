using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : WeaponAbstract
{

	public override void Attack()
	{
		if (hasAuthority)
		{
			print("Attacking");
			if(_meleeWeaponTargeting.GetMeleeTargetGameObject() != null)
			{
				_iPlayer = _meleeWeaponTargeting.GetMeleeTarget().GetComponent<IPlayer>();
				_iPlayer.TakeDamage(Damage);
			}
		}
	}

}
