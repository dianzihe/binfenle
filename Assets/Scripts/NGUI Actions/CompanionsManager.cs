using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompanionsManager : MonoBehaviour
{
	private static CompanionsManager instance;
	
	public CompanionSelect companionLandscape;
	public CompanionSelect companionPortrait;
	
	public UILabel chooseLabelLandscape;
	public UILabel chooseLabelPortrait;
		
	public UILabel targetLabelLandscape;
	public UILabel targetLabelPortrait; 
	public UILabel objectiveLabelLandscape;
	public UILabel objectiveLabelPortrait;
	
	public TokensLabel tokensLabelLandscape;
	public TokensLabel tokensLabelPortrait;
	
	public LevelDestroyTargets[] destroyTargets;
	
	protected Vector3 positionLandscape;
	protected Vector3 positionPortrait;

	public GameObject friendAvatarPrefab;
	
	public PlayMakerFSM gameEndFsm;
	
	public static CompanionsManager Instance {
		get {
			return instance;
		}
	}	
	
	// Use this for initialization
	void Awake()
	{
		instance = this;
	}

	public void OnDestroy() {
		instance = null;
	}	
	
	void Start()
	{
		// load the first level prefab to have the resources in memory and make the other levels load faster
		Resources.Load("Game/Levels/Level_Maleficent1");
		
		Debug.Log("app rater - last level: " + LoadLevelButton.lastUnlockedLevel);
		if (LoadLevelButton.lastUnlockedLevel > 10) {
//			AppRater.instance.TryShowAppRater(Language.Get("APP_RATE_TITLE"), Language.Get("APP_RATE_MESSAGE"), 
//				Language.Get("APP_RATE_BUTTON_RATE"), Language.Get("APP_RATE_BUTTON_NEVER"), Language.Get("APP_RATE_BUTTON_CANCEL"));
		}
	}

	public void UpdateCompanions(int characterIdx) 
	{
		chooseLabelLandscape.GetComponent<UILabel>().text = Language.Get("LEVEL_COMPANION");
		chooseLabelPortrait.GetComponent<UILabel>().text = Language.Get("LEVEL_COMPANION");
		
		CharacterSpecialAnimations.CharIdx = characterIdx;

		companionLandscape.SetCompanion(characterIdx);
		companionPortrait.SetCompanion(characterIdx);
		
		tokensLabelLandscape.UpdateStatus();
		tokensLabelPortrait.UpdateStatus();

		GameObject levelPrefab = Resources.Load("Game/Levels/Level_Maleficent" + MaleficentBlackboard.Instance.level) as GameObject;

		if (levelPrefab != null) 
		{
			Match3BoardRenderer levelData = levelPrefab.GetComponent<Match3BoardRenderer>();

			if (levelData != null) {
				string key = "Level" + MaleficentBlackboard.Instance.level + "Star1";

				if (TweaksSystem.Instance.intValues.ContainsKey(key)) {
					targetLabelLandscape.text = Language.Get("LEVEL_TARGET") + " " + ScoreSystem.FormatScore(TweaksSystem.Instance.intValues[key]);
				}
				else {
					targetLabelLandscape.text = Language.Get("LEVEL_TARGET") + " " + ScoreSystem.FormatScore((levelData.winConditions as WinScore).targetScore);
				}
				targetLabelPortrait.text = targetLabelLandscape.text;
				//objectiveLabelLandscape.text = Language.Get("LEVEL_OBJECTIVE") + "\n" + levelData.winConditions.GetObjectiveString();
				objectiveLabelLandscape.text = levelData.winConditions.GetShortObjectiveString(levelData.loseConditions);
				objectiveLabelPortrait.text = objectiveLabelLandscape.text;
				
				/*
				foreach (LevelDestroyTargets target in destroyTargets) {
					target.UpdateValues(levelData.winConditions);
				}

				Vector3 newPos = targetLabelPortrait.transform.localPosition;
				newPos.y = 112f;
				if (levelData.winConditions.GetType() == typeof(WinScore)) {
					newPos.y = 112f;
				}
				else {
					newPos.y = 86f;
				}
					
				targetLabelPortrait.transform.localPosition = newPos;
				targetLabelLandscape.transform.localPosition = targetLabelPortrait.transform.localPosition;
				*/
			}
		}
	}
}

