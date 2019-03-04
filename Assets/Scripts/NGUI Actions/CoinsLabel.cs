using UnityEngine;
using System.Collections;

public class CoinsLabel : MonoBehaviour
{
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		
		UpdateCoins();
		
		CoinsSystem.Instance.OnCoinsUpdated += UpdateCoins;
	}
	
	void UpdateCoins () 
	{
		label.text = CoinsSystem.Instance.SilverCoins.ToString() + " Silver     " + CoinsSystem.Instance.GoldCoins.ToString() + " Gold";
	}
}

