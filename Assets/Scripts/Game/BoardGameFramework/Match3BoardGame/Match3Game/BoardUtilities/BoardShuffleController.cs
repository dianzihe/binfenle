using UnityEngine;
using Holoville.HOTween;
using System.Collections;
using System.Collections.Generic;

public class BoardShuffleController : MonoBehaviour
{	
	protected static BoardShuffleController instance = null;
	
	protected int minNumParticles = 5;
	protected int maxNumParticles = 10;
	
	public GameObject prefabEffectReshuffle;
	
	private bool isBoardReshuffling = false;

	/// <summary>
	/// This event occurs when the board doesn't have any more possible matches and it might need a re-shuffle.
	/// </summary>
	public static event System.Action OnBoardShuffleRequired;
	public static event System.Action OnBoardShuffleFinished;
	public static event System.Action<bool> OnBoardShuffleWithMatch;
	
	public GameObjectEvent OnReshuffleRequired = new GameObjectEvent(null, "SendFsmEvent", "OnBoardReshuffleStart");
	public GameObjectEvent OnReshuffleFinished = new GameObjectEvent(null, "SendFsmEvent", "OnBoardReshuffleFinish");
	
	protected Transform effectReshuffle;
	protected ParticleSystem cachedParticleSystem;
		
	protected BoardData board;
	
	protected MatchesFinder matchesFinder;
	protected PossibleMatchesFinder possibleMatchesFinder;
	protected MatchesUndoer matchesUndoer;
	protected PossibleMatchGenerator possibleMatchGenerator;
	
	protected WaitForSeconds waitPollTime;
	protected bool hasPossibleMatchesCheckPending;
	
	protected List<Match3BoardPiece> piecesToReshuffle = new List<Match3BoardPiece>(32);
	
	protected bool matchFailed = false;
	
	void Awake() 
	{
		instance = this;
		
		waitPollTime = new WaitForSeconds(0.5f);
	}
	
	public bool IsBoardReshuffling {
		get {
			return isBoardReshuffling;
		}
		set {
			isBoardReshuffling = value;
		}
	}	
	
	public static BoardShuffleController Instance 
	{
		get {
			return instance;
		}
		set {
			instance = value;
		}
	}
	
	/// <summary>
	/// Defined to also give this component the ability to be enabled/disabled.
	/// </summary>
	void Start()
	{
		board = Match3BoardGameLogic.Instance.boardData;
		
		// Register to the board's OnStable event.
		Debug.Log("[BoardShuffleController] Registering to the board's OnStableBoard event...");
		Match3BoardGameLogic.OnStableBoard += OnStableBoardEvent;
		
		matchesFinder = new MatchesFinder(board);
		possibleMatchesFinder = new PossibleMatchesFinder(board);
		matchesUndoer = new MatchesUndoer(board);
		possibleMatchGenerator = new PossibleMatchGenerator(board);
	}

	public void OnStableBoardEvent()
	{
//		Debug.Log("[BoardShuffleController] OnStableBoardEvent called: " + Time.frameCount);
		
		if ( !hasPossibleMatchesCheckPending ) 
		{
			hasPossibleMatchesCheckPending = true;
			StartCoroutine( DoPossibleMatchesCheck() );
		}
	}
	
	protected IEnumerator DoPossibleMatchesCheck()
	{
		while(!enabled || Match3BoardGameLogic.Instance.IsGameOver || Match3BoardGameLogic.Instance.IsCheckingStableBoard || IsBoardReshuffling) 
		{
			//Debug.LogWarning("[BoardShuffleController] Waiting until possible matches checking can be done...");
			yield return waitPollTime;
		}

		// Check if there's at least one possible match on the board. If there's no possible match, check for safety if there's a pending match on the board.
		//bool boardHasPendingMatch = false;
		//if ( !possibleMatchesFinder.FindFirstPossibleMatch() && (!(boardHasPendingMatch = matchesFinder.FindMatches())) ) 
		if ( !possibleMatchesFinder.FindFirstPossibleMatch() && !matchesFinder.FindMatches() ) 
		{
//			Debug.LogWarning("[BoardShuffleController] No more possible matches! Re-shuffling...");
//			Debug.Break();
			RaiseBoardShuffleRequiredEvent();
		}
		else {
//			Debug.Log("[BoardShuffleController] Board still has possible matches (boardHasPendingMatch = " + boardHasPendingMatch + ")...");
		}
		
		hasPossibleMatchesCheckPending = false;
	}
		

	public void RaiseBoardShuffleRequiredEvent() 
	{
		RaiseBoardShuffleRequiredEvent(true);
	}

	public void RaiseBoardShuffleRequiredEvent(bool showText) 
	{
		Match3BoardGameLogic.Instance.unstableLock++;
		
		IsBoardReshuffling = true;
		
		if (OnBoardShuffleRequired != null) {
			OnBoardShuffleRequired();
		}

		if(showText)
			OnReshuffleRequired.RaiseEvent();
		
//		Debug.LogWarning("[BoardShuffleController] RaiseBoardShuffleRequiredEvent -> No more possible matches on the board...");
		
		board = Match3BoardGameLogic.Instance.boardData;

		// Collect all board pieces containing a normal tile to prepare for shuffling.
		board.ApplyActionToAll((boardPiece) => 
		{
			// Only re-shuffle normal tiles
			if (boardPiece.Tile != null && boardPiece.Tile.GetType() == typeof(NormalTile) && !boardPiece.Tile.IsDestroying)
			{
				piecesToReshuffle.Add(boardPiece as Match3BoardPiece);
				// Disable the tiles logic before starting the re-shuffle
				boardPiece.Tile.enabled = false;
			}
		});	

		// Disable tile switching input
		TileSwitchInput.Instance.DisableInput();

		StartReshuffleTilesAnim();
	}

	protected void StartReshuffleTilesAnim()
	{		
		// Start their re-shuffle tween animation.
		Match3Tile tile;
		
		// Spawn a reshuffle effect that will get auto-destroyed.
		effectReshuffle = (GameObject.Instantiate(prefabEffectReshuffle) as GameObject).transform;
		effectReshuffle.parent = Match3BoardGameLogic.Instance.boardVisualContainer;
		cachedParticleSystem = effectReshuffle.GetComponent<ParticleSystem>();
		
		// Auto destroy the effect.
		GameObject.Destroy(effectReshuffle.gameObject, 4.0f);
		
		Tweener lastTween = null;
//		if(piecesToReshuffle.Count == 0) {
//			Debug.LogWarning("[piecesToReshuffle.Count] " + piecesToReshuffle.Count);	
//		}
		
		for(int i = 0; i < piecesToReshuffle.Count; i++)
		{
			tile = piecesToReshuffle[i].Tile as Match3Tile;
			if (tile != null) 
			{
				// Stored the tile position in a separate vector because so the below lambda method will receive a copy of this value to execute correctly.
				Vector3 tilePos = tile.cachedTransform.position;
				// Offset effect to bring it in front of the tiles.
				tilePos.z -= 1f;
				
				lastTween = HOTween.To(tile.tileModelTransform, 0.5f, new TweenParms()
													  .Prop("localScale", Vector3.zero)
													  .Ease(EaseType.EaseOutQuad)
													  .Delay(0.9f)
													  .OnStart(() => {
															if (effectReshuffle != null) 
															{
																// Position the effect
																effectReshuffle.position = tilePos;
																cachedParticleSystem.Emit(Random.Range(minNumParticles, maxNumParticles));
															}
														}));
				
//				if (lastTween == null) {
//					Debug.LogWarning("Last tween is NULL!!!");
//				}
			}
//			else {
//				Debug.LogWarning("[StartReshuffleTilesAnim] Boardpiece has NULL tile = " + piecesToReshuffle[i]);
//			}
		}
		
		if (lastTween != null) 
		{
			lastTween.ApplyCallback(CallbackType.OnComplete, () => 
			{		
				ReshuffleTiles();
			});
		}
//		else {
//			Debug.LogWarning("[StartReshuffleTilesAnim] Error lastTween is NULL");
//		}
	}

	protected void ReshuffleTiles()
	{
		// Re-shuffle tiles
		for(int i = 0; i < piecesToReshuffle.Count; i++) 
		{
			int randomIdx = Random.Range(0, piecesToReshuffle.Count);
			
			if (randomIdx != i)
			{
				// Switch tiles
				Match3Tile tempTile = piecesToReshuffle[i].Tile as Match3Tile;
				piecesToReshuffle[i].Tile = piecesToReshuffle[randomIdx].Tile;
				piecesToReshuffle[randomIdx].Tile = tempTile;
			}
		}
		
		int numTries = 0;
		matchesUndoer.OnNewTileSpawned = (newTile) => 
		{
			newTile.tileModelTransform.localScale = Vector3.zero;
			newTile.enabled = false;
		};

		possibleMatchGenerator.OnNewTileSpawned = (newTile) => 
		{
			newTile.tileModelTransform.localScale = Vector3.zero;
			newTile.enabled = false;
		};
		
		matchFailed = true;
		while ((numTries++) < 10 && (matchesUndoer.FindMatchesAndUndo(false) || numTries == 1))
		{
//			Debug.Log("Board shuffle re-tries: " + numTries);
			
			matchFailed = !possibleMatchesFinder.FindFirstPossibleMatch();
			if ( matchFailed )
			{
//				Debug.LogWarning("Forcing a rematch because re-shuffle failed.");
				matchFailed = !possibleMatchGenerator.GenerateMatch(false);
			}
		}
		
		matchesUndoer.OnNewTileSpawned = null;
		possibleMatchGenerator.OnNewTileSpawned = null;
		
		// Show hidden tiles.
		Match3Tile tile = null;
		Tweener lastTween = null;
		
//		if(piecesToReshuffle.Count == 0) {
//			Debug.LogWarning("[piecesToReshuffle.Count] " + piecesToReshuffle.Count);	
//		}
		
		for(int i = 0; i < piecesToReshuffle.Count; i++)
		{
			tile = piecesToReshuffle[i].Tile as Match3Tile;
			piecesToReshuffle[i].ResetTilePosition();
			
			if (tile != null)
			{
				// Position the effect
//				effectReshuffle.position = tile.cachedTransform.position;
				//cachedParticleSystem.Emit(Random.Range(minNumParticles, maxNumParticles));

				lastTween = HOTween.To(tile.tileModelTransform, 0.5f, new TweenParms()
													  .Prop("localScale", Vector3.one)
													  .Ease(EaseType.EaseOutQuad));
//				if (lastTween == null) {
//					Debug.LogWarning("Last tween is NULL!!!");
//				}
			}
//			else {
//				Debug.LogWarning("[ReshuffleTiles] In show tiles anim Boardpiece has NULL tile = " + piecesToReshuffle[i]);
//			}
		}
		
		if (lastTween != null)
		{
			lastTween.ApplyCallback(CallbackType.OnComplete, () =>
			{
				RaiseBoardShuffleFinishedEvent();
				OnReshuffleFinished.RaiseEvent();
			});
		}
		else
		{
			RaiseBoardShuffleFinishedEvent();
			OnReshuffleFinished.RaiseEvent();
//			Debug.LogWarning("[StartReshuffleTilesAnim] Error lastTween is NULL");
		}
	}
		
	/// <summary>
	/// Raises the board shuffle finished event when the re-shuffle finishes everything.
	/// </summary>
	protected void RaiseBoardShuffleFinishedEvent()
	{
		IsBoardReshuffling = false;
		Match3BoardGameLogic.Instance.unstableLock--;
		
		if (OnBoardShuffleFinished != null) {
			OnBoardShuffleFinished();
		}
		
		if (OnBoardShuffleWithMatch != null) {
			OnBoardShuffleWithMatch(!matchFailed);
		}

		// Re-enable tiles logic
//		if(piecesToReshuffle.Count == 0) {
//			Debug.LogWarning("[piecesToReshuffle.Count] " + piecesToReshuffle.Count);	
//		}

		for(int i = 0; i < piecesToReshuffle.Count; i++) 
		{
			if (piecesToReshuffle[i].Tile != null) {
				piecesToReshuffle[i].Tile.enabled = true;
			} 
//			else 
//			{
//				Debug.LogWarning("[RaiseBoardShuffleFinishedEvent] Boardpiece has NULL tile = " + piecesToReshuffle[i]);
//			}
		}
		
		piecesToReshuffle.Clear();
				
		// Re-enable tile switch input
		TileSwitchInput.Instance.EnableInput();
		

		
		// Reset the IsBoardStable flag because the board has been reshuffled. 
		Match3BoardGameLogic.Instance.IsBoardStable = false;
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}

//	void OnGUI()
//	{
//		GUILayout.BeginVertical();
//		{
//			if ( GUILayout.Button("\nReshuffle\n") )
//			{
//				RaiseBoardShuffleRequiredEvent();
//			}
//		}
//		GUILayout.EndVertical();
//	}
	
	void OnDestroy() 
	{
		Debug.Log("[BoardShuffleController] Unregistering to the board's OnStableBoard event...");
		Match3BoardGameLogic.OnStableBoard -= OnStableBoardEvent;
	}
}
