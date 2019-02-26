using UnityEngine;
using System.Collections;

public class InGameSoundEventsHandler 
{
	public string[] matchComboSndIds = {
		"combo1_sfx",
		"combo2_sfx",
		"combo3_sfx",
		"combo4_sfx",
		"combo5_sfx",
		"combo6_sfx",
		"combo7_sfx",
		"combo8_sfx",
		"combo9_sfx",
		"combo10_sfx",
	};
	
	public string[] achieveStarsIds = {
		"star1_sfx",
		"star2_sfx",
		"star3_sfx",
	};
	
	protected SoundEffectController[] sndMatchCombos;
	protected SoundEffectController[] sndAchieveStars;
		
	protected SoundEffectController sndTileSwap;
	protected SoundEffectController sndTileSwapBad;
	protected SoundEffectController sndTileDropImpact;
	protected SoundEffectController sndTileMatch;
	/// <summary>
	/// The tile destroy sound is the same as the "sndTileMatch" sound except it allows multiple queued plays with up to 3 audio sources.
	/// It's required for example by the ColorBomb+ColorBomb combine effect.
	/// </summary>
	protected SoundEffectController sndTileDestroy;
	
	protected SoundEffectController sndDirectionalCreate;
	protected SoundEffectController sndDirectionalTileActivate;
	protected SoundEffectController sndDirectionalDirectionalCombine;
	protected SoundEffectController sndDirectionalBombCombine;

	protected SoundEffectController sndBombCreate;
	protected SoundEffectController sndTriggerBombTileFreeze;
	protected SoundEffectController sndTriggerBombTileDestroy;
	protected SoundEffectController sndBombBombCombine;
	
	protected SoundEffectController sndColorBombCreate;
	protected SoundEffectController sndTriggerColorBomb;
	protected SoundEffectController sndColorBombDirectionalCombine;
	protected SoundEffectController sndBombColorBombCombine;
	protected SoundEffectController sndColorBombColorBombCombine;
	
	protected SoundEffectController sndSnowTileLayerDestroy;
	
	protected SoundEffectController sndFrostPieceLayerDestroy;
	
	protected SoundEffectController sndLockedTileDestroy;
	
	protected SoundEffectController sndDropTileExit;
		
	protected SoundEffectController sndFreezerTileCreated;
	
	protected SoundEffectController sndWolfTileIdle;
	
	protected SoundEffectController sndWolfTileDestroy;
	
	protected SoundEffectController sndChocolateFlowing;

	protected SoundEffectController sndAddMagic;
	
	
	public InGameSoundEventsHandler() 
	{
		// Cache sounds references
		sndMatchCombos = new SoundEffectController[matchComboSndIds.Length];	
		for(int i = 0; i < matchComboSndIds.Length; i++) {
 			sndMatchCombos[i] = SoundManager.Instance[matchComboSndIds[i]];
		}
		
		sndAchieveStars = new SoundEffectController[achieveStarsIds.Length];
		for(int i = 0; i < achieveStarsIds.Length; i++) {
			sndAchieveStars[i] = SoundManager.Instance[achieveStarsIds[i]];
		}

		sndTileSwap = SoundManager.Instance["swap_sfx"];
		sndTileSwapBad = SoundManager.Instance["swap_bad_sfx"];
		sndTileDropImpact = SoundManager.Instance["fall_impact_sfx_01"];
		sndTileMatch = SoundManager.Instance["match_sfx"];
		sndTileDestroy = SoundManager.Instance["tile_destroy_sfx"];
			
		sndDirectionalTileActivate = SoundManager.Instance["winterchill_sfx"];
		sndDirectionalCreate = SoundManager.Instance["winter_create_sfx"];
		sndDirectionalDirectionalCombine = SoundManager.Instance["winterchill_winterchill_sfx"];
		sndDirectionalBombCombine = SoundManager.Instance["winterchill_iceberg_sfx"];
		
		sndBombCreate = SoundManager.Instance["iceberg_create_sfx"];
		sndTriggerBombTileFreeze = SoundManager.Instance["iceberg_freeze_sfx"];
		sndTriggerBombTileDestroy = SoundManager.Instance["iceberg_destroy_sfx"];
		sndBombBombCombine = SoundManager.Instance["iceberg_iceberg_sfx"];
		
		sndColorBombCreate = SoundManager.Instance["glacier_create_sfx"];
		sndTriggerColorBomb = SoundManager.Instance["glacier_sfx"];
		sndColorBombDirectionalCombine = SoundManager.Instance["winterchill_glacier_sfx"];
		sndBombColorBombCombine = SoundManager.Instance["glacier_iceberg_sfx"];
		sndColorBombColorBombCombine = SoundManager.Instance["glacier_glacier_sfx"];
		
		sndSnowTileLayerDestroy = SoundManager.Instance["snow_sfx"];
		
		sndFrostPieceLayerDestroy = SoundManager.Instance["frost_sfx"];
		
		sndLockedTileDestroy = SoundManager.Instance["locked_sfx"];
		
		sndDropTileExit = SoundManager.Instance["drop_sfx"];
		
		sndFreezerTileCreated = SoundManager.Instance["freezer_create"];

		sndWolfTileIdle = SoundManager.Instance["wolf_growl"];
		sndWolfTileDestroy = SoundManager.Instance["wolf_yelping"];
		
		sndChocolateFlowing = SoundManager.Instance["chocolate_flowing"];

		sndAddMagic = SoundManager.Instance["mana_earn_sfx"];
	}

	public void RegisterSoundEvents()
	{
		Match3BoardGameLogic.OnDestroyLastFoundMatches += OnMatchCombosSndEvent;
		Match3BoardGameLogic.OnUserStartedTilesSwitch += OnTilesStartSwitchSndEvent;
		Match3BoardGameLogic.OnUserTilesSwitchBad += OnTilesSwitchBadSndEvent;
		
		NormalTile.OnStartTileImpactBounce += OnStartTileImpactBounceSndEvent;
		NormalTile.OnStartNormalTileDestroy += OnStartNormalTileDestroySndEvent;
		
		NormalTile.OnBombTileCreatedFromMatch += OnBombTileCreatedSndEvent;
		BombTile.OnTriggerBombTileFreeze += OnTriggerBombTileFreezeSndEvent;
		BombTile.OnTriggerBombTileDestroy += OnTriggerBombTileDestroySndEvent;
		BombTile.OnBombBombCombine += OnBombBombCombineSndEvent;
		
		NormalTile.OnColorBombTileCreatedFromMatch += OnColorBombTileCreatedSndEvent;
		ColorBombTile.OnTriggerColorBombTile += OnTriggerColorBombSndEvent;
		ColorBombTile.OnColorBombDirectionalCombine += OnColorBombDirectionalCombineSndEvent;
		ColorBombTile.OnBombColorBombCombine += OnBombColorBombCombineSndEvent;
		ColorBombTile.OnColorBombColorBombCombine += OnColorBombColorBombCombineSndEvent;
		
		NormalTile.OnDirectionalTileCreatedFromMatch += OnDirectionalTileCreatedSndEvent;
		DirectionalDestroyTile.OnTriggerDirectionalDestroyTile += OnStartDirectionalTileSndEvent;
		DirectionalDestroyTile.OnDirectionalDirectionalCombine += OnDirectionalDirectionalCombineSndEvent;
		DirectionalDestroyTile.OnDirectionalBombCombine += OnDirectionalBombCombineSndEvent;
		
		WinScore.OnNewStarReached += OnNewStarSndEvent;
		
		SnowTile.OnSnowTileLayerBeginDestroy += OnSnowTileLayerDestroyedSndEvent;
		
		LayeredBoardPiece.OnNumLayersDecreased += OnFrostedPieceLayerDestroyedSndEvent;
		
		LockedTile.OnLockedTileDestroyed += OnLockedTileDestroyedSndEvent;
		
		DropTile.OnDropTileDropped += OnDropTileExited;
		
		FreezerTile.OnFreezerTileCreated += OnFreezerTileCreated;
		FreezerTile.OnFreezerTileDestroyed += OnFreezerTileDestroyed;
		
		WolfTile.OnWolfTileBeginDestroy += OnWolfTileBeginDestroy;
		WolfTile.OnWolfTileBeginIdle += OnWolfTileBeginIdle;
		
		HotChocolate.OnChocolateBeginFlowing += OnChocolateBeginFlowing;
		HotChocolate.OnChocolateStopFlowing += OnChocolateStopFlowing;
	}
	
	public void UnregisterSoundEvents()
	{
		Match3BoardGameLogic.OnDestroyLastFoundMatches -= OnMatchCombosSndEvent;
		Match3BoardGameLogic.OnUserStartedTilesSwitch -= OnTilesStartSwitchSndEvent;
		Match3BoardGameLogic.OnUserTilesSwitchBad -= OnTilesSwitchBadSndEvent;
		
		NormalTile.OnStartTileImpactBounce -= OnStartTileImpactBounceSndEvent;
		NormalTile.OnStartNormalTileDestroy -= OnStartNormalTileDestroySndEvent;
		
		NormalTile.OnBombTileCreatedFromMatch -= OnBombTileCreatedSndEvent;
		BombTile.OnTriggerBombTileFreeze -= OnTriggerBombTileFreezeSndEvent;
		BombTile.OnTriggerBombTileDestroy -= OnTriggerBombTileDestroySndEvent;
		BombTile.OnBombBombCombine -= OnBombBombCombineSndEvent;
		
		NormalTile.OnColorBombTileCreatedFromMatch -= OnColorBombTileCreatedSndEvent;
		ColorBombTile.OnTriggerColorBombTile -= OnTriggerColorBombSndEvent;
		ColorBombTile.OnColorBombDirectionalCombine -= OnColorBombDirectionalCombineSndEvent;
		ColorBombTile.OnBombColorBombCombine -= OnBombColorBombCombineSndEvent;
		ColorBombTile.OnColorBombColorBombCombine -= OnColorBombColorBombCombineSndEvent;
		
		NormalTile.OnDirectionalTileCreatedFromMatch -= OnDirectionalTileCreatedSndEvent;
		DirectionalDestroyTile.OnTriggerDirectionalDestroyTile -= OnStartDirectionalTileSndEvent;
		DirectionalDestroyTile.OnDirectionalDirectionalCombine -= OnDirectionalDirectionalCombineSndEvent;
		DirectionalDestroyTile.OnDirectionalBombCombine -= OnDirectionalBombCombineSndEvent;
		
		WinScore.OnNewStarReached -= OnNewStarSndEvent;
		
		SnowTile.OnSnowTileLayerBeginDestroy -= OnSnowTileLayerDestroyedSndEvent;
		
		LayeredBoardPiece.OnNumLayersDecreased -= OnFrostedPieceLayerDestroyedSndEvent;

		LockedTile.OnLockedTileDestroyed -= OnLockedTileDestroyedSndEvent;
		
		DropTile.OnDropTileDropped -= OnDropTileExited;
		
		FreezerTile.OnFreezerTileCreated -= OnFreezerTileCreated;
		FreezerTile.OnFreezerTileDestroyed -= OnFreezerTileDestroyed;
		
		WolfTile.OnWolfTileBeginDestroy -= OnWolfTileBeginDestroy;
		WolfTile.OnWolfTileBeginIdle -= OnWolfTileBeginIdle;
		
		HotChocolate.OnChocolateBeginFlowing -= OnChocolateBeginFlowing;
		HotChocolate.OnChocolateStopFlowing -= OnChocolateStopFlowing;
	}
	
	public void OnMatchCombosSndEvent()
	{
		SoundManager.Instance.PlayOneShot(sndMatchCombos[ScoreSystem.Instance.Multiplier - 1]);
	}
	
	public void OnTilesStartSwitchSndEvent(Match3Tile tileA, Match3Tile tileB)
	{
		SoundManager.Instance.PlayOneShot(sndTileSwap);
	}
	
	public void OnTilesSwitchBadSndEvent(Match3Tile tileA, Match3Tile tileB)
	{
		SoundManager.Instance.PlayOneShot(sndTileSwapBad);
	}
	
	public void OnStartDirectionalTileSndEvent(DirectionalDestroyTile directionalDestroyTile)
	{
		sndDirectionalTileActivate.PlayQueued();
	}
	
	public void OnStartTileImpactBounceSndEvent(NormalTile tile)
	{
		// Plays from 3 random audio clips setup on the sound effect
		sndTileDropImpact.PlayQueued();
	}
	
	public void OnStartNormalTileDestroySndEvent(NormalTile tile, bool isSingleDestroyed)
	{
		if ( !isSingleDestroyed ) {
			sndTileMatch.PlayQueued();
		}
		else {
			// Use another sound effect controller with different properties when multiple tiles will be destroyed one by one
			// to ensure that the next destroy sound won't stop the previous destroy sound (given the impression of a "machine-gun" sound play sequence).
			sndTileDestroy.PlayQueued();
		}
	}
	
	public void OnBombTileCreatedSndEvent(BombTile bombTile)
	{
//		sndBombCreate.Play();
		SoundManager.Instance.PlayOneShot(sndBombCreate);
	}
	
	public void OnColorBombTileCreatedSndEvent(ColorBombTile colorBombTile)
	{
//		sndColorBombCreate.Play();
		SoundManager.Instance.PlayOneShot(sndColorBombCreate);
	}
	
	public void OnDirectionalTileCreatedSndEvent(DirectionalDestroyTile directionalTile)
	{
//		sndDirectionalCreate.Play();
		SoundManager.Instance.PlayOneShot(sndDirectionalCreate);
	}
	
	public void OnDirectionalDirectionalCombineSndEvent(DirectionalDestroyTile directionalTile)
	{
//		sndDirectionalDirectionalCombine.Play();
		SoundManager.Instance.PlayOneShot(sndDirectionalDirectionalCombine);
	}
	
	public void OnTriggerBombTileFreezeSndEvent(BombTile bombTile)
	{
		sndTriggerBombTileFreeze.Play();
	}

	public void OnTriggerBombTileDestroySndEvent(BombTile bombTile)
	{
		sndTriggerBombTileDestroy.Play();
	}

	public void OnTriggerColorBombSndEvent(ColorBombTile colorBombTile)
	{
		sndTriggerColorBomb.Play();
	}
	
	public void OnDirectionalBombCombineSndEvent(DirectionalDestroyTile directionalTile) 
	{
//		sndDirectionalBombCombine.Play();
		SoundManager.Instance.PlayOneShot(sndDirectionalBombCombine);
	}
	
	public void OnColorBombDirectionalCombineSndEvent(ColorBombTile colorBombTile) 
	{
//		sndColorBombDirectionalCombine.Play();
		SoundManager.Instance.PlayOneShot(sndColorBombDirectionalCombine);
	}
	
	public void OnBombBombCombineSndEvent(BombTile bombTile)
	{
		sndBombBombCombine.Play();
//		SoundManager.Instance.PlayOneShot(sndBombBombCombine);
	}
	
	public void OnBombColorBombCombineSndEvent(ColorBombTile colorBombTile) 
	{
//		sndBombColorBombCombine.Play();
		SoundManager.Instance.PlayOneShot(sndBombColorBombCombine);
	}
	
	public void OnColorBombColorBombCombineSndEvent(ColorBombTile colorBombTile)
	{
//		sndColorBombColorBombCombine.Play();
		SoundManager.Instance.PlayOneShot(sndColorBombColorBombCombine);
	}
	
	public void OnNewStarSndEvent(int starCount)
	{
		SoundManager.Instance.PlayOneShot(sndAchieveStars[starCount - 1]);
	}
	
	public void OnSnowTileLayerDestroyedSndEvent(SnowTile snowTile)
	{
		sndSnowTileLayerDestroy.PlayQueued();
	}
	
	public void OnFrostedPieceLayerDestroyedSndEvent(LayeredBoardPiece boardPiece)
	{
		sndFrostPieceLayerDestroy.PlayQueued();
	}
	
	public void OnLockedTileDestroyedSndEvent(LockedTile lockedTile)
	{
		sndLockedTileDestroy.PlayQueued();
	}
	
	public void OnDropTileExited(DropTile dropTile)
	{
		sndDropTileExit.PlayQueued();
	}
	
	public void OnFreezerTileCreated(FreezerTile freezerTile)
	{
		SoundManager.Instance.PlayOneShot(sndFreezerTileCreated);
	}
	
	public void OnFreezerTileDestroyed(FreezerTile freezerTile)
	{
		sndSnowTileLayerDestroy.PlayQueued();
	}
	
	public void OnWolfTileBeginDestroy(WolfTile wolfTile)
	{
		sndWolfTileDestroy.Play();
	}
	
	public void OnWolfTileBeginIdle(WolfTile wolfTile)
	{
		sndWolfTileIdle.Play();
	}
	
	public void OnChocolateBeginFlowing(HotChocolate sender)
	{
		sndChocolateFlowing.Play();
	}
	
	public void OnChocolateStopFlowing(HotChocolate sender)
	{
		sndChocolateFlowing.Stop();
	}


}
