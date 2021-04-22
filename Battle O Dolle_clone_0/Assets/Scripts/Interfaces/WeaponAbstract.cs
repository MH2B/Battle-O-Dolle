using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class WeaponAbstract : NetworkBehaviour
{

    public enum WeaponTypes
    {
        MeleeWeapon,
        Gun
    }
    public enum WeaponStates
    {
        Reloading,
        Aiming,
        Idle
    };


    [SerializeField] private float damage = 30;
    public float Damage { get => damage; set => damage = value; }


    [SerializeField] private float health = 100f;
    public float Health { get => health ; set => health = value; }

    [SerializeField] private float reloadTime = 6f;
    public float ReloadTime { get => reloadTime; set => reloadTime = value; }


    [SerializeField] private WeaponTypes weaponType;
    public WeaponTypes WeaponType { get => weaponType; set => weaponType = value;}


    private WeaponStates weaponState = WeaponStates.Idle;
    public WeaponStates WeaponState { get => weaponState; set => weaponState = value;}


    protected MeleeWeaponTargeting _meleeWeaponTargeting = null;
    protected IPlayer _iPlayer = null;
    protected AimingDirection _aimingDirection;
    [SerializeField] protected GameObject aimingLine = null;

    protected bool isReloading = false;
    protected bool isAiming = false;

    private void Awake()
	{
        Initialize();
    }

    public virtual void Initialize()
	{
        _meleeWeaponTargeting = GetComponent<MeleeWeaponTargeting>();
        _aimingDirection = GetComponent<AimingDirection>();
    }

	public virtual void Attack() { }

    public virtual void Aim()
	{
        if (isReloading)
        {
            isAiming = false;
            return;
        }
        isAiming = true;
        _aimingDirection.enabled = true;
        aimingLine.SetActive(true);
    }

    public virtual void TakeDamage(float _healthDamage)
	{
        if (Health == 0)
        {
            DestroyWeapon();
        }
		else
		{
            Health -= _healthDamage;
            Health = Mathf.Clamp(Health, 0, 100);
        }
	}
    
    public virtual void DestroyWeapon()
	{
        MirrorSpawner.instance.DestoyGameObject(gameObject);
	}
    
}
