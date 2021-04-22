using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDoubleDamage : AbilityAbstract
{

    [SerializeField]
    private float doubleDamageLength = 3f;

    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        if (!weaponManager)
            throw new Exception("Player has no weapon manager. add it manually");
    }

    public override void AbilityIsStarting(GameObject aimingPref)
    {
        base.AbilityIsStarting(aimingPref);
    }


    public override void ExecuteAbility()
    {
        weaponManager.currentWeapon.Damage *= 2;

        StartCoroutine(DoubleDamageResetCountDown(doubleDamageLength, weaponManager.currentWeapon));
    }

    IEnumerator DoubleDamageResetCountDown(float doubleDamageLength, WeaponAbstract weapon)
    {
        yield return new WaitForSeconds(doubleDamageLength);

        weapon.Damage /= 2;
    }
    
    
    
}
