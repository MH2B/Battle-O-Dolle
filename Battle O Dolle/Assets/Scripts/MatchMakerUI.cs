using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakerUI : MonoBehaviour
{

	[SerializeField] private GameObject beforeRoomPanel = null;
	[SerializeField] private GameObject afterRoomPanel = null;
    [SerializeField] private InputField roomName = null;

    public void JoinGivenRoomNameBtn()
	{
		if (string.IsNullOrEmpty(roomName.text))
		{
			print("Enter room name");
			return;
		}
		else
		{
			string roomNameText = roomName.text.ToUpper();
			MirrorPlayer.localPlayer.JoinARoom(roomNameText);
		}
	}

	public void HostPublicBtn()
	{
		MirrorPlayer.localPlayer.HostARoom(true);
	}


	public void HostPrivateBtn()
	{
		MirrorPlayer.localPlayer.HostARoom(false);
	}

	public void SearchRandomeRoomBtn()
	{
		MirrorPlayer.localPlayer.JoinARoom(null);
	}
}
