using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Match3BoardPiece : AbstractBoardPiece {
	public enum LinkType {
		Top = 0,
		TopRight, 
		Right,
		BottomRight,
		Bottom,
		BottomLeft,
		Left,
		TopLeft,
		Count,
	}

	public static BoardCoord[] linkOffsets = new BoardCoord[(int)LinkType.Count] {
													new BoardCoord(-1, 0),	 // Top
													new BoardCoord(-1, 1),	 // TopRight
													new BoardCoord(0, 1),	 // Right
													new BoardCoord(1, 1),	 // BottomRight
													new BoardCoord(1, 0),	 // Bottom
													new BoardCoord(1, -1),	 // BottomLeft
													new BoardCoord(0, -1),	 // Left
													new BoardCoord(-1, -1),	 // TopLeft
												};
			
	public bool canDrawGizmos = true;
	
#if UNITY_EDITOR
	/// <summary>
	/// The editor board position used only by the level editor. 
	/// This property is accordingly set by the level editor.
	/// </summary>
	public BoardCoord editorBoardPos;
#endif
	
	/// <summary>
	/// The initial tile spawn rule that this board piece will have the first time the level starts. 
	/// This can make it have an initial random tile or for example not have an initial tile at all and let the tile spawners populate the level.
	/// </summary>
	public TileSpawnRule initialTileSpawnRule = new TileSpawnRule();

	/// <summary>
	/// A board piece is blocked if the tile in it is not moveable or if specified by level design some empty board pieces can be set as being blocked.
	/// This property is not a level design property. It's a board functionality property. It's dynamically changed when a new tile is set on the board piece.
	/// </summary>
	private bool isBlocked = false;
		
	/// <summary>
	/// A bord piece will set itself as being orphan if it doesn't have a tile on it and it can't receive new tiles from the 
	/// TopLeftLink, TopLink, TopRightLink because they are blocked or also orphan. When a board piece will set itself to be 
	/// orphan it will also notify the board pieces below it to check if they should be orphan also. (from BottomLeftLink, BottomLink, BottomRightLink)
	/// This is dynamically changed property. It's not a level design property.
	/// </summary>
	private bool isOrphan = false;
	
	private bool isTemporaryOrphan = false;
	
	/// <summary>
	/// Inidicates if this board piece allows tiles to fall through. Mainly used for board pieces that have the "IsEmpty" property set that can sometime allow tiles to fall
	/// through them or not depending on level design. But it can theoretically be used for other board pieces that can block tiles inside them by design.
	/// </summary>
	[SerializeField]
	private bool allowTileFalling = true;

	/// <summary>
	/// The lock count indicates when any external effect or power or anything can make this board piece temporary blocked by increasing the lock count.
	/// When the lockCount > 0 the board piece is not considered blocked. (<see cref="IsBlocked"/> property will return false) Because it should release the lock
	/// immediately after the animations finished with it.
	/// </summary>
	private int lockCount = 0;
	
	
	private int blockCount = 0;
		
	[SerializeField]
	private bool isTileSpawner = false;

	/// <summary>
	/// Stores game logic links between board pieces. For example a piece that has to fall over some empty board pieces, it will have the bottom link
	/// to the board piece passed those empty ones. These logical links can be null because of various game logic reasons or by level design.
	/// </summary>
	public Match3BoardPiece[] links;

	/// <summary>
	/// Stores the immediate neighbors of each board piece. (like a linked list) Useful for faster iterative access to neighbor board pieces than doing
	/// board table lookups. Neighbors can be null only for pieces at the border of the level.
	/// </summary>
	[System.NonSerialized]
	public Match3BoardPiece[] neighbors = new Match3BoardPiece[(int)LinkType.Count];

	/// <summary>
	/// If set to false you must manually set the links for the board piece, otherwise it they will be auto-determined by
	/// the method <see cref="UpdateLinks"/>. (if false, allows for custom crazy board behavior designs...theoretically :D)
	/// </summary>
	public bool autoDetermineLinks = true;
	
	protected bool isOrphaneStateLateUpdatePending = false;
		
	//TODO: Add comment (Mio)
	public List<RuleEntry> eligibleSpawnList;
	

	public override void Awake ()
	{
        base.Awake ();
		
		// Apply value currently set on the prefab first.
		IsTileSpawner = isTileSpawner;
		
		if (links == null || links.Length < (int)LinkType.Count) {
            links = new Match3BoardPiece[(int)LinkType.Count];
            System.Console.WriteLine("Match3BoardPiece->" + name + "--awake->" + links.Length);
        }
		
		if(eligibleSpawnList == null)
		{
			eligibleSpawnList = new List<RuleEntry>(2);
//			eligibleSpawnList.Add(new RuleEntry(TileSpawnType.NormalTile));
		}
		
	}

#region Editor methods
#if UNITY_EDITOR
	// Editor only properties
	public string editorSceneLabel = "";
	
	public static Texture2D MakeTex(int width, int height, Color col) {

        Color[] pix = new Color[width*height];

        for(int i = 0; i < pix.Length; i++) {
            pix[i] = col;
		}
        Texture2D result = new Texture2D(width, height);

        result.SetPixels(pix);
        result.Apply();
		
        return result;

    }

	public Match3Tile EditorTile {
		get {
			return tile as Match3Tile;
		}
		set {
			tile = value as Match3Tile;
		}
	}	
#endif
	
#endregion
		
	public override AbstractTile Tile {
		set {
			bool hadTileRef = base.Tile != null;
			base.Tile = value;

			if (value != null) {
				// Consider this board piece as being blocked if it has a tile without gravity and not moveable by the user.
				IsBlocked = !(value as Match3Tile).GravityUpdateEnabled && !value.IsUserMoveable;
			} else if (hadTileRef) {
				// Set the board piece flag IsBlocked to false only when we previously had a tile on it. 
				IsBlocked = false;
			}
		}
	}
	
	public bool IsBlocked {
		get {
			return isBlocked || IsBorderPiece || !AllowTileFalling || BlockCount > 0;
		}
		set {
			bool lastIsBlocked = isBlocked;
			isBlocked = value;
			
			//TODO: (floky) this piece of code below is going to bite our asses later:D in some levels where logical links between board pieces are switched or teleportation
			// is implemented. It's wrong to assume what other board pieces should get blocked if this board piece is locked.
			// This code might not even be necessarry because of the UpdateOrphanState() method and because that method updates a more important property: IsTemporaryOrphan.
			
			// If this is not an empty bloard piece, update "IsBlocked" flag for all bottom empty board pieces. (not for all board pieces)
			// We ignore this step for empty pieces because they must get updated only by top non-empty blocked neighbor board pieces.
			if ( !IsEmpty && lastIsBlocked != isBlocked) {
				Match3BoardPiece nextPiece = Bottom;
				while(nextPiece != null && nextPiece.IsEmpty) {
					nextPiece.IsBlocked = value;
					nextPiece = nextPiece.Bottom;
				}
			}
		}
	}
	
	public bool IsOrphan {
		get {
			return isOrphan;
		}
		set {
			isOrphan = value;
		}
	}
	
	public bool IsTemporaryOrphan {
		get {
			return isTemporaryOrphan;
		}
		set {
			isTemporaryOrphan = value;
		}
	}	

	public bool AllowTileFalling {
		get {
			return allowTileFalling;
		}
		set {
			allowTileFalling = value;
		}
	}

	public int LockCount {
		get {
			return lockCount;
		}
		set {
			lockCount = value;
		}
	}
	
	public int BlockCount {
		get {
			return blockCount;
		}
		set {
			blockCount = value;
		}
	}
	
	public override bool IsEmpty {
		set {
			base.IsEmpty = value;
			
			if (value) {
				// An empty board piece can't be a tile spawner or a border piece at the same time.
				IsTileSpawner = false;
				IsBorderPiece = false;
			}
		}
	}
	
	public override bool IsBorderPiece {
		set {
			// A border piece can't be an empty board piece at the same time. (the base implementation takes care of that)
			base.IsBorderPiece = value;
			
			if (value) {
				// A border piece can't be a tile spawner at the same time.
				IsTileSpawner = false;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this board piece is a tile spawner board piece. (above it new tiles will spawn).
	/// It will self register/unregister in the list of tile spawners from the <see cref="Match3BoardGameLogic"/>.
	/// </summary>
	/// <value>
	/// <c>true</c> if this instance is tile spawner; otherwise, <c>false</c>.
	/// </value>
	public bool IsTileSpawner {
		get {
			return isTileSpawner;
		}
		set {
			isTileSpawner = value;
			int registeredAtIdx = Match3BoardGameLogic.Instance.tileSpawners.IndexOf(this);
			if (isTileSpawner && registeredAtIdx < 0) 
			{
//				Debug.Log("[Match3BoardPiece] Registered " + name + " to game logic tile spawner list...");
				Match3BoardGameLogic.Instance.tileSpawners.Add(this);
			} 
			else if (!isTileSpawner && registeredAtIdx >= 0) 
			{
//				Debug.Log("[Match3BoardPiece] Unregistered " + name + " from game logic tile spawner list...");
				Match3BoardGameLogic.Instance.tileSpawners.RemoveAt(registeredAtIdx);
			}
		}
	}
	
//	public override void RaiseTileChangedEvent (AbstractTile newTile) {
//		base.RaiseTileChangedEvent (newTile);
//		
//		LateUpdateOrphanState();
//	}
	
//	public void LateUpdateOrphanState() {
//		if (isOrphaneStateLateUpdatePending) {
//			return;
//		}
////		Debug.LogWarning(name + " LateUpdateOrphanState() pending...");
//		isOrphaneStateLateUpdatePending = true;
//		StartCoroutine(WaitForLateUpdateOrphanState());
//	}
			
//	protected IEnumerator WaitForLateUpdateOrphanState() {
//		yield return Match3Globals.Instance.waitEndFrame;
//		
//		UpdateOrphanState();
//		isOrphaneStateLateUpdatePending = false;
//	}
		
	/// <summary>
	/// Updates the "IsOrphan" state of this board piece.
	/// </summary>
	public void UpdateOrphanState() {
		if (Tile == null && !IsTileSpawner &&
			(TopLink == null || TopLink.IsBlocked || TopLink.IsOrphan || TopLink.IsTemporaryOrphan))
		{
			IsTemporaryOrphan = true;
		} else {
			IsTemporaryOrphan = false;
		}

		// Check and update orphan state
//		Debug.LogWarning(name + " Executed LateUpdateOrphanState()...");
		if (Tile == null && !IsTileSpawner &&
			(TopLeftLink == null || TopLeftLink.IsBlocked || TopLeftLink.IsOrphan) &&
			(TopLink == null || TopLink.IsBlocked || TopLink.IsOrphan) &&
			(TopRightLink == null || TopRightLink.IsBlocked || TopRightLink.IsOrphan)) 
		{
			IsOrphan = true;
		} else {
			IsOrphan = false;
		}		
	}
	
	public void ExecuteInitialSpawnRule() {
		if (initialTileSpawnRule != null)
		{
			List<RuleEntry> tempRuleEntryList;
			List<TileSpawnRule> immediateSpawnList = Match3BoardGameLogic.Instance.immediateSpawnList;
			
			//Check immediateSpawnList for available rules that can be applied, with respect to the eligibleSpawnList.
			for(int i = 0; i < immediateSpawnList.Count; i++)
			{
				if(eligibleSpawnList != null && eligibleSpawnList.Count > 0) 
				{
					tempRuleEntryList = immediateSpawnList[i].CompareRuleEntries(eligibleSpawnList);
					immediateSpawnList.RemoveAt(i);
				}
				else
				{
					tempRuleEntryList = immediateSpawnList[i].ruleEntries;
				}
				
				if(tempRuleEntryList != null)
				{
					initialTileSpawnRule.ruleEntries = tempRuleEntryList;
					break;
				}
			}
			
			// Execute the initial spawn rule only if we have at least one RuleEntry.
			if (initialTileSpawnRule.ruleEntries.Count > 0)
			{
				Match3Tile newTile = initialTileSpawnRule.SpawnNewTile(true);
				Match3BoardRenderer.Instance.AttachTileToBoardAt(this, newTile, false, true);
			}
		}
	}

#region Accessors for board piece neighbors	
	public Match3BoardPiece GetNeighbor(LinkType neighborType) {
		return neighbors[(int)neighborType];
	}
	
	public void SetNeighbor(LinkType neighborType, Match3BoardPiece newNeighbor) {
		neighbors[(int)neighborType] = newNeighbor;
	}
	
	public Match3BoardPiece Top {
		get {
			return neighbors[(int)LinkType.Top];
		}
		set {
			neighbors[(int)LinkType.Top] = value;
		}
	}

	public Match3BoardPiece TopRight {
		get {
			return neighbors[(int)LinkType.TopRight];
		}
		set {
			neighbors[(int)LinkType.TopRight] = value;
		}
	}
	
	public Match3BoardPiece Right {
		get {
			return neighbors[(int)LinkType.Right];
		}
		set {
			neighbors[(int)LinkType.Right] = value;
		}
	}
	
	public Match3BoardPiece BottomRight {
		get {
			return neighbors[(int)LinkType.BottomRight];
		}
		set {
			neighbors[(int)LinkType.BottomRight] = value;
		}
	}
		
	public Match3BoardPiece Bottom {
		get {
			return neighbors[(int)LinkType.Bottom];
		}
		set {
			neighbors[(int)LinkType.Bottom] = value;
		}
	}

	public Match3BoardPiece BottomLeft {
		get {
			return neighbors[(int)LinkType.BottomLeft];
		}
		set {
			neighbors[(int)LinkType.BottomLeft] = value;
		}
	}
	
	public Match3BoardPiece Left {
		get {
			return neighbors[(int)LinkType.Left];
		}
		set {
			neighbors[(int)LinkType.Left] = value;
		}
	}
	
	public Match3BoardPiece TopLeft {
		get {
			return neighbors[(int)LinkType.TopLeft];
		}
		set {
			neighbors[(int)LinkType.TopLeft] = value;
		}
	}
#endregion
	
#region Accessors for board piece links
	public Match3BoardPiece GetLink(LinkType linkType) {
		return links[(int)linkType];
	}
	
	public void SetLink(LinkType linkType, Match3BoardPiece newLink) {
		links[(int)linkType] = newLink;
	}

	public Match3BoardPiece TopLink {
		get {
            System.Console.WriteLine("Match3BoardPiece->" + name + "->TopLink->" + links.Length + "--" + (int)LinkType.Top);
            return links[(int)LinkType.Top];
		}
		set {
			links[(int)LinkType.Top] = value;
		}
	}
	
	public Match3BoardPiece TopRightLink {
		get {
			return links[(int)LinkType.TopRight];
		}
		set {
			links[(int)LinkType.TopRight] = value;
		}
	}
	
	public Match3BoardPiece RightLink {
		get {
			return links[(int)LinkType.Right];
		}
		set {
			links[(int)LinkType.Right] = value;
		}
	}
	
	public Match3BoardPiece BottomRightLink {
		get {
			return links[(int)LinkType.BottomRight];
		}
		set {
			links[(int)LinkType.BottomRight] = value;
		}
	}
		
	public Match3BoardPiece BottomLink {
		get {
			return links[(int)LinkType.Bottom];
		}
		set {
			links[(int)LinkType.Bottom] = value;
		}
	}

	public Match3BoardPiece BottomLeftLink {
		get {
			return links[(int)LinkType.BottomLeft];
		}
		set {
			links[(int)LinkType.BottomLeft] = value;
		}
	}
	
	public Match3BoardPiece LeftLink {
		get {
			return links[(int)LinkType.Left];
		}
		set {
			links[(int)LinkType.Left] = value;
		}
	}
	
	public Match3BoardPiece TopLeftLink {
		get {
			return links[(int)LinkType.TopLeft];
		}
		set {
			links[(int)LinkType.TopLeft] = value;
		}
	}
	#endregion

	#region linktype_utilities
	public static LinkType GetLinkTypeFromMoveDirection(TileMoveDirection direction)
	{
		switch(direction)
		{
		case TileMoveDirection.Top:
			return LinkType.Top;
		case TileMoveDirection.Left:
			return LinkType.Left;
		case TileMoveDirection.Right:
			return LinkType.Right;
		case TileMoveDirection.Bottom:
			return LinkType.Bottom;
		default:
			return LinkType.Count;
		}
	}

	public static LinkType GetOppositeLinkType(Match3BoardPiece.LinkType linkType)
	{
		switch(linkType)
		{
		case Match3BoardPiece.LinkType.Top:
			return LinkType.Bottom;
		case Match3BoardPiece.LinkType.Left:
			return LinkType.Right;
		case Match3BoardPiece.LinkType.Right:
			return LinkType.Left;
		case Match3BoardPiece.LinkType.Bottom:
			return LinkType.Top;
		default:
			return Match3BoardPiece.LinkType.Count;
		}
	}
	#endregion
	/// <summary>
	/// Gets a value indicating whether this instance can be link for other board pieces or not.
	/// Override this for other types of Match3BoardPieces for custom behavior.
	/// </summary>
	/// <value>
	/// <c>true</c> if this instance can be link; otherwise, <c>false</c>.
	/// </value>
	public virtual bool CanBeLink {
		get {
			return !IsEmpty && !IsBorderPiece;
		}
	}

	/// <summary>
	/// Gets the link piece from the specified direction.
	/// </summary>
	/// <returns>
	/// The link piece.
	/// </returns>
	/// <param name='linkType'>
	/// Link type.
	/// </param>
	/// <param name='applyFilter'>
	/// Apply filter indicates if <see cref="CanBeLink"/> will be checked before returning the link.
	/// If "applyFilter" is true and the link board piece "CanBeLink" property returns false then "null" will be returned.
	/// </param>
	protected Match3BoardPiece GetLinkPiece(LinkType linkType, bool applyFilter = false) {
		Match3BoardPiece link = null;
		BoardCoord curPos = BoardPosition;
		curPos.OffsetBy( Match3BoardPiece.linkOffsets[(int)linkType] );
		if (curPos.row >= Board.NumRows || curPos.col >= Board.NumColumns || curPos.row < 0 || curPos.col < 0) {
			return null;
		}
		link = Board[curPos] as Match3BoardPiece;

		if (applyFilter) {
			return link.CanBeLink ? link : null;
		} else {
			return link;
		}
	}
	
	/// <summary>
	/// Updates the neighbors.
	/// Note: Should ONLY be called after the board is initialized and all board pieces are set up.
	/// </summary>
	public void UpdateNeighbors()
	{
		for(int i = 0; i < neighbors.Length; i++)
		{
			BoardCoord offsetPos = BoardPosition + Match3BoardPiece.linkOffsets[i];
			
			if (offsetPos.row >= Board.NumRows || offsetPos.col >= Board.NumColumns || offsetPos.row < 0 || offsetPos.col < 0)  {
				neighbors[i] = null;
			}
			else {
				neighbors[i] = Board[offsetPos] as Match3BoardPiece;
			}
		}
	}
	
	/// <summary>
	/// Copies the links from this board piece to a designated target board piece.
	/// </summary>
	public void CopyLinksTo(Match3BoardPiece target)
	{
		for(int i = 0; i < (int)LinkType.Count; i++) {
			target.links[i] = links[i];
		}
	}
	
	/// <summary>
	/// Updates all the links.
	/// This method is called only for board pieces that have <see cref="autoDetermineLinks"/> set to true.
	/// Note: Should ONLY be called after the board is initialized and all board pieces are set up.
	/// </summary>
	public void UpdateLinks() 
	{
        System.Console.WriteLine("Match3BoardPiece->UpdateLinks");
        // If the board piece can't be a link piece to someone else then it doesn't have any linked board pieces to it.
        if ( !CanBeLink ) 
		{
			if (IsEmpty) 
			{
				if (AllowTileFalling) 
				{
					if (Top != null) {
						TopLink = Top;
					}
					if (Bottom != null) {
						BottomLink = Bottom;
					}
				} 
				else 
				{
					TopLink = null;
					BottomLink = null;
					IsBlocked = true;
					
					// Cut the links from the top and bottom pieces above the current empty board piece
					if (Top != null) {
						Top.BottomLink = null;
					}
					if (Bottom != null) {
						Bottom.TopLink = null;
					}
				}
			}

			return;
		}
		
		// Get immediate links for this board piece. Links for the max border pieces (pieces near the NumRows, NumCols of the board) will be "null".
		// Don't change any links previously set by level design.
		if (TopLink == null) {
			TopLink = GetLinkPiece(LinkType.Top, true);
		}
		if (TopRightLink == null) {
			TopRightLink = GetLinkPiece(LinkType.TopRight, true);
		}
		if (RightLink == null) {
			RightLink = GetLinkPiece(LinkType.Right, true);
		}
		if (BottomRightLink == null) {
			BottomRightLink = GetLinkPiece(LinkType.BottomRight, true);
		}
		if (BottomLeftLink == null) {
			BottomLeftLink = GetLinkPiece(LinkType.BottomLeft, true);
		}
		if (LeftLink == null) {
			LeftLink = GetLinkPiece(LinkType.Left, true);
		}
		if (TopLeftLink == null) {
			TopLeftLink = GetLinkPiece(LinkType.TopLeft, true);
		}

		// Get the immediate bottom link piece wihtout applying the "CanBeLink" filter to it.
		if (BottomLink == null) 
		{
			BottomLink = GetLinkPiece(LinkType.Bottom);
			
			// Look iterativelly on the bottom for a board piece that "CanBeLink". (not empty or not a border piece)
			//TODO: revise this piece of code (hurried implementation by a build making for disney)
			while(BottomLink != null && !BottomLink.IsBorderPiece) 
			{
				if (BottomLink.IsEmpty) 
				{
					if (!BottomLink.AllowTileFalling) 
					{
						// This board piece can't have bottom link because we've reached an empty piece that doesn't allow tile falling through.
						BottomLink = null;
						break;
					}
					// Continue to the next empty tile because this one allows tile falling
				} 
				else if (BottomLink.CanBeLink) {
					// We found a valid BottomLink for this board piece
					break;
				} 
				else 
				{
					// We found a piece that isn't empty or it can't be a link piece stop here.
					BottomLink = null;
					break;
				}

				// Go to the next bottom tile
				BottomLink = BottomLink.GetLinkPiece(LinkType.Bottom);
			}

			if (BottomLink != null && BottomLink.IsBorderPiece) {
				BottomLink = null;
			}
			
			// Set the reverse link also
			if (BottomLink != null) {
				BottomLink.TopLink = this;
			}
		}
	}	
	
	void LateUpdate()
	{
		UpdateOrphanState();
	}
}
