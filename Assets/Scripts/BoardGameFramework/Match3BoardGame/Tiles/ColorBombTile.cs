using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class ColorBombTile : TriggerTile
{
	public static event System.Action<ColorBombTile> OnTriggerColorBombTile;
	public static event System.Action<ColorBombTile> OnColorBombDirectionalCombine;
	public static event System.Action<ColorBombTile> OnBombColorBombCombine;
	public static event System.Action<ColorBombTile> OnColorBombColorBombCombine;
		
	private NormalTile tileIterator;
	
	public TileColorType destroyColor;
	public GameObject prefabTilesDestroyEffect;
	DestroyEffect tilesDestroyEffect;
	
	//Effects related to ColorBomb + Directional
	public GameObject iceTrailEffect;
	public GameObject switchToDirectionalEffect;
	
	//Wait times required for ColorBomb + Directional
	public float iceTrailTravelTime = 0.5f;
	public float postConversionWaitTime = 0.5f;
	
	protected List<Match3Tile> tilesToDestroy = new List<Match3Tile>(10);
	
	[System.NonSerialized]
	public bool wasFirstTapped = false;
	
	/// <summary>
	/// The delay before this tile will destroy itself. This time depends on what effect this tile will produce (especiall when it's combined with other special tiles).
	/// </summary>
	[System.NonSerialized]
	public float delayBeforeTileDestroy;
		
	protected Match3BoardRenderer match3BoardRenderer;
	
	
	public override void InitComponent ()
	{
		tilesToDestroy.Clear();
		base.InitComponent ();
		
		wasFirstTapped = false;
		
		match3BoardRenderer = BoardRenderer as Match3BoardRenderer;
		tilesDestroyEffect = prefabTilesDestroyEffect.GetComponent<DestroyEffect>();
		
		delayBeforeTileDestroy = cachedAnimation["effect_winterchill_destruction"].length;
	}
	
	public override TileColorType TileColor {
		get {
			return base.TileColor;
		}
		set {
			base.TileColor = TileColorType.None;
		}
	}

	protected void SelectAndStartDestroySequence() {
		
		//Special case for DirectionalTile
		if(lastNeighborTile is DirectionalDestroyTile) {
			StartCoroutine(DestroyDirectionalTiles());
			return;
		}
		
		//Special case for BombTile. Combine with a BombTile
		if(lastNeighborTile is BombTile) {
			(lastNeighborTile as BombTile).spawnTimeBomb = false;
			StartCoroutine(CombineWithBombDestroy());
			return;
		}
		
		// Special case of combine with other ColorBomb tile.
		if (lastNeighborTile is ColorBombTile) {
			StartCoroutine( CombineWithColorBombDestroy() );
			return;
		}
		
		//Default behaviour
		StartCoroutine(DestroyColoredTiles());
	}
	
	protected override void TileDestroy(bool useEffect) 
	{	
		if (useDestroyEffect && useEffect) {
			SelectAndStartDestroySequence();
			
			if (freezeEffect)
			{
				freezeEffect.GetComponent<Animation>().Play("effect_winterchill_destruction_freeze");				
			}
			
			GetComponent<Animation>().Play("effect_winterchill_destruction");

		}
		else {
			tileModelTransform.gameObject.SetActive(false);
			
			if (this == BoardPiece.Tile) {
				BoardPiece.Tile = null;
			}
			
			Destroy(gameObject); //Zalo: They didn't destroy the tile... weird...
		}
	}
	
	public void OnDestroyAnimationEnded()
	{
		tileModelTransform.gameObject.SetActive(false);
		if (this == BoardPiece.Tile) {
			BoardPiece.Tile = null;
		}
	}
	
	
	public IEnumerator StartIdleAnim()
	{
		yield return new WaitForSeconds(GetComponent<Animation>()["effect_winterchill_idle"].length);
		
		if (!IsDestroying) {
			GetComponent<Animation>().Play("effect_winterchill_idle");
		}
	}
	
	IEnumerator DestroyColoredTiles()
	{
		
//		float offsetSize = Mathf.Abs(Board[0, 0].LocalPosition.x - Board[0, 1].LocalPosition.x) / 2f;
		if (OnTriggerColorBombTile != null) {
			OnTriggerColorBombTile(this);
		}
		
		if (destroyColor == TileColorType.None) {
			destroyColor = RuleEntry.genericColors[Random.Range(0, Match3BoardRenderer.maxNumBoardColors)];
		}
		
		for (int i = 0; i < Board.NumRows; i++) { 
			for (int j = 0; j < Board.NumColumns; j ++) {
				Match3Tile tile = Board[i, j].Tile as Match3Tile;

//				if (tile != null && !(tile is DropTile) && tile.TileColor == destroyColor)
				if (IsValidTarget(tile, destroyColor))
				{
					tilesToDestroy.Add(tile);
					if (iceTrailEffect != null) {
						Transform effectInstance = (Instantiate(iceTrailEffect) as GameObject).transform;
						effectInstance.position = WorldPosition;// + new Vector3(0f, 0f, -5f);
						effectInstance.parent = cachedTransform.parent;
						
						effectInstance.LookAt(tile.cachedTransform);
				
						tile.IsUserMoveable = false;
						StartCoroutine(MoveTargetTo(effectInstance, tile.cachedTransform, tilesDestroyEffect.destroyTileTime));
//						HOTween.To(effectInstance, tilesDestroyEffect.destroyTileTime, "localPosition", Board[i, j].LocalPosition);
						
						Destroy(effectInstance.gameObject, tilesDestroyEffect.lifeTime * 1.5f);
					}
				}
			}
		}
		
		yield return new WaitForSeconds(tilesDestroyEffect.destroyTileTime);

		for(int i = 0; i < tilesToDestroy.Count; i++) {
			if (tilesToDestroy[i]) {
				tilesToDestroy[i].IsMatched = true;
				tilesToDestroy[i].Destroy();
			}
		}
		
		if (tilesDestroyEffect.destroyTileTime < GetComponent<Animation>()["effect_winterchill_idle"].length) {
			yield return new WaitForSeconds(GetComponent<Animation>()["effect_winterchill_idle"].length - tilesDestroyEffect.destroyTileTime);
		}
		
		BaseTileDestroy(false);
		
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
	
	private void BaseTileDestroy(bool useEffect) {
		base.TileDestroy(useEffect);
	}

	protected IEnumerator CombineWithColorBombDestroy(float perTileWaitTime = 0.075f)
	{
		if (wasFirstTapped) {
			if (OnColorBombColorBombCombine != null) {
				OnColorBombColorBombCombine(this);
			}
			
			// Init the delay after each glacier shard will be thrown towards a tile.
			for(int colIdx = 0; colIdx < Board.NumColumns; colIdx++) {
				for(int rowIdx = 0; rowIdx < Board.NumRows; rowIdx++) {
					tileIterator = Board[rowIdx, colIdx].Tile as NormalTile;

					if ( IsGlacierShardTargetValid(tileIterator) && tileIterator != this && tileIterator != lastNeighborTile) 
					{
						// Throw the next glacier shard at the next found tile after "totalDelay" seconds.
						StartCoroutine(StartGlacierDestroyForTile(tileIterator, (targetTile) => 
						{
							if ( IsGlacierShardTargetValid(targetTile) && targetTile != this && targetTile != lastNeighborTile ) {
								targetTile.isSingleDestroyed = true;
								targetTile.IsMatched = true;
								targetTile.Destroy();
							}
						}));
						
						yield return new WaitForSeconds(perTileWaitTime);
					}
				}
			}
		}
		
		// Wait for the last shard to hit the board, wait an extra 1 second for safety and then destroy this tile.
		yield return new WaitForSeconds(tilesDestroyEffect.destroyTileTime + 0.5f);
	
		base.TileDestroy(false);
		
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
	
	
	protected bool IsGlacierShardTargetValid(NormalTile targetTile) {
		return targetTile != null /* && !(targetTile is SnowTile) && !(targetTile is FreezerTile) */&& !targetTile.IsFrozen() && targetTile.IsDestructible && !targetTile.IsDestroying;
	}
	
	/// <summary>
	/// Starts the glacier destroy for a targeted tile. Used by "CombineWithColorBombDestroy(...)" method.
	/// </summary>
	/// <returns>
	/// The glacier destroy for tile.
	/// </returns>
	/// <param name='targetTile'>
	/// Target tile.
	/// </param>
	/// <param name='afterDelay'>
	/// After delay.
	/// </param>
	protected IEnumerator StartGlacierDestroyForTile(NormalTile targetTile, System.Action<NormalTile> OnFinished = null) {
//		yield return new WaitForSeconds(afterDelay);
		targetTile.IsUserMoveable = false;
		
		Transform effectInstance = (Instantiate(iceTrailEffect) as GameObject).transform;
		effectInstance.position = WorldPosition;
		effectInstance.parent = cachedTransform.parent;
		
		effectInstance.LookAt(targetTile.cachedTransform);

		StartCoroutine(MoveTargetTo(effectInstance, targetTile.cachedTransform, tilesDestroyEffect.destroyTileTime));
		
		GameObject.Destroy(effectInstance.gameObject, tilesDestroyEffect.lifeTime * 1.5f);
		
		yield return new WaitForSeconds(tilesDestroyEffect.destroyTileTime);
		
		if (OnFinished != null) {
			OnFinished(targetTile);
		}
	}
	
	//Called When Color bomb meets Bomb Tile (Alternative effect - temporary until we decide wich will remain)
	//TODO: Lots of redundancy between this coroutine and DestroyDirectionalTiles
	IEnumerator CombineWithBombDestroy() {
	
		if (OnBombColorBombCombine != null) {
			OnBombColorBombCombine(this);
		}
		
		//Update the tiles to destroy list
		Board.ApplyActionToAll((boardPiece) => {
			if (IsValidTarget(boardPiece.Tile, destroyColor))
			{
					tilesToDestroy.Add(boardPiece.Tile as Match3Tile);
			}
		});
			
		//tilesToDestroy.Remove(neighborTile);
		//Destroy(neighborTile.gameObject);
		
		StartCoroutine(ConvertTilesToBombTiles());
		
		yield return new WaitForSeconds(postConversionWaitTime);
		
		for(int i = 0; i < tilesToDestroy.Count; i++) {
			tileIterator = tilesToDestroy[i] as NormalTile;
			
//			if ( IsGlacierShardTargetValid(tileIterator) ) {
//				if (prefabTilesDestroyEffect != null) {
//						Transform effectInstance = (Instantiate(prefabTilesDestroyEffect) as GameObject).transform;
//						effectInstance.position = WorldPosition;
//						effectInstance.parent = cachedTransform.parent;
//						
//						effectInstance.LookAt(tileIterator.cachedTransform);
//				
//						StartCoroutine(MoveTargetTo(effectInstance, tileIterator.cachedTransform, tilesDestroyEffect.destroyTileTime));
////						HOTween.To(effectInstance, tilesDestroyEffect.destroyTileTime, "localPosition", Board[i, j].LocalPosition);
//						
//						Destroy(effectInstance.gameObject, tilesDestroyEffect.lifeTime);
//				}
				
				yield return new WaitForSeconds(tilesDestroyEffect.destroyTileTime);
				
				// Repeat the tests for the targeted tile because things may have changed until the glacier shard reaches it.
				if ( IsGlacierShardTargetValid(tileIterator) ) {
					tileIterator.Destroy();
				}
//			}
		}
		
		// Wait for an extra 0.5f seconds for extra safety.
		yield return new WaitForSeconds(0.5f);
		
		base.TileDestroy(false);
		
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
		
	//Called When Color bomb meets directional tile
	//TODO: Lots of redundancy between this coroutine and DestroyDirectionalTiles
	IEnumerator DestroyDirectionalTiles() 
	{
		if (OnColorBombDirectionalCombine != null) {
			OnColorBombDirectionalCombine(this);
		}
		
		//Update the tiles to destroy list
		Board.ApplyActionToAll((boardPiece) => {
//			if (boardPiece.Tile != null && boardPiece.Tile.IsDestructible && (boardPiece.Tile as  Match3Tile).TileColor == destroyColor)
			if (IsValidTarget(boardPiece.Tile, destroyColor)) 
			{
				boardPiece.Tile.IsUserMoveable = false;
				tilesToDestroy.Add(boardPiece.Tile as Match3Tile);
			}
		});	
		
		StartCoroutine(ConvertToDirectionalDestroyers());
		
		yield return new WaitForSeconds(postConversionWaitTime);
		
		for(int i = 0; i < tilesToDestroy.Count; i++) 
		{
			tileIterator = tilesToDestroy[i] as NormalTile;
			
//			Ice bolt effect. Removed because no longer needed-------------------------------------------------------------------
//			if ( IsGlacierShardTargetValid(tileIterator) ) {
//				if (prefabTilesDestroyEffect != null) {
//						Transform effectInstance = (Instantiate(prefabTilesDestroyEffect) as GameObject).transform;
//						effectInstance.position = WorldPosition;
//						effectInstance.parent = cachedTransform.parent;
//						
//						effectInstance.LookAt(tileIterator.cachedTransform);
//				
//						StartCoroutine(MoveTargetTo(effectInstance, tileIterator.cachedTransform, tilesDestroyEffect.destroyTileTime));
////						HOTween.To(effectInstance, tilesDestroyEffect.destroyTileTime, "localPosition", Board[i, j].LocalPosition);
//						
//						Destroy(effectInstance.gameObject, tilesDestroyEffect.lifeTime);
//				}
//-------------------------------------------------------------------------------------------------------------------------------
			
				yield return new WaitForSeconds(tilesDestroyEffect.destroyTileTime);
				
				// Repeat the tests for the targeted tile because things may have changed until the glacier shard reaches it.
				if ( IsGlacierShardTargetValid(tileIterator) ) {
					tileIterator.Destroy();
				}
//			}
		}
		
		base.TileDestroy(false);
		
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
	
	/// <summary>
	/// TODO: temporary method used by "CombineWithBombDestroy" method until we decide which effect for this combo remains final.
	/// Converts the tiles from "tilesToDestroy" into bomb tiles.
	/// </summary>
	/// <returns>
	/// The tiles to bomb tiles.
	/// </returns>
	protected IEnumerator ConvertTilesToBombTiles() {
//		Debug.LogWarning("[ConvertTilesToBombTiles] Found: " + tilesToDestroy.Count + " elements.");
		
		Match3Tile cachedIterator;
		
		//Start the ICE TRAIL effect towards all the tiles soon to be converted.
		for( int i = 0;  i < tilesToDestroy.Count; i++) {
			cachedIterator = tilesToDestroy[i];
			cachedIterator.DisableTileLogic();
			
			if (cachedIterator is BombTile) {
				continue;
			}
			
			//TODO: Get rid of the reduntand code
			Transform effectInstance = (Instantiate(iceTrailEffect) as GameObject).transform;
			effectInstance.position = WorldPosition;// + new Vector3(0f, 0f, -5f);
			effectInstance.parent = cachedTransform.parent;
			effectInstance.LookAt(cachedTransform);

			StartCoroutine(MoveTargetTo(effectInstance, cachedIterator.cachedTransform, iceTrailTravelTime));
			GameObject.Destroy(effectInstance.gameObject, iceTrailTravelTime * 1.5f);
		}
		
		yield return new WaitForSeconds(iceTrailTravelTime);
		
		for( int i = 0;  i < tilesToDestroy.Count; i++) 
		{
			cachedIterator = tilesToDestroy[i];
		
			if (cachedIterator is BombTile) {
				continue;
			}
			if(cachedIterator is LockedTile)
			{
				(cachedIterator as LockedTile).OnConvertToOtherTile();
			}
			//TODO: Get rid of the reduntand code
			//instantiate and fire the conversion effect
			
			Transform effectInstance = (Instantiate(switchToDirectionalEffect) as GameObject).transform;
			effectInstance.position = cachedIterator.WorldPosition;// + new Vector3(0f, 0f, -5f);
			effectInstance.parent = cachedIterator.cachedTransform.parent;
			effectInstance.LookAt(cachedIterator.cachedTransform);
			effectInstance.GetComponent<Animation>().Play();
			
			GameObject.Destroy(effectInstance.gameObject, effectInstance.GetComponent<Animation>().clip.length);
			
			BombTile newBombTile = match3BoardRenderer.SpawnSpecificTileAt( cachedIterator.BoardPiece.BoardPosition.row,
											                             	cachedIterator.BoardPiece.BoardPosition.col,
																		 	typeof(BombTile),
																	     	TileColorType.None
																		  ) as BombTile;
			newBombTile.TileColor = cachedIterator.TileColor;
			newBombTile.UpdateMaterial();
			newBombTile.spawnTimeBomb = false;
			tilesToDestroy[i] = newBombTile;

			GameObject.Destroy(cachedIterator.gameObject);
		}		
		
		yield return new WaitForSeconds(switchToDirectionalEffect.GetComponent<Animation>().clip.length);
	}
	
	protected IEnumerator ConvertToDirectionalDestroyers() {
//		Debug.LogWarning("[ConvertToDirectionalTiles] Found: " + tilesToDestroy.Count + " elements.");
		
		Match3Tile cachedIterator;
		
		//Start the ICE TRAIL effect towards all the tiles soon to be converted.
		for( int i = 0;  i < tilesToDestroy.Count; i++) {
			
			cachedIterator = tilesToDestroy[i];
			cachedIterator.DisableTileLogic();
			
			if(cachedIterator is DirectionalDestroyTile || cachedIterator is BombTile ) {
				continue;
			}
			if(cachedIterator is LockedTile)
			{
				(cachedIterator as LockedTile).OnConvertToOtherTile();
			}
			
			//TODO: Get rid of the reduntand code
			Transform effectInstance = (Instantiate(iceTrailEffect) as GameObject).transform;
			effectInstance.position = WorldPosition;// + new Vector3(0f, 0f, -5f);
			effectInstance.parent = cachedTransform.parent;
			effectInstance.LookAt(cachedTransform);

			StartCoroutine(MoveTargetTo(effectInstance, cachedIterator.cachedTransform, iceTrailTravelTime));
			GameObject.Destroy(effectInstance.gameObject, iceTrailTravelTime * 1.5f);
		}
		
		yield return new WaitForSeconds(iceTrailTravelTime);
		
		for( int i = 0;  i < tilesToDestroy.Count; i++) {
			
			cachedIterator = tilesToDestroy[i];
			
			if(cachedIterator is DirectionalDestroyTile) {
				continue;
			}
			
			//TODO: Get rid of the reduntand code
			//instantiate and fire the conversion effect
			
			Transform effectInstance = (Instantiate(switchToDirectionalEffect) as GameObject).transform;
			effectInstance.position = cachedIterator.WorldPosition;// + new Vector3(0f, 0f, -5f);
			effectInstance.parent = cachedIterator.cachedTransform.parent;
			effectInstance.LookAt(cachedIterator.cachedTransform);
			effectInstance.GetComponent<Animation>().Play();
			
			GameObject.Destroy(effectInstance.gameObject, effectInstance.GetComponent<Animation>().clip.length);
			
			tilesToDestroy[i] = match3BoardRenderer.SpawnSpecificTileAt( cachedIterator.BoardPiece.BoardPosition.row,
											                             cachedIterator.BoardPiece.BoardPosition.col,
																		 Random.Range(0,2) == 0 ? typeof(ColumnDestroyTile) : typeof(RowDestroyTile),
																	     TileColorType.None
																	   );
			tilesToDestroy[i].TileColor = cachedIterator.TileColor;
			(tilesToDestroy[i] as DirectionalDestroyTile).UpdateMaterial();
			
			Destroy (cachedIterator.gameObject);			
		}		
		
		yield return new WaitForSeconds(switchToDirectionalEffect.GetComponent<Animation>().clip.length);
	}
		
	/// <summary>
	/// Linearly moves the specified target to the destination transform in "duration" seconds.
	/// If the "destination" transform is suddenly destroyed, it will still move towards it's last position.
	/// </summary>
	/// <returns>
	/// The target to.
	/// </returns>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='destination'>
	/// Destination.
	/// </param>
	/// <param name='duration'>
	/// Duration.
	/// </param>
	private IEnumerator MoveTargetTo(Transform target, Transform destination, float duration) {

		duration = iceTrailTravelTime;
		float t = 0f;
		float durationOffset = 1f / duration;
		Vector3 startPos = target.position;
		Vector3 lastDestinationPos = Vector3.zero;

		while(true) {
			if (destination != null) {
				lastDestinationPos = destination.position;
			}

			target.position = Vector3.Lerp(startPos, lastDestinationPos, t);
			// The stop condition is here so when t==1f we will position the object at the exact final position
			if (t >= 1f) {
				break;
			}
			t = Mathf.Clamp01(t + Time.deltaTime * durationOffset);
			
			yield return null;
		};
	}
	
	public override void RaiseEventTileSwitchAnimBegan(Match3Tile _neighborTile) 
	{
		base.RaiseEventTileSwitchAnimBegan(_neighborTile);
		
		//One of the few cases were the color bomb swipe is invalid and a switchback is required
		if(!lastNeighborTile.IsDestructible) {
			SwitchBackOnMatchFail = true;
			return;
		}
		
		destroyColor = lastNeighborTile.TileColor;

		// Disable colliders for this tile and it's neighbor tile if the neighbor is a ColorBomb tile.
		// Because these will do a combine effect and they shouldn't be picked up by other destroy sensors in the meantime.
		if (lastNeighborTile is ColorBombTile) {
			DisableTileLogic();
			lastNeighborTile.DisableTileLogic();
		}
		
		// Cached the "TappedFirst" property here for this tile because it will be reset in "RaiseEventTileSwitchAnimEnded" event.
		wasFirstTapped = TappedFirst;
		
		if(lastNeighborTile is DirectionalDestroyTile) {
			lastNeighborTile.CanBeMatched = false;
		}
		
		if(lastNeighborTile is BombTile) {
			lastNeighborTile.CanBeMatched = false;
		}
		
		if(_neighborTile.GetType() == typeof(NormalTile)/* && wasFirstTapped*/)
		{
			(_neighborTile as NormalTile).CanBeMatched = false;
			if ( !wasFirstTapped ) 
			{
				Match3BoardGameLogic.Instance.loseConditions.NewMove();
			}
		}	
	}
	
	public override void RaiseEventTileSwitchAnimEnded(AbstractTile neighbourTile) 
	{
		base.RaiseEventTileSwitchAnimEnded(neighbourTile);
		
		//One of the few cases were the color bomb swipe was invalid. reverting switchback to its default value
		if(lastNeighborTile == null || !lastNeighborTile.IsDestructible) {
			SwitchBackOnMatchFail = false;
			return;
		}
		
//		System.Type neighborType = neighbourTile.GetType();
		
		if(wasFirstTapped)
		{
			Match3BoardGameLogic.Instance.loseConditions.NewMove();
		}
		
		Destroy();
	}
	
	/// <summary>
	/// Determines whether this tile instance is a valid target for colorBombInteraction
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is valid target; otherwise, <c>false</c>.
	/// </returns>
	public bool IsValidTarget(AbstractTile tile, TileColorType destroyColor)
	{ 
		NormalTile target = tile as NormalTile;
		
//		if (target != null &&
//			target.IsDestructible &&
//			target.TileColor == destroyColor)
//		{
//			return true;
//		}
//		return false;
//		
		return target != null &&
			   target.IsDestructible &&
			   target.TileColor == destroyColor &&
			   !target.IsDestroying &&	
			   /* !(target is SnowTile) &&*/
			   !target.IsFrozen();
	}
	#region implemented abstract members of TriggerTile
	public override void TriggerTileAbility ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}

