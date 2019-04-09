using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;
using System.Collections.Generic;
// this class retrieves the information from the current chapter buttons and
// detects if it's necessary to do an animation when entering into the map scene or to set the fire
public class FireAnimations : MonoBehaviour
{
	public PlayMakerFSM levelFsm;
	public string showEvent = "AutoShow";

	// buttons info
	LoadLevelButton currentButton;

	// fire
	BookAnimations bookAnimations;
	[HideInInspector]
	public GameObject firePrefabInstance;
	[HideInInspector]
	public GameObject firePrefabInstance2;
	LoadLevelButton lastCurrentButtonInstance;
	public GameObject firePrefab;
	bool startAnimationIsDone;

	bool waitingActionOnNewChapterShown = false;

	private static FireAnimations instance = null;
	public static FireAnimations Instance {
		get {
			return instance;
		}
	}

	void Awake()
	{
		instance = this;

		lastCurrentButtonInstance = null;
		firePrefabInstance = null;
		firePrefabInstance2 = null;
		startAnimationIsDone = false;
		//bookAnimations = transform.parent.parent.Find("Chapters").GetComponent<BookAnimations>();
		bookAnimations = GetComponent<BookAnimations>();

		bookAnimations.OnNewChapterShown += ActionOnNewChapterShown;
	}
	
	// Use this for initialization
	void Start ()
	{	
		if (UserManagerCloud.Instance != null) {
			UserManagerCloud.Instance.UserHasBeenDownloadedFromCloud += DownloadedFromCloud;
		}
	}
	
	// this function is called when new info comes from cloud
	void DownloadedFromCloud(object sender, UserCloudDownloadDelegateEventArgs e)
	{
		currentButton = null;
		ChaptersManager.Instance.RefreshChaptersInfo();
	}

	// this function is called every time a chapter needs to be painted or refreshed
	public bool UpdateAvatar(bool positionMayChange)
	{
		bool levelsAnimationIsNeeded = false;
		if(positionMayChange && lastCurrentButtonInstance != currentButton && firePrefabInstance != null)
		{
			Destroy(firePrefabInstance);
			Destroy(firePrefabInstance2);
		}

		// button on fire
		if(currentButton != null && lastCurrentButtonInstance != currentButton)
		{
			firePrefabInstance = GameObject.Instantiate(firePrefab) as GameObject;
			firePrefabInstance.transform.parent = currentButton.transform;
			firePrefabInstance.transform.localPosition = currentButton.transform.localPosition;
			firePrefabInstance.transform.rotation = Quaternion.identity;
			firePrefabInstance.transform.localScale = new Vector3(300f,300f,300f);
		}

		// the start animation is done just once
		if(!startAnimationIsDone)
		{
			// check if needs to the the animation from one level to the other
			levelsAnimationIsNeeded =
				!ChaptersManager.Instance.newChapterUnlocked &&
				LoadLevelButton.newUnlockedLevel &&
					LoadLevelButton.secondTolastButton != null;

			DoStartAnimation(levelsAnimationIsNeeded);
		}

		if(currentButton == null)
			return false;

		// save the current button as the last current button instance (it may change at downloading data)
		lastCurrentButtonInstance = currentButton;
		return levelsAnimationIsNeeded;
	}

	// instantiate fire prefab and places it behind some level button
	void SetButtonOnFire(LoadLevelButton button,GameObject go)
	{
		go = GameObject.Instantiate(firePrefab) as GameObject;
		go.transform.parent = button.transform;
		go.transform.localPosition = button.transform.localPosition;
		go.transform.rotation = Quaternion.identity;
		go.transform.localScale = new Vector3(300f,300f,300f);
	}

	// does the animation when entering to the map
	void DoStartAnimation(bool levelsAnimationIsNeeded)
	{
		// new chapter animation
		if(ChaptersManager.Instance.newChapterUnlocked)
		{
			if(LoadLevelButton.lastUnlockedLevelInChapter != null)
			{
				bookAnimations.GoToLevelButton(LoadLevelButton.lastUnlockedLevelInChapter,false);
				ChaptersManager.Instance.GoToNextChapter();
				//yield return new WaitForSeconds(2.7f);
				waitingActionOnNewChapterShown = true;
			}
		}
		else
		{
			// new unlocked level animation
			if (LoadLevelButton.newUnlockedLevel) 
			{
				if (levelsAnimationIsNeeded) 
				{
					firePrefabInstance2 = GameObject.Instantiate(firePrefab) as GameObject;
					firePrefabInstance2.transform.parent = LoadLevelButton.secondTolastButton.transform;
					firePrefabInstance2.transform.localPosition = LoadLevelButton.secondTolastButton.transform.localPosition;
					firePrefabInstance2.transform.rotation = Quaternion.identity;
					firePrefabInstance2.transform.localScale = new Vector3(300f,300f,300f);
	
					bookAnimations.StartCoroutine(bookAnimations.DoLevelsTransition(ActionOnMoveComplete));
				}
				else 
				{
					if(LoadLevelButton.lastButton != null)
						bookAnimations.GoToLevelButton(LoadLevelButton.lastButton,true,ActionOnMoveComplete);
				}

			}
			else if (LoadLevelButton.showBuyLives) 
			{
				// show buy lives panel
				LoadLevelButton.showBuyLives = false;
				levelFsm.SendEvent(showEvent);
			}
			else
			{
				if(LoadLevelButton.lastLevelPlayed != null)
				{
					// the normal case, goes to last level played button
					bookAnimations.GoToLevelButton(LoadLevelButton.lastLevelPlayed);
				}
			}
		}
		startAnimationIsDone = true;
	}

	public void ActionOnMoveComplete()
	{
		LoadLevelButton.newUnlockedLevel = false;
		LoadLevelButton.newUnlockedLevel = false;
		Debug.Log("ActionOnMoveComplete");
		Dictionary<string,object> dict=new Dictionary<string, object>();
		dict.Add(LoadLevelButton.lastUnlockedLevel.ToString(),1);
		TalkingDataGA.OnEvent(LoadLevelButton.lastUnlockedLevel.ToString()+Language.Get("In_Level_Desc"),dict); 

		// will just show the next level panel in case "NextLevel" class consider it necessary
		if (LoadLevelButton.showNextLevel) 
		{
			LoadLevelButton.showNextLevel = false;

			if (LoadLevelButton.lastButton != null) 
			{
				LoadLevelButton.lastButton.OnClick();
				LoadLevelButton.lastButton.levelFsm.SendEvent(showEvent);
			}
		}
	}

	// just starts action's coroutine
	void ActionOnNewChapterShown()
	{
		if(waitingActionOnNewChapterShown)
		{
			StartCoroutine(OnNewChapterShownCoroutine());
			waitingActionOnNewChapterShown = false;
		}
	}
	IEnumerator OnNewChapterShownCoroutine()
	{
		yield return new WaitForSeconds(1f);
		bookAnimations.GoToLevelButton(LoadLevelButton.lastButton,true,ActionOnMoveComplete);
	}

	void OnDestroy()
	{
		if (UserManagerCloud.Instance != null) {
			UserManagerCloud.Instance.UserHasBeenDownloadedFromCloud -= DownloadedFromCloud;
		}
		instance = null;
		bookAnimations.OnNewChapterShown -= ActionOnNewChapterShown;
	}
	
	public void UpdateFirePosition(LoadLevelButton button)
	{
		this.currentButton = button;
	}
}

