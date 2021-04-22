using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ChangeBtnToJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	private enum btnFunctionality { Ability , Weapon}

	[SerializeField] private btnFunctionality function = btnFunctionality.Ability;

	[SerializeField] private GameObject joystick = null;

	[SerializeField] private Image shoot = null;


	[SerializeField] private UIBtns uIBtns = null;

	private Button changingBtn = null;

	private bool isAiming = false;

	private void Awake()
	{
		changingBtn = GetComponent<Button>();
	}


	public void OnDrag(PointerEventData eventData)
	{
		if (!changingBtn.interactable)
		{
			return;
		}
		if (joystick.activeInHierarchy)
		{
			joystick.GetComponent<FixedJoystick>().OnDrag(eventData);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!changingBtn.interactable)
		{
			return;
		}
		isAiming = true;
		joystick.GetComponent<FixedJoystick>().OnPointerDown(eventData);
		shoot.enabled = false;
		joystick.SetActive(true);
		switch (function)
		{
			case btnFunctionality.Ability:
				uIBtns.onAimingSelectDelegate();
				break;
			case btnFunctionality.Weapon:
				uIBtns.onWeaponBtnSelectDelegate();
				break;
		}
		
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		//if (!changingBtn.interactable)
		//{
		//	return;
		//}
		if (!isAiming)
		{
			return;
		}
		isAiming = false;
		joystick.GetComponent<FixedJoystick>().OnPointerUp(eventData);
		shoot.enabled = true;
		joystick.SetActive(false);
		switch (function)
		{
			case btnFunctionality.Ability:
				uIBtns.onAimingDeSelectDelegate();
				break;
			case btnFunctionality.Weapon:
				uIBtns.onWeaponBtnDeSelectDelegate();
				break;
		}
		
	}
}
