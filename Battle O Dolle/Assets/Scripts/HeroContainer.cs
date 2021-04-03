using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeroType { Jallad , Rammal , Parastar , Sarbaz}
public class HeroContainer : MonoBehaviour
{
	[SerializeField] private HeroType heroType = HeroType.Jallad;


	public void ChooseHeroBtn()
	{
		MatchMakerUI.instance.ChooseHero(heroType);
	}

	public void HeoInfoBtn()
	{
		MatchMakerUI.instance.HeoInfo(heroType);
	}
}
