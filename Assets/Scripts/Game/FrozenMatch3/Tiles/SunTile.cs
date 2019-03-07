using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SunTile : NormalTile
{
	public string animNameConvention;
	public int initialSize;
	
	protected Match3BoardPiece boardPieceIterator;
	
	public TilesTriggerListener triggerListener;
	public SunTileDestroyEffect sunTileDestroyEffect;
	
	public List<ParticleSystem> particleSystems;
	
	protected int targetSize = -1;
	protected int currentSize = -1;
	protected int previousSize = -1;
	
	protected int  newMoveCounter = 0;
	protected bool  animatedThisTurn = false;
	
	public const int STATE_EXPLODE = 5;
	public const int STATE_FADE = 0;
	private const float DESTROY_TRIGGER_SIZE = 3f;
	
	protected bool isGrowing = false;
	protected bool isFading = false;
	
	protected bool tileLogicEnabled = true;
	
	//List of neighbors this SunTile is currently subscribed to.
	protected List<Match3BoardPiece> subscriptionList;
	
	//Tiles collected by the trigger
	protected List<NormalTile> tilesToDestroy;
	
	//Collect all the targeted boardPieces and destroy Frost if no tile is present on that boardPiece
	protected List<Match3BoardPiece> boardPiecesToDestroy;
	
	public static event System.Action<SunTile> OnSunTileCreated;
	public static event System.Action<SunTile> OnSunTileDestroyed;
	
	public override RuleEntry DefaultRuleEntry
	{
		get
		{
			RuleEntry defaultRule = new RuleEntry();
			defaultRule.RuleTileType = GetType();
			defaultRule.randomColor = false;
			defaultRule.ColorSelection = ColorSelectionMethod.ColorBased;
			defaultRule.RuleTileColor = TileColorType.None;
			
			return defaultRule;
		}
	}
	
	protected override void Awake ()
	{
		base.Awake ();
		
		cachedAnimation = GetComponent<Animation>();
		
		currentSize = 1;
		TargetSize = initialSize;
		
		subscriptionList = new List<Match3BoardPiece>();
		tilesToDestroy = new List<NormalTile>();
		boardPiecesToDestroy = new List<Match3BoardPiece>();
	}
	
	#region Initialization
	
	public override void InitComponent ()
	{
		base.InitComponent();	
		TileColor = TileColorType.None;
		RaiseOnSunTileCreatedEvent();
	}
	
	public override void InitAfterAttachedToBoard ()
	{
		base.InitAfterAttachedToBoard ();
		
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove += OnNewMove;
		Match3BoardGameLogic.OnPostStableBoard += OnPostStableBoard;
		
		StartCoroutine(AnimationManager());	
		
		RenewNeighborSubscription();
	}
	
	#endregion
	
	#region Event Related Methods
	
	protected void RaiseOnSunTileCreatedEvent()
	{
		if (OnSunTileCreated != null)
		{
			OnSunTileCreated(this);
		}
	}
	
	protected void RaiseOnSunTileDestroyedEvent()
	{
		if (OnSunTileDestroyed != null)
		{
			OnSunTileDestroyed(this);
		}
	}
	
	/// <summary>
	/// Raises the event tile moving changed. Override to renew neighbor subscribtion when IsMoving is FALSE
	/// </summary>
	public override void RaiseEventTileMovingChanged ()
	{
		if (!IsMoving)
		{
			RenewNeighborSubscription();
		}
		
		base.RaiseEventTileMovingChanged ();
	}
	
	public void OnNeighborDestroy(AbstractBoardPiece sender, AbstractTile neighbor)
	{
		NormalTile targetTile = neighbor as NormalTile;
		
		if (targetTile.IsMatched)
		{
			if (!isGrowing)
			{
				TargetSize++;
			}
		}
	}
	
	public void OnNewMove()
	{
		newMoveCounter++;
		animatedThisTurn = false;
	}
	
	protected void OnPostStableBoard()
	{
		StartCoroutine(OnPostStableBoardCoroutine());
	}
	
	protected IEnumerator OnPostStableBoardCoroutine()
	{	
		yield return new WaitForEndOfFrame();
		
		while (BoardShuffleController.Instance.IsBoardReshuffling)
		{
			yield return null;
		}
		
		if (Match3BoardGameLogic.Instance.IsGameOver)
		{
			yield break;
		}
		
		if (newMoveCounter != 0 && !animatedThisTurn)
		{
			TargetSize--;
			newMoveCounter = 0;
		}
	}

	#endregion
	
	/// <summary>
	/// Gets or sets the targetSize for the Suntile.
	/// Changing this value will make the SunTile grow or fade from its currentSize to the specified value.
	/// </summary>
	public int TargetSize
	{
		get
		{
			return targetSize;
		}
		
		set
		{
			targetSize = Mathf.Clamp(value, STATE_FADE, STATE_EXPLODE);
		}
	}
	
	/// <summary>
	/// Handles SunTile animations (transitions + destroy animations).
	/// Acts according to the value specified in the TargetTile property.
	/// </summary>
	protected IEnumerator AnimationManager()
	{	
		while(true)
		{
			float animationWaitTime = 0f;
			
			// End coroutine if SunTile is destroying
			if (IsDestroying)
			{
				yield break;
			}
			
			// Early out if target size has been reached
			if (currentSize == targetSize || IsDestroying)
			{
				yield return null;
				continue;
			}
			else
			{
				previousSize = currentSize;
				
				if (currentSize < targetSize)
				{
					
					isGrowing = true;
					currentSize++;
//					Debug.LogError("Size increase detected! From " + previousSize  + " to " + currentSize);
				}
				else
				{
					//No fading allowed in game over ( In the FreeFall stage of the game this tile is only allowed to grow
					if (Match3BoardGameLogic.Instance.IsGameOver)
					{
						yield return null;
						continue;
					}
					
					isFading = true;
					currentSize--;
//					Debug.LogError("Size decrease detected! From " + previousSize + " to " + currentSize);
	
				}
			}
		
			animatedThisTurn = true;
			
			// Determine the transition animation that must be used and play it
			if (currentSize > STATE_FADE)
			{
				AnimationState transitionAnimState = FetchTransitionAnimState(previousSize, currentSize);
				
				//Debug.LogWarning("AnimationManager: Playing " + transitionAnimState.clip.name);
				cachedAnimation.Play(transitionAnimState.clip.name);
				animationWaitTime = transitionAnimState.clip.length;
			}
			
			
			// If the current state is a destroy state mark tile as IsDestroying and do the following:
			//  - Subscribe to the TriggerListener instance that collects all the tiles that need to be destroyed
			//  - start DestroyEffect Trigger Animation ( scale it from 0 to  DESTROY_TRIGGER_SIZE ) 
			//  - Disable tile logic
			//  - Collect all the boardPieces around that SunTile that need to be affected by its destruction.
			//  - Block the sunTile on its current BoardPiece as well as all the collected board pieces.
			//  - Stop listening for any 'neighbor destroyed' events because this tile is about to be destroyed anyway.
			if (IsInDestroyState(currentSize))
			{	
				IsDestroying = true;
				
				if (currentSize == STATE_EXPLODE)
				{
					triggerListener.OnTileEntered += OnTileToDestroyEntered;
					triggerListener.OnTileExit += OnTileToDestroyExit;
					
					sunTileDestroyEffect.scaleTarget = Match3BoardRenderer.horizTileDistance * DESTROY_TRIGGER_SIZE;
					sunTileDestroyEffect.StartColliderScaleEfect();
					
					DisableTileLogic();
					CollectNeighborBoardPiece();
					
					(BoardPiece as Match3BoardPiece).BlockCount++;
					
					for(int i = 0 ; i < boardPiecesToDestroy.Count; i++)
					{
						if (boardPiecesToDestroy[i])
						{
							boardPiecesToDestroy[i].BlockCount++;
						}
					}
				}
				
				ClearNeighborSubscription();
			}
			
			
			// Wait for the transition animation to finish the play the idle / destroy animation
			// appropriate for the currentSize of this SunTile
			if (animationWaitTime != 0f)
			{
				//Debug.LogWarning("Waitting : " + animationWaitTime);
				yield return new WaitForSeconds(animationWaitTime);
				isGrowing = false;
				isFading = false;
			}
			
			//Debug.LogWarning("Playing: " + animNameConvention + currentSize);
			cachedAnimation.Play(animNameConvention + currentSize);
		}
	}
	
	/// <summary>
	/// Return the animation that needs to be played to illustrate a transition from startIndex to endIndex.
	/// </param>
	protected AnimationState FetchTransitionAnimState(int startIndex, int endIndex)
	{
//		Debug.LogWarning("[FetchTransitionAnimState] : " + startIndex + endIndex);
		
		AnimationState transitionAnimState;
		
		if (startIndex < endIndex)
		{
			transitionAnimState = GetComponent<Animation>()[animNameConvention + startIndex + "-" + endIndex];
			transitionAnimState.speed = 1;
		}
		else
		{
			transitionAnimState = GetComponent<Animation>()[animNameConvention + endIndex + "-" + startIndex];
			transitionAnimState.speed = -1f;
			transitionAnimState.normalizedTime = 1f;
		}
		
		return transitionAnimState;
	}
	
	#region Animation Events
	
	protected void OnExplodeAnimStarted()
	{
		Match3BoardGameLogic.Instance.IsBoardStable = false;
		
		for(int i = 0; i < particleSystems.Count; i++)
		{
			particleSystems[i].Play();
		}
	}
	
	protected void OnExplodeAnimExplode()
	{
		
		DestroyNeighborTiles();
	}
	
	protected void OnExplodeAnimRelease()
	{
		//BoardPiece.Tile = null;
	}
	
	protected void OnExplodeAnimDestroy()
	{
		(BoardPiece as Match3BoardPiece).BlockCount--;
		
		for(int i = 0 ; i < boardPiecesToDestroy.Count; i++)
		{
			if (boardPiecesToDestroy[i])
			{
				boardPiecesToDestroy[i].BlockCount--;
			}
		}
		
		IsDestroying = false;
		Destroy();
		
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
	
	protected void OnFadeAnimEnded()
	{
		IsDestroying = false;
		numPoints = 0;
		Destroy();
		
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}

	#endregion
	
	/// <summary>
	/// Clear the previous subscription list and register again to the surrounding neighbors of a tile.
	/// This is generally called OnMovingChanged when IsMoving is false (a.k.a the tile moved and it just stopped.)
	/// </summary>
	private void RenewNeighborSubscription()
	{
		if (IsMoving)
		{
			return;
		}
		
		ClearNeighborSubscription();
		
		Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
		
		for(int i = 0; i < match3BoardPiece.neighbors.Length; i+=2)
		{	
			boardPieceIterator = match3BoardPiece.neighbors[i];
			
			if (boardPieceIterator == null)
			{
				continue;
			}
			
			boardPieceIterator.OnTileDestroyed += OnNeighborDestroy;
			subscriptionList.Add(boardPieceIterator);
		}
	}

	private void ClearNeighborSubscription()
	{
		subscriptionList.ForEach ( (subscriptionItem) => { subscriptionItem.OnTileDestroyed -= OnNeighborDestroy; });
		subscriptionList.Clear();
	}
	
	#region Destruction
	
	protected bool IsInDestroyState(int targetValue)
	{
		if (targetValue == STATE_FADE || targetValue == STATE_EXPLODE)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	protected bool IsValidDestroyTarget(AbstractTile target)
	{
		if (target && !(target is DropTile))
		{
			return true;
		}
	
		return false;
	}
	
	public override bool Freeze ()
	{
		return true;
	}
	
	public void OnTileToDestroyEntered(NormalTile tile)
	{
		tilesToDestroy.Add(tile);
	}
	
	public void OnTileToDestroyExit(NormalTile tile)
	{
		tilesToDestroy.Remove(tile);
	}
	
	private void CollectNeighborBoardPiece()
	{
		Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
		
		for(int i = 0; i < match3BoardPiece.neighbors.Length; i++)
		{	
			boardPieceIterator = match3BoardPiece.neighbors[i];
			
			if (boardPieceIterator == null)
			{
				continue;
			}
			
			boardPiecesToDestroy.Add(boardPieceIterator);
			
			if (i % 2 == 0)
			{
				boardPiecesToDestroy.Add(boardPieceIterator.neighbors[i]);
			}
		}
	}
	
	private void DestroyNeighborTiles()
	{
		triggerListener.OnTileEntered -= OnTileToDestroyEntered;
		triggerListener.OnTileExit -= OnTileToDestroyExit;
		
		for(int i = 0; i < boardPiecesToDestroy.Count; i++)
		{
			if (boardPiecesToDestroy[i])
			{
				if (boardPiecesToDestroy[i] is LayeredBoardPiece && boardPiecesToDestroy[i].Tile == null)
				{
					(boardPiecesToDestroy[i] as LayeredBoardPiece).NumLayers--;
				}
			}
		}
		
		for(int i = 0; i < tilesToDestroy.Count; i++)
		{
			if (tilesToDestroy[i])
			{
				tilesToDestroy[i].Destroy();
			}
		}
	}
	
	public override void DisableTileLogic ()
	{
		base.DisableTileLogic();

		ClearNeighborSubscription();
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove -= OnNewMove;
		Match3BoardGameLogic.OnPostStableBoard -= OnPostStableBoard;
	}
	
	public override void Destroy()
	{
		if (!IsInDestroyState(currentSize))
		{
			if (!isGrowing)
			{
				TargetSize++;
			}
		}
		else 
		{
			base.Destroy ();
		}
	}
	
	protected override void TileDestroy (bool useEffect)
	{
		base.TileDestroy (false);
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove -= OnNewMove;
		Match3BoardGameLogic.OnPostStableBoard -= OnPostStableBoard;
			
		RaiseOnSunTileDestroyedEvent();
	}
	
	public override void OnDestroy()
	{
		base.OnDestroy();
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove -= OnNewMove;
		Match3BoardGameLogic.OnPostStableBoard -= OnPostStableBoard;
	}
	#endregion
}
