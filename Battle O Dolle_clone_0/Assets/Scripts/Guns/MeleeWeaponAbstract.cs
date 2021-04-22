using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponAbstract : WeaponAbstract
{
    private MeleeWeaponPositioning meleeWeaponPositioning = null;

    protected virtual void Awake()
    {
        WeaponType = WeaponTypes.MeleeWeapon;
    }

    public override void Attack()
    {
        if (!meleeWeaponPositioning)
        {
            meleeWeaponPositioning = GetComponent<MeleeWeaponPositioning>();
            
            if(!meleeWeaponPositioning)
                return;
        }

        if (meleeWeaponPositioning.currentState == MeleeWeaponPositioning.States.Attacking)
            return;
            
        
        meleeWeaponPositioning.currentState = MeleeWeaponPositioning.States.Attacking;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasAuthority)
        {
            return;
        }
        if (other.tag != transform.parent.tag)
        {
            if (other.tag == "BlueTeam" || other.tag == "RedTeam")
            {
                other.gameObject.GetComponent<IPlayer>().TakeDamage(Damage);

                this.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        
        
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
