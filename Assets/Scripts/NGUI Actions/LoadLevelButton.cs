using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadLevelButton : UIControl 
{
	public PlayMakerFSM levelFsm;
	public static int lastLevelPlayedIdx;
	public static int lastUnlockedLevel = 1;
	public static int maxLevels = 105;
	public static bool newUnlockedLevel = false;
	// indicates if level panel needs to be shown when entering the map scene
	public static bool showNextLevel = false;
	// indicates if lives panel needs to be shown when entering the map scene
	public static bool showBuyLives = false;

	[System.NonSerialized]
	public int levelIdx = -1;
	public int backgroundIdx = 1;
	public int characterIdx;
	[HideInInspector]
	public bool unlocked = false;

	// button graphic elements
	UISprite background;
	UILabel label;
	UISprite stars;
	GameObject glow;

	// load level buttons, in order to move the camera, set the fire, etc.
	public static LoadLevelButton lastButton;
	public static LoadLevelButton secondTolastButton;
	public static LoadLevelButton lastUnlockedLevelInChapter;
	public static LoadLevelButton lastLevelPlayed;

	void Awake()
	{
		// level idx is taken directly from the GO name
		levelIdx = int.Parse(transform.name);

		background = transform.Find("Background").GetComponent<UISprite>();
		label = transform.Find("Label").GetComponent<UILabel>();
		stars = transform.Find("Stars").GetComponent<UISprite>();
		glow = transform.Find("Glow").transform.gameObject;
		label.text = "";
		LoadLevelButton.lastLevelPlayedIdx = PlayerPrefs.GetInt("lastLevelPlayedIdx",1);

		// the last level will be pointing to a different FSM
		if (levelIdx == maxLevels + 1)
			levelFsm = CompanionsManager.Instance.gameEndFsm;

		// level buttons are off by default
		background.spriteName = "map_location_01_off";
		background.alpha = 0f;
		stars.alpha = 0f;
		glow.SetActive(false);
	}

	public void UpdateButtonStatus()
	{
		if (UserManagerCloud.Instance == null) {
			return;
		}

		if(levelIdx <= lastUnlockedLevel)
		{
			if(lastUnlockedLevelInChapter == null || levelIdx > lastUnlockedLevelInChapter.levelIdx)
				lastUnlockedLevelInChapter = this;
			unlocked = true;
		}
		else
		{
			unlocked = false;
		}

		if(glow != null)
			glow.SetActive(unlocked);

		if (levelIdx == lastUnlockedLevel)
		{
			lastButton = this;
			// already has the fire one on it
			glow.SetActive(false);

			FireAnimations.Instance.UpdateFirePosition(this);
		}
		else if (newUnlockedLevel && levelIdx == lastUnlockedLevel - 1) 
		{
			secondTolastButton = this;
		}

		if(levelIdx == lastLevelPlayedIdx)
			lastLevelPlayed = this;

		StartCoroutine("WaitOneFrame");
	}

	IEnumerator WaitOneFrame()
	{
		yield return new WaitForEndOfFrame();
		background.alpha = 1f;
		stars.alpha = 1f;
		UpdateButtonGraphics();
	}

	void UpdateButtonGraphics()
	{
		if (levelIdx > LoadLevelButton.lastUnlockedLevel)
		{
			background.spriteName = "map_location_01_off";
			label.text = "";
			stars.enabled = false;
		}
		else
		{
			background.spriteName = "map_location_01";
			label.text = levelIdx == maxLevels + 1 ? "?" : levelIdx.ToString();
			int numStars = UserManagerCloud.Instance.GetStarsForLevel(levelIdx);
			stars.enabled = numStars > 0;
			stars.spriteName = "star_map_0"+numStars;
		}
	}

	// a level button is clicked (up because of possible dragging)
	public override void OnTouchUp(BravoInputManager.TouchCollision _collisionInfo) {
		if(!unlocked || BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;
	
		levelFsm.SendEvent("AutoShow");
		OnClick();
	}

	// action when a level button is clicked or simulated to be clicked (check showNextLevel var)
	public void OnClick() 
	{
		if (levelIdx <= maxLevels)
		{
			MaleficentBlackboard.Instance.level = levelIdx;
			MaleficentBlackboard.Instance.levelBg = backgroundIdx;
			CompanionsManager.Instance.UpdateCompanions(characterIdx);
		}
	}

	void OnDestroy()
	{
		lastButton = null;
		secondTolastButton = null;
		lastUnlockedLevelInChapter = null;
		lastLevelPlayed = null;
	}

}
