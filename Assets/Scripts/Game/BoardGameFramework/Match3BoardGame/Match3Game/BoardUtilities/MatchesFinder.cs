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
			
			if (matchesBatch.Count >= 3) {
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
		if ( targetTile == null || !targetTile.CanBeMatched ) 
		{
			// If there is no target tile to check matches with yet, try to set the currentTile as the targetTile and 
			// get back to the loop so we can move to the next tile to compare it with the new targetTile.
			if (currentTile != null && currentTile.CanBeMatched) 
			{
				targetTile = currentTile;
				if (matchesBatch.Count > 0) {
					matchesBatch.Clear();
				}
				matchesBatch.Add(targetTile);
			}
		} else if (currentTile != null && currentTile.IsMatchWith(targetTile)) {
			// If we found a matching tile with the targetTile add it to our temporary matches buffer.
			matchesBatch.Add(currentTile);
		} else {
			// If we found a tile different from the targetTile then we finished collecting tiles for the current batch and
			// we must check if we found at least 3 to add to our final "lastFoundMatches" result.
			// We also make the current tile the new targetTile because it belongs in a different batch of matches.
			targetTile = currentTile;
			if (matchesBatch.Count >= 3) {
				CollectNewFoundMatches(matchesBatch, isVerticalPass);
			}
			// Clear the current temporary matches buffer.
			matchesBatch.Clear();
			
			// We check here if the new target tile can be matched then we already add it to the new batch of matches.
			if (targetTile != null && targetTile.CanBeMatched) {
				matchesBatch.Add(currentTile);
			}
		}		
	}

	protected void CollectNewFoundMatches(List<Match3Tile> newMatches, bool isVerticalPass) 
	{
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

		lastFoundMatches.AddRange(newMatches);
	}
}

// Alternate recursive implementation:
//	public void RecursiveFindBoardMatches() 
//	{
//		lastFoundMatches.Clear();
//		// Vertical matches check.
//		for(int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) {
//			BottomMatches(boardData[0, colIdx], null, 0);
//		}
//		
//		// Horizontal matches check.
//		for(int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++) {
//			RightMatches(boardData[rowIdx, 0], null, 0);
//		}
//	}
//	
//	protected int RightMatches(AbstractBoardPiece currentPiece, AbstractBoardPiece previousPiece, int leftMatches)
//	{
//		return DetectMatches(currentPiece, previousPiece, leftMatches, TileMatchDirection.Left, TileMatchDirection.Right, 0, 1);
//	}
//	
//	protected int BottomMatches(AbstractBoardPiece currentPiece, AbstractBoardPiece previousPiece, int topMatches)
//	{
//		return DetectMatches(currentPiece, previousPiece, topMatches, TileMatchDirection.Top, TileMatchDirection.Bottom, 1, 0);
//	}
//	
//	protected int DetectMatches(AbstractBoardPiece currentPiece, AbstractBoardPiece prevPiece, int prevMatches, TileMatchDirection prevDir, TileMatchDirection nextDir, int rowOffset, int colOffset)
//	{
//		BoardCoord nextPos = currentPiece.BoardPosition;
//		nextPos.OffsetByAndClamp(rowOffset, colOffset, boardData.NumRows - 1, boardData.NumColumns - 1);
//		
//		if (currentPiece.Tile == null) {
//			if (nextPos != currentPiece.BoardPosition) {
//				DetectMatches(boardData[nextPos], currentPiece, 0, prevDir, nextDir, rowOffset, colOffset);
//			}
//			
//			return -1;
//		}
//		
//		Match3Tile currentTile = currentPiece.Tile as Match3Tile;
//		Match3Tile prevTile = prevPiece != null ? prevPiece.Tile as Match3Tile : null;
//		
//		currentTile.matchCount[(int)nextDir] = 0;
//		int result = -1;
//		
//		if (prevTile == null || !prevTile.IsMatchWith(currentTile)) {
//			currentTile.matchCount[(int)prevDir] = 0;
//			if (nextPos != currentPiece.BoardPosition) {
//				currentTile.matchCount[(int)nextDir] = DetectMatches(boardData[nextPos], currentPiece, 0, prevDir, nextDir, rowOffset, colOffset) + 1;
//			} 
//		} 
//		else {
//			currentTile.matchCount[(int)prevDir] = prevMatches + 1;
//			if (nextPos != currentPiece.BoardPosition) {
//				currentTile.matchCount[(int)nextDir] = DetectMatches(boardData[nextPos], currentPiece, prevMatches + 1, prevDir, nextDir, rowOffset, colOffset) + 1;
//			}
//			
//			result = currentTile.matchCount[(int)nextDir];
//		}
//		
//		if (currentTile.matchCount[(int)prevDir] + currentTile.matchCount[(int)nextDir] >= 2) {
//			lastFoundMatches.Add(currentTile);
//		}
//		
//		return result;
//	}
