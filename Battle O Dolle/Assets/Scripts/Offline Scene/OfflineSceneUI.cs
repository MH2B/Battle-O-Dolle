using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OfflineSceneUI : MonoBehaviour
{

    [SerializeField] private InputField nameInputField = null;
	[SerializeField] private GameObject connectionToserverPanel = null;
	[SerializeField] private GameObject inputPlayerNamePanel = null;

	private void Awake()
	{
		if (PlayerPrefs.HasKey("playerName"))
		{
			nameInputField.text = PlayerPrefsManager.GetThePlayerName();
		}
	}

	public void ConnectToServerBtn()
	{
		if (string.IsNullOrEmpty(nameInputField.text))
		{
			print("Plz enter the name");
		}
		else
		{
			PlayerPrefsManager.SetThePlayerName(nameInputField.text);
			inputPlayerNamePanel.SetActive(false);
			connectionToserverPanel.SetActive(true);
			AutoHostClient.instance.StartTheHostingOrClienting();

		}
	}

}
