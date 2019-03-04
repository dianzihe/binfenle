using UnityEngine;
using System.Collections;

public class AwardCoinsLabel : MonoBehaviour
{
	UILabel label;
	int initSilverCoins;
	int initGoldCoins;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		
		initSilverCoins = CoinsSystem.Instance.SilverCoins;
		initGoldCoins = CoinsSystem.Instance.GoldCoins;
		
		UpdateCoins();
		
		CoinsSystem.Instance.OnCoinsUpdated += UpdateCoins;
	}
	
	void UpdateCoins () 
	{
		label.text = "+" + (CoinsSystem.Instance.SilverCoins - initSilverCoins).ToString() + " Silver"; 
		
		int gold = CoinsSystem.Instance.GoldCoins - initGoldCoins;
		if (gold > 0) {
			label.text = label.text + "     +" + gold.ToString() + " Gold";
		}
	}
}

