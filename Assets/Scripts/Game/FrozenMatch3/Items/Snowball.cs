using UnityEngine;
using System.Collections;

public class Snowball : BasicItem
{
	public int extraMoves = 5;
	
	public override string ItemName {
		get {
			return "Snowball";
		}
	}

	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		
		StartItemEffects();
		
		SoundManager.Instance.PlayOneShot("snowball_sfx");
	}
	
	protected override void DoItem()
	{
		if ((boardLogic.loseConditions as LoseMoves) != null) {
			(boardLogic.loseConditions as LoseMoves).RemainingMoves += extraMoves;
		}
		
		base.DoItem();
	}	
}

