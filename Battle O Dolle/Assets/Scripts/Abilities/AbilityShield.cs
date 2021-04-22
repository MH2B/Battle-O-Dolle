using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityShield : AbilityAbstract
{

    [SerializeField]
    private float shieldLength = 3f;

    
    private PlayerMatchData playerMatchData;

    private void Start()
    {
        playerMatchData = GetComponent<PlayerMatchData>();
        if (!playerMatchData)
            throw new Exception("Player has no PlayerMatchData! WTF!!!!");
    }

    public override void ExecuteAbility()
    {
        playerMatchData.CanTakeDamage = false;

        StartCoroutine(ShieldLengthCountDown(shieldLength, playerMatchData));

    }

    IEnumerator ShieldLengthCountDown(float shieldLengthTime, PlayerMatchData playerMatchData)
    {
        yield return new WaitForSeconds(shieldLengthTime);

        playerMatchData.CanTakeDamage = true;
    }

    public override void AbilityIsStarting(GameObject aimingPref)
    {
        base.AbilityIsStarting(aimingPref);
    }
    
    
    
}
