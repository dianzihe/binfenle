using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Match3AIPlayer : MonoBehaviour 
{
	public bool useOnlyRandomMoves = false;
	public bool useOnlyPossibleMoves = true;
	
	public int maxSimultaneousMoves = 10;
	public float delayBetweenEachMove = 0.3f;
	
	public float maxDelayBetweenMoves = 0.6f;
	
	public float tryTimeUntilDecideNoMatchMade = 4f;
	public float waitTimeBeforeNextRetry = 3f;
	protected float lastTimeMatchFound;
	
	protected bool gameStarted = false;
	
	protected Match3BoardGameLogic gameLogic;
	protected List<Match3BoardPiece> validBoardPieces = new List<Match3BoardPiece>(65);
	protected List<Match3Tile> possibleTargetTiles = new List<Match3Tile>(5);
	
	protected PossibleMatchesFinder possibleMatchFinder;
		
	
	void Awake() 
	{
		if (enabled) 
		{
			Match3BoardGameLogic.OnStartGame += OnGameStarted;
			Match3BoardGameLogic.OnDestroyLastFoundMatches += OnDestroyLastFoundMatches;
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		gameLogic = Match3BoardGameLogic.Instance;				
	}
	
	public void OnGameStarted() 
	{
		possibleMatchFinder = new PossibleMatchesFinder(gameLogic.boardData);
//		possibleMatchFinder.debugEnabled = true;
		
		validBoardPieces.Clear();
		
		// Collect valid board pieces to pick from when playing.
		gameLogic.boardData.ApplyActionToAll((piece) => 
		{
			if ( !piece.IsBorderPiece && !piece.IsEmpty ) 
			{
				validBoardPieces.Add(piece as Match3BoardPiece);
			}
		});

		gameStarted = true;
		lastTimeMatchFound = Time.time;
		
		// Start the "mighty" AI logic
		StartCoroutine("UpdateAI");
		
		StartCoroutine("CheckTimeElapsedSinceLastMatchFound");
	}
	
	public void OnDestroyLastFoundMatches()
	{
		lastTimeMatchFound = Time.time;
	}
	
	public IEnumerator CheckTimeElapsedSinceLastMatchFound()
	{
		while(true) 
		{
			// Don't do anything if game is over
			if (MustStopUpdate) 
			{
				yield return new WaitForSeconds(2f);
				
				continue;
			}

			if (Time.time - lastTimeMatchFound >= tryTimeUntilDecideNoMatchMade) 
			{
				Debug.LogWarning("[RandomAIPlayer] No more matches destroyed for too long! Taking a break...");
				StopCoroutine("UpdateAI");
				
				yield return new WaitForSeconds(waitTimeBeforeNextRetry);
				
				lastTimeMatchFound = Time.time;
				StartCoroutine("UpdateAI");
			}
			
			yield return new WaitForSeconds(1f);
		}
	}
	
	public IEnumerator UpdateAI()
	{
		Debug.LogWarning("[RandomAIPlayer] Started AI Player...");
		
		while(true)
		{
			// Don't do anything if game is over
			if (MustStopUpdate) 
			{
				yield return new WaitForSeconds(2f);

				continue;
			}

			for(int i = 0; i < maxSimultaneousMoves; i++) 
			{
				if (MustStopUpdate) {
					break;
				}
				// Wait until we find and do a valid move.
				yield return StartCoroutine( FindAndDoMove() );
				
				yield return new WaitForSeconds(delayBetweenEachMove);
			}

			yield return new WaitForSeconds(Random.Range(delayBetweenEachMove, maxDelayBetweenMoves));
		}
	}
	
	protected bool MustStopUpdate {
		get {
			return gameLogic.IsGameOver || !gameStarted || !TileSwitchInput.Instance.IsEnabled || !enabled;
		}
	}

	public IEnumerator FindAndDoMove()
	{
//		while(true) 
//		{
		Match3BoardPiece boardPiece = null;
		
		// Try to find a possible match to move
		bool foundPossibleMatchLastTime = possibleMatchFinder.FindFirstPossibleMatch();

		if ( foundPossibleMatchLastTime && !useOnlyRandomMoves) {
			boardPiece = possibleMatchFinder.lastFoundIsolatedTile.BoardPiece as Match3BoardPiece;
		}
		else if ( !useOnlyPossibleMoves ) 
		{
			foundPossibleMatchLastTime = false;
			// Choose a random board piece to move from because no possible match found last time
			boardPiece = validBoardPieces[Random.Range(0, validBoardPieces.Count)];
		}
		
		if (boardPiece != null && boardPiece.Tile != null && boardPiece.Tile.IsUserMoveable)
		{
			// Try to move using the last found possible match first
			if (foundPossibleMatchLastTime) 
			{
				if ( !TryPossibleMatchMove() ) {
//						Debug.LogWarning("AI failed possible match move...");
				}
				yield break;
			}
			else if ( !useOnlyPossibleMoves && TryRandomMoveAround(boardPiece) )
			{
				// Found a random valid move (not necessary a matching move).
				yield break;
			}
			
		}
		
		yield return null;
//		}
	}

	public bool TryPossibleMatchMove() 
	{
		// Knowing the isolated tile, determine the target coord where the isolated tile needs to be moved to make the match.
		Match3Tile[] lastFoundMatch = possibleMatchFinder.foundPossibleMatch;
		Match3Tile[] otherMatchTiles = new Match3Tile[2];
		int j = 0;
		
		// Determine the other 2 tiles (non-isolated) in the "otherMatchTile" array.
		for(int i = 0; i < possibleMatchFinder.numFoundPossibleMatches; i++) 
		{
			if (lastFoundMatch[i] != possibleMatchFinder.lastFoundIsolatedTile)
			{
				//TODO: for some reason an instance becomes null here, but in the same frame inside the possible matches finder it's not null.
				//That's why i've put this safety check here.
				if (lastFoundMatch[i] == null) {
					return false;
				}
				otherMatchTiles[j++] = lastFoundMatch[i];
			}
		}
		
		// Determine the board coord where the isolated tile must move to match.
		BoardCoord isolatedTargetPos = possibleMatchFinder.lastFoundIsolatedTile.BoardPiece.BoardPosition;
//		Debug.Log(Time.frameCount + " otherMatchTiles[0] = " + otherMatchTiles[0]);
//		Debug.Log(Time.frameCount + " otherMatchTiles[1] = " + otherMatchTiles[1]);
//		Debug.Log(Time.frameCount + " lastFoundIsolatedTile = " + possibleMatchFinder.lastFoundIsolatedTile);
		
		BoardCoord otherTilesOffset;
		if (j == 1) {
			// When 2 special trigger tiles are found as a possible match, there is only 1 other tile besides the "isolated" tile in that match.
			otherTilesOffset = isolatedTargetPos - otherMatchTiles[0].BoardPiece.BoardPosition;
		} else {
			otherTilesOffset = otherMatchTiles[0].BoardPiece.BoardPosition - otherMatchTiles[1].BoardPiece.BoardPosition;
		}
		
		if (otherTilesOffset.row == 0) {
			// Check if the isolated tile is already on the same row, then we have to target the col. by recalculating the offset between the columns.
			if (possibleMatchFinder.lastFoundIsolatedTile.BoardPiece.BoardPosition.row == otherMatchTiles[0].BoardPiece.BoardPosition.row) 
			{
				int colOffset = otherMatchTiles[0].BoardPiece.BoardPosition.col - possibleMatchFinder.lastFoundIsolatedTile.BoardPiece.BoardPosition.col;
				colOffset = Mathf.Clamp(colOffset, -1, 1);
				isolatedTargetPos.col += colOffset;
			}
			else {				
				isolatedTargetPos.row = otherMatchTiles[0].BoardPiece.BoardPosition.row;
			}
		}
		else 
		{
			// Check if the isolated tile is already on the same column, then we have to target the row by recalculating the offset between the rows.
			if (possibleMatchFinder.lastFoundIsolatedTile.BoardPiece.BoardPosition.col == otherMatchTiles[0].BoardPiece.BoardPosition.col) 
			{
				int rowOffset = otherMatchTiles[0].BoardPiece.BoardPosition.row - possibleMatchFinder.lastFoundIsolatedTile.BoardPiece.BoardPosition.row;
				rowOffset = Mathf.Clamp(rowOffset, -1, 1);
				isolatedTargetPos.row += rowOffset;
			}
			else {
				isolatedTargetPos.col = otherMatchTiles[0].BoardPiece.BoardPosition.col;
			}
		}
		
		Match3BoardPiece targetPiece = gameLogic.boardData[isolatedTargetPos] as Match3BoardPiece;
		if (TileSwitchInput.Instance.IsEnabled && targetPiece != null && targetPiece.Tile != null && targetPiece.Tile.IsUserMoveable) 
		{
			// Try to move the isolated tile to make the match.
			if ( !gameLogic.TryToMoveTile(possibleMatchFinder.lastFoundIsolatedTile, targetPiece.Tile) ) 
			{
				// If for some reason we failed the above move, try a random move.
				if ( !useOnlyPossibleMoves ) 
				{
					if ( TryRandomMoveAround(possibleMatchFinder.lastFoundIsolatedTile.BoardPiece as Match3BoardPiece) ) {
						return true;
					}
				}
				else {
					return false;
				}
			}
			else {
				// Possible match successed
				return true;
			}
		}
		
		return false;
	}
		
	public bool TryRandomMoveAround(Match3BoardPiece piece) 
	{
		// Get all valid neighbor tiles for the current starting board piece position
		GetValidTargetNeighborTilesFor(piece);
		
		if ( possibleTargetTiles.Count > 0 && TileSwitchInput.Instance.IsEnabled &&
			gameLogic.TryToMoveTile(piece.Tile, possibleTargetTiles[Random.Range(0, possibleTargetTiles.Count)]) ) 
		{
			return true;
		}
		
		return false;
	}
	
	public void GetValidTargetNeighborTilesFor(Match3BoardPiece piece)
	{
		possibleTargetTiles.Clear();
		
		for(int i = 0; i < (int)Match3BoardPiece.LinkType.Count; i += 2) 
		{
			Match3BoardPiece neighbor = piece.GetNeighbor( (Match3BoardPiece.LinkType)i );
			
			if (neighbor != null && neighbor.Tile != null && neighbor.Tile.IsUserMoveable) {
				possibleTargetTiles.Add(neighbor.Tile as Match3Tile);
			}
		}
	}
	
	void OnDestroy() 
	{
		Match3BoardGameLogic.OnStartGame -= OnGameStarted;
		Match3BoardGameLogic.OnDestroyLastFoundMatches -= OnDestroyLastFoundMatches;
	}
}
