using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPositioning : MonoBehaviour
{
    private Transform parent = null;
    [SerializeField] private Vector3 offset;

    private Vector3 weaponOriginalPosition;
    
    public enum States
    {
        Reloading,
        Aiming,
        Idle
    };

    public States currentState;
    
    private void Start()
    {
        currentState = States.Idle;
        
        parent = transform.parent;
        weaponOriginalPosition = parent.position + offset;
    }

    private void Update()
    {
        weaponOriginalPosition = parent.position + offset;
        transform.position = weaponOriginalPosition;
        
        transform.localScale = new Vector3(-0.3f,0.3f, 0.3f);

        if (currentState == States.Reloading)
        {
            transform.RotateAround(weaponOriginalPosition, Vector3.forward, 200 * Time.deltaTime); 
        }

        else if (currentState == States.Idle)
        {
            
        }
        else if (currentState == States.Aiming)
        {
            
        }
    }
}
