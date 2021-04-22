using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponTargeting : MonoBehaviour
{

    [SerializeField] private float targetingRadius = 10;
    [SerializeField] private LayerMask target;

    public GameObject GetMeleeTarget()
    {
        return ClosestTargetCalculator(FindAvailableTargets(transform.position, targetingRadius, target));
    }

    public GameObject GetMeleeTargetGameObject()
    {
        return ClosestTargetCalculator(FindAvailableTargets(transform.position, targetingRadius, target));
    }


    private List<Transform> FindAvailableTargets(Vector2 origin, float radius, LayerMask target)
    {
        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(origin, radius, target);

        List<Transform> availableTargets = new List<Transform>();
        foreach (var item in targetsHit)
        {
            if(item.GetComponent<IPlayer>() != null)
			{
                availableTargets.Add(item.transform);
            }
        }
        return availableTargets;
    }

    private GameObject ClosestTargetCalculator(List<Transform> targets)
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (var i in targets)
        {
            float newDistance = Mathf.Abs(Vector2.Distance(transform.position, i.position));

            if (newDistance < closestDistance)
            {
                closestDistance = newDistance;
                closestTarget = i;
            }
        }
        if (closestTarget != null)
        {
            return closestTarget.gameObject;
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, targetingRadius);
    }
}
