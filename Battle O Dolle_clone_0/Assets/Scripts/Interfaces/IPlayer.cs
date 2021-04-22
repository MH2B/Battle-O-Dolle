using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{

	void Revived();

	void AddComponent(string component);

	void InteractBtnTurnOnOrOff(bool availibility);

	bool HasTrapGetter();

	void HasTrapSetter(bool activity);

	string TeamGetter();

	void TeamSetter(string team);

	void AddAbility();

	void TakeDamage(float damageAmount);

	void Heal(float healAmount);

	void StuckPlayer(float timeRate);

	void AbilityUsed(float abilityCoolDown);

}
