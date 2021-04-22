using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MeleeWeaponPositioning : NetworkBehaviour
{
    private Transform parent = null;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float weaponClampRadius = 7;
    private Vector3 weaponOriginalPosition;

    public enum States
    {
        Targeting,
        Attacking
    }; public States currentState;

    private MeleeWeaponTargeting meleeWeaponTargeting;
    private Transform target;

    private bool isAttacking = false;
    private Quaternion rotateTowards;
    
    private void Start()
    {
        if (!hasAuthority)
        {
            Destroy(this);
            return;
        }

        currentState = States.Targeting;
        
        parent = transform.parent;
        //weaponOriginalPosition = parent.position + offset;
        
        meleeWeaponTargeting = GetComponent<MeleeWeaponTargeting>();
    }

    private void Update()
    {
        //weaponOriginalPosition = parent.position + offset;
        
        if (currentState == States.Targeting)
        {
            target = meleeWeaponTargeting.GetMeleeTarget().transform;
            if (target)
            {
                LookTowardsTarget(target);
                //ExtendTowardsTarget(weaponOriginalPosition, target, weaponClampRadius);
            }
           
        }
        else if (currentState == States.Attacking)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                //GetComponent<BoxCollider2D>().enabled = true;
                StartCoroutine(DoAttackMovement());
            }
            
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTowards, 8 * Time.deltaTime);
            
        }
    }

    private IEnumerator DoAttackMovement()
    {
        float newAngle;
        newAngle = transform.rotation.eulerAngles.z;
        newAngle -= 60; ;
        
        rotateTowards = Quaternion.AngleAxis(newAngle, Vector3.forward);

        yield return new WaitForSeconds(0.8f);

        newAngle = transform.rotation.eulerAngles.z;
        newAngle += 120;

        rotateTowards = Quaternion.AngleAxis(newAngle, Vector3.forward);

        yield return new WaitForSeconds(0.4f);
        
        newAngle = transform.rotation.eulerAngles.z;
        newAngle -= 60;

        rotateTowards = Quaternion.AngleAxis(newAngle, Vector3.forward);
        
        yield return new WaitForSeconds(0.2f);

        isAttacking = false;
        //this.GetComponent<BoxCollider2D>().enabled = false;
        currentState = States.Targeting;

    }
    
    private void ExtendTowardsTarget(Vector3 origin, Transform target, float radius)
    {
        Vector3 newPosition = Vector3.MoveTowards(this.transform.position, target.position, 50000 * Time.deltaTime);

        float distanceFromOrigin = Vector3.Distance(newPosition, origin);
        if (distanceFromOrigin > radius)
        {
            Vector3 relativeToOrigin = newPosition - origin;
            relativeToOrigin *= radius / distanceFromOrigin;

            newPosition = origin + relativeToOrigin;
            this.transform.position = newPosition;
        }
        else
        {
            this.transform.position = newPosition;
        }
    }
    
    private void LookTowardsTarget(Transform target)
    {
        Vector2 vectorRelativeToTarget = target.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorRelativeToTarget.y, vectorRelativeToTarget.x) * Mathf.Rad2Deg;

        //if (parent.parent.localScale.x < 0 && transform.localScale.x > 0)
        //{
        //    var xScale = transform.localScale.x;
            
        //    Vector2 newScale = transform.localScale;
        //    newScale.x = -xScale;
        //    transform.localScale = newScale;
        //}
        //else if (parent.parent.localScale.x > 0 && this.transform.localScale.x < 0)
        //{
        //    var xScale = transform.localScale.x;
            
        //    Vector2 newScale = transform.localScale;
        //    newScale.x = Mathf.Abs(xScale);
        //    transform.localScale = newScale;
        //}

        Quaternion temp = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, temp, 15 * Time.deltaTime);
    }


}
