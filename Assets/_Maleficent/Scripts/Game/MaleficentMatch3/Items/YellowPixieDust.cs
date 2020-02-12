using UnityEngine;
using System.Collections;

public class YellowPixieDust : ManaItem {

	public int extraMoves = 5;

	public override string ItemName {
		get {
			return "YellowPixieDust";
		}
	}

	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		
		StartItemEffects();
		AvatarParticles.Instance.ActivateEffect(AvatarParticlesEffectType.YellowPixieDust);
		
		SoundManager.Instance.Play("powerup_yellowpixiedust_sfx");

		TileSwitchInput.Instance.DisableInput();
	}
	
	protected override void DoItem()
	{
		if ((boardLogic.loseConditions as LoseMoves) != null) {
			(boardLogic.loseConditions as LoseMoves).RemainingMoves += extraMoves;
		}

		TileSwitchInput.Instance.EnableInput();
		base.DoItem();
	}
}
