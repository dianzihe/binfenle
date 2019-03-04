using UnityEngine;
using System.Collections;

public class NextLevel : MonoBehaviour 
{
	public PlayMakerFSM fsm;
	public string sendEvent = "NextLevel";
	public GameObject replayButton;
	
	public void UpdateButton()
	{
		if (MaleficentBlackboard.Instance.level < LoadLevelButton.lastUnlockedLevel || 
			(MaleficentBlackboard.Instance.level == LoadLevelButton.maxLevels && 
			UserManagerCloud.Instance.GetStarsForLevel(MaleficentBlackboard.Instance.level) > 0)) 
		{
			gameObject.SetActive(false);
			replayButton.SetActive(true);
		}

		if(MaleficentBlackboard.Instance.level == LoadLevelButton.maxLevels && 
		   UserManagerCloud.Instance.GetStarsForLevel(MaleficentBlackboard.Instance.level) > 0 &&
		   PlayerPrefs.GetInt("GameOver",0) == 0)
		{
			LoadLevelButton.showNextLevel = true;
			PlayerPrefs.SetInt("GameOver",1);
		}
	}
	
	void OnClick()
	{
		LoadLevelButton.showNextLevel = true;
		fsm.SendEvent(sendEvent);
	}
}
