using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchesFinder 
{
	public delegate void MatchesFinderEventHandler(MatchesFinder sender);
	
//	public bool useBugTrap = false;
	
	/// <summary>
	/// This event is fired after <see cref="FindMatches"/> is called and any match has been found on the board.
	/// The results are stored in "lastFoundMathes" list.
	/// </summary>
	public static event MatchesFinderEventHandler OnBoardMatchesFound;
		
	/// <summary>
	/// Reference to the board data on which matches will be searched.
	/// </summary>
	private BoardData boardData;

	public List<Match3Tile> lastFoundMatches = new List<Match3Tile>(32);
	protected List<Match3Tile> matchesBatch = new List<Match3Tile>(10);


	public MatchesFinder() { }
	
	public MatchesFinder(BoardData _boardData) 
	{
		Board = _boardData;
	}
	
	public BoardData Board 
	{
		get {
			return boardData;
		}
		set {
			boardData = value;
		}
	}
	private bool CanUpgrade(NormalTile thisTile, NormalTile thatTile)
	{
		if (thisTile.TileColor == thatTile.TileColor)
			return true;
		return false;
		//return (thisTile.value != maxValue && thisTile.power == thatTile.power && !thisTile.upgradedThisTurn && !thatTile.upgradedThisTurn);
	}

	public bool FindDQ2MatchesDown()
	{
		Match3Tile targetTile = null;

		lastFoundMatches.Clear();
		matchesBatch.Clear();

		// Clear the match count status for all tiles before looking new matches.
		boardData.ApplyActionToAll((boardPiece) =>
		{
			Match3Tile tile = boardPiece.Tile as Match3Tile;

			if (tile != null)
			{
				tile.ResetMatchCountDirections();
			}
		});

		// Vertical matches check 
		//rowIdx  NumRows  
		//colIdx  NumColumns 
		for (int colIdx = 0; colIdx < boardData.NumColumns; colIdx++)
		{
			targetTile = null;
			// Reset the targetTile that will be used to detect batches of matches. 
			for (int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++)
			{
				CheckMatch(boardData[rowIdx, colIdx].Tile as Match3Tile, ref targetTile, true);
			}
			matchesBatch.Clear();
		}

		if (lastFoundMatches.Count > 0)
		{
			if (OnBoardMatchesFound != null)
			{
				OnBoardMatchesFound(this);
			}

		}

		return lastFoundMatches.Count > 0;
	}

	public bool FindDQ2MatchesUp()
	{
		Match3Tile targetTile = null;

		lastFoundMatches.Clear();
		matchesBatch.Clear();

		// Clear the match count status for all tiles before looking new matches.
		boardData.ApplyActionToAll((boardPiece) =>
		{
			Match3Tile tile = boardPiece.Tile as Match3Tile;

			if (tile != null)
			{
				tile.ResetMatchCountDirections();
			}
		});

		// Vertical matches check rowIdx colIdx NumRows NumColumns .
		for (int colIdx = 0; colIdx < boardData.NumColumns; colIdx++)
		{
			targetTile = null;
			for (int rowIdx = boardData.NumRows - 1; rowIdx > 0; rowIdx--)
			{
				CheckMatch(boardData[rowIdx, colIdx].Tile as Match3Tile, ref targetTile, true);
			}

			matchesBatch.Clear();
		}

		if (lastFoundMatches.Count > 0)
		{
			if (OnBoardMatchesFound != null)
			{
				OnBoardMatchesFound(this);
			}

		}

		return lastFoundMatches.Count > 0;
	}


	public bool FindDQ2MatchesLeft()
	{
		Match3Tile targetTile = null;

		lastFoundMatches.Clear();
		matchesBatch.Clear();

		// Clear the match count status for all tiles before looking new matches.
		boardData.ApplyActionToAll((boardPiece) =>
		{
			Match3Tile tile = boardPiece.Tile as Match3Tile;

			if (tile != null)
			{
				tile.ResetMatchCountDirections();
			}
		});

		//rowIdx  NumRows  
		//colIdx  NumColumns 
		for (int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++)
		{
			targetTile = null;
			// Reset the targetTile that will be used to detect batches of matches. 
			for (int colIdx = boardData.NumColumns - 1 ; colIdx > 0; colIdx--)
			{
				CheckMatch(boardData[rowIdx, colIdx].Tile as Match3Tile, ref targetTile, true);
			}
			matchesBatch.Clear();
		}

		if (lastFoundMatches.Count > 0)
		{
			if (OnBoardMatchesFound != null)
			{
				OnBoardMatchesFound(this);
			}

		}

		return lastFoundMatches.Count > 0;
	}

	public bool FindDQ2MatchesRight()
	{
		Match3Tile targetTile = null;

		lastFoundMatches.Clear();
		matchesBatch.Clear();

		// Clear the match count status for all tiles before looking new matches.
		boardData.ApplyActionToAll((boardPiece) =>
		{
			Match3Tile tile = boardPiece.Tile as Match3Tile;

			if (tile != null)
			{
				tile.ResetMatchCountDirections();
			}
		});

		//rowIdx  NumRows  
		//colIdx  NumColumns 
		for (int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++)
		{
			targetTile = null;
			for (int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) {
				CheckMatch(boardData[rowIdx, colIdx].Tile as Match3Tile, ref targetTile, true);
			}

			matchesBatch.Clear();
		}

		if (lastFoundMatches.Count > 0)
		{
			if (OnBoardMatchesFound != null)
			{
				OnBoardMatchesFound(this);
			}

		}

		return lastFoundMatches.Count > 0;
	}

	public bool FindMatches() 
	{
//		Debug.LogWarning("[Match3BoardGameLogic] FindBoardMatches()...");
		Match3Tile targetTile = null;

		lastFoundMatches.Clear();
		matchesBatch.Clear();
		
		// Clear the match count status for all tiles before looking new matches.
		boardData.ApplyActionToAll((boardPiece) => 
		{
			Match3Tile tile = boardPiece.Tile as Match3Tile;
			
			if (tile != null) {
				tile.ResetMatchCountDirections();
			}
		});
		
		// Vertical matches check.
		for(int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) {
			targetTile = null;
			// Reset the targetTile that will be used to detect batches of matches. 
			for(int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++) {
				// Check each tile on this row with the current targetTile for match.
				// If the targetTile is null, meaning we don't have a current target tile to match with, we
				// set targetTile to the current tile were on (boardData[rowIdx, colIdx].Tile).
				CheckMatch(boardData[rowIdx, colIdx].Tile as Match3Tile, ref targetTile, true);
			}
			
			if (matchesBatch.Count >= 2) {
				CollectNewFoundMatches(matchesBatch, true);
			}
			matchesBatch.Clear();
		}
		
		// Horizontal matches check.
		for(int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++) {
			targetTile = null;
			for(int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) {
				CheckMatch(boardData[rowIdx, colIdx].Tile as Match3Tile, ref targetTile, false);
			}

			if (matchesBatch.Count >= 3) {
				CollectNewFoundMatches(matchesBatch, false);
			}
			matchesBatch.Clear();
		}
		
		if (lastFoundMatches.Count > 0)
		{
			if (OnBoardMatchesFound != null) {
				OnBoardMatchesFound(this);
			}
			
//			if (useBugTrap)
//			{
//				for(int i = 0; i < lastFoundMatches.Count; i++) 
//				{
//					if (lastFoundMatches[i] == null || lastFoundMatches[i].IsMoving || !lastFoundMatches[i].HasReachedBoardPieceArea() || lastFoundMatches[i].IsDestroying)
//					{
//						Debug.LogWarning("ALARM!!! Invalid tile found in matches: " + lastFoundMatches[i] + " frame: " + Time.frameCount);
//						Debug.LogWarning("IsMoving = " + lastFoundMatches[i].IsMoving);
//						Debug.LogWarning("HasReachedBoardPieceArea = " + lastFoundMatches[i].HasReachedBoardPieceArea());
//						Debug.LogWarning("IsDestroying = " + lastFoundMatches[i].IsDestroying);
//						Debug.Break();
//					}
//				}
//			}
		}
		
		return lastFoundMatches.Count > 0;
	}
	
	protected void CheckMatch(Match3Tile currentTile, ref Match3Tile targetTile, bool isVerticalPass) 
	{
		//		Debug.Log("Current tile: " + currentTile + "\n" + "targetTile: " + targetTile);
		if (targetTile == null || !targetTile.CanBeMatched)
		{
			if (currentTile != null && currentTile.CanBeMatched)
			{
				targetTile = currentTile;
				if (matchesBatch.Count > 0)
				{
					matchesBatch.Clear();
				}
				matchesBatch.Add(targetTile);
			}
		}
		else if (currentTile != null && currentTile.IsMatchWith(targetTile))
		{
			// If we found a matching tile with the targetTile add it to our temporary matches buffer.
			matchesBatch.Add(currentTile);
			currentTile.CanBeMatched = false;
			CollectNewFoundMatches(matchesBatch, isVerticalPass);
			matchesBatch.Clear();
		}
		else if (currentTile != null && !currentTile.IsMatchWith(targetTile))
		{
			targetTile = currentTile;
			matchesBatch.Clear();
		}
		else { 
		}
	}

	protected void CollectNewFoundMatches(List<Match3Tile> newMatches, bool isVerticalPass) 
	{
		Logic.EventCenter.Log(LOG_LEVEL.WARN, "[MatchesFinder] CollectNewFoundMatches -================");
		/*
		if (isVerticalPass) {
			for(int i = 0; i < newMatches.Count; i++) {
				newMatches[i].matchCount[(int)TileMatchDirection.Left] = 0;
				newMatches[i].matchCount[(int)TileMatchDirection.Right] = 0;
//				newMatches[i].ResetMatchCountDirections();
				
				newMatches[i].matchCount[(int)TileMatchDirection.Top] = i;
				newMatches[i].matchCount[(int)TileMatchDirection.Bottom] = newMatches.Count - 1 - i;
			}
		} else {
			for(int i = 0; i < newMatches.Count; i++) {
				newMatches[i].matchCount[(int)TileMatchDirection.Left] = i;
				newMatches[i].matchCount[(int)TileMatchDirection.Right] = newMatches.Count - 1 - i;
			}
		}
		*/
		for (int i = 0; i < newMatches.Count; i++)
		{
			Logic.EventCenter.Log(LOG_LEVEL.WARN, "[MatchesFinder] CollectNewFoundMatches -> " + newMatches[i].name);

		}
		lastFoundMatches.AddRange(newMatches);

	}
}
