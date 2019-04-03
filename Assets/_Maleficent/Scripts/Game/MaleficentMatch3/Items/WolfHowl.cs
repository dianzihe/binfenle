using UnityEngine;
using System.Collections;

public class WolfHowl : ManaItem {

	private GameObject wolf;

	public override string ItemName
	{
		get {
			return "WolfHowl";
		}
	}
	
	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		TileSwitchInput.Instance.DisableInput();
		StartItemEffects();
		
		AvatarParticles.Instance.ActivateEffect(AvatarParticlesEffectType.YellowPixieDust);
		
		//Instantiate wolf
		wolf = GameObject.Instantiate(Resources.Load("Characters/Avatar_Diaval_Wolf")) as GameObject;
		WolfConfig wolfConfig = wolf.GetComponent< WolfConfig >();
		wolfConfig.cameraLandscapeWolf.gameObject.SetActive(false);
		wolfConfig.cameraPortrait.gameObject.SetActive(false);
		
		wolfConfig.cameraLandscapeWolf.transform.parent = CharacterConfig.Instance.cameraLandscapeManager.transform;
		wolfConfig.cameraLandscapeWolf.transform.localPosition = Vector3.zero;
		wolfConfig.cameraLandscapeWolf.transform.localRotation = Quaternion.identity;
		
		wolfConfig.wolf.gameObject.SetActive(false);
		MaleficentTools.DoAfterSeconds(this, wolfConfig.timeStartPlatingAnim, () => {
			wolfConfig.wolf.gameObject.SetActive(true);
			
			wolfConfig.wolf.GetComponent<Animation>().Play("Take 001");
		});
		
		MaleficentTools.DoAfterSeconds(this, wolfConfig.timeStartPlatingAnim + 1.0f, () => {
			//CharacterConfig.Instance.cameraPortraitManager.MoveToTarget(wolfConfig.cameraPortrait, 0.5f);
			CharacterConfig.Instance.cameraLandscapeManager.MoveToTarget(wolfConfig.cameraLandscape, 0.5f); //Move camera to wolf
		});
		
		MaleficentTools.DoAfterSeconds(this, wolfConfig.timePlayEnteringParticles, () => {
			SoundManager.Instance.Play("Magic_reveal_sfx");	
		});
		MaleficentTools.DoAfterSeconds(this, wolfConfig.timePlayEnteringParticles, () => {
			wolfConfig.enteringParticles.Play();
		});
		
		MaleficentTools.DoAfterSeconds(this, wolfConfig.timeStartHowlSound, () => {
			SoundManager.Instance.Play("DIAVAL Wolf Vocal 3");	
		});
		
		//Particles leaving
		MaleficentTools.DoAfterSeconds(this, wolfConfig.timePlayLeavingParticles - 0.4f, () => {	
			SoundManager.Instance.Play("Magic_reveal_sfx");	
		});
		MaleficentTools.DoAfterSeconds(this, wolfConfig.timePlayLeavingParticles, () => {	
			wolfConfig.leavingParticles.Play();
		});
		
		//Howl
		MaleficentTools.DoAfterSeconds(this, wolfConfig.timeMoveCameraBack, () => {
			wolfConfig.cameraLandscapeWolf.transform.parent = wolfConfig.transform;
		
			//Camera back to maleficent
			CharacterConfig.Instance.cameraLandscapeManager.MoveToTarget(CharacterConfig.Instance.cameraLandscape, 0.5f);
			CharacterConfig.Instance.cameraPortraitManager.MoveToTarget(CharacterConfig.Instance.cameraPortrait, 0.5f);
		});
	
		base.StartUsingItem(_boardLogic);
	}
	
	protected override void DoItem()
	{
		BoardShuffleController.Instance.RaiseBoardShuffleRequiredEvent(false);
		
		TileSwitchInput.Instance.EnableInput();
		base.DoItem();
	}
	
	protected override void DoDestroy(){
	}
	
	protected override void FinishUsingItem()
	{
		AvatarParticles.Instance.ActivateEffect(AvatarParticlesEffectType.YellowPixieDust);
		
		IngameCrow ingameCrow = GameObject.FindObjectOfType< IngameCrow >();
		if(ingameCrow != null) {
			MaleficentTools.DoAfterSeconds(ingameCrow, 4.0f, () => {
				//Wail until Diaval gets back
				ingameCrow.GetBack(BaseFinishUsingItem);
				Destroy(wolf);
				Destroy(gameObject);
			});
		} else {
			BaseFinishUsingItem();
		}
	}
	
	private void BaseFinishUsingItem() {
		base.FinishUsingItem();
	}
}
