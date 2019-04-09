using UnityEngine;
using System.Collections;

public class CompanionSelect : MonoBehaviour
{	
	public PlayMakerFSM targetFsm;
	//public GameObject tokensContainer;
	public ItemDescriptionLabel itemDescription;
	public static string iconPrefix = "menu_character_";
	public static string[] icons = new string[]{"youngmaleficent","adultmaleficent3", "adultmaleficent3", "adultmaleficent3"};

	public void SetCompanion(int index)
	{
		UISprite spr=GetComponentInChildren<UISprite> ();
		if(spr!=null)
			spr.spriteName = iconPrefix + icons[index];
		itemDescription.UpdateDescription(index);
	}
}

