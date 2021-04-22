using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class MovementAbstract : NetworkBehaviour
{

	#region Vars

	//Classes
	protected FloatingJoystick movementJoystick = null;
	protected FieldOFView fielOfView = null;
	protected AnimatorController animatorController = null;

	//Components
	protected Rigidbody2D rb = null;

	//Fields
	[Range(0, 2000)] [SerializeField] protected float moveSpeed = 1000f;
	[Range(0, 50)] [SerializeField] protected float viewDistance = 10f;
	[Tooltip("Which layer should this characters FOV see as a blocker")] [SerializeField] private LayerMask fovLayerMask = new LayerMask();

	#endregion


	private void OnEnable()
	{
		if (!hasAuthority)
		{
			return;
		}
		if (fielOfView != null)
		{
			SetTheFOVSettings();
		}
	}


	public virtual void Start()
	{
		if (!hasAuthority)
		{
			return;
		}

		// Get the components from the Hierachy

		movementJoystick = UIBtns.instance.movementJoystick.GetComponent<FloatingJoystick>();
		fielOfView = GameObject.FindGameObjectWithTag("Fov").transform.GetComponent<FieldOFView>();
		animatorController = GetComponent<AnimatorController>();
		rb = GetComponent<Rigidbody2D>();

		// Set the settings
		SetTheFOVSettings();
		SetTheFollowTarget();
	}


	public void SetTheFollowTarget()
	{
		GameObject.FindGameObjectWithTag("Vcam").GetComponent<CameraFollow>().SettheFollowTarget(transform);
	}

	public virtual void SetTheFOVSettings()
	{
		fielOfView.ViewDistance = viewDistance;
		fielOfView.fovLayerMask = fovLayerMask;
	}

	public virtual void Update()
	{
		if (!hasAuthority)
		{
			return;
		}

		//Get the direction from joystick
		Vector2 direction = GetDirection();
		rb.AddForce(direction * moveSpeed * Time.deltaTime, ForceMode2D.Force);

		//Set the FOV vars
		fielOfView.SetTheOrigin(new Vector2(transform.position.x, transform.position.y));

		//Aniamtions
		if (animatorController != null)
		{
			if (direction != Vector2.zero)
			{
				animatorController.CanWalk();
			}
			else
			{
				animatorController.CanIdle();
			}
		}
	}

	public virtual Vector2 GetDirection()
	{
		//Horizontal Input
		float horizontal = movementJoystick.Horizontal;
		//Flip ToRight
		if (horizontal >= 0.2f)
		{
			if (transform.localScale.x > 0)
			{
				Vector2 newScale = transform.localScale;
				newScale.x = -newScale.x;
				transform.localScale = newScale;
			}
		}
		//Flip ToLeft
		else if (horizontal <= -0.2f)
		{
			if (transform.localScale.x < 0)
			{
				Vector2 newScale = transform.localScale;
				newScale.x = -newScale.x;
				transform.localScale = newScale;
			}
		}
		else
		{
			horizontal = 0;
		}

		//Vertical Input
		float vertical = movementJoystick.Vertical;
		if (vertical < 0.2f && vertical > -0.2f)
		{
			vertical = 0;
		}

		Vector2 dir = new Vector2(horizontal, vertical);
		return dir.normalized;
	}


}
