using UnityEngine;
using System.Collections;

public class Hourglass : BasicItem
{
	public int extraTime = 15;
	public int soundPlays = 3;

	public override string ItemName {
		get {
			return "Hourglass";
		}
	}

	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		(boardLogic.loseConditions as LoseTimer).pendingTimeUpdate = true;
		
		StartItemEffects();
		
		StartCoroutine(PlaySound());
	}
	
	IEnumerator PlaySound()
	{
		SoundEffectController soundEffect = SoundManager.Instance["hourglass_sfx"];
		WaitForSeconds waitTime = new WaitForSeconds(soundEffect.SoundClip.length / soundEffect.sound.pitch);
		
		for (int i = 0; i < soundPlays; ++i) {
			soundEffect.Play();
			yield return waitTime;
		}
	}
	
	protected override void DoItem()
	{
		if ((boardLogic.loseConditions as LoseTimer) != null) {
			(boardLogic.loseConditions as LoseTimer).RemainingTime += extraTime;
			(boardLogic.loseConditions as LoseTimer).pendingTimeUpdate = false;
		}
		
		base.DoItem();
	}	
}

