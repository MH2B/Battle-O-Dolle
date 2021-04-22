using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float bulletSpeed = 100;
    public Vector3 movementDirection;

    private float damage = 70f;
    public float Damage { get => damage; set => damage = value; }


    private string targetTag;
    public string TargetTag { get => targetTag; set => targetTag = value; }


    private string shooterTag;
    public string ShooterTag { get => shooterTag; set => CmdSetTheShooterTag(value); }


	public override void OnStartClient()
	{
		if (hasAuthority)
		{
            Invoke("InvokeDestroy", 5f);
		}
		else
		{
            Destroy(this);
		}
	}


	private void InvokeDestroy()
	{
        MirrorSpawner.instance.DestoyGameObject(gameObject);
    }

    private void Update()
    {
        transform.Translate(movementDirection * bulletSpeed * Time.deltaTime);
    }



    [Command]
    private void CmdSetTheShooterTag(string _value)
	{
        RPCSetShooterTag(_value);
    }

    [ClientRpc] private void RPCSetShooterTag(string value)
    { 
        shooterTag = value;
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasAuthority || tag == "Untagged")
        {
            return;
        }

        if (other.tag != shooterTag && other.tag != "Weapon" && other.tag != "BuyZone" && other.tag != "ObjectiveReachPoint")
        {
            if (other.tag == "BlueTeam" || other.tag == "RedTeam")
			{
                other.GetComponent<PlayerMatchData>().TakeDamage(Damage);
            }
            print("destroy bullet that has tag : " + tag + " by tag : " + other.tag);
            Destroy(gameObject);
        }
    }
}
