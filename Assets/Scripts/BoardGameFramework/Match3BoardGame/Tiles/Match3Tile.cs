using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public enum TileColorType : int 
{
	None = 0,
	Red,
	Green,
	Blue,
	Yellow,
	Purple,
	Orange,
	
	Count,
}

public enum TileMatchDirection : int 
{
	Left = 0,
	Top,
	Right,
	Bottom,
	
	Count,
}

public abstract class Match3Tile : AbstractTile 
{
	public delegate void TileDestroyed(Match3Tile tile);
	public delegate void Match3TileEventHandler(Match3Tile tile);

	public static event TileDestroyed OnAnyTileDestroyed;
	public static event Match3TileEventHandler OnTileStartedActiveGravity;
	public static event System.Action<Match3Tile, Match3BoardPiece> OnTileFinishedActiveGravity;
	public static event Match3TileEventHandler OnTileInitAfterAttachedToBoard;
	
	public bool canDrawGizmos = true;
//	protected float maxFallSpeed = 15f;
	
	private bool wasInitialized = false;

	/// <summary>
	/// TODO: not yet used and integrated anywhere but will be useful soon enough. Might be useful for level editing too
	/// Indicates if this tile can have a color (even if it initially has the "TileColor" property set to None)
	/// </summary>
	public bool canHaveColor = true;
		
	/// <summary>
	/// If this is false the tile destroy shouldn't do it's destroy effect. 
	/// TODO: It's used hackishly and inconsistently applied. Review if there's time.
	/// </summary>
	[System.NonSerialized]	
	public bool useDestroyEffect = true;
		
	//--- Related to tile movement physics ----
	[System.NonSerialized]	
	public float accel = 15f;
	
	[System.NonSerialized]
	public float maxVel = 13f;
	
	[System.NonSerialized]
	public float initialVel = 1f;
	protected float velWaitDampFactor = 0.45f;
	
	[System.NonSerialized]
	public float moveVel = 0f;
	
	[System.NonSerialized]
	public Vector3 moveDir;
	
	[System.NonSerialized]
	public Vector3 fallDestination = Vector3.zero;
	// ------------------------------------
	
	public GameObject prefabDestroyEffect;

	// Delegate with the following method signature: OnTilesSwitchAnimEnded(AbstractTile neighbourTile).
	private System.Action<AbstractTile> onTileSwitchAnimEnded;
	
	/// <summary>
	/// The number of matches found for each "TileMatchDirection".
	/// </summary>
	[System.NonSerialized]
	public int[] matchCount = new int[(int)TileMatchDirection.Count];
	
	private float colliderRadius;
	private float sqrColliderRadius;
	
	[SerializeField]
	private bool canBeMatched = true;
	
	private bool isMatched = false;
	
	private bool isTileSwitching = false;
	
	/// <summary>
	/// Flag that indicates if this tile is ignored by the matches undo system. (MatchesUndoer)
	/// </summary>
	private bool isTileIgnoredByAntiMatchSystems = false;

	/// <summary>
	/// Turns on/off gravity for a tile. It will start/stop the coroutine in charge of checking and updating the tile's gravity state.
	/// If this is false, changing the "GravityEnabled" property won't have any effect.
	/// </summary>
	[SerializeField]
	private bool gravityUpdateEnabled = true;
	
	/// <summary>
	/// Flag for temporarilly disabling gravity for a tile. This property is not meant for level design usage. It's internally changed 
	/// for example when the user is switching 2 tiles so the tiles won't start falling down.
	/// </summary>
	private bool gravityEnabled = true;
	
	// Inidicates if a tile has just moved diagonally
	[System.NonSerialized]
	public bool tileMovedDiagonally = false;
	[System.NonSerialized]
	public bool debugPassiveGravity = false;
	[System.NonSerialized]
	public bool debugActiveGravity = false;
	
	/// <summary>
	/// The color of the tile.
	/// </summary>
	[SerializeField]
	private TileColorType color;

	[SerializeField]
	private bool switchBackOnMatchFail = true;
	
	private int lastBoardPieceSyncFrame;
	
	[System.NonSerialized]
	public Transform tileModelTransform;
	[System.NonSerialized]
	public Vector3 tileModelLocalPos;
	[System.NonSerialized]
	public Renderer tileModelRenderer;
	
	protected bool addedScore = false;
	
	private bool tappedFirst = false;
	
	protected WaitForEndOfFrame waitEndOfFrame;
	
	
	protected override void Awake () 
	{
		base.Awake();
		
		waitEndOfFrame = new WaitForEndOfFrame();
		
		tileModelTransform = cachedTransform.Find("Model");
		tileModelRenderer = tileModelTransform.GetComponent<Renderer>();
		tileModelLocalPos = tileModelTransform.localPosition;
	}
	
	public override void InitComponent() 
	{
		base.InitComponent();
		
		// Mark this tile component as being initialized.
		WasInitialized = true;

		TappedFirst = false;

		// Init collider radius.
		Vector3 size = Vector3.Scale(cachedCollider.bounds.size, cachedTransform.localScale);
		ColliderRadius = Mathf.Max(size.x, size.y);
		ResetMatchCountDirections();	
	}
	
	/// <summary>
	/// This method must be called on tiles after they are placed on the board for the first time.
	/// </summary>
	public virtual void InitAfterAttachedToBoard()
	{
		// Apply editor setting to make sure this property sets up any required internal stuff like the starting/stopping the "UpdateGravity()" coroutine.
		GravityUpdateEnabled = gravityUpdateEnabled;

		if(OnTileInitAfterAttachedToBoard != null)
		{
			OnTileInitAfterAttachedToBoard(this);
		}
	}
	
	public bool WasInitialized {
		get {
			return wasInitialized;
		}
		set {
			wasInitialized = value;
		}
	}
	
	public abstract RuleEntry DefaultRuleEntry
	{
		get;
	}
	
	public virtual TileColorType TileColor {
		get {
			return color;
		}
		set {
			color = value;
		}
	}
	
	public bool IsTileSwitching
	{
		get {
			return isTileSwitching;
		}
		set {
			isTileSwitching = value;
		}
	}
	
	public bool IsTileIgnoredByAntiMatchSystems {
		get {
			return isTileIgnoredByAntiMatchSystems;
		}
		set {
			isTileIgnoredByAntiMatchSystems = value;
		}
	}		

	public float ColliderRadius {
		get {
			return colliderRadius;
		}
		protected set {
			colliderRadius = value;
			SqrColliderRadius = colliderRadius * colliderRadius;
		}
	}
	
	public float SqrColliderRadius {
		get {
			return sqrColliderRadius;
		}
		protected set {
			sqrColliderRadius = value;
		}
	}
	
	public bool GravityEnabled {
		get {
			return gravityEnabled;
		}
		set {
			gravityEnabled = value;
		}
	}
	
	public bool TappedFirst {
		get {
			return tappedFirst;
		}
		set {
			tappedFirst = value;
		}
	}	

	public bool GravityUpdateEnabled {
		get {
			return gravityUpdateEnabled;
		}
		set {			
			// Restart gravity checker coroutine making sure we stop any previous ones already running.
			if (value) 
			{
				debugGravityEnabled = false;
				StopCoroutine("UpdateGravity");
				StartCoroutine("UpdateGravity");
			} 
			else 
			{
				debugGravityEnabled = false;
//				Debug.LogWarning(name + " stopping gravity.");
				StopCoroutine("UpdateGravity");
			}

			gravityUpdateEnabled = value;
		}
	}	

	public System.Action<AbstractTile> OnTileSwitchAnimEnded {
		get {
			return onTileSwitchAnimEnded;
		}
		set {
			onTileSwitchAnimEnded = value;
		}
	}
	
	public virtual void RaiseEventTileTappedFirst() {
		tappedFirst = true;
	}
	
	/// <summary>
	/// Raises the tile switch animation ended event after this tile finishes the switch animation with another neighbour tile.
	/// This event should be raised by the <see cref="Match3BoardGameLogic"/>.
	/// </summary>
	/// <param name='neighborTile'>
	/// Neighbour tile with which this tile did the switch.
	/// </param>
	public virtual void RaiseEventTileSwitchAnimEnded(AbstractTile neighborTile) {
		if (!IsMatched) {
			tappedFirst = false;
		}

		if (onTileSwitchAnimEnded != null) {
			onTileSwitchAnimEnded(neighborTile);
		}
	}
	
	/// <summary>
	/// Disables the tile logic. Used usually when the tile will eventuall be destroyed after a custom animation.
	/// But until the animation ends it will stay on the board in an unmatchable and undetectable by other effects, state.
	/// </summary>
	public virtual void DisableTileLogic() {
//		GravityEnabled = false; (redundant because after end switch animation it's set back to "true"
		GravityUpdateEnabled = false;
//		IsMoveable = false; ???
		IsMoving = true;
		CanBeMatched = false;
		cachedCollider.enabled = false;
		SwitchBackOnMatchFail = false;
	}
	
	/// <summary>
	/// Raises the tile switch animation begin event when this tile starts the switch animation with another neighbour tile.
	/// This event should be raised by the <see cref="Match3BoardGameLogic"/>.
	/// </summary>
	/// <param name='neighborTile'>
	/// Neighbour tile with which this tile does the switch.
	/// </param>
	public virtual void RaiseEventTileSwitchAnimBegan(Match3Tile neighborTile) {
		
	}
	
	public virtual void RaiseEventSwitchBackOnFail(Match3Tile neighborTile) {
		
	}
	
	public virtual void RaiseEventSwitchSuccess(Match3Tile neighborTile) {  }
	
	protected override bool PreDestroy ()
	{
		if ( !base.PreDestroy() ) {
			return false;
		}

		// Before destroying this tile make sure we're marking it as a non-moveable tile. (no user input allowed on it) and
		// disable it's gravity checking.
		IsUserMoveable = false;
		GravityUpdateEnabled = false;
		
		if (OnAnyTileDestroyed != null) {
			OnAnyTileDestroyed(this);
		}
		
		if (BoardPiece != null) {
			BoardPiece.RaiseEventTileDestroyed(this);
		}
		
		return true;
	}
		
	public bool SwitchBackOnMatchFail {
		get {
			return switchBackOnMatchFail;
		}
		set {
			switchBackOnMatchFail = value;
		}
	}
			
	public void ResetMatchCountDirections() {
		for(int i = 0; i < matchCount.Length; i++) {
			matchCount[i] = 0;
		}
	}
	
	public bool IsMatched {
		get {
			return isMatched ||
				   matchCount[(int)TileMatchDirection.Top] + matchCount[(int)TileMatchDirection.Bottom] >= 2 ||
				   matchCount[(int)TileMatchDirection.Left] + matchCount[(int)TileMatchDirection.Right] >= 2;
		}
		set {
			isMatched = value;
		}
	}

	public virtual bool CanBeMatched
	{
		get {
			return canBeMatched && TileColor != TileColorType.None && !IsMoving && !IsDestroying;
		}
		set {
			canBeMatched = value;
		}
	}
		
	public bool IsMatchWith(Match3Tile otherTile) {
		return otherTile != null && otherTile.CanBeMatched && CanBeMatched && TileColor == otherTile.TileColor;
	}
	
	/// <summary>
	/// Checks if this tile's local position has reached the local position of its board piece.
	/// This method is not accurate in detecting if the tile is at the exact board piece position because it is only
	/// used as a fast approximation where there's no need for this accuracy.
	/// </summary>
	/// <value>
	/// <c>true</c> if it reached its board piece local position, otherwise <c>false</c>.
	/// </value>
	public bool HasReachedBoardPieceArea() 
	{
		return LocalPosition == BoardPiece.LocalPosition || 
			   (LocalPosition - BoardPiece.LocalPosition).sqrMagnitude < Match3BoardRenderer.sqrVertTileDistance * 0.15f; //TODO: 0.15f multiply is wrong because of using squared testing
	}
	
	public bool HasTileInArea(Match3Tile otherTile)
	{
		return (otherTile.LocalPosition - LocalPosition).sqrMagnitude <= Match3BoardRenderer.sqrVertTileDistance;
	}	

	public override void WriteToStream (System.IO.BinaryWriter writeStream)
	{
		base.WriteToStream (writeStream);
		
		Debug.Log("[WriteToStream] -> TileColor: " + TileColor);
		writeStream.Write((int)TileColor);
	}
	
	public override void ReadFromStream (int fileVersion, System.IO.BinaryReader readStream)
	{
		base.ReadFromStream (fileVersion, readStream);
		
		if (fileVersion == 1) {
			TileColor = (TileColorType)readStream.ReadInt32();
			Debug.Log("[ReadFromStream] -> TileColor: " + TileColor);
		} else {
			throw new UnityException("[ReadFromStream] Reading tile from file version: " + fileVersion + " not supported!");
		}
	}
	
	/// <summary>
	/// Syncs the board piece position
	/// </summary>
//	public void SyncBoardPieceGridPosition(bool forceUpdate = false) {
//		// Make sure we don't re-sync the current piece
//		if (Time.frameCount == lastBoardPieceSyncFrame && !forceUpdate) {
//			return;
//		}
//		lastBoardPieceSyncFrame = Time.frameCount;
//		
//		// Convert this tile's local position to grid position. If the tile will be outside the grid the resulting grid position
//		// will be clamped.
//		Match3BoardRenderer boardRenderer = BoardRenderer as Match3BoardRenderer;
//		BoardCoord gridPos = new BoardCoord(0, 0);
//		gridPos.row = (int)((LocalPosition.y - boardRenderer.halfVertTileDistance) * boardRenderer.boardYDirectionSign / boardRenderer.vertTileDistance);
//		gridPos.col = (int)((LocalPosition.x + boardRenderer.halfHorizTileDistance) / boardRenderer.horizTileDistance);
//		gridPos.row = Mathf.Clamp(gridPos.row, 0, Board.NumRows);
//		gridPos.col = Mathf.Clamp(gridPos.col, 0, Board.NumColumns);
//		
//		// Change the board piece this tile belongs to.
//		BoardPiece = Board[gridPos];
//	}
	
	//TODO: revise this code (quickly written; it's actually a loopable 2-state FSM: PassiveState => ActiveState)
	private bool debugGravityEnabled = false;
	public IEnumerator UpdateGravity() 
	{
		// Safety check
//		if (debugGravityEnabled) {
//			Debug.LogWarning("!!! UpdateGravity coroutine started a second time for tile: " + name);
//		}
		
		debugGravityEnabled = true;
		
		while(GravityUpdateEnabled) 
		{
			if (!enabled || !GravityEnabled || BoardPiece == null) {
				yield return null;
				
				continue;
			}
			
			yield return StartCoroutine(PassiveGravityChecker());
			
			yield return StartCoroutine(ActiveGravityChecker());
		}
		
		debugGravityEnabled = false;
	}

	/// <summary>
	/// Passive gravity check coroutine.
	/// </summary>
	/// <returns>
	/// The gravity check.
	/// </returns>
	protected IEnumerator PassiveGravityChecker() {
		Match3BoardPiece curPiece = null;
		
		while(GravityUpdateEnabled) 
		{	
			if (!enabled || !GravityEnabled || BoardPiece == null) 
			{
				yield return null;
				continue;
			}
						
			debugPassiveGravity = true;
			curPiece = BoardPiece as Match3BoardPiece;
			
			// Check for active gravity enabling only if we're not moving and we're in a board piece that has no lock set on it.
			if ( !IsMoving && curPiece.LockCount <= 0 ) 
			{
				if ( !HasReachedBoardPieceArea() ||
					
					 (curPiece.BottomLink != null && !curPiece.BottomLink.IsBlocked && curPiece.BottomLink.Tile == null) ||
					
					 (curPiece.BottomLeftLink != null && !curPiece.BottomLeftLink.IsBlocked && !curPiece.BottomLeftLink.IsTileSpawner && 
					 curPiece.BottomLeftLink.Tile == null && 
					 (curPiece.Left == null || curPiece.Left.IsBlocked || curPiece.Left.IsOrphan)) || 
					 
					 (curPiece.BottomRightLink != null && !curPiece.BottomRightLink.IsBlocked && !curPiece.BottomRightLink.IsTileSpawner &&
					 curPiece.BottomRightLink.Tile == null && 
					 (curPiece.Right == null || curPiece.Right.IsBlocked || curPiece.Right.IsOrphan)) ) 
				{
					
//					if (curPiece.BoardPosition.row == 0) {
//						Debug.LogWarning("PassiveGravity started for: " + curPiece.name + "\n" +
//							"curPiece.BottomRightLink != null => " + (curPiece.BottomLeftLink != null) + "\n" +
//							"!curPiece.BottomRightLink.IsBlocked => " + (!curPiece.BottomRightLink.IsBlocked) + "\n" +
//							"curPiece.BottomRightLink.Tile == null => " + (curPiece.BottomRightLink.Tile == null) + "\n" + 
//							"curPiece.Right == null => " + (curPiece.Right == null) + "\n" +
//							"curPiece.Right.IsBlocked => " + (curPiece.Right.IsBlocked) + "\n" +
//							"curPiece.Right.IsOrphan => " + (curPiece.Right.IsOrphan) + "\n");
//						
//						Debug.Break();
//					}
					
					// Pass the control to the Active gravity checker.
					IsMoving = true;
					// Add the initial velocity to the current move velocity (that should be reset to zero when the tile finishes falling).
					moveVel += initialVel;
					tileMovedDiagonally = false;

					RaiseEventTileStartedActiveGravity();
					
					break;
				}
			}
			
			yield return null;
		}
		
		debugPassiveGravity = false;
	}
	
	/// <summary>
	/// Active gravity check coroutine. When it stops, it should go back up to passive gravity checking.
	/// </summary>
	/// <returns>
	/// The gravity check.
	/// </returns>
	protected IEnumerator ActiveGravityChecker() 
	{
		Match3BoardPiece curPiece = null;
		Match3BoardPiece nextPiece = null;
		Match3BoardPiece startingPiece = null;
		
		bool waitedForOtherTile = false;
		bool boardCoordUpdated;
		
		while(GravityUpdateEnabled) 
		{
			// Safety check block. (especially for when a tile might be removed from the board meaning it has no BoardPiece that it doesn't belong to it.
			if (!enabled || !GravityEnabled || BoardPiece == null) 
			{
				yield return null;
				continue;
			}

			debugActiveGravity = true;
			
			curPiece = BoardPiece as Match3BoardPiece;
			if (startingPiece == null) {
				startingPiece = curPiece;
			}
			
			fallDestination = curPiece.LocalPosition;
			boardCoordUpdated = false;
			waitedForOtherTile = false;
			bool hasReachedBoardPieceArea = HasReachedBoardPieceArea();
			
			// If the tile has previously moved diagonally, we must wait for it to reach it's new current board piece position first and then
			// continue to fall down vertically.
			if (/*hasReachedBoardPieceArea &&*/ curPiece.LockCount <= 0 && curPiece.BottomLink != null
				&& !curPiece.BottomLink.IsBlocked && (tileMovedDiagonally && hasReachedBoardPieceArea || !tileMovedDiagonally))
			{
				//TODO: to the same loop check like below for the diagonal tiles to fix the bug where 2 tiles one above the other can start to move diagonally at the same time
				if (curPiece.BottomLink.Tile == null)
				{
					nextPiece = curPiece.BottomLink;
					boardCoordUpdated = true;
					tileMovedDiagonally = false;
				}
				else if (curPiece.BottomLink.Tile.IsMoving && HasTileInArea(curPiece.BottomLink.Tile as Match3Tile)) 
				{
					// Adopt the velocity of the moving tile in front only if it's in the vicinity of this current tile.
					moveVel = (curPiece.BottomLink.Tile as Match3Tile).moveVel;					
				}
			}
			
			//TODO: separate and merge these repeating block of code in a nice method that can be called here and remove boilerplate code (when there is human time to do so).
			//Was left like this for easier debugging and code flow following.
			
			// Don't move sideways if we can already move vertically and until the tile has reached it's target fallDestination.
			if ( !boardCoordUpdated && hasReachedBoardPieceArea )
			{
				if (curPiece.BottomLeftLink != null && !curPiece.BottomLeftLink.IsBlocked && !curPiece.BottomLeftLink.IsTileSpawner)
				{
					if (curPiece.Left == null || curPiece.Left.IsBlocked || curPiece.Left.IsOrphan || curPiece.Left.IsTemporaryOrphan)	
					{
						if (curPiece.BottomLeftLink.Tile != null && curPiece.BottomLeftLink.Tile.IsMoving) 
						{
							float nextVel = initialVel;
							bool raisedTileStoppedMoving = false;
							
							do
							{
								if ( !raisedTileStoppedMoving && UpdateTilePhysics(curPiece) ) 
								{
									raisedTileStoppedMoving = true;
									TileStoppedMoving(ref startingPiece);
								}

								nextVel = (curPiece.BottomLeftLink.Tile as Match3Tile).moveVel;
								if (curPiece.BottomLeftLink.Tile.IsMoving) {
									moveVel = nextVel;
								}
								
								yield return null;
							} while(curPiece.BottomLeftLink.Tile != null && curPiece.BottomLeftLink.Tile.IsMoving);
							
							waitedForOtherTile = true;
							// Apply the move speed from the moving tile after waiting
							moveVel = nextVel;
						}

						if (curPiece.BottomLeftLink.Tile == null)
						{
							if (waitedForOtherTile) {
								moveVel = Mathf.Clamp(moveVel * (1f - velWaitDampFactor), initialVel, maxVel);
							}

							nextPiece = curPiece.BottomLeftLink;
							boardCoordUpdated = true;
							tileMovedDiagonally = true;
						}
						else if (waitedForOtherTile) {
							// The tile that we've been waiting for changed its status to IsMoving=false and it didn't clear the way for this tile
							// so we have to go back up and re-analyze the possible directions in which this tile can move.
							continue;
						}
					}
				}

				if (!boardCoordUpdated && curPiece.BottomRightLink != null && !curPiece.BottomRightLink.IsBlocked && !curPiece.BottomRightLink.IsTileSpawner)
				{
					if (curPiece.Right == null || curPiece.Right.IsBlocked || curPiece.Right.IsOrphan || curPiece.Right.IsTemporaryOrphan)
					{
						if (curPiece.BottomRightLink.Tile != null && curPiece.BottomRightLink.Tile.IsMoving)
						{
							float nextVel = initialVel;
							bool raisedTileStoppedMoving = false;

							do
							{
								if ( !raisedTileStoppedMoving && UpdateTilePhysics(curPiece) ) 
								{
									raisedTileStoppedMoving = true;
									TileStoppedMoving(ref startingPiece);
								}

								nextVel = (curPiece.BottomRightLink.Tile as Match3Tile).moveVel;
								if (curPiece.BottomRightLink.Tile.IsMoving) {
									moveVel = nextVel;
								}

								yield return null;
							} while(curPiece.BottomRightLink.Tile != null && curPiece.BottomRightLink.Tile.IsMoving);
							
							waitedForOtherTile = true;
							// Apply the move speed from the moving tile after waiting
							moveVel = nextVel;
						}
						
						if (curPiece.BottomRightLink.Tile == null)
						{
							if (waitedForOtherTile) {
								// Apply velocity damping for tiles that waited last time
								moveVel = Mathf.Clamp(moveVel * (1f - velWaitDampFactor), initialVel, maxVel);
							}
							
							nextPiece = curPiece.BottomRightLink;
							boardCoordUpdated = true;
							tileMovedDiagonally = true;
						}
						else if (waitedForOtherTile) {
							// The tile that we've been waiting for changed its status to IsMoving=false and it didn't clear the way for this tile
							// so we have to go back up and re-analyze the possible directions in which this tile can move.
							continue;
						}
					}
				}
			}
			
			if (boardCoordUpdated) 
			{
				fallDestination = nextPiece.LocalPosition;
				curPiece.MoveTileTo(nextPiece);
				curPiece.UpdateOrphanState();
			}
			
			bool hasTileReachedFallDestination = UpdateTilePhysics(curPiece);

			// Check gravity stop condition
			if ( !boardCoordUpdated && hasTileReachedFallDestination &&
					(nextPiece == null || (nextPiece.Tile != null && !nextPiece.Tile.IsMoving) || 
					curPiece == nextPiece || curPiece.LockCount > 0) )
			{
				// Attach this tile to its current board piece. (correctly updating all other internal states of the board piece)
				curPiece.Tile = this;

				// Set the local position of this tile to the local position of it's new owner board piece
				curPiece.ResetTilePosition();

				TileStoppedMoving(ref startingPiece);
				
				RaiseEventTileFinishedActiveGravity(startingPiece);
				
				// Reset the current move velocity of the tile
				moveVel = 0f;
				
				break;
			}

			yield return null;
		}
		
		debugActiveGravity = false;
	}
	
	protected virtual void TileStoppedMoving(ref Match3BoardPiece startingPiece) { }
	
	public bool UpdateTilePhysics(Match3BoardPiece curPiece) {
		// Update tile physics (apply gravity).
//			if (!waitingForOtherTile) {
		UpdateFallingTilePosition(curPiece);

		bool hasTileReachedFallDestination = HasTileReachedFallDestination(ref fallDestination);
		if ( !hasTileReachedFallDestination ) {
			// Update and limit tile maximum speed.
			moveVel = Mathf.Min(moveVel + accel * Time.smoothDeltaTime, maxVel);
		}
		else {
			LocalPosition = fallDestination;
		}
		
		return hasTileReachedFallDestination;
	}
	
	/// <summary>
	/// Updates the falling tile position.
	/// </summary>
	/// <returns>
	/// The distance between the tile and the current fall destination.
	/// </returns>
	/// <param name='destination'>
	/// Destination.
	/// </param>
	protected void UpdateFallingTilePosition(Match3BoardPiece curPiece) 
	{
		// Update fall direction
		moveDir = (fallDestination - LocalPosition).normalized;
		
		// Calculate and apply displacement.
		LocalPosition += moveDir * moveVel * Time.deltaTime;
	}
	
	protected bool HasTileReachedFallDestination(ref Vector3 destination) {
		Vector3 moveHeading = destination - LocalPosition;
		
		return LocalPosition == destination || Vector3.Dot(moveHeading, moveDir) <= 0f;
	}
	
	public virtual void RaiseEventTileStartedActiveGravity() {
		if (OnTileStartedActiveGravity != null) {
			OnTileStartedActiveGravity(this);
		}
	}
	
	/// <summary>
	/// Raises the tile finished active gravity event.
	/// This event should set the "IsMoving" property to false when it finishes any tile bounce animation.
	/// </summary>
	public virtual void RaiseEventTileFinishedActiveGravity (Match3BoardPiece startingBoardPiece) 
	{
		if (OnTileFinishedActiveGravity != null) {
			OnTileFinishedActiveGravity(this, startingBoardPiece);
		}
	}
	
	public void AddScore() 
	{
		if (addedScore || Points == 0) {
			return;
		}
		
		addedScore = true;
		
		int earnedPoints = ScoreSystem.Instance.AddScore(Points);
		/* 
		if(ScoreSpawner.instance) {
			ScoreSpawner.instance.SpawnScore(WorldPosition, earnedPoints);
		}
		*/
	}
	
	protected override void TileDestroy(bool useEffect) 
	{		
		AddScore();
		
		base.TileDestroy(useEffect);
	}
	
	public override string ToString ()
	{
		return string.Format ("[" + this.GetType().ToString() + ": IsMoveable={0}, " +
			"TileColor={1}, IsDestructible={2}, IsDestroying={3}, IsMoving={4}, BoardCoord={5}]", 
			IsUserMoveable, TileColor, IsDestructible, IsDestroying, IsMoving, BoardPiece != null ? BoardPiece.BoardPosition.ToString() : "");
	}
}
