using UnityEngine;
using System.Collections;

public class TargetScoreLabel : MonoBehaviour
{
	public Match3BoardGameLogic gameLogic;
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		label.text = Language.Get("LEVEL_TARGET") + " " + ScoreSystem.FormatScore((gameLogic.winConditions as WinScore).targetScore);
		//label.text = "Target: " + ScoreSystem.FormatScore((gameLogic.winConditions as WinScore).targetScore);
	}
}

