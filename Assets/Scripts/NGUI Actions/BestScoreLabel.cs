using UnityEngine;
using System.Collections;

public class BestScoreLabel : MonoBehaviour
{
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		UpdateBestScore();
	}
	
	public void UpdateBestScore()
	{
		label.text = Language.Get("BEST").Replace("<SPACE>"," ") + ":\n" + ScoreSystem.FormatScore(WinScore.bestScore);
	}
}

