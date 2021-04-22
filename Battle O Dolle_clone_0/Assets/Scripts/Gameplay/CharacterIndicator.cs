using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterIndicator : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;

	[SerializeField] private PlayerMatchData _iPlayer = null;
    [SerializeField] private Sprite redTeamIndicatorSprite;
    [SerializeField] private Sprite blueTeamIndicatorSprite;


	public override void OnStartClient()
	{
		Invoke("IndicatorWaiter", 1f);
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	private void IndicatorWaiter()
	{
		if (_iPlayer.TeamGetter() == "BlueTeam")
		{
			spriteRenderer.sprite = blueTeamIndicatorSprite;
		}
		else
		{
			spriteRenderer.sprite = redTeamIndicatorSprite;
		}
	}


}
