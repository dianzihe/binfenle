using UnityEngine;
using System.Collections;

public class LivesDestroyer : MonoBehaviour 
{
	public Match3BoardGameLogic boardLogic;
	
	protected bool canDestroyLife = false;
	protected bool pauseLifeDestroyed = false;
	protected int pauseLives = 0;
	
	public bool CanDestroyLife {
		get {
			return canDestroyLife;
		}
	}
	
	public int Lives {
		get {
			return UpdateLives(false);
		}
	}
	
	void Start()
	{
		boardLogic.loseConditions.OnNewMove += ThouShallObliterateLife;
		boardLogic.winConditions.OnWinChecked += ThouShallProtectLife;
		BasicItem.OnActuallyUsingAnyItem += OnActuallyUsingAnyItem;
	}

	void OnActuallyUsingAnyItem (BasicItem item)
	{
		ThouShallObliterateLife();
	}
	
	void ThouShallObliterateLife()
	{
		canDestroyLife = true;
		boardLogic.loseConditions.OnNewMove -= ThouShallObliterateLife;
		BasicItem.OnActuallyUsingAnyItem -= OnActuallyUsingAnyItem;
	}
	
	void ThouShallProtectLife()
	{
		canDestroyLife = false;
	}
	
	int UpdateLives(bool save, bool updateNotifications = false)
	{
		LivesSystem.lifeRefillTime = TweaksSystem.Instance.intValues["LifeRefillTime"];
		LivesSystem.maxLives = TweaksSystem.Instance.intValues["MaxLives"];
		
		long time = LivesSystem.TimeSeconds();
		int lives = PlayerPrefs.GetInt(LivesSystem.livesKey, LivesSystem.maxLives);
		long waitTime = lives < LivesSystem.maxLives ? long.Parse(PlayerPrefs.GetString(LivesSystem.livesTimeKey, time.ToString())) : time;
		
		int newLives = (int)(time - waitTime) / (int)LivesSystem.lifeRefillTime;
		if (newLives + lives >= LivesSystem.maxLives) {
			waitTime = time;
			lives = Mathf.Max(LivesSystem.maxLives, lives);
		}
		else {
			lives += newLives;
			waitTime += newLives * LivesSystem.lifeRefillTime;
		}
		
		LivesSystem.SaveLivesAndNotify(lives, waitTime, false);
		
		if (save) {
			PlayerPrefs.Save();
		}
		
		return lives;
	}
	
	public void DestroyLife()
	{
		if (!canDestroyLife) {
			return;
		}
		
		canDestroyLife = false;
		
		int lives = UpdateLives(false);
		lives = Mathf.Max(0, lives - 1);
		
		LivesSystem.SaveLivesAndNotify(lives, long.Parse(PlayerPrefs.GetString(LivesSystem.livesTimeKey, LivesSystem.TimeSeconds().ToString())));
		PlayerPrefs.Save();
	}
	
	public void RetryLivesCheck()
	{
		if (Lives == 0) {
			Debug.LogWarning("SHOW BUY LIVES TRUE");
			LoadLevelButton.showBuyLives = true;
			//AdController.buyLivesPanelFlag = true;
		}
	}
	
//	void OnApplicationQuit() 
//	{
//		DestroyLife();
//	}
	
	void OnApplicationPause(bool pause) 
	{
		int lives = UpdateLives(false);
		int oldLives = lives;
		
		if (pause) {
			if (canDestroyLife) {
				Debug.Log("Destroying life");
				pauseLives = lives;
				lives = Mathf.Max(0, lives - 1);
				pauseLifeDestroyed = true;
			}
		}
		else if (pauseLifeDestroyed) {
			Debug.Log("Restoring life");
			lives = Mathf.Min(Mathf.Max(LivesSystem.maxLives, pauseLives), lives + 1);
			pauseLifeDestroyed = false;
		}
		
		if (lives != oldLives) {
			Debug.Log("Saving new lives");
			LivesSystem.SaveLivesAndNotify(lives, long.Parse(PlayerPrefs.GetString(LivesSystem.livesTimeKey, LivesSystem.TimeSeconds().ToString())));
			PlayerPrefs.Save();
		}
	}
	
	void OnDestroy()
	{
		if (boardLogic) {
			if (boardLogic.loseConditions) {
				boardLogic.loseConditions.OnNewMove -= ThouShallObliterateLife;
			}
		}
		
		BasicItem.OnActuallyUsingAnyItem -= OnActuallyUsingAnyItem;
	}
}
