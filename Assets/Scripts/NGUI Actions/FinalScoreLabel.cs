using UnityEngine;
using System.Collections;

public class FinalScoreLabel : MonoBehaviour
{
	public Match3BoardGameLogic gameLogic;
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		UpdateFinalScore();
	}
	
	void UpdateFinalScore () 
	{
		label.text = Language.Get("GAME_SCORE").Replace("<SPACE>"," ") + ": " + ScoreSystem.FormatScore(ScoreSystem.Instance.Score) + 
			" / " + ScoreSystem.FormatScore((gameLogic.winConditions as WinScore).targetScore);
	}
}

