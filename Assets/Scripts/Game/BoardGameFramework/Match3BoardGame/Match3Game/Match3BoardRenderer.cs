using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Board renderer spawns the corresponding board tiles and positions them accordingly.
/// Also sets up board pieces graphics/effects according to the info from the BoardData component.
/// </summary>
[RequireComponent(typeof(BoardData))]
public class Match3BoardRenderer : AbstractBoardRenderer {
	public static event System.Action OnDetermineBoardLinksAndNeighbors;
	public static event System.Action OnRandomGenericColorInitialized;
	
	private static Match3BoardRenderer instance;
	
	public static string baseLevelPathName = "Game/Levels/Level_Maleficent";
	
	// Static board renderer properties that are initialized at run-time and used for faster access throughout the code.
	public static int maxNumBoardColors;
	
	// Immediate spawn rules are to be processed only once ever [var] seconds
	public static float minDelayForceSpawnRules = 1f;
	
	public static float horizTileDistance;
	public static float horizTileOffset;
	public static float vertTileDistance;
	public static float sqrVertTileDistance;
//	public static float sqrAproxTileDistance;
	public static float vertTileOffset;
	
	public static float maxBoardRowSize;
	public static float maxBoardColSize;
	public static float halfHorizTileDistance;
	public static float halfVertTileDistance;
	public static int boardYDirectionSign;
	
	/// <summary>
	/// The win/lose conditions for the current board setup. 
	/// When the level is loaded by <see cref="Match3BoardGameLogic"/> these will be linked to the game logic.
	/// </summary>
	public AbstractWinCondition winConditions;
	public AbstractLoseCondition loseConditions;
	
	/// <summary>
	/// The index of the level background that will be loaded for this board setup.
	/// On Awake() this index will be set on the <see cref="BackgroundLoader"/> class.
	/// </summary>
	//public int levelBackgroundIndex = 0;
		
	/// <summary>
	/// The maximum number of board colors the current level configuration will have.
	/// </summary>
	public int maximumNumBoardColors = (int)GenericColorType.Count;
		
	public Dictionary<System.Type, Match3BoardPiece> piecesDictionary;
	public TilesDictionary tilesDictionary;
	
	public float horizontalTileDistance = 1.008f;
	public float horizontalTileOffset = 0f;
	public float verticalTileDistance = 1.008f;
	public float verticalTileOffset = 0f;
	
	/// <summary>
	/// If it's true a random board configuration of random colored tiles will generated (this was used for early tests only).
	/// If it's false this GameObject should contain the child board pieces and tiles which from which the renderer will build it's board data config.
	/// </summary>
	public bool autoGenerateRandom = true;
	
	/// <summary>
	/// Use editor mode to setup the tiles on the board correctly spaced.
	/// You can enable this flag at run-time in the editor and change the tile positioning params
	/// like <see cref="horizTileDistance"/> or <see cref="vertTileDistance"/>.
	/// </summary>
	public bool useEditorMode = false;
		
//	[System.NonSerialized]
//	public string boardBgGameObjName = "Match3Board/BoardVisuals/BoardBackground";
	
//	protected GameObject boardBackground;

	
	protected override void Awake () {
        System.Console.WriteLine("Match3BoardRenderer -> Awake->");
        base.Awake ();
		instance = this;
				
		// Initialize static board renderer properties for easier access
		maxNumBoardColors = maximumNumBoardColors;
		
		Match3BoardRenderer.horizTileOffset = horizontalTileOffset;
		Match3BoardRenderer.horizTileDistance = horizontalTileDistance;
		Match3BoardRenderer.vertTileOffset = verticalTileOffset;
		Match3BoardRenderer.vertTileDistance = verticalTileDistance;
		
		// Pre-calculate some in-game required board renderer stuff.
		Match3BoardRenderer.halfHorizTileDistance = horizontalTileDistance * 0.5f;
		Match3BoardRenderer.halfVertTileDistance = verticalTileDistance * 0.5f;
		Match3BoardRenderer.boardYDirectionSign = -(int)Mathf.Sign(cachedTransform.up.y);
		Match3BoardRenderer.maxBoardColSize = horizontalTileDistance * Board.NumColumns;
		Match3BoardRenderer.maxBoardRowSize = verticalTileDistance * Board.NumRows;
		
		Match3BoardRenderer.sqrVertTileDistance = Match3BoardRenderer.vertTileDistance * Match3BoardRenderer.vertTileDistance;
//		Match3BoardRenderer.sqrAproxTileDistance = (Match3BoardRenderer.vertTileDistance + Match3BoardRenderer.halfVertTileDistance * 0.5f) *
//												   (Match3BoardRenderer.vertTileDistance + Match3BoardRenderer.halfVertTileDistance * 0.5f);
	}
	
	public static Match3BoardRenderer Instance 
	{
		get {
            System.Console.WriteLine("Match3BoardRenderer -> get Instance->");
            return instance;
		}
	}

	void OnDestroy() {
        System.Console.WriteLine("Match3BoardRenderer -> destroy Instance->");

        instance = null;
	}
	
	/// <summary>
	/// Initializes the random generic colors list from the <see cref="RuleEntry"/> class to allow
	/// <see cref="TileSpawnRule"/> classes to correctly pick unique random generic colors each time the level is restarted.
	/// </summary>
	public void InitializeRandomGenericColors() 
	{
		System.Console.WriteLine("Match3BoardRenderer -> InitializeRandomGenericColors->");
		// Get the needed colors from the win condition
		List<TileColorType> neededColors = new List<TileColorType>();
		WinDestroyTiles winDestroy = winConditions as WinDestroyTiles;
		if (winDestroy) {
			foreach (DestroyTilesPair pair in winDestroy.destroyTiles) {
				if (pair.type.TileColor != TileColorType.None) {
					neededColors.Add(pair.type.TileColor);
				}
			}
		}
		
		// Init colors bag with all colors.
		List<TileColorType> allColors = new List<TileColorType>((int)TileColorType.Count - 1);
		for(int i = 1; i < (int)TileColorType.Count; i++) {
			if (neededColors.Contains((TileColorType)i)) {
				allColors.Insert( 0, (TileColorType)i );
			}
			else {
				allColors.Add( (TileColorType)i );
			}
		}
		
		
		// Shuffle the colors list.
		for(int i = 0; i < allColors.Count; i++) {
			int randomIdx;
			if (i < neededColors.Count) {
				randomIdx = Random.Range(0, neededColors.Count);
			}
			else {
				randomIdx = Random.Range(neededColors.Count, allColors.Count);
			}
			// Switch current color position to new randomIdx.
			TileColorType curColor = allColors[i];
			allColors[i] = allColors[randomIdx];
			allColors[randomIdx] = curColor;
		}
		
		// Initialize the generic colors list
		for(int i = 0; i < (int)GenericColorType.Count; i++) {
			RuleEntry.genericColors[i] = allColors[i];
		}

		if(OnRandomGenericColorInitialized != null) {
			OnRandomGenericColorInitialized();
		}
	}
	
#region Overridden methods from AbstractBoardRenderer
	public override void InitComponent ()
	{	
		System.Console.WriteLine("Match3BoardRenderer -> InitComponent->");
		InitializeRandomGenericColors();
		base.InitComponent();
	}
	
	public override void RaiseBoardStartedSetupEvent () 
	{
		System.Console.WriteLine("Match3BoardRenderer -> RaiseBoardStartedSetupEvent->");
		base.RaiseBoardStartedSetupEvent ();
		
		// Setup board pieces prefabs
		piecesDictionary = new Dictionary<System.Type, Match3BoardPiece>();
		for(int i = 0; i < prefabsPieces.Length; i++) {
			Match3BoardPiece boardPiece = prefabsPieces[i].GetComponent<Match3BoardPiece>();
			piecesDictionary.Add(boardPiece.GetType(), boardPiece);
            //System.Console.WriteLine("User Name is {0} --->The id is {1}", boardPiece.GetType().ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            //Debug.Log(" Match3BoardRenderer -> RaiseBoardStartedSetupEvent " + boardPiece.GetType().ToString() + "---" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }
        autoGenerateRandom = false;

        if (autoGenerateRandom) {
			GenerateRandomBoard(Board.NumRows, Board.NumColumns);
		} else {
			LoadBoardSetupFromHierarchy();
		}
	}

    //	public override void RaiseBoardFinishedSetupEvent ()
    //	{
    //		base.RaiseBoardFinishedSetupEvent();
    //		
    //		BoardTilesInitializer initializer = new BoardTilesInitializer(Board);
    //		initializer.Initialize();
    //	}

    /// <summary>
    /// Setup the board tiles prefabs specified in the <see cref="tilesPrefabs"/> array,
    /// based on their type and color for quick access at run-time.
    /// </summary>
    public override void SetupBoardTiles() {
        System.Console.WriteLine("Match3BoardRenderer -> SetupBoardTiles->");
        tilesDictionary = new TilesDictionary();

        for (int i = 0; i < tilesPrefabs.Length; i++) {
            Match3Tile tile = tilesPrefabs[i].GetComponent<Match3Tile>();
			tilesDictionary[tile.GetType(), tile.TileColor] = tile;
            System.Console.WriteLine("Match3BoardRenderer -> SetupBoardTiles->" + i + "--" + tile.GetType() + "--" + tile.TileColor.ToString());

        }
    }

	protected override void SetupBoardRendering() {
        System.Console.WriteLine("Match3BoardRenderer -> SetupBoardRendering");
        if (null == tilesDictionary)
            System.Console.WriteLine("SetupBoardRendering SpawnSpecificTile -> is null");
        else
            System.Console.WriteLine("SetupBoardRendering  SpawnSpecificTile-> is not null");
        Vector3 boardPiecePos = Vector3.zero;
		
		if (autoGenerateRandom) {
			Board.ApplyActionToRow(0, (boardPiece) => { 
	//			(boardPiece as Match3BoardPiece).IsTileSpawner = true;
				boardPiece.gameObject.AddComponent<TileSpawnerBehavior>();
			});

			//TODO: ======BEGIN TESTING GROUND FOR CODE-GENERATED BOARD CONFIGURATIONS======
	//		BombTile bombTile = SpawnSpecificTileAt(1, 3, typeof(BombTile), TileColorType.None, false) as BombTile;
	//		bombTile.TileColor = TileColorType.Orange;
	//		bombTile.UpdateMaterial();
	//
	//		RowDestroyTile rowTile = SpawnSpecificTileAt(2, 3, typeof(RowDestroyTile), TileColorType.None, false) as RowDestroyTile;
	//		rowTile.TileColor = TileColorType.Blue;
	//		rowTile.UpdateMaterial();
			if (MaleficentBlackboard.Instance.level == 0) {
				SpawnBoardPiece(3, 2, typeof(LayeredBoardPiece));
				SpawnBoardPiece(3, 3, typeof(LayeredBoardPiece));
				SpawnBoardPiece(3, 4, typeof(LayeredBoardPiece));
				SpawnBoardPiece(3, 5, typeof(LayeredBoardPiece));
				
//				SpawnSpecificTileAt(3, 3, typeof(DropTile), TileColorType.Red, false);
//				SpawnSpecificTileAt(3, 4, typeof(DropTile), TileColorType.Green, false);
				SpawnSpecificTileAt(4, 1, typeof(SnowTile), TileColorType.None, false, true);
				SpawnSpecificTileAt(5, 2, typeof(SnowTile), TileColorType.None, false, true);
				SpawnSpecificTileAt(5, 5, typeof(SnowTile), TileColorType.None, false, true);
				SpawnSpecificTileAt(4, 6, typeof(SnowTile), TileColorType.None, false, true);
			}
	
			if (MaleficentBlackboard.Instance.level == 1) {
				SpawnSpecificTileAt(4, 1, typeof(LockedTile), (TileColorType)Random.Range(1, (int)TileColorType.Count), false, true);
				SpawnSpecificTileAt(5, 2, typeof(LockedTile), (TileColorType)Random.Range(1, (int)TileColorType.Count), false, true);
				SpawnSpecificTileAt(5, 5, typeof(LockedTile), (TileColorType)Random.Range(1, (int)TileColorType.Count), false, true);
				SpawnSpecificTileAt(4, 6, typeof(LockedTile), (TileColorType)Random.Range(1, (int)TileColorType.Count), false, true);
			}
			else if (MaleficentBlackboard.Instance.level == 2) {
//				Match3BoardGameLogic.Instance.boardBackground.renderer.material.SetTexture("_AlphaMask", Resources.Load("Test/board_ice_mask_02") as Texture);
				SpawnBoardPiece(2, 3, typeof(EmptyBoardPiece));
				SpawnBoardPiece(2, 4, typeof(EmptyBoardPiece));
				SpawnBoardPiece(3, 2, typeof(EmptyBoardPiece));
				SpawnBoardPiece(3, 3, typeof(EmptyBoardPiece));
				SpawnBoardPiece(3, 4, typeof(EmptyBoardPiece));
				SpawnBoardPiece(3, 5, typeof(EmptyBoardPiece));
				SpawnBoardPiece(4, 2, typeof(EmptyBoardPiece));
				SpawnBoardPiece(4, 3, typeof(EmptyBoardPiece));
				SpawnBoardPiece(4, 4, typeof(EmptyBoardPiece));
				SpawnBoardPiece(4, 5, typeof(EmptyBoardPiece));
				SpawnBoardPiece(5, 3, typeof(EmptyBoardPiece));
				SpawnBoardPiece(5, 4, typeof(EmptyBoardPiece));
			}
			else if (MaleficentBlackboard.Instance.level == 3) {
//				Match3BoardGameLogic.Instance.boardBackground.renderer.material.SetTexture("_AlphaMask", Resources.Load("Test/board_ice_mask_01") as Texture);
				
				SpawnBoardPiece(4, 1, typeof(EmptyBoardPiece));
				SpawnBoardPiece(2, 1, typeof(EmptyBoardPiece));
				SpawnBoardPiece(3, 1, typeof(EmptyBoardPiece));
				SpawnBoardPiece(3, 2, typeof(EmptyBoardPiece));
				SpawnBoardPiece(4, 6, typeof(EmptyBoardPiece));
				SpawnBoardPiece(2, 6, typeof(EmptyBoardPiece));
				SpawnBoardPiece(3, 6, typeof(EmptyBoardPiece));
				SpawnBoardPiece(3, 5, typeof(EmptyBoardPiece));
				
				(Board[4, 1] as Match3BoardPiece).AllowTileFalling = false;
				(Board[2, 1] as Match3BoardPiece).AllowTileFalling = false;
				(Board[3, 1] as Match3BoardPiece).AllowTileFalling = false;
				(Board[3, 2] as Match3BoardPiece).AllowTileFalling = false;
				(Board[4, 6] as Match3BoardPiece).AllowTileFalling = false;
				(Board[2, 6] as Match3BoardPiece).AllowTileFalling = false;
				(Board[3, 6] as Match3BoardPiece).AllowTileFalling = false;
				(Board[3, 5] as Match3BoardPiece).AllowTileFalling = false;
			}
			//TODO: ======END TESTING GROUND FOR CODE-GENERATED BOARD CONFIGURATIONS======
		}
		
		
		// Determine the board pieces neighbors and logical links.
		DetermineBoardLinksAndNeighbors();
		
		// Pre-calculate the board pieces positions and execute initial spawn rules for board pieces that have them set to 
		// pre-populate the board with tiles (if this level is configured this way). If not tiles will be spawned, then board pieces
		// will wait for tiles from the tile spawners.
		for(int rowIdx = 0; rowIdx < Board.NumRows; rowIdx++) 
		{
			boardPiecePos.y = -rowIdx * verticalTileDistance + verticalTileOffset;

			for(int colIdx = 0; colIdx < Board.NumColumns; colIdx++)
			{
				Match3BoardPiece boardPiece = Board[rowIdx, colIdx] as Match3BoardPiece;
				boardPiecePos.x = colIdx * horizontalTileDistance + horizontalTileOffset;
				
				if (boardPiece == null) {
					Debug.LogError("BoardPiece at board pos: " + rowIdx + ", " + colIdx + " is NULL! ");
				}
				
				boardPiece.LocalPosition = boardPiecePos;

				if (autoGenerateRandom) 
				{
                    System.Console.WriteLine("SetupBoardRendering -----");

					//TODO: Fallback for the old method of level generation. This should be removed at some point....or not...
					if (!boardPiece.IsEmpty && !boardPiece.IsBorderPiece && boardPiece.Tile == null && rowIdx < Board.NumRows - 3) {
						SpawnTileAt(rowIdx, colIdx, false, true, true);
					}
				}
				else 
				{
                    System.Console.WriteLine("SetupBoardRendering ========");

                    if (!boardPiece.IsEmpty && !boardPiece.IsBorderPiece) 
					{
						// Only override this flag if it hasn't been already set to true.
						if ( !boardPiece.initialTileSpawnRule.isTileIgnoredByAntiMatchSystems ) {
							boardPiece.initialTileSpawnRule.isTileIgnoredByAntiMatchSystems = boardPiece.initialTileSpawnRule.ShouldSpawnedTileIgnoreAntiMatchSystems();
						}
						
						boardPiece.ExecuteInitialSpawnRule();
					}
				}
			}
		}
				
		// Attach the board tiles to the board pieces to trigger corresponding board events and initialize tiles.
		InitBoardTiles();
	}
		
#endregion
	
#region Match3BoardRenderer specific methods
	/// <summary>
	/// Inits any manully placed tiles. This is required because some tiles may be placed manually in the level
	/// prefab at design time which wouldn't allow them to get initialized in the same order with other spawned tiles.
	/// So these have to be separatelly inititalized after the board determines its neighbors and links.
	/// </summary>
	public void InitBoardTiles()
	{
        System.Console.WriteLine("Match3BoardRenderer -> InitBoardTiles");
        // Ensure the board has at least one possible match (but not already made matches).
        int numTries = 0;
//		matchesUndoer.OnNewTileSpawned = (newTile) => {
//			newTile.enabled = false;
//		};
//
//		possibleMatchGenerator.OnNewTileSpawned = (newTile) => {
//			newTile.enabled = false;
//		};
		MatchesUndoer matchesUndoer = new MatchesUndoer(Board);
		PossibleMatchesFinder possibleMatchesFinder = new PossibleMatchesFinder(Board);
		PossibleMatchGenerator possibleMatchGenerator = new PossibleMatchGenerator(Board);
		
		while ((numTries++) < 10 && (matchesUndoer.FindMatchesAndUndo(true) || numTries == 1))
		{
			if ( !possibleMatchesFinder.FindFirstPossibleMatch() )
			{
				Debug.LogWarning("Forcing a rematch! Last tiles init failed");
				possibleMatchGenerator.GenerateMatch(true);
			}
		}
//		matchesUndoer.OnNewTileSpawned = null;
//		possibleMatchGenerator.OnNewTileSpawned = null;

		
		List<Match3Tile> tilesToInitAfterBoardAttach = new List<Match3Tile>(32);
		
		// Init all tiles that on the board that haven't been initialized.
		Board.ApplyActionToAll((boardPiece) =>
		{
			if ( boardPiece.TileRef != null && !(boardPiece.TileRef as Match3Tile).WasInitialized )
			{
				// Set the tile's board piece owner (because it might have been manually placed at design time).
				// Correctly attach the tile to the board piece as if it would have been spawned at run-time.
				AbstractTile tile = boardPiece.TileRef; 
				boardPiece.TileRef = null;
				tile.BoardPieceRef = null;
				
				tile.InitComponent();
				
				boardPiece.Tile = tile;
				tilesToInitAfterBoardAttach.Add(tile as Match3Tile);
//				(tile as Match3Tile).InitAfterAttachedToBoard();
			}
		});
		
		// Execute InitAfterAttachedToBoard for all above initialized tiles. (like Unity does first Awake, then Start for all game objects).
		for(int i = 0; i < tilesToInitAfterBoardAttach.Count; i++) {
			tilesToInitAfterBoardAttach[i].InitAfterAttachedToBoard();
		}
	}

	/// <summary>
	/// Determines the board links and neighbors. 
	/// Each board piece will have the reference to its direct neighbor (Top, TopRight, Right, etc.) or null for border pieces.
	/// Also, each board piece will have the reference to its logical linked neighbor (TopLink, TopRightLink, etc.) or null if they don't have 
	/// any link in a certain direction. 
	/// The logical links are mostly used for the logic behind the tiles gravity system.
	/// For example when dealing with empty board pieces, the piece above the empty slot will have logical link to the non-empty-piece below the empty slot.
	/// Thus it will fall through the empty pieces.
	/// </summary>
	public void DetermineBoardLinksAndNeighbors() {
		System.Console.WriteLine("Match3BoardRenderer -> DetermineBoardLinksAndNeighbors->");
		// Auto-determine board pieces neighbors. (only for pieces that have the "autoDetermineNeighbors" flag set)
		Board.ApplyActionToAll((boardPiece) => 
		{
			Match3BoardPiece match3Piece = boardPiece as Match3BoardPiece;
			match3Piece.UpdateNeighbors();

			if (match3Piece.autoDetermineLinks) {
				match3Piece.UpdateLinks(); 
			}
		});

		if (OnDetermineBoardLinksAndNeighbors != null) {	
			OnDetermineBoardLinksAndNeighbors();
		}
	}
	
	/// <summary>
	/// Scans the hierarchy children of the board renderer and sets up the BoardData accordingly.
	/// </summary>
	public void LoadBoardSetupFromHierarchy()
	{
		System.Console.WriteLine("Match3BoardRenderer -> LoadBoardSetupFromHierarchy->");
//		List<Match3Tile> tilesPlacedAtDesignTime = new List<Match3Tile>(30);
		
		// Enable all the board pieces and initialize them
		Match3BoardPiece[] childBoardPieces = GetComponentsInChildren<Match3BoardPiece>(true);
		for(int i = 0; i < childBoardPieces.Length; i++) {
			childBoardPieces[i].gameObject.SetActive(true);
			childBoardPieces[i].InitComponent(this);
			
			// Destroy tiles that are on board pieces that have a spawn rule. 
			// Leave only custom configured tiles that are on board pieces without any spawn RuleEntries.
			if (childBoardPieces[i].Tile != null && childBoardPieces[i].initialTileSpawnRule.ruleEntries.Count > 0) 
			{
//				Debug.LogWarning("Destroying " + childBoardPieces[i].Tile.name);
				DestroyImmediate(childBoardPieces[i].Tile.gameObject);
			}
			else if (childBoardPieces[i].Tile != null) 
			{
				// If we let the tile placed by the designer, we need to manually activate it and initialize it later.
//				tilesPlacedAtDesignTime.Add(childBoardPieces[i].Tile as Match3Tile);
				childBoardPieces[i].Tile.gameObject.SetActive(true);
//				Debug.LogWarning("Activated custom tile: " + childBoardPieces[i].Tile.name);
			}

			BoardCoord boardPos = BoardCoord.ParseCoordFromString(childBoardPieces[i].name);
			// Set this board piece in the corresponding Board position
			Board[boardPos] = childBoardPieces[i];
		}

//		// Manually activate and initialize tiles placed at design time. (not spawned by the game)
//		for(int i = 0; i < tilesPlacedAtDesignTime.Count; i++) 
//		{
//			tilesPlacedAtDesignTime[i].gameObject.SetActive(true);
//			tilesPlacedAtDesignTime[i].InitComponent();
//		}
	}
	
#if UNITY_EDITOR
	public int EditorLoadBoardSetupFromHierarchy(Match3BoardPiece[,] gridDestination) {
		// Iterate through all the child board pieces
		Match3BoardPiece[] childBoardPieces = GetComponentsInChildren<Match3BoardPiece>(true);
		for(int i = 0; i < childBoardPieces.Length; i++) {
			BoardCoord boardPos = BoardCoord.ParseCoordFromString(childBoardPieces[i].name);
			// Set this board piece in the corresponding Board position
			gridDestination[boardPos.row, boardPos.col] = childBoardPieces[i];
			childBoardPieces[i].editorBoardPos = boardPos;
		}
		
		// Return the number of board pieces found.
		return childBoardPieces.Length;
	}
#endif
	
	/// <summary>
	/// TODO: temporary quick & dirty implementation to generate a test board
	/// TODO: an AbstractBoardSerializer will be in charge of populating a BoardData according to either random params or from a previously saved file.
	/// TODO: the Match3BoardSerializeer will be overridding AbstractBoardSerializer to handle accordingly the loading of match3 level files and populating the BoardData
	/// with correct data types.
	/// </summary>
	public void GenerateRandomBoard(int numRows, int numCols) {	
		System.Console.WriteLine("Match3BoardRenderer -> GenerateRandomBoard->");
		Debug.LogWarning("Generating random board: " + numRows + " rows,  " + numCols + " cols");
		
		for(int rowIdx = 0; rowIdx < Board.NumRows; rowIdx++) {
			for(int colIdx = 0; colIdx < Board.NumColumns; colIdx++) {
				SpawnBoardPiece(rowIdx, colIdx, typeof(Match3BoardPiece));

//				GameObject boardPieceGO = new GameObject(string.Format("[{0},{1}]BoardPiece", rowIdx, colIdx));
//				boardData[rowIdx, colIdx] = boardPieceGO.AddComponent<Match3BoardPiece>();
//				boardData[rowIdx, colIdx].InitComponent(boardData, rowIdx, colIdx);
			}
		}

		//TODO: just for quick testing smaller boards (set the remaining board pieces as borderpieces)
		Debug.LogWarning("Generating board pieces for the rest of the board...");
		for(int rowIdx = numRows; rowIdx < Board.NumRows; rowIdx++) {
			for(int colIdx = 0; colIdx < Board.NumColumns; colIdx++) {
				SpawnBoardPiece(rowIdx, colIdx, typeof(BorderBoardPiece));
//				GameObject boardPieceGO = new GameObject(string.Format("[{0},{1}]BoardPiece", rowIdx, colIdx));
//				boardData[rowIdx, colIdx] = boardPieceGO.AddComponent<Match3BoardPiece>();
//				boardData[rowIdx, colIdx].InitComponent(boardData, rowIdx, colIdx);
//				boardData[rowIdx, colIdx].IsBorderPiece = true;
			}
		}		
		
		for(int rowIdx = 0; rowIdx < Board.NumRows; rowIdx++) {
			for(int colIdx = numCols; colIdx < Board.NumColumns; colIdx++) {
				SpawnBoardPiece(rowIdx, colIdx, typeof(BorderBoardPiece));
//				GameObject boardPieceGO = new GameObject(string.Format("[{0},{1}]BoardPiece", rowIdx, colIdx));
//				boardData[rowIdx, colIdx] = boardPieceGO.AddComponent<Match3BoardPiece>();
//				boardData[rowIdx, colIdx].InitComponent(boardData, rowIdx, colIdx);
//				boardData[rowIdx, colIdx].IsBorderPiece = true;
			}
		}
		
	}

	public Match3BoardPiece SpawnBoardPiece(int rowIdx, int colIdx, System.Type pieceType, bool autoDestroyOldRef = true) {
        /*
        foreach (KeyValuePair<System.Type, Match3BoardPiece> pair in piecesDictionary) {
            System.Console.WriteLine("Key:{0}", pair.Key.ToString());
        }
        */
        Match3BoardPiece boardPiece = (Instantiate(piecesDictionary[pieceType].gameObject) as GameObject).GetComponent<Match3BoardPiece>();
        System.Console.WriteLine("SpawnBoardPiece-> {0}-{1}", pieceType.ToString(), boardPiece.GetType().ToString());
        //if(null == boardPiece.cachedTransform)
        //    boardPiece.cachedTransform = Instantiate(transform) as Transform;
        //boardPiece.cachedTransform.position = Vector3.zero;
		
		boardPiece.name = string.Format("[{0},{1}] {2}", rowIdx, colIdx, boardPiece.name);
		boardPiece.InitComponent(this);
		
		Match3BoardPiece oldBoardPiece = Board[rowIdx, colIdx] as Match3BoardPiece;
		
		// If there is already a board piece GameObject instantiated in this slot, destroy it first.
		if (oldBoardPiece != null && autoDestroyOldRef) 
		{
			Destroy(oldBoardPiece.gameObject);
		}
		
		Board[rowIdx, colIdx] = boardPiece;
		
		return boardPiece;
	}
	
	public Match3Tile SpawnTileAt(BoardCoord boardPos, bool offBoardTile = false, bool noLockedTiles = false, bool isBoardSetup = false) 
	{
		return SpawnTileAt(boardPos.row, boardPos.col, offBoardTile, noLockedTiles);
	}

	public Match3Tile SpawnTileAt(int rowIdx, int columnIdx, bool offBoardTile = false, bool noLockedTiles = false, bool isBoardSetup = false) 
	{
		int rand;
		if (noLockedTiles) 
		{
			//TODO: temporary spawn hack to spawn only new normal tiles on the board.
			rand = Random.Range(0, 6); //a normal tile
		} 
		else 
		{
			rand = Random.Range(0, 100); // current chance for locked tile is 6/100 = 6%
			if (rand < 6) 
			{
				rand += 6; //a locked tile
			} else 
			{
				rand = Random.Range(0, 6); //a normal tile
			}
		}
		
		Match3Tile newTilePrefab = tilesPrefabs[rand].GetComponent<Match3Tile>();
		
		return SpawnSpecificTileAt(rowIdx, columnIdx, newTilePrefab.GetType(), newTilePrefab.TileColor, offBoardTile, isBoardSetup);
	}
		
	public Match3Tile SpawnSpecificTileAt(BoardCoord spawnPos, System.Type tileType, TileColorType prefabDefaultColor, bool offBoardTile = false) 
	{
		return SpawnSpecificTileAt(spawnPos.row, spawnPos.col, tileType, prefabDefaultColor, offBoardTile);
	}
	
	public Match3Tile SpawnSpecificTileAt(BoardCoord spawnPos, System.Type tileType, TileColorType prefabDefaultColor, bool offBoardTile, bool isBoardSetup)
	{
		return SpawnSpecificTileAt(spawnPos.row, spawnPos.col, tileType, prefabDefaultColor, offBoardTile, isBoardSetup);
	}

	public Match3Tile SpawnSpecificTileAt(int rowIdx, int columnIdx, System.Type tileType, TileColorType prefabDefaultColor, bool offBoardTile = false, bool isBoardSetup = false) 
	{
        //		Debug.LogWarning("[SpawnSpecificTileAt] " + tileType.ToString() + " " + prefabDefaultColor);
        if (null == tilesDictionary)
            System.Console.WriteLine("SpawnSpecificTileAt SpawnSpecificTile-> is null");
        else
            System.Console.WriteLine("SpawnSpecificTileAt SpawnSpecificTile-> is not null");
        // Spawn the specified tile
        Match3Tile newTile = SpawnSpecificTile(tileType, prefabDefaultColor, isBoardSetup);
		newTile.name = string.Format("[{0},{1}] {2}", rowIdx, columnIdx, newTile.name);

		// Assign the tile to the specified BoardPiece.
		AttachTileToBoardAt(rowIdx, columnIdx, newTile, offBoardTile, isBoardSetup);
		
		return newTile;
	} 
	
	public Match3Tile SpawnSpecificTile(System.Type tileType, TileColorType prefabDefaultColor, bool isBoardSetup = false) 
	{
        System.Console.WriteLine("SpawnSpecificTile-> {0}-{1}", tileType.ToString(), prefabDefaultColor.ToString());
        //foreach (KeyValuePair<System.Type, Match3BoardPiece> pair in piecesDictionary) {
        //    System.Console.WriteLine("Key:{0}", pair.Key.ToString());
        //}
        if (null == tilesDictionary)
            System.Console.WriteLine("SpawnSpecificTile-> is null");
        else
            System.Console.WriteLine("SpawnSpecificTile-> is not null");
        tilesDictionary.toString();
        Match3Tile newTile = (Instantiate(tilesDictionary[tileType, prefabDefaultColor].gameObject) as GameObject).GetComponent<Match3Tile>();
		
		newTile.cachedTransform.parent = cachedTransform;
		newTile.BoardRenderer = this;
		
		if ( !isBoardSetup ) {
			newTile.InitComponent();
		}
		
		return newTile;
	}

	public void AttachTileToBoardAt(Match3BoardPiece targetBoardPiece, Match3Tile tile, bool offBoardTile, bool isBoardSetup = false, bool resetTilePosition = true) 
	{
		BoardCoord boardPos = targetBoardPiece.BoardPosition;
		AttachTileToBoardAt(boardPos.row, boardPos.col, tile, offBoardTile, isBoardSetup, resetTilePosition);
	}
	
	public Match3BoardPiece AttachTileToBoardAt(int rowIdx, int columnIdx, Match3Tile tile, bool offBoardTile, bool isBoardSetup = false, bool resetTilePosition = true) 
	{
		System.Console.WriteLine("Match3BoardRenderer -> AttachTileToBoardAt->");
		// Assign the tile to the specified BoardPiece.
		Match3BoardPiece targetBoardPiece = Board[rowIdx, columnIdx] as Match3BoardPiece;
		
		if (isBoardSetup) {
			targetBoardPiece.TileRef = tile;
		}
		else 
		{
			targetBoardPiece.Tile = tile;
			
//			if (targetBoardPiece.BottomLink != null && targetBoardPiece.BottomLink.Tile != null && targetBoardPiece.BottomLink.Tile.IsMoving) {
//				Match3Tile nextTile = targetBoardPiece.BottomLink.Tile as Match3Tile;
//				newTile.moveVel = Mathf.Clamp(nextTile.moveVel - newTile.initialVel, -newTile.initialVel, newTile.maxVel);
//			}
		}
		
		if (resetTilePosition) {
			targetBoardPiece.ResetTilePosition();
		}
		
//		if (offBoardTile) 
//		{
//			Match3BoardPiece bottomLinkPiece = targetBoardPiece.BottomLink;
//			Vector3 tileLocalPos = newTile.LocalPosition;
//			
//			if (bottomLinkPiece != null && bottomLinkPiece.Tile != null && bottomLinkPiece.LocalPosition.y < bottomLinkPiece.Tile.LocalPosition.y) 
//			{
//				tileLocalPos.y = bottomLinkPiece.Tile.LocalPosition.y + verticalTileDistance + verticalTileOffset;
//			}
//			else {
//				tileLocalPos.y = targetBoardPiece.LocalPosition.y + verticalTileDistance + verticalTileOffset;
//			}
//			
//			newTile.LocalPosition = tileLocalPos;
//
////			Debug.LogWarning("Spawning offboard tile at : " + tileLocalPos);
//			//Debug.Break();
//		}
		
		if ( !isBoardSetup && !offBoardTile ) {
			tile.InitAfterAttachedToBoard();
		}
		
		return targetBoardPiece;
	}
		
#if UNITY_EDITOR
	private float lastHorizTileDistance = Mathf.Infinity;
	private float lastHorizTileOffset = Mathf.Infinity;
	private float lastVertTileDistance = Mathf.Infinity;
	private float lastVertTileOffset = Mathf.Infinity;

	/// <summary>
	/// Updates the board pieces positions. (where the tiles will be placed)
	/// Used only when setting up the board config in edit mode for faster tiles positioning. (when useEditorMode is true)
	/// </summary>
	void UpdateBoardPiecesPosition() 
	{
		Vector3 boardPiecesGridOffset = Vector3.zero;

		for(int rowIdx = 0; rowIdx < Board.NumRows; rowIdx++) {
			boardPiecesGridOffset.y = -rowIdx * verticalTileDistance + verticalTileOffset;

			for(int colIdx = 0; colIdx < Board.NumColumns; colIdx++) {
				boardPiecesGridOffset.x = colIdx * horizontalTileDistance + horizontalTileOffset;
				Board[rowIdx, colIdx].LocalPosition = boardPiecesGridOffset;
			}
		}
	}

	void Update() {
		if ( !useEditorMode ) {
			return;
		}
		
		if (lastHorizTileDistance != horizontalTileDistance || lastHorizTileOffset != horizontalTileOffset || lastVertTileDistance != verticalTileDistance ||
			lastVertTileOffset != vertTileOffset) {
			lastHorizTileDistance = horizontalTileDistance;
			lastHorizTileOffset = horizontalTileOffset;
			lastVertTileDistance = verticalTileDistance;
			lastVertTileOffset = verticalTileOffset;

			UpdateBoardPiecesPosition();
		}
	}
		
#endif
#endregion
}
