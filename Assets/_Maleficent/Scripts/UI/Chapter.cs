using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Chapter : MonoBehaviour {
	[HideInInspector]
	public int lastLevelIdx;	// last level idx in the chapter
	LevelButtons levelButtons;

	[HideInInspector]
	public bool unlocked = false;

	public GameObject previousChapter;
	public GameObject nextChapter;
	GameObject mapElements;
	bool needToUnloadAssets;

	public void Awake()
	{
		gameObject.SetActive(false);
		levelButtons = transform.Find("LevelButtons").GetComponent<LevelButtons>();
		lastLevelIdx = int.Parse(levelButtons.transform.GetChild(levelButtons.transform.childCount-1).transform.name);
	
		CheckUnlocked();

		// check if the phone is able to keep all map info stored at once or not
		needToUnloadAssets = QualitySetter.shouldUnloadMapAssets;
		// if so, load all map elements at the beginning already
		if(!needToUnloadAssets)
			LoadMapElements();
	}

	// checks if the chapter has been unlocked already
	public void CheckUnlocked()
	{
		int firstLevelIdx = int.Parse(levelButtons.transform.GetChild(0).transform.name);
		int lastLevelIdx = int.Parse(levelButtons.transform.GetChild(levelButtons.transform.childCount-1).transform.name);
		/* 
		if(UserManagerCloud.Instance.CurrentUser.LastFinishedLvl+1 >= firstLevelIdx)
		{
			unlocked = true;
		}
		else
		{
			unlocked = false;
		}
		*/
		unlocked = true;
	}

	// every time the chapter is shown
	public void OnEnable()
	{
		Init();

		if(needToUnloadAssets)
			LoadMapElements();
	}

	// every time the chapter is hidden
	public void OnDisable()
	{
		if(needToUnloadAssets)
			UnloadMapElements();
	}

	void LoadMapElements()
	{
		mapElements = GameObject.Instantiate(Resources.Load("Map/"+transform.name+"_map_elements") as GameObject,Vector3.zero,Quaternion.identity) as GameObject;
		mapElements.transform.parent = this.transform;
		mapElements.transform.localScale = Vector3.one;
		mapElements.transform.localPosition = Vector3.zero;
	}

	void UnloadMapElements()
	{
		Destroy(mapElements);
		mapElements = null;
		Resources.UnloadUnusedAssets();
	}
	
	public void Init(bool forceRedraw = false)
	{
		// set all chapter info to be refreshed
		levelButtons.Init(forceRedraw);
	}

	public void RefreshLevelsInfo()
	{
		levelButtons.RefreshLevelsInfo();
	}
}
