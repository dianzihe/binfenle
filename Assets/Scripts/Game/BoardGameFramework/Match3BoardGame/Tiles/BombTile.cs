using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombTile : TriggerTile
{
	public static event System.Action<BombTile> OnTriggerBombTileFreeze;
	public static event System.Action<BombTile> OnTriggerBombTileDestroy;
	public static event System.Action<BombTile> OnBombBombCombine;
	
	public GameObject prefabDoubleDestroyEffect;
			
	public Material[] coloredMaterials;
	public bool spawnTimeBomb = true;
	public bool useBigTimeBomb = false;
	
	protected List<NormalTile> tilesToDestroy = new List<NormalTile>(9);
	protected List<Match3BoardPiece> layeredBoardPiecesToDestroy = new List<Match3BoardPiece>(9);
	
	protected Match3Tile spawnedTimeBomb;
	
	public Renderer tileModelAdditionalRenderer;
	public Animation tileEffectAnimation;
	
	public GameObject PrefabEffectBreakFragments;
	public GameObject[] FreezeEffectPrefabArray;
		
	protected TileDestroyEffect effectBreakFragmentsInstance;
	
	//Bool result for CheckBomb CheckDirectional CheckColorBomb
	private bool spawnResult;
	
	public Material[] PrefabFreezeDestroyMaterialArray;


	//MALEFICENT NEW
	public Material[] backLightMaterials;
	public Material[] breakEffectMaterials;
//	public Material[] frontLightMaterials;
	
	public Renderer backLightRenderer;
	public Renderer breakEffectRenderer;
//	public Renderer frontLightRenderer;
	// END MALEFICENT NEW
	
	
	public override void InitComponent ()
	{
		base.InitComponent ();
		tilesToDestroy.Clear();
		layeredBoardPiecesToDestroy.Clear();
	}
	
	public virtual void UpdateMaterial()
	{
		tileModelRenderer.material = coloredMaterials[(int)TileColor - 1];
		tileModelAdditionalRenderer.material = coloredMaterials[(int)TileColor - 1 + coloredMaterials.Length / 2];
		prefabFreezeEffect = FreezeEffectPrefabArray[(int)TileColor - 1];

		//MALEFICENT NEW
		backLightRenderer.material = backLightMaterials[(int)TileColor - 1];
		breakEffectRenderer.material = breakEffectMaterials[(int)TileColor - 1];
//		frontLightRenderer.material = frontLightMaterials[(int)TileColor - 1];
		//END MALEFICENT NEW
	}
	
	protected override void TileDestroy(bool useEffect) 
	{	
		spawnResult = CheckForSpawnPatterns();
		
		cachedTransform.localPosition += Vector3.forward * -2f;

		if (spawnTimeBomb && !spawnResult) {
			spawnedTimeBomb = SpawnTimeBomb();
		}
		
		if (useDestroyEffect && useEffect) 
		{
			// Check if this bomb is a bomb resulted from a Bomb+Bomb combination
			if (prefabDestroyEffect == prefabDoubleDestroyEffect)
			{
				if (OnBombBombCombine != null) {
					OnBombBombCombine(this);
				}
			}
				
			TileBombDestroyEffect bombDestroyEffect = SpawnDestroyEffect(new Vector3(0f, 0f, -19f)).gameObject.GetComponent<TileBombDestroyEffect>();	
			bombDestroyEffect.explosionSpreadTime = GetComponent<Animation>().clip.length - bombDestroyEffect.delayAfterExplosionSpread;
			bombDestroyEffect.InitBombDestroyEffect(this, WorldPosition);
			bombDestroyEffect.triggerListener.OnTileEntered = OnTileEnteredDestroyTrigger;
			bombDestroyEffect.triggerListener.OnBoardPieceEntered = OnBoardPieceEnteredFreezeTrigger;
			bombDestroyEffect.OnEffectFinished = OnBombDestroyEffectFinished;
			
			//TODO:Remove redundant chunks of code if u will ever have the time / decency to do so
			//TODO: MIO: Revise this .. it was way too late when you wrote this.
			Transform effectInstance = (Instantiate(PrefabEffectBreakFragments) as GameObject).transform;
			effectInstance.position = WorldPosition;
			Destroy(effectInstance.gameObject, destroyEffect.lifeTime);
			effectBreakFragmentsInstance = effectInstance.GetComponent<TileDestroyEffect>();
			
			if (freezeEffect) {
				Destroy(freezeEffect);
				effectBreakFragmentsInstance.GetComponent<TileDestroyEffect>().UpdateMaterial(PrefabFreezeDestroyMaterialArray[(int)TileColor - 1]);
			}
			else {
				effectBreakFragmentsInstance.GetComponent<TileDestroyEffect>().UpdateMaterial(TileColor);
			}
			
			tileModelRenderer.enabled = false;
			//--------------------------------------------------------------------------------------
			
			GetComponent<Animation>().Play();
			
			if(tileModelAdditionalRenderer != null) {
				tileModelAdditionalRenderer.transform.parent.GetComponent<Animation>().Play("tile_iceberg_destroy");
			}
			
			if (OnTriggerBombTileFreeze != null) {
				OnTriggerBombTileFreeze(this);
			}
		}
		else {
			base.TileDestroy(false);
		}
	}
	
	/// <summary>
	/// Event raised by the <see cref="WinterchillEffect"/> spawned in the "SpawnEffect(float angle)" method when the freeze trigger
	/// collider is disabled.
	/// </summary>
	/// <param name='sender'>
	/// Sender.
	/// </param>
	void OnBombDestroyEffectFinished(DestroyEffect sender) {
		if (OnTriggerBombTileDestroy != null) {
			OnTriggerBombTileDestroy(this);
		}
			
		sender.OnEffectFinished = null;
		// Disable the bomb destroy effect trigger.
		(sender as TileBombDestroyEffect).destroyTrigger.gameObject.SetActive(false);

		LayeredBoardPiece layeredBoardPiece;
		for(int i = 0; i < layeredBoardPiecesToDestroy.Count; i++) {
			layeredBoardPiece = layeredBoardPiecesToDestroy[i] as LayeredBoardPiece;
			
			if(layeredBoardPiece != null && layeredBoardPiece.Tile == null)
			{
				layeredBoardPiece.NumLayers--;
			}
		}
		
//		StartCoroutine(DestroyTilesAfterDelay(0.1f));
		// Destroy the detected tiles
		NormalTile tile;
		for(int i = 0; i < tilesToDestroy.Count; i++) {
			tile = tilesToDestroy[i];
			if (tile != null && !tile.IsDestroying && tile.IsDestructible) {
//				tile.ApplyPunchEffectToNeighbors(-1, 1, 1);
				tile.Destroy();
			}
		}
		
//		// Destroy this tile without triggering any visual effect.
		base.TileDestroy(false);
	}

	/// <summary>
	/// Event raised by the <see cref="TileBombDestroyEffect"/> when a tile enters the destroy trigger of the effect.
	/// </summary>
	/// <param name='tile'>
	/// Tile.
	/// </param>
	void OnTileEnteredDestroyTrigger(NormalTile tile) {
		// Make sure we don't add to the destroy list this tile or the time bomb tile it may have spawned.
		if ( tile != this && tile.IsDestructible && !(tile is TimeBombTile) && tile.Freeze()) {
			//TODO: Make the tile not match any other tiles because it will be destroyed by this effect anyway???
			tilesToDestroy.Add(tile);
		}
	}
	
	void OnBoardPieceEnteredFreezeTrigger(Match3BoardPiece boardPiece)
	{
		if(boardPiece is LayeredBoardPiece)
		{
			layeredBoardPiecesToDestroy.Add(boardPiece);
		}
	}
	
	public override void RaiseEventTileSwitchAnimBegan (Match3Tile neighborTile) {
		base.RaiseEventTileSwitchAnimBegan (neighborTile);
		
		if ((neighborTile is BombTile || neighborTile is ColorBombTile) && TappedFirst)
		{
			Match3BoardGameLogic.Instance.loseConditions.NewMove();
		}
		
		if (neighborTile is BombTile)
		{
			// Don't allow this tile and it's neighbour with who it was switched to switch back on match fail. 
			BombTile neighborBomb = neighborTile as BombTile;
			neighborBomb.SwitchBackOnMatchFail = false;
			neighborBomb.DisableTileLogic();
			// Replace the default destroy effect with a bigger destroy effect
			neighborBomb.prefabDestroyEffect = neighborBomb.prefabDoubleDestroyEffect;
			neighborBomb.useBigTimeBomb = true;
		}
	}
	
	public override void RaiseEventTileSwitchAnimEnded (AbstractTile neighbourTile) {
		base.RaiseEventTileSwitchAnimEnded (neighbourTile);
		
		if (neighbourTile is BombTile) {
			// Destroy and trigger neighbor tile.
			neighbourTile.Destroy();
		}
	}
	
	Match3Tile SpawnTimeBomb() {
		BombTile bombTile = (BoardRenderer as Match3BoardRenderer).SpawnSpecificTileAt(BoardPiece.BoardPosition, 
																					   typeof(TimeBombTile), TileColorType.None) as BombTile;

		if (useBigTimeBomb) {
			bombTile.prefabDestroyEffect = bombTile.prefabDoubleDestroyEffect;
		}

		bombTile.TileColor = TileColor;
		bombTile.UpdateMaterial();
		BoardPiece.Tile = bombTile;
		
		return bombTile;
	}

	#region implemented abstract members of TriggerTile
	public override void TriggerTileAbility ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}