using UnityEngine;
using System.Collections;

public class EndLabel : MonoBehaviour
{
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
	}
	
	public void UpdateText(bool win) 
	{
		if (win) {
			WinScore winCondition = Match3BoardGameLogic.Instance.winConditions as WinScore;
			if (winCondition != null && ScoreSystem.Instance.Score >= winCondition.targetScore3Stars) {
				label.text = Language.Get("END_SHORT_WIN_GREAT");
			}
			else if (winCondition != null && ScoreSystem.Instance.Score >= winCondition.targetScore2Stars) {
				label.text = Language.Get("END_SHORT_WIN_GOOD");
			}
			else {
				label.text = Language.Get("END_SHORT_WIN");
			}
		}
		else {
			label.text = Language.Get("END_SHORT_LOSE");
		}
	}
	
	public void UpdateTextFreeFall() 
	{
		label.text = Language.Get("WIN_SHORT_FREE_FALL");
	}
}

