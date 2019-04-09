using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ChaptersManager : MonoBehaviour {

	static ChaptersManager instance;
	public static ChaptersManager Instance {
		get {
			return instance;
		}
	}
	
	string currentChapterKey = "CurrentChapter";
	string maxChapterKey = "MaxChapter";
	
	[HideInInspector]
	int currentChapter = 1;
	int previousChapter = -1;
	int currentMaxChapter;
	[HideInInspector]
	public bool newChapterUnlocked;

	// for level buttons collision
	// it's instanced here so that there's no need to put it into the inspector of all 'LevelButton' game objects
	public GameObject pagesGO;
	MeshCollider pagesCollider;

	// related scripts
	BookAnimations bookAnimations;

	bool updateDetected = false;

	void Awake()
	{
		instance = this;

		// load player prefs info
		currentChapter = PlayerPrefs.GetInt(currentChapterKey,1);
		currentMaxChapter = PlayerPrefs.GetInt(maxChapterKey,1);

		// get last unlocked level
		UserCloud currentUser = UserManagerCloud.Instance.CurrentUser;
		LoadLevelButton.lastUnlockedLevel = currentUser.LastFinishedLvl + 1;

		// Refresh info in case of update (game was over but then more levels were loaded)
		// we can't get last level info from cloud (this info is not stored)
		if(LoadLevelButton.lastUnlockedLevel != LoadLevelButton.maxLevels + 1)
		{
			updateDetected = PlayerPrefs.GetInt("GameOver") == 1;
			PlayerPrefs.SetInt("GameOver",0);
		}

		bookAnimations = GetComponent<BookAnimations>();
		pagesCollider = pagesGO.GetComponent<MeshCollider>();
	}

	void Start()
	{
		// we force last page in case of update
		if(updateDetected)
		{
			currentChapter = currentMaxChapter;
			LoadLevelButton.lastLevelPlayedIdx = LoadLevelButton.lastUnlockedLevel-1;
			LoadLevelButton.newUnlockedLevel = true;
		}

		newChapterUnlocked = UpdateCurrentMaxChapter() && LoadLevelButton.newUnlockedLevel;

		SetCurrentChapter();

		if(LoadLevelButton.newUnlockedLevel)
			SavePlayerPrefs(LoadLevelButton.lastUnlockedLevel,newChapterUnlocked?currentMaxChapter:currentChapter);
	}

	void OnDestroy()
	{
		instance = null;
	}

	public MeshCollider GetPagesCollider()
	{
		return pagesCollider;
	}

	Chapter GetPreviousChapter()
	{
		return transform.GetChild(previousChapter-1).GetComponent<Chapter>();
	}

	public Chapter GetCurrentChapter()
	{
		return transform.GetChild(currentChapter-1).GetComponent<Chapter>();
	}

	// update max chapter info if necessary, returns true in case a new chapter has been unlocked
	bool UpdateCurrentMaxChapter()
	{
		int lastUnlockedChapter = 0;
		foreach(Transform child in transform.Cast<Transform>().OrderBy(t=>t.name))
		{
			if(child.GetComponent<Chapter>().unlocked)
				lastUnlockedChapter = int.Parse(child.transform.name);
		}

		bool newChapterUnlocked = lastUnlockedChapter > currentMaxChapter;
		currentMaxChapter = lastUnlockedChapter;
		return newChapterUnlocked;
	}

	// at clicking arrow left
	public void GoToPreviousChapter()
	{
		if(currentChapter > 1)
		{
			previousChapter = currentChapter;
			currentChapter--;
			bookAnimations.FlipToPreviousPage();
		}
	}

	// at clicking arrow right
	public void GoToNextChapter()
	{
		if(currentChapter < transform.childCount)
		{
			previousChapter = currentChapter;
			currentChapter++;
			if(currentChapter > currentMaxChapter)
				currentMaxChapter = currentChapter;
			bookAnimations.FlipToNextPage();
		}
	}

	// changes current chapter
	public void SetCurrentChapter() {
		if(previousChapter > 0)
			GetPreviousChapter().gameObject.SetActive(false);

		GetCurrentChapter().gameObject.SetActive(true);
	}

	// used from DataCheaterEditor, corresponding to DataCheater GameObject
	public void Cheat(int lastLevel)
	{
		for(int i=1;i<lastLevel;i++)
			UserManagerCloud.Instance.SetScoreForLevel(i, 5000, 3);

		RefreshChaptersInfo();
	}

	// this function is just called when downloading info from cloud or cheating player data
	public void RefreshChaptersInfo()
	{
		UserCloud currentUser = UserManagerCloud.Instance.CurrentUser;
		LoadLevelButton.lastLevelPlayedIdx = LoadLevelButton.lastUnlockedLevel = currentUser.LastFinishedLvl + 1;

		// refresh which chapters are unlocked and which are not
		foreach(Transform child in transform.Cast<Transform>().OrderBy(t=>t.name))
			child.GetComponent<Chapter>().CheckUnlocked();
		// update max chpater info
		UpdateCurrentMaxChapter();

		// refresh info in the map
		GetCurrentChapter().Init(true);
		GetCurrentChapter().RefreshLevelsInfo();

		// save info just in case of restart
		SavePlayerPrefs(LoadLevelButton.lastLevelPlayedIdx,currentMaxChapter);
	}

	// this function is called before starting a level
	public void SaveMapInfo()
	{
		SavePlayerPrefs(MaleficentBlackboard.Instance.level,currentChapter);
	}

	// stores the necessary information to load the map next time (level and chapter)
	void SavePlayerPrefs(int levelIdx,int chapterIdx)
	{
		PlayerPrefs.SetInt(currentChapterKey, chapterIdx);
		PlayerPrefs.SetInt(maxChapterKey,currentMaxChapter);
		PlayerPrefs.SetInt("lastLevelPlayedIdx", levelIdx);

		PlayerPrefs.Save();
	}
}
