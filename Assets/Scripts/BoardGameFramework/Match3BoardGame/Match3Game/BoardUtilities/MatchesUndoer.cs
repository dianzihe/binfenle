using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Board tiles initializer.
/// 
/// Looks up a given board setup and if there are any matches found on it will break them by destroying tiles and spawning new ones with 
/// different colors.
/// </summary>
/// TODO: URGENT!!! THIS ALGORITHM WILL HAVE A CHANCE TO FAIL for boards with maximum 4 colors!!!
public class MatchesUndoer 
{
	protected enum LookupDirection : int {
		Horizontal = 0,
		Vertical = 2,
	}
	
	protected BoardData boardData;
	protected MatchesFinder matchesFinder;
		
	protected Match3BoardPiece.LinkType[,] baseLookupPattern = 
	{
		{ Match3BoardPiece.LinkType.Top, 	Match3BoardPiece.LinkType.Top 	 },
		{ Match3BoardPiece.LinkType.Top, 	Match3BoardPiece.LinkType.Bottom },
		{ Match3BoardPiece.LinkType.Bottom, Match3BoardPiece.LinkType.Bottom },
	};
	
	protected bool[] colorsToIgnore = new bool[(int)TileColorType.Count];
	protected int ignoreCount;
	protected List<Match3BoardPiece> lastMatchedBoardPieces;
	
	/// <summary>
	/// This action is triggered every time a new tile is spawned to allow customization for it.
	/// </summary>
	public System.Action<Match3Tile> OnNewTileSpawned;
	
	
	public MatchesUndoer(BoardData _boardData)
	{
		boardData = _boardData;
		matchesFinder = new MatchesFinder(boardData);
		
		lastMatchedBoardPieces = new List<Match3BoardPiece>(32);
	}
	
	public bool FindMatchesAndUndo(bool isBoardSetup)
	{
//		Debug.Log("[MatchesUndoer] FindMatchesAndUndo called...");

		// Look for any tile matches on the given board data.
		if ( matchesFinder.FindMatches() )
		{
			List<Match3Tile> lastMatches = matchesFinder.lastFoundMatches;
			lastMatchedBoardPieces.Clear();
			
			// Cache the board pieces from the last matched tiles found.
			for(int i = 0; i < lastMatches.Count; i++) 
			{
				// When "isBoardSetup" = true we must undo matches made by all tiles that can have color and that can be matched (so we undo initial locked tiles matches).
				if ( (!isBoardSetup || !lastMatches[i].IsTileIgnoredByAntiMatchSystems) && 
					(lastMatches[i].GetType() == typeof(NormalTile) || 
					isBoardSetup && lastMatches[i].canHaveColor && lastMatches[i].CanBeMatched && !(lastMatches[i] is TriggerTile)) ) 
				{
					lastMatchedBoardPieces.Add(lastMatches[i].BoardPiece as Match3BoardPiece);
				}
			}
			
			// Undo all the matches found by looking at the board positions the matches were found.
//			Debug.Log("Initial matches found: " + lastMatches.Count);
			for(int i = 0; i < lastMatchedBoardPieces.Count; i++)
			{
				// Check if this board piece has an initial spawn rule that contains at least one "randomColor" based RuleEntry
				List<RuleEntry> ruleEntries = lastMatchedBoardPieces[i].initialTileSpawnRule.ruleEntries;
				bool tileCanBeChanged = false;
				for(int j = 0; j < ruleEntries.Count; j++)
				{
					if (ruleEntries[j].randomColor) {
						tileCanBeChanged = true;
						break;
					}
				}
				
				if ( !tileCanBeChanged ) {
					continue;
				}
				
				TileColorType[] ignoreTileColors = ComputeIgnoreColorListFromNeighbors(lastMatchedBoardPieces[i]);
				
				if ( ignoreTileColors != null )
				{
					TileColorType newTileColor = TileSpawnRule.GetRandomTileColorDifferentFrom(ignoreTileColors);
					if (newTileColor == TileColorType.None) 
					{
//						Debug.LogWarning("[MatchesUndoer] Couldn't find a color to replace tile for match undo: " + lastMatchedBoardPieces[i].Tile);
					} 
					else
					{
//						Debug.Log("Changing tile: " + lastMatchedBoardPieces[i].Tile);
						
						System.Type lastTileType = lastMatchedBoardPieces[i].Tile.GetType();
	
						GameObject.DestroyImmediate(lastMatchedBoardPieces[i].Tile.gameObject);
	
						// Spawn new tile of the same type but random color different from the ones computed above with "ComputeIgnoreTilesListFromNeighbors".
						Match3Tile newTile = Match3BoardRenderer.Instance.SpawnSpecificTileAt(lastMatchedBoardPieces[i].BoardPosition, lastTileType,
																		 					  newTileColor, false, isBoardSetup);					
						
						if (OnNewTileSpawned != null) {
							OnNewTileSpawned(newTile);
						}
	
//						Debug.Log("with tile: " + newTile);
					}
				}
			}
			
			return true;
		}
		
		return false;
	}
	
	protected TileColorType[] ComputeIgnoreColorListFromNeighbors(Match3BoardPiece startPiece)
	{
		ignoreCount = 0;
		for(int i = 0; i < colorsToIgnore.Length; i++) {
			colorsToIgnore[i] = false;
		}
		
		LookupMatchingColors(startPiece, LookupDirection.Vertical);
		LookupMatchingColors(startPiece, LookupDirection.Horizontal);
		
		if (ignoreCount == 0) {
			return null;
		}
		else 
		{
			// Prepare lookup results. (an array of colors to ignore when picking a new random color for the current tile (on the "startPiece" board piece).
			TileColorType[] result = new TileColorType[ignoreCount];
			int resultIdx = 0;
			for(int i = 1; i < colorsToIgnore.Length; i++) 
			{
				if ( colorsToIgnore[i]) {
					result[resultIdx++] = (TileColorType)i;
				}
			}
			
			return result;
		}
	}

	protected void LookupMatchingColors(Match3BoardPiece startPiece, LookupDirection lookupDir)
	{
		int lookupOffset = (int)lookupDir;
		
		int numPatterns = baseLookupPattern.GetLength(0);
		for(int i = 0; i < numPatterns; i++)
		{
			Match3BoardPiece.LinkType lookupDirA = (Match3BoardPiece.LinkType)( (int)baseLookupPattern[i, 0] + lookupOffset );
			Match3BoardPiece.LinkType lookupDirB = (Match3BoardPiece.LinkType)( (int)baseLookupPattern[i, 1] + lookupOffset );
			
			ignoreCount = AddToIgnoreList( GetMatchingColorBetweenDirections(startPiece, lookupDirA, lookupDirB) );
		}
	}
		
	protected int AddToIgnoreList(TileColorType tileColor)
	{
		if (tileColor == TileColorType.None || colorsToIgnore[(int)tileColor]) {
			return ignoreCount;
		}
		else 
		{
			colorsToIgnore[(int)tileColor] = true;
			return ignoreCount + 1;
		}
	}
		
	protected TileColorType GetMatchingColorBetweenDirections(Match3BoardPiece startPiece, Match3BoardPiece.LinkType dirA, Match3BoardPiece.LinkType dirB)
	{
		Match3BoardPiece pieceDirA = startPiece.GetNeighbor(dirA);
		Match3BoardPiece pieceDirB = null;
		
		if (dirA != dirB) 
		{
			pieceDirB = startPiece.GetNeighbor(dirB);
		}
		else if (pieceDirA != null) 
		{
			pieceDirB = pieceDirA.GetNeighbor(dirA);
		}
			
		if (pieceDirA != null && pieceDirB != null) 
		{
			Match3Tile tileDirA = pieceDirA.Tile as Match3Tile;
			Match3Tile tileDirB = pieceDirB.Tile as Match3Tile;

			if (tileDirA != null && tileDirB != null && tileDirA.CanBeMatched && tileDirB.CanBeMatched && tileDirA.IsMatchWith(tileDirB))
			{
				return tileDirA.TileColor;
			}
		}
		
		return TileColorType.None;
	}		
}
