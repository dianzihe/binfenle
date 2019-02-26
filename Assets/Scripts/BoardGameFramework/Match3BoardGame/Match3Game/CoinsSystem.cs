using UnityEngine;
using System.Collections;

public class CoinsSystem : MonoBehaviour
{
	public delegate void CoinsUpdated();
	
	protected static CoinsSystem instance;
	
	protected int silverCoins;
	protected int goldCoins;
	
	public event CoinsUpdated OnCoinsUpdated;
	
	public int SilverCoins {
		get {
			return silverCoins;
		}
		set {
			silverCoins = value;
			PlayerPrefs.SetInt("SilverCoins", silverCoins);
		}
	}
	
	public int GoldCoins {
		get {
			return goldCoins;
		}
		set {
			goldCoins = value;
			PlayerPrefs.SetInt("GoldCoins", goldCoins);
		}
	}
	
	public TweaksSystem Tweaks {
		get {
			return TweaksSystem.Instance;
		}
	}
	
	public static CoinsSystem Instance {
		get {
			if (instance == null) {
//				Debug.LogError("CoinsSystem hasn't been initialized");
			}
			
			return instance;
		}
	}
	
	void Awake() 
	{
		instance = this;
		
		Reset();
	}
	
	public void Reset()
	{
		silverCoins = PlayerPrefs.GetInt("SilverCoins", TweaksSystem.Instance.intValues["StartSilver"]);
		goldCoins = PlayerPrefs.GetInt("GoldCoins", TweaksSystem.Instance.intValues["StartGold"]);
	}
	
	public void AddCoins(int silver, int gold)
	{
		SilverCoins += silver;
		GoldCoins += gold;
		
		RaiseCoinsUpdatedEvent();
	}
	
	public bool RemoveCoins(int silver, int gold)
	{
		if (silver <= silverCoins && gold <= goldCoins)
		{
			SilverCoins -= silver;
			GoldCoins -= gold;
			
			RaiseCoinsUpdatedEvent();
			
			return true;
		}
		
		return false;
	}
	
	public bool RemoveGoldCoins(int gold)
	{
		return RemoveCoins(0, gold);
	}
	
	public bool RemoveSilverCoins(int silver)
	{
		return RemoveCoins(silver, 0);
	}
	
	protected void RaiseCoinsUpdatedEvent()
	{
		if (OnCoinsUpdated != null) {
			OnCoinsUpdated();
		}
	}
}

