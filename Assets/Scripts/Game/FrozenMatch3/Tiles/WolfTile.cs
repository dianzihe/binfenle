using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public enum WolfAnimState
{
	None = 0,
	Idle,
	Destroying,
	Jumping,
	
	Count,
};

public class WolfTile : NormalTile
{
	// Trap region
//	public static int wolfCounter = 0;
	
	public static event System.Action<WolfTile> OnWolfTileCreated;
	public static event System.Action<WolfTile> OnWolfTileBeginDestroy;
	public static event System.Action<WolfTile> OnWolfTileBeginIdle;
	
	public static event System.Action<WolfTile> OnWolfTileFinishJump;
	
	// true - jumps on Normal board pieces ; false - jumps on Empty board pieces
	public static bool isNormalWolf;
	public int turnsToJump = 1;
	
	#region WolfTile animations
	[System.NonSerialized]
	public WolfAnimState currentAnimState = WolfAnimState.None;
	
	public Animation wolfAnimation;
	public string idleAnimName; 
	public string destroyAnimName;
	
	[System.NonSerialized]
	public float timeUntilPlayIdle;
	
	[System.NonSerialized]
	public float scaleOnX;
	
	[System.NonSerialized]
	public float scaleOnY;
	public float scaleCreateDuration;
	public float scaleDownDuration;
	public float scaleUpDuration;
	public float jumpDelay;
	public float scaleDestroyDuration;
	public float extraDelay;
	public float bounceOverShoot;
		
	//The shake parameters
	public float deltaShake;
	public float shakeAmplitude;
	public float shakePeriod;
		
	//The min and max waiting times until the animation will play
	public float minIdleTime = 3f;
	public float maxIdleTime = 7f;
	#endregion
	
	[System.NonSerialized]
	public int wolfIndex;
	
	[System.NonSerialized]
	public int turnsLeftToJump;
	
	public int remainingHitPoints = 1;
	
	[System.NonSerialized]
	public bool pieceCachedAllowTileFalling;	
	
	[System.NonSerialized]
	public Match3BoardPiece targetJumpBoardPiece;
	
	protected Match3BoardPiece boardPieceIterator;
	public List<Match3BoardPiece> validJumpList = new List<Match3BoardPiece>();
	
	protected int jumpAnimAcount = 0;
	
	
	public override void InitComponent ()
	{
		base.InitComponent();
		
		TileColor = TileColorType.None;

		turnsLeftToJump = turnsToJump;
		tileModelRenderer = tileModelTransform.GetComponent<Renderer>();
		scaleOnX = cachedTransform.localScale.x;
		scaleOnY = cachedTransform.localScale.y;
		cachedTransform.localScale = Vector3.zero;
		
		if (WolfController.Instance == null)
		{
			Match3BoardGameLogic.Instance.gameObject.AddComponent<WolfController>();
		}
		
		timeUntilPlayIdle = Random.Range(minIdleTime, maxIdleTime);
		
//		wolfCounter++;
//		GetComponentInChildren<TextMesh>().text = wolfCounter.ToString();
	}
	
	public override void InitAfterAttachedToBoard ()
	{
		base.InitAfterAttachedToBoard ();
		
		// Start wolf intro animation
		HOTween.To(cachedTransform, scaleCreateDuration, new TweenParms()
			.Prop("localScale", new Vector3(scaleOnX, scaleOnY,0)));
		
		DestroyLinks(BoardPiece as Match3BoardPiece);
		
		RegisterNeighborBoardPieces(true);
		
		// Set initial animation state for this tile
		SetAnimState(WolfAnimState.Idle);
		
		WolfController.Instance.allWolves.Add(this);
		RaiseOnWolfTileCreated(this);
	}
	
	public void RegisterNeighborBoardPieces(bool subscribe)
	{
		Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
		
		for (int i = 0; i < match3BoardPiece.neighbors.Length; i += 2)
		{
			
			boardPieceIterator = match3BoardPiece.neighbors[i];
			
			if (boardPieceIterator == null) {
				continue;
			}
		
			if (subscribe) {
				boardPieceIterator.OnTileDestroyed += OnNeighborDestroy;
			} 
			else {
				boardPieceIterator.OnTileDestroyed -= OnNeighborDestroy;
			}
		}
	}

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
	
	public void OnNeighborDestroy(AbstractBoardPiece sender, AbstractTile neighbor)
	{
		
		if (neighbor && (neighbor as NormalTile).IsMatched)
		{
			Destroy();
		}
	}
	

/// <summary>
/// [Not Used At the moment]  Gets or sets the hit points.
/// </summary>
//	protected int HitPoints
//	{
//		get 
//		{
//			return remainingHitPoints;
//		}
//		
//		set	
//		{
//			remainingHitPoints = value;
//			
//			if (remainingHitPoints == 0)
//			{
//				Destroy();
//			}
//		}
//	}
	
	
	void ScaleDownFinished(TweenEvent tweenEvent)
	{
//		Debug.LogWarning("Scale Down Finished");
		WolfTile wolfTile = tweenEvent.parms[0] as WolfTile;
		
		// Fill the list of valid board pieces on which the wolf tiles could move to
		for (int rowIdx = 0; rowIdx < Board.NumRows; rowIdx++)
		{ 
			for (int colIdx = 0; colIdx < Board.NumColumns; colIdx++)
			{
				Match3BoardPiece boardPiece = Match3BoardGameLogic.Instance.boardData[rowIdx, colIdx] as Match3BoardPiece;
				if ( IsValidTarget(boardPiece) )
				{
					validJumpList.Add(boardPiece);
				}
			}
		}
		
//		Match3BoardPiece targetPiece = tweenEvent.parms[1] as Match3BoardPiece;
		Match3BoardPiece targetPiece = validJumpList[Random.Range(0, validJumpList.Count)];
		
		validJumpList.Clear ();
		
		
		if (targetPiece.Tile != null) {
			targetPiece.Tile.Destroy();
		}
		
		wolfTile.CreateLinks(wolfTile.BoardPiece as Match3BoardPiece);
		
		// move the wolf tile to target board piece
		wolfTile.BoardPiece.Tile = null;
		wolfTile.BoardPiece = targetPiece;
		targetPiece.Tile = wolfTile;	
		wolfTile.BoardPiece.ResetTilePosition();
		
		wolfTile.RegisterNeighborBoardPieces(true);
		wolfTile.DestroyLinks(targetPiece);
	}
			
	/// <summary>
	/// Destroy the links.
	/// </summary>
	/// <param name='target'>
	/// Target.
	/// </param>
	public void DestroyLinks(Match3BoardPiece target)
	{
		if(target is EmptyBoardPiece)
		{
			if (target.AllowTileFalling == true && FindValidNeighbor(-1,target) != null && FindValidNeighbor(1,target))
			{
				(target.Tile as WolfTile).pieceCachedAllowTileFalling = target.AllowTileFalling;
				target.AllowTileFalling = false;
				Match3BoardPiece targetTopNeighbor = FindValidNeighbor(-1,target);
				targetTopNeighbor.SetLink(Match3BoardPiece.LinkType.Bottom,null);
			}
		}
	}
	
	/// <summary>
	/// Creates the links.
	/// </summary>
	/// <param name='target'>
	/// Target.
	/// </param>
	public void CreateLinks(Match3BoardPiece target)
	{
		if (target is EmptyBoardPiece)
		{
			Match3BoardPiece topNonEmptyPiece = FindValidNeighbor(-1, target);
			Match3BoardPiece bottomNonEmptyPiece = FindValidNeighbor(1, target);
				
			if ((target.Tile as WolfTile).pieceCachedAllowTileFalling == true && topNonEmptyPiece != null && bottomNonEmptyPiece != null)
			{
				topNonEmptyPiece.SetLink(Match3BoardPiece.LinkType.Bottom, bottomNonEmptyPiece);
				target.AllowTileFalling = (target.Tile as WolfTile).pieceCachedAllowTileFalling;
			}
		}
	}
	
	/// <summary>
	/// Iterates through board pieces above or below the start board piece and looks for the first non-EmptyBoardPiece.
	/// </summary>
	/// <returns>
	/// The first non EmptyBoardPiece found in the specified look-up direction.
	/// </returns>
	/// <param name='lookupDir'>
	/// Search Direction. If it's -1 it will look for top valid neighbor else if it's 1 it will look for bottom valid neighbor.
	/// </param>
	/// <param name='startPiece'>
	/// Start board piece.
	/// </param>
	public static Match3BoardPiece FindValidNeighbor(int lookupDir, Match3BoardPiece startPiece)
	{
		int rowIdx = startPiece.BoardPosition.row;
		int colIdx = startPiece.BoardPosition.col;
		
		rowIdx = rowIdx + lookupDir;
		
		while(rowIdx >= 0 && rowIdx < startPiece.Board.NumRows)
		{
			Match3BoardPiece testPiece = Match3BoardGameLogic.Instance.boardData[rowIdx, colIdx] as Match3BoardPiece;
			if ( !(testPiece is EmptyBoardPiece) )
			{
				// Found the first valid neighbor.
				return testPiece;
			}
			
			rowIdx = rowIdx + lookupDir;
		}
		
		return null;
	}
	
	
	/// <summary>
	/// Creates 'numWolfTiles' tiles on different random valid boardpieces.
	/// </summary>
	/// <param name='numWolfTiles'>
	/// Number of wolf tiles.
	/// </param>
	/// <param name='createdOnStart'>
	/// Created at the start of the game.
	/// </param>
	public static void CreateWolves(int numWolfTiles) 
	{	
		List<Match3BoardPiece> validList = new List<Match3BoardPiece>();
		
		// Fill the list of valid board pieces on which the wolf tile can be created on
		for (int rowIdx = 0; rowIdx < Match3BoardRenderer.Instance.Board.NumRows; rowIdx++) 
		{ 
			for (int colIdx = 0; colIdx < Match3BoardRenderer.Instance.Board.NumColumns; colIdx++)
			{
				Match3BoardPiece boardPiece = Match3BoardGameLogic.Instance.boardData[rowIdx, colIdx] as Match3BoardPiece;
				if ( IsValidTarget(boardPiece) )
				{
					validList.Add(boardPiece);
				}
			}
		}

		// Create the set number of wolves on random valid board pieces
		for (int i = 0; i < numWolfTiles; i++)
		{
			int randomIndex = Random.Range(0, validList.Count - 1);

			if (validList.Count != 0)
			{
	
				Match3BoardPiece targetBoardPiece = validList[randomIndex];
		
				if(targetBoardPiece.Tile != null) {
					targetBoardPiece.Tile.Destroy();	
				}
				
				// Make target empty board piece blocked and reset the board piece from where the wolf moved 
				Match3BoardRenderer.Instance.SpawnSpecificTileAt(targetBoardPiece.BoardPosition, typeof(WolfTile), TileColorType.None, false);								
				validList.RemoveAt(randomIndex);	
			}
		}
	}
	
	
	public void RaiseOnWolfTileCreated(WolfTile sender)
	{
		if (OnWolfTileCreated != null)
		{
			OnWolfTileCreated(this);
		}
	}
	
	/// <summary>
	/// Determines whether this instance is valid target the specified boardPiece.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is valid target the specified boardPiece; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='boardPiece'>
	/// If set to <c>true</c> board piece.
	/// </param>
	public static bool IsValidTarget(AbstractBoardPiece boardPiece)
	{
		Match3BoardPiece targetBoardPiece = boardPiece as Match3BoardPiece;
		
		if (isNormalWolf == true)
		{
			NormalTile target = targetBoardPiece.Tile as NormalTile;
			
			//Added !target.isMoving
			return (target == null && !(targetBoardPiece is EmptyBoardPiece)) ||
				   ( !(targetBoardPiece is EmptyBoardPiece) && target.IsDestructible && !target.IsDestroying && !target.IsMoving &&
			   	   (targetBoardPiece.Tile.GetType() == typeof(NormalTile)) &&
		  		   !target.IsFrozen() );
		}		
		else
		{
			return 	(targetBoardPiece is EmptyBoardPiece) && !(targetBoardPiece.Tile is WolfTile) &&
					(NumV4NeighborNormalBoardPieces(targetBoardPiece) > 1) &&
					( (targetBoardPiece.GetNeighbor(Match3BoardPiece.LinkType.Bottom) == null) ||
					!(targetBoardPiece.GetNeighbor(Match3BoardPiece.LinkType.Bottom).IsTileSpawner) );
		}
	}
	
	/// <summary>
	/// Counts how many neighbors of a board piece are non-EmptyBoardPiece.
	/// </summary>
	/// <returns>
	/// The number of v4 neighbors that aren't EmptyBoardPieces. (v4 neighbors -> top, bottom, left, right)
	/// </returns>
	/// <param name='boardPiece'>
	/// Board piece.
	/// </param>
	public static int NumV4NeighborNormalBoardPieces(AbstractBoardPiece boardPiece)
	{
		Match3BoardPiece piece = boardPiece as Match3BoardPiece;
		
		int numV4Neighbors = 0;
		
		for (int i = 0; i < piece.neighbors.Length; i += 2)
		{
			if (!(piece.neighbors[i] is EmptyBoardPiece) && piece.neighbors[i] != null)	{
				numV4Neighbors++;
			}
		}
		return numV4Neighbors;
	}
	
	public void SetAnimState(WolfAnimState newAnimState)
	{
		if (currentAnimState == WolfAnimState.Destroying) 
		{
			return;
		}
		
		if (currentAnimState != newAnimState)
		{
//			Debug.LogWarning("[WolfTile] [SetAnimState] " + (name + wolfCounter) + " animState: " + newAnimState);
			
			StopCoroutine("WolfAnimState" + currentAnimState);
			
			currentAnimState = newAnimState;
			
			StartCoroutine("WolfAnimState" + newAnimState);
		}
	}
	
	protected IEnumerator WolfAnimStateIdle()
	{
		timeUntilPlayIdle = Random.Range(minIdleTime, maxIdleTime);
		
		yield return new WaitForSeconds(timeUntilPlayIdle);
		
		RaiseOnWolfTileBeginIdle(this);
		
		wolfAnimation.Play(idleAnimName);
	}
	
	
	protected IEnumerator WolfAnimStateDestroying()
	{
//		Debug.LogWarning("[WolfTile] [AnimationUpdater] Started Destroying State.");
				
		RegisterNeighborBoardPieces(false);
	
		CreateLinks(BoardPiece as Match3BoardPiece);
		WolfController.Instance.allWolves.Remove(this);
		
		if (OnWolfTileBeginDestroy != null)
		{
			OnWolfTileBeginDestroy(this);
		}
		
		wolfAnimation.Play(destroyAnimName);
		
		HOTween.Shake(cachedTransform, wolfAnimation[destroyAnimName].length , new TweenParms()
			.Prop("position", cachedTransform.position + Vector3.right * deltaShake)
			.Delay(0.1f), shakeAmplitude, shakePeriod);
		
		yield return new WaitForSeconds(wolfAnimation[destroyAnimName].length + extraDelay);
		
		HOTween.To (cachedTransform, scaleDestroyDuration, new TweenParms()
			.Prop("localScale", Vector3.zero)
		);
		
//		Debug.LogWarning(" End destroy Animation");
		
		yield return new WaitForSeconds(scaleDestroyDuration + extraDelay);
		
		base.TileDestroy(false);
	}
	
	protected IEnumerator WolfAnimStateJumping()
	{
//		jumpCounter++;
		
		IsMoving = true;
		
		RegisterNeighborBoardPieces(false);
		
		yield return new WaitForSeconds(jumpDelay * wolfIndex);
		
		Sequence sequence = new Sequence();				
		sequence.Append(HOTween.To(cachedTransform, scaleDownDuration, new TweenParms()
			.Prop("localScale", new Vector3(0,0,0)))
		);
		
		sequence.AppendCallback(ScaleDownFinished, this, targetJumpBoardPiece);
		
		sequence.Append(HOTween.To(cachedTransform, scaleUpDuration, new TweenParms()
			.Prop("localScale", new Vector3(scaleOnX,scaleOnY,0)))
		);
		
//		sequence.AppendCallback(ScaleUpFinished, this, targetJumpBoardPiece);
		
		sequence.Play();
		
		yield return new WaitForSeconds(sequence.duration);
		
		IsMoving = false;
		
		turnsLeftToJump = turnsToJump;

		RaiseOnWolfTileFinishJump(this);
		
		SetAnimState(WolfAnimState.Idle);
	}
	
	public void RaiseOnWolfTileFinishJump(WolfTile sender)
	{
		if (OnWolfTileFinishJump != null)
		{
			OnWolfTileFinishJump(sender);
		}
	}
	
	public void RaiseOnWolfTileBeginIdle(WolfTile sender)
	{
		if (OnWolfTileBeginIdle != null)
		{
			OnWolfTileBeginIdle(sender);
		}
	}
	
	protected override void TileDestroy(bool useEffect)
	{
//		Debug.LogWarning("[WolfTile] [TileDestroy] + useEffect" + useEffect);
		SetAnimState(WolfAnimState.Destroying);
	}
	
	public override bool Freeze() 
	{
		if ( !IsDestroying ) 
		{
			return true;
		}
		
		return false;
	}	
}
