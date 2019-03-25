using UnityEngine;
using System.Collections;

public class LoseFree : AbstractLoseCondition
{
	public override bool Check()
	{
		//the game is never lost with a Free lose condition.
		return false;
	}
	
	public override string GetString()
	{
		return "-";
	}
	
	public override string GetStringValue()
	{
		return "";
	}
	
//	public override void DoWin()
//	{
//		CoinsSystem.Instance.AddCoins(Mathf.FloorToInt(ScoreSystem.Instance.Score * TweaksSystem.Instance.floatValues["ScoreSilverMultiplier"]), 0);
//		
//		base.DoWin();
//	}
}
