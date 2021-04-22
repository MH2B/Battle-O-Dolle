using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{

	private Animator anim;

	private void Awake()
	{
		anim = GetComponent<Animator>();
	}


	public void CanDoAbility()
	{
		if (!anim.GetBool("ulti"))
		{
			anim.SetBool("ulti", true);
			Invoke("ResetAbilityBool", 0.1f);
		}
	}

	private void ResetAbilityBool()
	{
		anim.SetBool("ulti", false);
	}

	public void CanTakeHit()
	{
		if (!anim.GetBool("hit"))
		{
			anim.SetBool("hit", true);
			Invoke("ResetHitBool", 0.1f);
		}
	}

	private void ResetHitBool()
	{
		anim.SetBool("hit", false);
	}

	public void CanWalk()
	{
		if (!anim.GetBool("walk"))
		{
			anim.SetBool("walk", true);
		}
	}

	public void CanIdle()
	{
		if (anim.GetBool("walk"))
		{
			anim.SetBool("walk", false);
		}
	}

}
