using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Possible matches finder.
/// </summary>
public class PossibleMatchesFinder
{
//	public bool debugEnabled = false;
	
	/// <summary>
	/// The found possible match of the last call to <see cref="FindFirstPossibleMatch()"/>
	/// </summary>/	
	public Match3Tile[] foundPossibleMatch;
	/// <summary>
	/// The number of tiles found in the possible match.
	/// </summary>
	public int numFoundPossibleMatches = 0;
	/// <summary>
	/// If it's a 3 tiles colored based match, this stores the color of the match.
	/// </summary>
	public TileColorType foundPossibleMatchColor;
	/// <summary>
	/// The reference of the isolated tile from a match group.
	/// </summary>
	public Match3Tile lastFoundIsolatedTile;
	
	// Indices that represent the Top, Bottom, Left, Right direction for a neighbor
	protected Match3BoardPiece.LinkType[] dirIndices;
	
	// A list of buffers (a list of maximum 3 items for each color type) that store the currently found neighbor matches for a tile. There are TileColorType.Count lists for each type of color.
	protected List<Match3Tile>[] partialTileMatchesLists = new List<Match3Tile>[(int)TileColorType.Count];
	// A list of buffers (a list of maximum 3 items for each color type) that each stores the possibly isolated tiles for each color type found.
	protected List<Match3Tile>[] isolatedTilesLists = new List<Match3Tile>[(int)TileColorType.Count];
	
	// Contains the partial list of trigger tiles match that can be combined. (this will eventually be copied to the final result: "foundPossibleMatch" array).
	// This list can contain trigger tiles or normal tiles (if there is at least a ColorBomb tile).
	protected List<Match3Tile> triggerTileMatchFound = new List<Match3Tile>(3);
	
	protected BoardData boardData;
	
	
	public PossibleMatchesFinder() : this(null) { }
	
	public PossibleMatchesFinder(BoardData _boardData)
	{		
		Board = _boardData;
		
		foundPossibleMatch = new Match3Tile[3];
		
		// Initialize the partial matches buffers. There can only be a maximum of 3 tiles found in a buffer (list).
		for(int i = 0; i < partialTileMatchesLists.Length; i++) {
			partialTileMatchesLists[i] = new List<Match3Tile>(3);
			isolatedTilesLists[i] = new List<Match3Tile>(3);
		}
		
		// Cache the direction indices indicated by the "Match3BoardPiece" enum. (only in 4 directions)
		dirIndices = new Match3BoardPiece.LinkType[4];
		dirIndices[0] = Match3BoardPiece.LinkType.Top;
		dirIndices[1] = Match3BoardPiece.LinkType.Right;
		dirIndices[2] = Match3BoardPiece.LinkType.Bottom;
		dirIndices[3] = Match3BoardPiece.LinkType.Left;
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

	/// <summary>
	/// Finds the first possible match. This is the main method from this class that needs to be called.
	/// </summary>
	/// <returns>
	/// The first possible match.
	/// </returns>
	public bool FindFirstPossibleMatch() 
	{
		numFoundPossibleMatches = 0;
		lastFoundIsolatedTile = null;

		foundPossibleMatch[0] = foundPossibleMatch[1] = foundPossibleMatch[2] = null;
		foundPossibleMatchColor = TileColorType.None;
				
		ClearPartialMatchesBuffers();
		
		// Start searching for the first possible match
		Board.ApplyCancellableActionToAll(FindMatchingBoardPiecesAround);
		
		// Check if we previously found a possible match and store the result separatelly.
		if (foundPossibleMatchColor != TileColorType.None)
		{
			List<Match3Tile> foundPossibleMatchList = partialTileMatchesLists[(int)foundPossibleMatchColor];
			
//			if (debugEnabled) {
//				Debug.Log(Time.frameCount + " normal possible match found: ");
//			}
			
			for(int i = 0; i < 3; i++) {
				foundPossibleMatch[i] = foundPossibleMatchList[i];
				
//				if (debugEnabled) {
//					Debug.Log("Match tile: " + i + ": " + foundPossibleMatch[i]);
//				}
			}
			numFoundPossibleMatches = 3;
			
			return true;
		}
		else if (triggerTileMatchFound.Count >= 2)
		{
//			if (debugEnabled) {
//				Debug.Log(Time.frameCount + " trigger tiles possible match found: ");
//			}
			for(int i = 0; i < triggerTileMatchFound.Count; i++) {
				foundPossibleMatch[i] = triggerTileMatchFound[i];
//				if (debugEnabled) {
//					Debug.Log("Match tile: " + i + ": " + foundPossibleMatch[i]);
//				}
			}
			numFoundPossibleMatches = triggerTileMatchFound.Count;
			
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Finds the matching board pieces around the specified "boardPiece", looking at all neighbors in the 4 directions of this board piece
	/// and caches the pairs of colors found in the partial matches buffers "partialTileMatches".
	/// If any color type buffer from the "partialTileMatches" reaches a count of 3 then we found a possible match for this board piece.
	/// This method 
	/// </summary>
	/// <returns>
	/// <c>true</c> if the method allows the processing to continue to the next board piece.
	/// </returns>
	/// <param name='boardPiece'>
	/// The current board piece being processed.
	/// </param>
	protected bool FindMatchingBoardPiecesAround(AbstractBoardPiece boardPiece) 
	{
		Match3BoardPiece curPiece = boardPiece as Match3BoardPiece;
		
		// Skip this board piece.
		if ( curPiece.Tile == null || curPiece.IsBlocked /*|| !(curPiece.Tile as Match3Tile).CanBeMatched*/ || 
			!curPiece.Tile.IsUserMoveable || curPiece.Tile.IsDestroying  || (curPiece.Tile as NormalTile).IsTileSwitching || (curPiece.Tile as NormalTile).IsFrozen() ) 
		{
			return true;
		}
		
		// Clear partial matches buffers
		ClearPartialMatchesBuffers();
		
		// If the current board piece we're on is a special trigger tile, add it to the trigger tiles bag in case we find another one around it.
		if (curPiece.Tile is TriggerTile) {
			triggerTileMatchFound.Add(curPiece.Tile as Match3Tile);
		}

		// Go to maximum 2 recursive neighbors (in each of the 4 directions) and check if they match.
		// If the first neighbor in the current direction doesn't match the next neighbor then don't add the 2nd neighbor in the
		// "partialTileMatches" buffer and only add the first one. Otherwise, add them both if they match.
		for(int i = 0; i < dirIndices.Length; i++) 
		{
			if ( !FindMatchingNeighborPiecesFor(curPiece, dirIndices[i]) ) {
				return false;
			}
		}

		return true;
	}
	
	/// <summary>
	/// Find the neighbor pieces of the specified "currentPiece" in the direction "direction" that match between them.
	/// This method is called by <see cref="ProcessBoardPiece(AbstractBoardPiece boardPiece)"/>.
	/// </summary>
	/// <returns>
	/// The neighbor pieces for.
	/// </returns>
	/// <param name='currentPiece'>
	/// If set to <c>true</c> current piece.
	/// </param>
	/// <param name='direction'>
	/// If set to <c>true</c> direction.
	/// </param>
	protected bool FindMatchingNeighborPiecesFor(Match3BoardPiece currentPiece, Match3BoardPiece.LinkType direction) 
	{
		Match3BoardPiece prevPiece = null;
//		// Reference to the piece who's neighbors we're about to process.
//		Match3BoardPiece originPiece = currentPiece;
		
		// Look at the next 2 neighbors in the specified "direction".
		for(int i = 0; i < 2; i++) 
		{
			currentPiece = currentPiece.GetNeighbor(direction);
			
			// If this board piece is invalid or contains an invalid tile, stop the current neighbor lookup for the "currentPiece"
			// but continue the search with the next board pieces.
			if (currentPiece == null || currentPiece.Tile == null || currentPiece.Tile.IsDestroying || (currentPiece.Tile as NormalTile).IsTileSwitching ||
				(currentPiece.Tile as NormalTile).IsFrozen() || !(currentPiece.Tile as Match3Tile).CanBeMatched && !(currentPiece.Tile is TriggerTile) ||
				(currentPiece.Tile is TriggerTile) && !currentPiece.Tile.IsUserMoveable)
			{
				return true;
			}

			Match3Tile currentTile = currentPiece.Tile as Match3Tile;
			if (prevPiece != null)
			{
				Match3Tile prevTile = prevPiece.Tile as Match3Tile;
				
				if ( prevTile.IsMatchWith(currentTile) )
				{
					// Add this tile because it matches the previous one.
					AddNewPossibleMatchTile(currentTile);

					if ( HasFoundPossibleMatchForColor(currentTile.TileColor) ) {
						// Stop looking for other possible matches.
						return false;
					}
				}
			} 
			else
			{
//				int lastTriggerTileIdx = triggerTileMatchFound.Count - 1;
//				if (lastTriggerTileIdx >= 0 && triggerTileMatchFound[lastTriggerTileIdx] is ColorBombTile) {
//					Debug.LogWarning("[PossibleMatchesFinder] originalPiece = " + originalPiece + "\n -> search direction: " + direction + "\n -> " + " currentTile = " + currentTile);
//				}

				// Add this tile to the partial matches buffer because it's the first one
				AddNewPossibleMatchTile(currentTile);
				
				if ( HasFoundPossibleMatchForColor(currentTile.TileColor) ) {
					// Stop looking for other possible matches.
					return false;
				}
			}

			prevPiece = currentPiece;
		}
		
		// Can continue to the next board piece.
		return true;
	}
	
	/// <summary>
	/// Adds a new tile to the possible matches list corresponding to this new tile's color and determine if 
	/// this tile is a possible isolated tile in a list of possible matches.
	/// An isolated tile in a possible match is the tile that can be moved to complete the match (this tile must be user moveable and matchable).
	/// </summary>
	/// <param name='newTile'>
	/// The new found tile.
	/// </param>
	protected void AddNewPossibleMatchTile(Match3Tile newTile)
	{
		int lastTriggerTileIdx = triggerTileMatchFound.Count - 1;
		
		// If the last trigger tile added was a ColorBomb the it can match with this new tile also even if it's a normal tile.
		// If this is a special (trigger) tile, add to the trigger tiles possible match list
		if (lastTriggerTileIdx >= 0 && triggerTileMatchFound[lastTriggerTileIdx] is ColorBombTile && 
			triggerTileMatchFound[lastTriggerTileIdx].BoardPiece.IsAdjacentTo(newTile.BoardPiece) ||
			newTile is TriggerTile && (triggerTileMatchFound.Count == 0 || triggerTileMatchFound[lastTriggerTileIdx].BoardPiece.IsAdjacentTo(newTile.BoardPiece)))
		{
//			if (lastTriggerTileIdx >= 0 && triggerTileMatchFound[lastTriggerTileIdx] is ColorBombTile) {
//				Debug.LogWarning("[PossibleMatchesFinder] newTile = " + newTile);
//			}
			triggerTileMatchFound.Add(newTile);
		}
			
		partialTileMatchesLists[(int)newTile.TileColor].Add(newTile);
		
		List<Match3Tile> isolatedTiles = isolatedTilesLists[(int)newTile.TileColor];

		// Find the first adjacent isolated tile from the list corresponding to this tile's color. 
		bool foundIsolatedAdjacentTile = false;
		for(int i = 0; i < isolatedTiles.Count; i++) 
		{
			// Remove one of the other isolated tiles in the list because we found that the new tile is adjacent to it.
			if (isolatedTiles[i].BoardPiece.IsAdjacentTo(newTile.BoardPiece))
			{
				isolatedTiles.RemoveAt(i);
				foundIsolatedAdjacentTile = true;
				break;
			}
		}

		// If we didn't find any isolated tile adjacent to this new one, add it to the possibly isolated tiles list corresponding
		// to this tile's color.
		if ( !foundIsolatedAdjacentTile ) {
			isolatedTiles.Add(newTile);
		}
	}
	

	protected bool HasFoundPossibleMatchForColor(TileColorType tileColor)
	{
		int lastTriggerTileIdx = triggerTileMatchFound.Count - 1;
		
		// Check first if we found at least 2 trigger tiles that can be combined.
		// If thereis a ColorBomb in this list then it can be combined with any user moveable tile(ex:NormalTile) or any other trigger tile.
		if (triggerTileMatchFound.Count >= 2)
		{
			lastFoundIsolatedTile = triggerTileMatchFound[lastTriggerTileIdx];
//			Debug.LogWarning("[PossibleMatchesFinder] lastFoundIsolatedTile = " + lastFoundIsolatedTile);

			return true;
		}	
		
		// Check if we found a possible match.
		if (partialTileMatchesLists[(int)tileColor].Count == 3) 
		{
			List<Match3Tile> isolatedTiles = isolatedTilesLists[(int)tileColor];
			lastFoundIsolatedTile = null;
			
			// Usually in the isolated tiles list for a certain tile color, only one tile is left because the other 2 are adjacent.
			// In case there are more than 1 tiles left (usually all 3 are left) is because it means that none of them is adjacent to the other
			// and we have to see which 2 tiles are collinear (are on the same line horizontal/vertical) so we know that the 3rd tile is isolated.
			if (isolatedTiles.Count > 1) 
			{
				// Search the 2 tiles that are collinear and mark them null so the list is left only with the isolated tile.
				for(int i = 0; i < isolatedTiles.Count - 1; i++) 
				{
					Match3Tile tile = isolatedTiles[i];
					for(int j = i + 1; j < isolatedTiles.Count; j++)
					{
						Match3Tile other = isolatedTiles[j];
						if ( tile.BoardPiece.IsCollinearWith(other.BoardPiece) )  {
							// We found the 2 (and only possible) collinear tiles.
							// Mark them as null so we are left only with the isolated tile. Don't remove them because it's slower in this case.
							isolatedTiles[i] = null;
							isolatedTiles[j] = null;
							goto FinishedSearch;
						}
					}
				}
				
			FinishedSearch:
				// Find the remaining isolated tile
				for(int i = 0; i < isolatedTiles.Count; i++) 
				{
					if (isolatedTiles[i] != null) {
						lastFoundIsolatedTile = isolatedTiles[i];
						break;
					}
				}
			} 
			else {
				lastFoundIsolatedTile = isolatedTiles[0];
			}
			
			// Validate the isolated tile for this possible match group.
			if (lastFoundIsolatedTile == null) 
			{
//				Debug.LogWarning("[PossibleMatchesFinder] For some reason the isolated tile wasn't found for a possible match! This shouldn't happen!");
//				for(int i = 0; i < partialTileMatchesLists[(int)tileColor].Count; i++) 
//				{
//					Debug.LogWarning(partialTileMatchesLists[(int)tileColor][i].name + ", ");
//				}
			} 
			else if ( !lastFoundIsolatedTile.IsUserMoveable )
			{
				// This tile is not user moveable. This possible match group is not valid.
				ClearPartialMatchesBuffers();
				
				return false;
			}
			
//			isolatedTile.cachedTransform.localScale *= 1.25f;
//			Debug.Log("[PossibleMatchesFinder] The valid isolated tile is: " + lastFoundIsolatedTile.name);
			
			// Store the color type of the possible match. Stop the search here.
			foundPossibleMatchColor = tileColor;
//			Debug.LogWarning("We found a possible match: " +  foundPossibleMatchColor);

			return true;
		}
		
		return false;
	}
		
	protected void ClearPartialMatchesBuffers()
	{		
		// Clear partial matches buffers
		for(int i = 0; i < partialTileMatchesLists.Length; i++) {
			partialTileMatchesLists[i].Clear();
			isolatedTilesLists[i].Clear();
		}
		
		triggerTileMatchFound.Clear();
	}	
}
