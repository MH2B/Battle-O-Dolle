using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerContainer : NetworkBehaviour
{

    public static PlayerContainer instance = null;

	[Header("Canvas Settings")]
    [SerializeField] private Text playerNameText = null;
	[SerializeField] private Image heroImage = null;

	[Header("Sync Vars")]
    [SyncVar] public string playerName = null;
	[SyncVar] public string heroName = null;

	[Header("Heros Sprites")]
	[SerializeField] private List<Sprite> herosSprites = new List<Sprite>();


	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}

	public void SetPlayerName(string _playerName)
	{
		playerName = _playerName;
		playerNameText.text = _playerName;
	}

	public void SetPlayerHero(string _heroName)
	{
		heroName = _heroName;
		foreach(Sprite heroSprite in herosSprites)
		{
			if(heroSprite.name == _heroName)
			{
				heroImage.sprite = heroSprite;
				return;
			}
		}
	}


}
