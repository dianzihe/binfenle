using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public enum TileMoveDirection
{
	Left = 0,
	Top, 
	Right,
	Bottom,
	
	Count,
}

public class Match3BoardGameLogic : AbstractBoardGameLogic {
	/// <summary>
	/// Occurs when on the board is stable. (nothing else is happening on it)
	/// </summary>
	public static event System.Action OnStableBoard;
	
	/// <summary>
	/// Occurs right after the board becomes stable. This event is raised right after OnStableBoard.
	/// </summary>
	public static event System.Action OnPostStableBoard;
	
	/// <summary>
	/// Occurs when matches are found and they are destroyed.
	/// </summary>
	public static event System.Action OnDestroyLastFoundMatches;
	
	/// <summary>
	/// Occurs when the user started a tiles switch.
	/// </summary>
	public static event System.Action<Match3Tile, Match3Tile> OnUserStartedTilesSwitch;
	
	/// <summary>
	/// Occurs when the last started tiles switch was bad and a switch back will follow.
	/// </summary>
	public static event System.Action<Match3Tile, Match3Tile> OnUserTilesSwitchBad;

	/// <summary>
	/// Occurs when the last started tiles switch was good so there's no switch back
	/// </summary>
	public static event System.Action<Match3Tile, Match3Tile> OnUserTilesSwitchGood;
	
	public static System.DateTime baseDate = new System.DateTime(2013, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
	
	/// <summary>
	/// Occurs when on "IsCheckingStableBoard" state changes (true/false) meaning that the board 
	/// started or finished checking it's stable status. (nothing else is moving on the board)
	/// When this event triggers it will also send the new state of the "IsCheckingStableBoard".
	/// </summary>
	public static event System.Action<bool> OnStableBoardCheckingStatusChanged;
	public static event System.Action OnStartGame;
	public static event System.Action<AbstractTile,TileMoveDirection> OnTryingToMoveTile;
	
	protected static Match3BoardGameLogic instance;
	
	public Transform boardVisualContainer;
	
	/// <summary>
	/// Reference to the board background (not the game background) but the board itself that contains the grid.
	/// </summary>
	public Transform boardBackground;
	
	public float firstRowEmptyCheckInterval = 0.1f;
	
	public Camera gameCamera;
	
	public Transform match3Board;
	
	public int unstableLock = 0;
	
	//List of imediate spawn rules that need to be picked up asap by the spawners
	public List<TileSpawnRule> immediateSpawnList = new List<TileSpawnRule>(10);

	protected WaitForEndOfFrame waitEndFrame;
	
	private bool isGameOver = false;

	public GameObjectEvent OnLosePending = new GameObjectEvent(null, "SendFsmEvent", "OnBoardLosePending");
	public GameObjectEvent OnLoseFinished = new GameObjectEvent(null, "SendFsmEvent", "OnBoardLoseFinished");
	public GameObjectEvent OnWinPending = new GameObjectEvent(null, "SendFsmEvent", "OnBoardWinPending");
	public GameObjectEvent OnWinFinished = new GameObjectEvent(null, "SendFsmEvent", "OnBoardWinFinished");
	public GameObjectEvent OnFreeFall = new GameObjectEvent(null, "SendFsmEvent", "OnBoardFreeFall");
	public GameObjectEvent OnGameOver = new GameObjectEvent(null, "SendFsmEvent", "OnGameOver");
			
	/// <summary>
	/// List of Match3BoardPieces that act as spawners for the current level.
	/// The Board pieces will register/unregister themselves in this list.
	/// </summary>
	[System.NonSerialized]
	public List<Match3BoardPiece> tileSpawners = new List<Match3BoardPiece>(8);

	/// <summary>
	/// Handles finding and storing board matches.
	/// </summary>
	public MatchesFinder matchesFinder;
		
	[System.NonSerialized]
	public BoardCoord[] tilesMoveDirections = new BoardCoord[(int)TileMoveDirection.Count] {
											 	   new BoardCoord(0, -1),  // Left Direction
												   new BoardCoord(-1, 0),  // Top
												   new BoardCoord(0, 1),   // Right,
												   new BoardCoord(1, 0),   // Bottom
											   };	
	
	protected Texture2D boardHolesMask;
	
	/// <summary>
	/// The game sound events handler. (tile swapping, matching sounds are played by this handler)
	/// </summary>
	protected InGameSoundEventsHandler gameSoundEventsHandler;
	
	/// <summary>
	/// Inidicates if the stable board checker has been triggered by any event on the board (matches, powers destroying stuff, etc.)
	/// </summary>
	private bool isCheckingStableBoard = false;

	private bool isBoardStable = false;
	
	/// <summary>
	/// The level won flag is set in StableBoardWin/Lose.
	/// </summary>
	[System.NonSerialized]
	public bool levelWon = false;
	[System.NonSerialized]
	public string characterUsed;

	//public PlayMakerFSM fsm;
	public static Match3BoardGameLogic Instance 
	{
		get {
			return instance;
		}
	}
	
	public Match3BoardAnimations BoardAnimations 
	{
		get {
			return boardAnimations as Match3BoardAnimations;
		}
	}

	protected override void Awake ()
	{
        instance = this;

		base.Awake ();
		
		waitEndFrame = new WaitForEndOfFrame();
			
		HOTween.Init(false, false, false);

        /* 
		characterUsed = CharacterSpecialAnimations.CharIdx < 0 || CharacterSpecialAnimations.CharIdx >= CompanionSelect.icons.Length ? "" : CompanionSelect.icons[CharacterSpecialAnimations.CharIdx];
	
		if(characterUsed == "") {
			Debug.LogWarning("No icon found for character index " + CharacterSpecialAnimations.CharIdx);
		}
		*/

        MaleficentBlackboard.Instance.level = 1;
        LoadLevel(MaleficentBlackboard.Instance.level);
		
		// Once the level is loaded get the reference to the board data
		boardData = boardRenderer.GetComponent<BoardData>();
		
		gameSoundEventsHandler = new InGameSoundEventsHandler();
		gameSoundEventsHandler.RegisterSoundEvents();

        //InitComponent();
    }
		
	//public override void InitComponent() 
	protected virtual void Start()
	{
        System.Console.WriteLine("Match3BoardGameLogic -> InitComponent->");
        base.InitComponent();
		
		SetupBoardHoles();
		
		// Create an instance of the board matches finder.
		matchesFinder = new MatchesFinder(boardData);
		//TODO:remove this bug trap after finishing the bug fix
//		matchesFinder.useBugTrap = true;
		
		// Register to AbstractBoardAnimations to receive animation events.
		BoardAnimations.OnTilesSwitchAnimFinished += OnTilesSwitchAnimFinished;

		Match3Tile.OnTileMovingChanged += OnTileMovingChanged;

		if (loseConditions != null) {
			loseConditions.gameObject.SetActive(true);
			loseConditions.Pause(true);
			loseConditions.OnLoseChecked += OnLoseChecked;
		}

		if (winConditions != null) {
			winConditions.gameObject.SetActive(true);
			loseConditions.Pause(true);
			winConditions.OnWinChecked += OnWinChecked;
		}

		// Pause the board game logic
		SetBoardEnabledState(false);
	}
	
	public static long TimeDaysSinceBaseUtc()
	{
		return (long)System.DateTime.Now.Subtract(baseDate).TotalDays;
	}

	public bool IsCheckingStableBoard {
		get 
		{
			return isCheckingStableBoard;
		}
		set 
		{
			bool lastValue = isCheckingStableBoard;
			isCheckingStableBoard = value;
			if (lastValue != isCheckingStableBoard)
			{
				if (OnStableBoardCheckingStatusChanged != null) {
					OnStableBoardCheckingStatusChanged(isCheckingStableBoard);
				}
			}
		}
	}
	
	public bool IsBoardStable {
		get {
			return isBoardStable;
		}
		set {
			isBoardStable = value;
		}
	}

	public bool IsGameOver {
		get {
			return isGameOver;
		}
		set {
			isGameOver = value;
			if (isGameOver)
			{
				OnGameOver.RaiseEvent();
			}
		}
	}

	/// <summary>
	/// Loads the level. Indexed from 1.
	/// If the level is not found it will load a  default level prefab called: DefaultLevel.
	/// </summary>
	/// <param name='numLevel'>
	/// Number level.
	/// </param>
	public void LoadLevel(int numLevel) {
        // Check if there isn't a level already in the hierarchy before trying to instatiate one.
        Match3BoardRenderer level = cachedTransform.parent.GetComponentInChildren<Match3BoardRenderer>();

        if (level == null) 
		{
			System.Console.WriteLine("[Match3BoardGameLogic] Trying to load level: " + Match3BoardRenderer.baseLevelPathName + numLevel);
			
			GameObject levelPrefab = Resources.Load(Match3BoardRenderer.baseLevelPathName + numLevel) as GameObject;
			// If for some reason we can't find this level, load the default level
			if (levelPrefab == null) {
				System.Console.WriteLine("[Match3BoardGameLogic] Failed to load level: " + Match3BoardRenderer.baseLevelPathName + numLevel + ". Falling back to default level!");
				levelPrefab = Resources.Load(Match3BoardRenderer.baseLevelPathName + "_Dummy") as GameObject;
			}
            GameObject newLevelGO = Instantiate(levelPrefab) as GameObject;
			level = newLevelGO.GetComponent<Match3BoardRenderer>();
			
			Vector3 prefabLocalPos = level.cachedTransform.localPosition;
			level.cachedTransform.parent = boardVisualContainer;
			level.cachedTransform.localPosition = prefabLocalPos;
			level.cachedTransform.localScale = Vector3.one;
		} else {
			System.Console.WriteLine("[Match3BoardGameLogic] Level already found in the scene! Loading that one: " + level.name);
		}
		// Couple the Match3BoardRenderer that contains the level configuration.
		boardRenderer = level;
		boardAnimations.boardRenderer = level;

		winConditions = level.winConditions;
		loseConditions = level.loseConditions;
	}
	
	public void SetupBoardHoles() 
	{
		// Create the level board holes mask
		BoardHolesMaskGenerator boardHoles = new BoardHolesMaskGenerator(boardData);
		boardHolesMask = boardHoles.CreateMaskTexture(256, 256);
		boardBackground.GetComponent<Renderer>().material.SetTexture("_AlphaMask", boardHolesMask);
	}
		
	/// <summary>
	/// Sets the state of the board. (meant for pausing/resuming the main board game logic)
	/// </summary>
	/// <param name='isEnabled'>
	/// Is enabled.
	/// </param>
	public void SetBoardEnabledState(bool isEnabled)
	{
		// Pause/Resume possible matches controller
		if (isEnabled)
		{	
			PossibleMatchesController.Instance.EnabledCount++;
		}
		else
		{
			PossibleMatchesController.Instance.EnabledCount--;
		}
		
//		PossibleMatchesController.Instance.enabled = isEnabled;
		
		// Pause/Resume lose and win conditions controllers
		if (loseConditions != null) {
			loseConditions.Pause( !isEnabled );
		}
		
		if (winConditions != null) {
			winConditions.Pause( !isEnabled );
		}		
		
		// Pause/Resume board tiles and tile spawners.
		boardData.ApplyActionToAll((boardPiece) => {
			if (boardPiece.Tile != null) 
			{
				boardPiece.Tile.enabled = isEnabled;
			}
			
			TileSpawnerBehavior tileSpawner = boardPiece.GetComponent<TileSpawnerBehavior>();
			if (tileSpawner != null) 
			{
				tileSpawner.enabled = isEnabled;
			}
		});
	}
	
	public void StartGame()
	{		
		Debug.Log("[Match3BoardGameLogic] StartGame called...");
		System.Console.WriteLine("Match3BoardGameLogic -> StartGame->");
		//AnalyticsBinding.LogEventGameAction(GetLevelType(), "game_start", null, null, MaleficentBlackboard.Instance.level);
		SetLevelStartAnalyticTimeEvent();
		
		IsGameOver = false;
		
		// Resume the main board game logic.
		SetBoardEnabledState(true);
		
		if (OnStartGame != null) {
			OnStartGame();
		}
		
		TryCheckStableBoard();
	}
	
	public void SetLevelStartAnalyticTimeEvent(bool levelFinished = false)
	{
		System.Console.WriteLine("Match3BoardGameLogic -> SetLevelStartAnalyticTimeEvent->");
		//TODO: when facebook will be integrated we must log per user analytics stats here.
		int level = MaleficentBlackboard.Instance.level;
		string analyticsLevelKey = string.Format("AnalyticsPlayedLevelTime{0}", level);
		int levelPlayedTime = PlayerPrefs.GetInt(analyticsLevelKey, -1);
		
		if (levelPlayedTime == -1) 
		{
			//TALIN - Check if the level was finished before, for example from iCloud
			if (UserManagerCloud.Instance.GetStarsForLevel(level) > 0)
			{
				// TALIN - This level was already finished, no timing event should be sent
				PlayerPrefs.SetInt(analyticsLevelKey, -2);
				return;
			}
			
			// Set the current played time for this level that has been started for the first time.
			levelPlayedTime = (int)TimeDaysSinceBaseUtc();
			PlayerPrefs.SetInt(analyticsLevelKey, levelPlayedTime);
			
			PlayerPrefs.Save();
			
			Debug.Log("[Match3BoardGameLogic] SetLevelStartAnalyticTimeEvent set the first timestamp for level: " + level);
		}
		else if (levelPlayedTime < -1) {
			// This level has been handled by the analytics timing event and we don't need to do it again.
			Debug.Log("[Match3BoardGameLogic] SetLevelStartAnalyticTimeEvent -> Analytics timing event already handled for this level: " + level);
			return;
		}
		
		if (levelFinished)
		{
			// Make sure this event is sent only once per level in the entire game's lifetime. (or until it's deleted and re-installed :D )
			if (levelPlayedTime >= 0)
			{
				// Determine the amount of time passed since this level was first played until now(when it was completed).
				int elapsedDaysUntilFinished = (int)TimeDaysSinceBaseUtc() - levelPlayedTime;
				if (elapsedDaysUntilFinished < 0) {
					elapsedDaysUntilFinished = 0;
				}
				
				// Mark this level as handled by the analytics event.
				PlayerPrefs.SetInt(analyticsLevelKey, -2);
				
				//AnalyticsBinding.LogEventTimingAction(string.Format("{0}_{1}", GetLevelType(), level), (float)elapsedDaysUntilFinished);
				
				PlayerPrefs.Save();
			}	
		}
	}
		
	public string GetLevelType() 
	{
		return winConditions.GetLevelType(loseConditions);
	}
	
	public void Pause(bool _pause) {
		// This should pause the whole game
		if(!freeFalling) {
			loseConditions.Pause(_pause);
		}
	}
	
	// OBSOLETE
	public void AcceptOfferOnLose(int packIndex) 
	{
//		IsGameOver = false;
//		loseConditions.AcceptOffer(packIndex);
	}

	public static TileMoveDirection GetTileMoveDirection(Vector2 swipeTotalAmount) {
		// Detect the drag direction. Check if it's horizontal first.
		if ( Mathf.Abs(swipeTotalAmount.x) >= Mathf.Abs(swipeTotalAmount.y) ) {
			// Check if we're trying to move the tile to the right.
			if (swipeTotalAmount.x >= 0) {
				return TileMoveDirection.Right;
			} else {
				return TileMoveDirection.Left;
			}
		} else {
			// We're moving the tile vertically.
			if (swipeTotalAmount.y >= 0) {
				return TileMoveDirection.Top;
			} else {
				return TileMoveDirection.Bottom;
			}
		}
	}

	public virtual bool CanMoveTile(AbstractTile tile, AbstractBoardPiece targetBoardPiece) 
	{
		// In case we're trying to do a switch with the same tile. (failed input or reached board margin)
		if (tile.BoardPiece == targetBoardPiece) {
//			Debug.LogWarning("[Match3BoardGameLogic] Trying to move tile " + tile.name + " in the same board piece as the tile: " + targetBoardPiece);
			return false;
		}
		
		// Safety-check in case we might try to move a tile to a board piece which has no tile on it to switch with. 
		// In case we will want this mechanic later in the game we will let the tile decide if it can move to an empty board piece or not.
		if (targetBoardPiece.Tile == null) {
//			Debug.LogError("[Match3BoardGameLogic] In method CanMoveTile: " + tile.name + " error: targetBoardPiece doesn't have any tile to switch with!");
			return false;
		}
				
		// Check if tiles can switch positions.
		return tile.CanMoveAt(targetBoardPiece) && targetBoardPiece.Tile.CanMoveAt(tile.BoardPiece);
	}
	
	public override bool TryToMoveTile(AbstractTile srcTile, AbstractTile dstTile) 
	{
		if (srcTile == dstTile || srcTile == null || dstTile == null) {
			return false;
		}

		if ( CanMoveTile(srcTile, dstTile.BoardPiece) )
		{
			Match3Tile srcMatch3Tile = srcTile as Match3Tile;
			Match3Tile dstMatch3Tile = dstTile as Match3Tile;
				
			if (OnUserStartedTilesSwitch != null) {
				OnUserStartedTilesSwitch(srcMatch3Tile, dstMatch3Tile);
			}

			srcMatch3Tile.RaiseEventTileTappedFirst();
			
			srcMatch3Tile.IsTileSwitching = true;
			dstMatch3Tile.IsTileSwitching = true;
			BoardAnimations.SwitchTilesAnim(true, srcMatch3Tile, dstMatch3Tile);

			return true;
		}
		
		return false;
	}
	
	public override bool TryToMoveTile(AbstractTile tile, TileMoveDirection moveDirection) 
	{
		//TODO: ask tile to determine if it can move to given position => trigger tile move anim => register to anim finish event => 
		// notify back the gameboard logic to check for matches => 
		// if no match -> switch back (if tile allows it) 
		// if found matches => validate match pattern -> spawn new tiles, new effects, trigger other tiles, etc -> mark for destroy matched tiles
		// -> update board state (check all tiles that should be falling and set them to fall and let them update their positions until they stop

		//BoardCoord offsetDir = tilesMoveDirections[(int)moveDirection];
		BoardCoord targetBoardPos = tile.BoardPiece.BoardPosition;

		if (OnTryingToMoveTile != null) {
			OnTryingToMoveTile(tile,moveDirection);
		}

		// Offset the target board position by the tile movement direction offset and clamp the result to the border of the board.
		targetBoardPos.OffsetByAndClamp(tilesMoveDirections[(int)moveDirection], boardData.NumRows - 1, boardData.NumColumns - 1);
		
//		Debug.Log("[Match3BoardGameLogic] BeginMoveTile " + tile.name + " -> from: " + tile.BoardPiece.BoardPosition + 
//			" to " + (tile.BoardPiece.BoardPosition.row + offsetDir.row)  + ", " + (tile.BoardPiece.BoardPosition.col + offsetDir.col));
		
		return TryToMoveTile(tile, boardData[targetBoardPos].Tile);
	}
	
	public override void TileMoving(AbstractTile targetTile) {
		
	}
	
	public override void EndMoveTile(AbstractTile targetTile) {
		
	}
	
	public void OnTilesSwitchAnimFinished(AbstractBoardAnimations sender, AbstractTile srcTile, AbstractTile dstTile) {
//		Debug.Log("Switch anim finished!");
		// Update the board positions of the animated tiles (update the board logic after the animation finishes).
		boardData.SwitchTiles(srcTile, dstTile);

		Match3Tile srcMatch3Tile = srcTile as Match3Tile;
		Match3Tile dstMatch3Tile = dstTile as Match3Tile;
		
		bool foundMatches = matchesFinder.FindMatches();
		if (( !foundMatches || (!srcMatch3Tile.IsMatched && !dstMatch3Tile.IsMatched) ) &&
			(srcMatch3Tile.SwitchBackOnMatchFail && dstMatch3Tile.SwitchBackOnMatchFail)) 
		{


			srcMatch3Tile.IsTileSwitching = true;
			dstMatch3Tile.IsTileSwitching = true;

			if (OnUserTilesSwitchBad != null) {
				OnUserTilesSwitchBad(srcMatch3Tile, dstMatch3Tile);
			}

			BoardAnimations.SwitchTilesAnim(false, srcMatch3Tile, dstMatch3Tile,
				(_sender, _srcTile, _dstTile) => 
				{
					boardData.SwitchTiles(_srcTile, _dstTile);
					_srcTile.IsTileSwitching = false;
					_dstTile.IsTileSwitching = false;
				}
			);
			
			srcMatch3Tile.RaiseEventSwitchBackOnFail(dstMatch3Tile);
			dstMatch3Tile.RaiseEventSwitchBackOnFail(srcMatch3Tile);
		} 
		else if (srcMatch3Tile.IsMatched || dstMatch3Tile.IsMatched) {

			srcMatch3Tile.IsTileSwitching = false;
			dstMatch3Tile.IsTileSwitching = false;

			if (OnUserTilesSwitchGood != null) {
				OnUserTilesSwitchGood(srcMatch3Tile, dstMatch3Tile);
			}

			loseConditions.NewMove();
			
			srcMatch3Tile.RaiseEventSwitchSuccess(dstMatch3Tile);
			dstMatch3Tile.RaiseEventSwitchSuccess(srcMatch3Tile);
		}
		else if (!srcMatch3Tile.SwitchBackOnMatchFail || !dstMatch3Tile.SwitchBackOnMatchFail) { 
			// Reset the "IsTileSwitching" property for tiles that don't switch back on match fail because they finished their switch animation.
			srcMatch3Tile.IsTileSwitching = false;
			dstMatch3Tile.IsTileSwitching = false;
		}
				
		DestroyLastFoundMatches();
	}	

	public void DestroyLastFoundMatches() 
	{
		bool matchesFound = matchesFinder.lastFoundMatches.Count > 0;
		
		for(int i = 0; i < matchesFinder.lastFoundMatches.Count; i++) {			
			matchesFinder.lastFoundMatches[i].Destroy();			
		}
		
		if (matchesFound) {
			ScoreSystem.Instance.IncreaseMultiplier();
			
			if (OnDestroyLastFoundMatches != null) {
				OnDestroyLastFoundMatches();
			}
			
			TryCheckStableBoard();
		}
	}
	
	public void OnTileMovingChanged(AbstractTile tile)
	{
		if (!tile.IsMoving) {
			TryCheckStableBoard();
		}
	}
	
	public void OnLoseChecked()
	{		
		OnLosePending.RaiseEvent();
		
		IsBoardStable = false;
		TryCheckStableBoard();
		OnStableBoard += StableBoardLose;
		
		IsGameOver = true;
	}
	
	public void OnWinChecked()
	{
		loseConditions.Pause(true);
		OnWinPending.RaiseEvent();
		
		IsBoardStable = false;
		TryCheckStableBoard();
		OnStableBoard += StableBoardWin;

		IsGameOver = true;
	}
	
	public void TryCheckStableBoard()
	{
		if ( !IsCheckingStableBoard ) {
			StartCoroutine(CheckStableBoard());
		}
	}
	
	
	protected IEnumerator CheckStableBoard()
	{
		IsCheckingStableBoard = true;
		
		yield return waitEndFrame;
		
		bool stable = false;
		float waitTime = 0.3f;
		bool hasDelayedActionInProgress = false;
		
		while (!stable || waitTime >= 0f)
		{
			stable = true;
			hasDelayedActionInProgress = false;
			
			if (unstableLock > 0) 
			{
				stable = false;
				IsBoardStable = false;
			}
			else 
			{
				NormalTile tileIterator;
				
				for (int i = 0; i < boardData.NumRows; ++i)
				{
					for (int j = 0; j < boardData.NumColumns; ++j)
					{
						tileIterator = boardData[i, j].Tile as NormalTile;
						
						if (tileIterator != null) 
						{
							if (tileIterator.IsMoving || tileIterator.IsTileSwitching || tileIterator.IsDestroying)
							{
								stable = false;
								IsBoardStable = false;
								
								break;
							}
							else if (tileIterator is TimeBombTile || tileIterator.IsFrozen())
							{
								hasDelayedActionInProgress = true;
							}
						}
					}
					
					if (!stable) {
						break;
					}
				}
			}

			if (stable)
			{
				if (waitTime >= 0f) {
					waitTime -= Time.deltaTime;
				}
				
				if (hasDelayedActionInProgress) {
					waitTime = 0.3f;
				}

				if ( matchesFinder.FindMatches() )
				{
					DestroyLastFoundMatches();
					stable = false;
					waitTime = 0.3f;
				}

				// Exit the loop now while we're out of waitTime and the board is stable; not the next frame at the end of this while because things
				// can start changing in one frame. (like a directional combine + bomb switch can be put in motion)
				if (waitTime < 0f) {
					break;
				}
			} 
			else 
			{
				waitTime = 0.3f;
//				Debug.LogWarning("Reset 3: " + Time.frameCount);
			}
			
			//TODO: we're trying at every frame because there were some issues when OnStableBoard was being called during certain events on the table.
//			yield return new WaitForSeconds(0.1f);

			yield return waitEndFrame;
			
//			Debug.LogWarning("CheckStableBoard!!");
		}

		IsCheckingStableBoard = false;
		
		if ( !IsBoardStable ) 
		{
			IsBoardStable = true;
			
//			BugTrapStableBoard(waitTime);
			
			if (OnStableBoard != null) {
				OnStableBoard();
			}
			
			if (OnPostStableBoard != null)
			{
				OnPostStableBoard();
			}
		}
	}
	
	
	protected GameObjectEvent WinLoseRaiseEvent;
	protected bool freeFalling = false;
	
	protected void StableBoardLose()
	{	

		levelWon = false;
		
		OnStableBoard -= StableBoardLose;
		
		WinLoseRaiseEvent = OnLoseFinished;
		
		if (winConditions.GetType() == typeof(WinScore)) {
			callFreeFallEvent = true;
			StartCoroutine(FreeFall());
		}
		else 
		{
			SendEndGameAnalytics();
			//OnLoseFinished.RaiseEvent();
			ShowPucharsePropertyPage();
		}
	}



	
	protected void StableBoardWin()
	{
		levelWon = true;

		OnStableBoard -= StableBoardWin;
		
		loseConditions.DoWin();
		WinLoseRaiseEvent = OnWinFinished;
		
		if (!freeFalling) {
			callFreeFallEvent = true;
			StartCoroutine(FreeFall());
		}

		//OnWinFinished.RaiseEvent();
	}
		
	bool callFreeFallEvent = true;
	
	IEnumerator DestroyAllTriggerTiles()
	{
		List<TriggerTile> allTriggerTiles = new List<TriggerTile>(65);
		bool keepLooking = true;
		
		while (keepLooking || !IsBoardStable) 
		{
			allTriggerTiles.Clear();
			
			boardData.ApplyActionToAll((boardPiece) => {
				TriggerTile tile = boardPiece.Tile as TriggerTile;
				if (tile != null && tile.IsDestructible && !tile.IsDestroying) {
					allTriggerTiles.Add(tile);
				}
			});
			
			if (allTriggerTiles.Count > 0) {
				if (callFreeFallEvent) 
				{					
					callFreeFallEvent = false;
					OnFreeFall.RaiseEvent();
					
					yield return new WaitForSeconds(2f);
				}
				
				int index = Random.Range(0, allTriggerTiles.Count);
				TriggerTile chosenTile = allTriggerTiles[index];
				allTriggerTiles.RemoveAt(index);
				
				chosenTile.Destroy();
				IsBoardStable = false;
				TryCheckStableBoard();
				yield return new WaitForSeconds(1.5f);

				keepLooking = true;
			}
			else {
				keepLooking = false;
				yield return null;
			}
		}
	}
	
	IEnumerator FreeFall()
	{
		freeFalling = true;
		
		yield return StartCoroutine(DestroyAllTriggerTiles());
		
		LoseMoves loseConditionMoves = loseConditions as LoseMoves;
		
		if (loseConditionMoves == null || loseConditionMoves.RemainingMoves == 0) {
			OnStableBoard += FreeFallFinished;
			IsBoardStable = false;
			TryCheckStableBoard();
			yield break;
		}
	
		List<Match3BoardPiece> allNormalBoardPieces = new List<Match3BoardPiece>();
		SoundEffectController sndDirectionalCreate = SoundManager.Instance["winter_create_sfx"];
		
		float waitTime = 0.5f;
		while (loseConditionMoves.RemainingMoves > 0) 
		{
			if (callFreeFallEvent) 
			{
				callFreeFallEvent = false;
				OnFreeFall.RaiseEvent();
				
				yield return new WaitForSeconds(1.5f);
			}
			
			yield return new WaitForSeconds(waitTime);
			waitTime = Mathf.Clamp(waitTime - 0.03f, 0.03f, 0.5f);
				
			allNormalBoardPieces.Clear();
			
			boardData.ApplyActionToAll((boardPiece) => {
				Match3Tile tile = boardPiece.Tile as Match3Tile;
				if (tile != null && !tile.IsMoving && tile.IsDestructible && !tile.IsDestroying && tile.GetType() == typeof(NormalTile)) {
					allNormalBoardPieces.Add(boardPiece as Match3BoardPiece);
				}
			});
			
			loseConditions.NewMove();
			loseConditionMoves.RemainingMoves--; //because the lose condition is paused at this time
			
			if (allNormalBoardPieces.Count <= 0) {
				continue;
			}
			
			int index = Random.Range(0, allNormalBoardPieces.Count);
			Match3BoardPiece chosenPiece = allNormalBoardPieces[index];
			Match3Tile chosenTile = chosenPiece.Tile as Match3Tile;
			allNormalBoardPieces.RemoveAt(index);
			
			chosenPiece.Tile = (boardRenderer as Match3BoardRenderer).SpawnSpecificTileAt(chosenPiece.BoardPosition.row,
				chosenPiece.BoardPosition.col, (Random.Range(0, 2) == 0) ? typeof(RowDestroyTile) : typeof(ColumnDestroyTile), TileColorType.None);
			(chosenPiece.Tile as Match3Tile).TileColor = chosenTile.TileColor;
			(chosenPiece.Tile as DirectionalDestroyTile).UpdateMaterial();
			Destroy(chosenTile.gameObject);
			ScoreSystem.Instance.AddScore(TweaksSystem.Instance.intValues["MovesScoreMultiplier"], false);
			
			SoundManager.Instance.PlayOneShot(sndDirectionalCreate);
			
			if (loseConditionMoves.RemainingMoves == 0) {
				break;
			}

			TryCheckStableBoard();
		}

		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(DestroyAllTriggerTiles());
		
		OnStableBoard += FreeFallFinished;
		IsBoardStable = false;
		TryCheckStableBoard();
	}
	
	public void SendEndGameAnalytics() 
	{
		/* 
		int remainingPlayAmount = winConditions.CachedRemainingPlayAmount < 0 ? 0 : winConditions.CachedRemainingPlayAmount;
		string usedPowerUps = ManaItemHolder.GetNumUsedPowerUps().ToString();
		if (levelWon)
		{
			AnalyticsBinding.LogEventGameAction(GetLevelType(), "success", remainingPlayAmount.ToString(), usedPowerUps, MaleficentBlackboard.Instance.level);
			AnalyticsBinding.LogEventGameAction(GetLevelType(), "star_rating", (winConditions as WinScore).StarsReached.ToString(),ScoreSystem.Instance.GetPointsShortOfNextStar().ToString(), MaleficentBlackboard.Instance.level);
			
			SetLevelStartAnalyticTimeEvent(true);
		}
		else {
			AnalyticsBinding.LogEventGameAction(GetLevelType(), "fail", 
				Mathf.FloorToInt((winConditions as WinScore).CalculateObjectiveProgress() * 100f).ToString(), 
				usedPowerUps, MaleficentBlackboard.Instance.level);
		}
		*/
	}
	
	protected void FreeFallFinished()
	{
		OnStableBoard -= FreeFallFinished;
		freeFalling = false;
		
		SendEndGameAnalytics();


		WinLoseRaiseEvent.RaiseEvent();

		//ShowPucharsePropertyPage();
	}


	
	void OnDestroy() {
		if (boardHolesMask != null) {
			Debug.Log("[Match3BoardGameLogic] Destroying the board holes mask texture...");
			Destroy(boardHolesMask);
		}
		
		if (BoardAnimations != null) {
			BoardAnimations.OnTilesSwitchAnimFinished -= OnTilesSwitchAnimFinished;
		}
		Match3Tile.OnTileMovingChanged -= OnTileMovingChanged;
		
		gameSoundEventsHandler.UnregisterSoundEvents();
		gameSoundEventsHandler = null;
		
		OnStableBoard -= StableBoardLose;
		OnStableBoard -= StableBoardWin;
		OnStableBoard -= FreeFallFinished;

		instance = null;
	}

	//public PlayMakerFSM buyFSM;
	//public PlayMakerFSM fsm;
	void ConfirmDelegateFunc()
	{

	}

	IEnumerator ShowBuyTips(float fSec )
	{
		yield return new WaitForSeconds(fSec);
		//SDKTipsWindowController.getInstance().PopWindow(Language.Get("TIPSFOR_PROPERTY_BUY") ,ConfirmDelegateFunc,null);
	}
	public void ShowPucharsePropertyPage()
	{
		/* 
	//	ShopPanel.getInstance().gameObject.SetActive(true);
		//buyFSM = GameObject.Find ("Buy Panel");
		Chalinger.getInstance().AddLevelChanTimes(MaleficentBlackboard.Instance.level.ToString());
		if(Chalinger.getInstance().GetLevelChanTimes(MaleficentBlackboard.Instance.level.ToString())>=3)
		{
			buyFSM.SendEvent("Buy");
//			GameGiftBuy.instanse.clikcbuy();
			ShopUINotifyer.getInstance().IndexUI=0;
			ShopUINotifyer.getInstance().Notify();
			StartCoroutine(ShowBuyTips(2));
			m_bShowLosePanel=true;
		}
		else
		{
			//WinLoseRaiseEvent.RaiseEvent();
			OnLoseFinished.RaiseEvent();
		}
		*/
	}

	public  bool m_bShowLosePanel=false;
	public void ShowLoseEvent()
	{
		if(m_bShowLosePanel==true)
		{
			OnLoseFinished.RaiseEvent();
			m_bShowLosePanel=false;
		}
	}
	
}

