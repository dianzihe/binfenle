using UnityEngine;
using Holoville.HOTween;
using System.Collections;
using System.Collections.Generic;

public class DirectionalDestroyTile : TriggerTile
{
	public enum Direction {
		Horizontal = 0,
		Vertical,
		Cross
	}
	
	/// <summary>
	/// Occurs when on the directional destroy tile effect is activated (horizontal or vertical)
	/// </summary>
	public static event System.Action<DirectionalDestroyTile> OnTriggerDirectionalDestroyTile;
	public static event System.Action<DirectionalDestroyTile> OnDirectionalDirectionalCombine;
	public static event System.Action<DirectionalDestroyTile> OnDirectionalBombCombine;
	
	[System.NonSerialized]
	public Direction direction;
	
	public Animation additionalChildAnimationComponent;
	
	[System.NonSerialized]
	public bool canDestroyBaseTileAfterWinterchill = true;
	[System.NonSerialized]
	public bool isCombineWithBombEffect = false;
	protected int bombCombineEffectColumn;
	
	private System.Action onFinishedDirectional;
	
	[System.NonSerialized]
	public float delayBetweenCrossDirections = 0f;
	
	
	//NO ANIMATION NAMING CONVENTIONS !?!?!
	protected string parentDestroyAnimationName = "effect_directional_destroy";
	protected string childDestroyAnimationName = "tile_directional_new_destroy";
	protected string childIdleAnimationName = "tile_directional_new_idle";
	
	public GameObject prefabTilesDestroyEffect;
	// Tiles destroy effect when combining directional destroy with a bomb.
	public GameObject prefabBombCombineDestroyEffect;
	// The tile effect that needs to appear when a BombTile combine effect starts (for example to spawn only visually a bigger tile that gets created and destroyed).
	public GameObject prefabBombCombinedTileEffect;

	public BoardPieceTouchController touchController;

	int numEffectsFinished = 0;

	private List<AbstractTile> tilesToDestroy = new List<AbstractTile>(9);
	private List<Match3BoardPiece> layeredBoardPiecesToDestroy = new List<Match3BoardPiece>(9);
	private List<MatchCounterBoardPiece> matchCountersBoardPiecesToTrigger = new List<MatchCounterBoardPiece>(9);
	private List<WinterchillEffect> winterchillEffects = new List<WinterchillEffect>(4);
	
	[System.NonSerialized]
	public WinterchillEffect tilesDestroyEffect;
	
	// Where the winterchill effect will be positioned before starting. By default is at it's own transform's position.
	[System.NonSerialized]
	public Match3Tile winterchilOriginTile;
	
	public Material[] coloredMaterials;
	
	//MIO's Hack Session---------------------------
	public GameObject[] FreezeEffectPrefabArray;
	
	public GameObject PrefabEffectBreakFragments;
	protected TileDestroyEffect effectBreakFragmentsInstance;

	public Material[] PrefabFreezeDestroyMaterialArray;
	//--------------------------------------------
	
	protected Match3BoardRenderer match3BoardRenderer;
		
		
	public override void InitComponent () {
		base.InitComponent ();
		
		match3BoardRenderer = BoardRenderer as Match3BoardRenderer;
		numEffectsFinished = 0;
		winterchilOriginTile = this;
		
		tilesToDestroy.Clear();
		winterchillEffects.Clear();

	 	additionalChildAnimationComponent.Play(childIdleAnimationName);
	}

	protected override void Start()
	{
		base.Start();

//		Match3Tile.OnTileTap += OnMyTileTap;
	}
	
	public virtual void UpdateMaterial()
	{
//		OLD
//		tileModelTransform.Find("tile_directional1").renderer.material = coloredMaterials[(int)TileColor - 1];
//		tileModelTransform.Find("tile_directional2").renderer.material = coloredMaterials[(int)TileColor - 1];
		
//		NEW
		tileModelTransform.GetComponent<Renderer>().material = coloredMaterials[(int)TileColor - 1];
		prefabFreezeEffect = FreezeEffectPrefabArray[(int)TileColor - 1];
	}
	
	#region implemented abstract members of TriggerTile
	public override void TriggerTileAbility () {
//		StartCoroutine(StartDirectionalDestroy());
		StartDirectionalDestroy();
		
//		if (lastNeighborTile is BombTile) {
//			StartCoroutine(DelayedTileRelease(delayBetweenCrossDirections * 2f + 1f));
//		} else {
			if (tilesDestroyEffect.waitBetweenTiles) {
				StartCoroutine(DelayedTileRelease(tilesDestroyEffect.destroyTileTime));
			} else {
				StartCoroutine(DelayedDestroy());
			}
//		}
	}
	#endregion
	
	
	public void SetVerticalColumnsLock(int centerColumnIdx, bool locked) {
		Board.ApplyActionToColumn(centerColumnIdx, (piece) => {
			if (locked) {
				(piece as Match3BoardPiece).BlockCount++;
			} else {
				(piece as Match3BoardPiece).BlockCount--;
			}
		});
		
		if (centerColumnIdx - 1 >= 0) {
			Board.ApplyActionToColumn(centerColumnIdx - 1, (piece) => {
				if (locked) {
					(piece as Match3BoardPiece).BlockCount++;
				} else {
					(piece as Match3BoardPiece).BlockCount--;
				}
			});			
		}
		
		if (centerColumnIdx + 1 < Board.NumColumns) {
			Board.ApplyActionToColumn(centerColumnIdx + 1, (piece) => {
				if (locked) {
					(piece as Match3BoardPiece).BlockCount++;
				} else {
					(piece as Match3BoardPiece).BlockCount--;
				}
			});			
		}
	}
	
//	public void SetNeighborPiecesLockState(bool locked) {
//		Match3BoardPiece piece = BoardPiece as Match3BoardPiece;
//		int numNeighbors = (int) Match3BoardPiece.LinkType.Count;
//		for(int i = 0; i < numNeighbors; i++) {
//			if (piece.neighbors[i] != null) {
//				if (locked) {
//					piece.neighbors[i].LockCount++;
//				} else {
//					piece.neighbors[i].LockCount--;
//					
//					if (piece.neighbors[i].LockCount == 0) {
//						piece.neighbors[i].IsOrphan = false;
//					}
//				}
//			}
//		}		
//	}
	
	protected void StartDirectionalDestroy(System.Action onFinished = null) {
		onFinishedDirectional = onFinished;
		
		if (direction == Direction.Horizontal || direction == Direction.Cross) {
			// Get the units size of a board column
			float totalLineSize = Match3BoardRenderer.maxBoardColSize;

			float percent1 = BoardPiece.BoardPosition.col / (float)Board.NumColumns;
			float percent2 = (Board.NumColumns - BoardPiece.BoardPosition.col - 1) / (float)Board.NumColumns;
			if (BoardPiece.BoardPosition.col > 0) {
				SpawnEffect(180f).Launch(this, percent1, Mathf.Max(percent1, percent2), totalLineSize);
			}
			if (BoardPiece.BoardPosition.col < Board.NumColumns - 1) {
				SpawnEffect(0f).Launch(this, percent2, Mathf.Max(percent1, percent2), totalLineSize);
			}
		}

//		if (delayBetweenCrossDirections > 0f) {
//			canDestroyBaseTileAfterWinterchill = false;
//			
//			yield return new WaitForSeconds(delayBetweenCrossDirections);
//		}

		if (direction == Direction.Vertical || direction == Direction.Cross) {
			// Get the units size of a board row
			float totalLineSize = Match3BoardRenderer.maxBoardRowSize;
			
			float percent1 = BoardPiece.BoardPosition.row / (float)Board.NumRows;
			float percent2 = (Board.NumRows - BoardPiece.BoardPosition.row - 1) / (float)Board.NumRows;
			if (BoardPiece.BoardPosition.row > 0) {
				SpawnEffect(270f).Launch(this, percent1, Mathf.Max(percent1, percent2), totalLineSize);
			}
			if (BoardPiece.BoardPosition.row < Board.NumRows - 1) {
				SpawnEffect(90f).Launch(this, percent2, Mathf.Max(percent1, percent2), totalLineSize);
			}
		}
		
		if (direction != Direction.Cross) 
		{
			if (OnTriggerDirectionalDestroyTile != null) {
				OnTriggerDirectionalDestroyTile(this);
			}
		}
		else {
			if (OnDirectionalDirectionalCombine != null) {
				OnDirectionalDirectionalCombine(this);
			}
		}
	}
		
	protected override void TileDestroy(bool useEffect) 
	{
		CheckForSpawnPatterns();
		
		tilesDestroyEffect = prefabTilesDestroyEffect.GetComponent<WinterchillEffect>();
		tilesToDestroy.Clear();
//		StartCoroutine(DestroyRow());
//		StartCoroutine(DestroyColumn());

		if (useEffect) {
			if (prefabDestroyEffect != null) {
				SpawnDestroyEffect(new Vector3(0f, 0f, -10f));
			} 
			else {
				destroyEffect = tilesDestroyEffect;
			}
			
			TriggerTileAbility();
			
			// to see the tile destroy animation
			if (useDestroyEffect) {
				tileModelTransform.gameObject.SetActive(true);
					
				//TODO:Remove redundant chunks of code if u will ever have the time / decency to do so
				//TODO: MIO: Revise this .. it was way too late when you wrote this.
				if(PrefabEffectBreakFragments) {
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
				}
				//--------------------------------------------------------------------------------------
				
				GetComponent<Animation>().Play(parentDestroyAnimationName, PlayMode.StopAll);
				additionalChildAnimationComponent.Play(childDestroyAnimationName, PlayMode.StopAll);
			}
			
			if(freezeEffect) {
				Destroy(freezeEffect);
			}
		} else {
			if (tilesDestroyEffect.waitBetweenTiles) {
				tileModelTransform.gameObject.SetActive(false);
				
				if (this == BoardPiece.Tile) {
					BoardPiece.Tile = null;
				}
			}
			else {
				base.TileDestroy(false);
			}
		}
	}
	
	/// <summary>
	/// Spawns the directional destroy effect oriented with "angle" degrees around the world X axis of the effects transform thus rotating the 
	/// it's forward axis in the direction the effect will do it's animation.
	/// </summary>
	/// <returns>
	/// The effect.
	/// </returns>
	/// <param name='angle'>
	/// The X rotation angle in degrees.
	/// On the board, the possible angle of the effect corresponds to the following animation directions:
	/// - 0 degrees: right direction
	/// - 90: bottom
	/// - 180: left
	/// - 270: up
	/// </param>
//	public override bool Freeze ()
//	{
//		bool effectApplied = base.Freeze();
//		
//		if(effectApplied) {
//			freezeEffect.GetComponent<FreezeEffectMaterialPicker>().AssignMaterialsForColor(TileColor);
//		}
//		
//		return effectApplied;
//	}
	
	WinterchillEffect SpawnEffect(float angle) {
		Transform effectInstance = (Instantiate(prefabTilesDestroyEffect) as GameObject).transform;
		effectInstance.position = winterchilOriginTile.WorldPosition + new Vector3(0f, 0f, -19f);
		Vector3 newRotation = effectInstance.eulerAngles;
		newRotation.x = angle;
		effectInstance.eulerAngles = newRotation;
		
		// Disable this tiles collider to avoid being detected by its own effect trigger (or by any other effects trigger)
		cachedCollider.enabled = false;

		WinterchillEffect effect = effectInstance.GetComponent<WinterchillEffect>();
		// Register to the freeze trigger.
		//effect.triggerListener.OnTileEntered = OnTileEnteredFreezeTrigger;
		//effect.triggerListener.OnBoardPieceEntered = OnBoardPieceEnteredFreezeTrigger;
		
		effect.OnEffectFinished = OnWinterchillEffectFinished;
		
		Destroy(effectInstance.gameObject, effect.lifeTime);
		
		winterchillEffects.Add(effect);
		numEffectsFinished++;
		
		return effect;
	}
	
	/// <summary>
	/// Event raised by the <see cref="WinterchillEffect"/> spawned in the "SpawnEffect(float angle)" method when the freeze trigger
	/// collider is disabled.
	/// </summary>
	/// <param name='sender'>
	/// Sender.
	/// </param>
	void OnWinterchillEffectFinished(DestroyEffect sender) {
		numEffectsFinished--;
		
		// All winter chill effects finished
		if (numEffectsFinished <= 0) {
			StartCoroutine(DestroyTilesAfterDelay(0.1f));
		}
		
		if(sender) 
		{
			sender.OnEffectFinished = null;
		}
	}
	
	public override void RaiseEventTileSwitchAnimBegan (Match3Tile neighborTile) {
		base.RaiseEventTileSwitchAnimBegan(neighborTile);
		
		if (neighborTile is BombTile) 
		{
			BombTile neighborBomb = neighborTile as BombTile;
			// Mark this tile and it's neighbor as moving and without any gravity because there will be a custom animation on them 
			// to try and keep things consistent.
			neighborBomb.DisableTileLogic();
			DisableTileLogic();

			// Prepare the bigger directional destroy effect on this directional tile.
			prefabTilesDestroyEffect = prefabBombCombineDestroyEffect;
		}
		
		if (neighborTile is DirectionalDestroyTile) 
		{
			DirectionalDestroyTile directionalTile = neighborTile as DirectionalDestroyTile;
			directionalTile.SwitchBackOnMatchFail = false;
			SwitchBackOnMatchFail = false;

			CanBeMatched = false;
			(neighborTile as NormalTile).CanBeMatched = false;
		}
	}
	
	public override void RaiseEventTileSwitchAnimEnded (AbstractTile neighborTile) {
		
		// Check if this directional tile was combined with a BombTile.
		
		// isCombineWithBombEffect flag check was added to remove some unwanted behaviors where (2x bomb/directional effects would spawn)
		// TODO: Investigate further into why this is happening.
		
		if (neighborTile is BombTile && !isCombineWithBombEffect)
		{
			Match3BoardGameLogic.Instance.loseConditions.NewMove();
	
			StartCoroutine(StartCombineWithBombEffect(neighborTile as BombTile));
		}
		
		// Setup directional with directional tile combine effect.
		if (neighborTile is DirectionalDestroyTile && TappedFirst)
		{
			Match3BoardGameLogic.Instance.loseConditions.NewMove();	
			
			IsDestroying = true;
			neighborTile.IsDestroying = true;
			
			Match3Tile crossTile = match3BoardRenderer.SpawnSpecificTileAt( BoardPiece.BoardPosition.row,
													                        BoardPiece.BoardPosition.col,
																			typeof(CrossDestroyTile),
																			TileColorType.None
																  		  );
			crossTile.TileColor = TileColor;
			crossTile.DisableTileLogic();
			(crossTile as DirectionalDestroyTile).UpdateMaterial();
			
			if(neighborTile.BoardPiece.Tile == neighborTile)
			{
				neighborTile.BoardPiece.Tile = null;
			}
			
			Destroy(neighborTile.gameObject);
			Destroy(gameObject);
		}
		
		if (neighborTile is ColorBombTile && TappedFirst)
		{
			Match3BoardGameLogic.Instance.loseConditions.NewMove();	
		}
		
		// In the base classes "movedByInput" and "tappedFirst" flags are reset so we call the base at the end of the overriden code.
		base.RaiseEventTileSwitchAnimEnded(neighborTile);
	}
	
	protected IEnumerator StartCombineWithBombEffect(BombTile neighborBombTile) {
		
		Match3Tile nextToEffectOriginTile;
//		TileColorType movedTileColor;
		
		IsDestroying = true;
		neighborBombTile.IsDestroying = true;
		
		if (OnDirectionalBombCombine != null) {
			OnDirectionalBombCombine(this);
		}
		
		isCombineWithBombEffect = true;
		
		if ( !TappedFirst ) {
			// Do the combo destroy effect from the other tiles world position because that one was moved by input.
			winterchilOriginTile = neighborBombTile;
			nextToEffectOriginTile = this;

		} else {
			nextToEffectOriginTile = neighborBombTile;
		}

		// Spawn the combined tile effect (an enlarging cross directional tile that builds up and the destroys itself)
		CrossBombCombineVisualEffect combinedTileEffect = (Instantiate(prefabBombCombinedTileEffect) as GameObject).GetComponent<CrossBombCombineVisualEffect>();
		combinedTileEffect.InitComponent();
		
		// Bring this giant tile in the front
		combinedTileEffect.cachedTransform.position = winterchilOriginTile.WorldPosition - Vector3.forward * 0.5f;
		// Set the delay between the horizontal and vertical direction destroy effect.
		delayBetweenCrossDirections = combinedTileEffect.horizontalAnimTime;
		
		HOTween.To(nextToEffectOriginTile.cachedTransform, 0.2f, new TweenParms()
											  							  .Prop("localPosition", winterchilOriginTile.LocalPosition)
																		  .Prop("localScale", Vector3.zero));
		
		bombCombineEffectColumn = winterchilOriginTile.BoardPiece.BoardPosition.col;
		
		yield return new WaitForSeconds(0.2f);
		
		neighborBombTile.gameObject.SetActive(false);
		
		// Disable the default visual effect for this directional tile because we're doing a special combine effect.
		useDestroyEffect = false;
		
		StartCoroutine(DelayedTileRelease(0.1f));
		
		// Destroy the cross tile effect after a delay.
		GameObject.Destroy(combinedTileEffect.gameObject, combinedTileEffect.totalAnimTime);		

		SetVerticalColumnsLock(bombCombineEffectColumn, true);
		
		canDestroyBaseTileAfterWinterchill = false;
		
		direction = Direction.Horizontal;
		StartDirectionalDestroy(() => 
		{
			canDestroyBaseTileAfterWinterchill = true;
			direction = Direction.Vertical;
			StartDirectionalDestroy();
					
			if(neighborBombTile.BoardPiece)
			{
				neighborBombTile.BoardPiece.Tile = null;
			}
			
			GameObject.Destroy(neighborBombTile.gameObject, 0.2f);
		});
	}
	
	/// <summary>
	/// Event raised by the <see cref="WinterchillEffect"/> spawned in the "SpawnEffect(float angle)" method when a tile 
	/// entered the freeze trigger of the effect.
	/// </summary>
	/// <param name='tile'>
	/// Tile.
	/// </param>
	void OnTileEnteredFreezeTrigger(NormalTile tile) {
		//TODO: currently if a tile doesn't support the freeze effect it will not get destroyed.
		if ( tile != this && tile.IsDestructible && tile.Freeze() ) {
			//TODO: Make the tile not match any other tiles because it will be destroyed by this effect anyway. ???
			tilesToDestroy.Add(tile);
		}
	}
	
	/// <summary>
	/// Event raised by the <see cref="WinterchillEffect"/> spawned in the "SpawnEffect(float angle)" method when boardpiece 
	/// entered the freeze trigger of the effect.
	/// </summary>
	/// <param name='tile'>
	/// Tile.
	/// </param>
	void OnBoardPieceEnteredFreezeTrigger(Match3BoardPiece boardPiece)
	{
		if(boardPiece is LayeredBoardPiece)
		{
			layeredBoardPiecesToDestroy.Add(boardPiece);
		}else if (boardPiece is MatchCounterBoardPiece) {
			MatchCounterBoardPiece matchCounterBoardPiece = boardPiece as MatchCounterBoardPiece;
			matchCountersBoardPiecesToTrigger.Add(matchCounterBoardPiece);
		}
	}
	
	IEnumerator DestroyTilesAfterDelay(float delay) {
		yield return new WaitForSeconds(delay);
		
		// Disable all winterchill effect freeze triggers after the winterchill ends.
		for(int i = 0; i < winterchillEffects.Count; i++) {
			WinterchillEffect winterchillEffect = winterchillEffects[i];
			if (winterchillEffect != null && winterchillEffect.freezeTrigger != null) {
				winterchillEffect.freezeTrigger.enabled = false;
			}
		}
		
		LayeredBoardPiece layeredBoardPiece;
		for(int i = 0; i < layeredBoardPiecesToDestroy.Count; i++)
		{
			layeredBoardPiece = layeredBoardPiecesToDestroy[i] as LayeredBoardPiece;
			
			if(layeredBoardPiece != null && layeredBoardPiece.Tile == null)
			{
				layeredBoardPiece.NumLayers--;
			}
		}
		layeredBoardPiecesToDestroy.Clear();

		foreach(MatchCounterBoardPiece counterBoardPiece in matchCountersBoardPiecesToTrigger) {
			counterBoardPiece.PieceReached(counterBoardPiece, counterBoardPiece.Tile);
		}
		matchCountersBoardPiecesToTrigger.Clear();


		
		// Destroy all tiles touched by the winterchill effect freeze triggers
		AbstractTile tile;
		for(int i = 0; i < tilesToDestroy.Count; i++) {
			tile = tilesToDestroy[i];
			if (tile != null) {
				tile.Destroy();
			}
		}
		tilesToDestroy.Clear();
		
		if (canDestroyBaseTileAfterWinterchill) {
			if (isCombineWithBombEffect) {
				SetVerticalColumnsLock(bombCombineEffectColumn, false);
			}

			BaseTileDestroy(false);
		}
		
		if (isCombineWithBombEffect && onFinishedDirectional != null) {
			onFinishedDirectional();
			onFinishedDirectional = null;
		}
	}
	
	private void BaseTileDestroy(bool useEffect) {
		base.TileDestroy(useEffect);
	}

//	// changes tile direction on tapping
//	public void OnMyTileTap(AbstractTile abstractTile)
//	{
//		if(abstractTile as Match3Tile == this)
//		{
//			// gets the info from the previous tile and destroys it
//			Match3Tile me =  abstractTile as Match3Tile;
//			TileColorType previousColor = me.TileColor;
//			BoardCoord previousCoord = me.BoardPiece.BoardPosition;
//			GameObject.Destroy(me.gameObject);
//
//			// creation of the new tile, same color, different direction
//			DirectionalDestroyTile newTile = null;
//			if(abstractTile is RowDestroyTile)
//			{
//				newTile = Match3BoardRenderer.Instance.SpawnSpecificTileAt(previousCoord, typeof(ColumnDestroyTile), TileColorType.None) as DirectionalDestroyTile;
//			}
//			if(abstractTile is ColumnDestroyTile)
//			{
//
//				newTile = Match3BoardRenderer.Instance.SpawnSpecificTileAt(previousCoord, typeof(RowDestroyTile), TileColorType.None) as DirectionalDestroyTile;
//			}
//			newTile.TileColor = previousColor;
//			newTile.UpdateMaterial();
//
//			Match3BoardGameLogic.Instance.loseConditions.NewMove();
//		}
//	}

	public override void OnDestroy()
	{
		base.OnDestroy();
//		Match3Tile.OnTileTap -= OnMyTileTap;
	}
}