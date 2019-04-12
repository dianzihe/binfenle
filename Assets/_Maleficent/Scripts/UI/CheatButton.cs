using UnityEngine;
using System.Collections;

public class CheatButton : UIControl {

	void Awake()
	{
		transform.gameObject.SetActive(MaleficentTools.IsDebugBuild);
	}

	void OnClick()
	{
		for(int i=1;i<=LoadLevelButton.maxLevels;i++)
		{
			UserManagerCloud.Instance.SetScoreForLevel(i, 5000, 3);
		}
		PlayerPrefs.SetInt("GameOver",1);
		ChaptersManager.Instance.RefreshChaptersInfo();
	}
}
