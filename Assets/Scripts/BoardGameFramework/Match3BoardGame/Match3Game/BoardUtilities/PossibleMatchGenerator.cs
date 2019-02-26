using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PossibleMatchGenerator {
	protected BoardData board;
	
	// Tile buffers per color (the first buffer can be ignored because it corresponds to TileColor.None)
	private List<Match3Tile>[] tilesBuffers = new List<Match3Tile>[(int)TileColorType.Count];
	private List<Match3Tile> normalTiles = new List<Match3Tile>(32);
//	private List<int> tileBufferIndices = new List<int>( (int)TileColorType.Count );
	protected bool isBoardSetup;
	
	private List<Match3BoardPiece> possibleMatchSlots = new List<Match3BoardPiece>(4);
	
	public System.Action<Match3Tile> OnNewTileSpawned;
	
	public PossibleMatchGenerator() : this(null) { }
	
	
	public PossibleMatchGenerator(BoardData _boardData)
	{
		Board = _boardData;
		
		// Allocate tile buffers
		for(int i = 0; i < tilesBuffers.Length; i++) 
		{
			tilesBuffers[i] = new List<Match3Tile>(10);
		}
	}
	
	public BoardData Board
	{
		get {
			return board;
		}
		set {
			board = value;
		}
	}
	
	public bool GenerateMatch(bool _isBoardSetup)		
	{
		isBoardSetup = _isBoardSetup;
		
		normalTiles.Clear();
		
		for(int i = 0; i < tilesBuffers.Length; i++) {
			tilesBuffers[i].Clear();
		}
		
//		tileBufferIndices.Clear();
		possibleMatchSlots.Clear();
		
		// Collect all normal tiles and create tile buffers for each color.
		Board.ApplyActionToAll((boardPiece) => 
		{
			if (boardPiece.Tile != null && boardPiece.Tile.GetType() == typeof(NormalTile) && (!isBoardSetup || !(boardPiece.Tile as NormalTile).IsTileIgnoredByAntiMatchSystems))
			{
				Match3Tile tile = boardPiece.Tile as Match3Tile;
				normalTiles.Add(tile);
				
				// Add tile to corresponding color buffer.
				tilesBuffers[(int)tile.TileColor].Add(tile);
			}
		});
		
		// No tiles allowed to be overriden by possible match generator
		if (normalTiles.Count == 0) {
//			Debug.LogWarning("[PossibleMatchGenerator] No normal tiles allowed to be touched by PossibleMatchGenerator...");
			
			return false; 
		}

//		for(int i = 1; i < tilesBuffers.Length; i++)
//		{
//			Debug.LogWarning((TileColorType)i + " => " + tilesBuffers[i].Count);
//		}
		
		// Sort tile buffer indices descending.
		// After sorting the tile buffers the first tile buffer will contain the tiles that have the biggest count on the board. 
		// So tileBuffers[0] won't represent the colored tiles buffer of the TileColorType.None anymore. (which was an empty buffer anyway)
		SortTilesBuffersCountDescending();
		
		if ( tilesBuffers[0].Count < 3 )
		{
			TileColorType tileColorNeeded = tilesBuffers[0][0].TileColor;
//			Debug.LogWarning("[PossibleMatchGenerator] The color we need to generate: " + tileColorNeeded);
		
			// Go through each tile buffer and convert tiles into the color we need above to get at least 3 tiles of the same color
			// so we can create a possible match on the board.
			for(int i = 1; i < tilesBuffers.Length && tilesBuffers[0].Count < 3; i++)
			{
				int numAvailableTiles = tilesBuffers[i].Count;

				for(int j = 0; j < numAvailableTiles && tilesBuffers[0].Count < 3; j++)
				{
					Match3BoardPiece tilePiece = tilesBuffers[i][j].BoardPiece as Match3BoardPiece;

					// Destroy old tiles
					GameObject.DestroyImmediate(tilePiece.Tile.gameObject);
					tilesBuffers[i][j] = null;

					// Create new tile of the color we need.
				 	Match3Tile newTile = Match3BoardRenderer.Instance.SpawnSpecificTileAt(tilePiece.BoardPosition, typeof(NormalTile),
																	 					 tileColorNeeded, false, isBoardSetup);
					// Do any customization to new tiles spawned by this system.
					if (OnNewTileSpawned != null) {
						OnNewTileSpawned(newTile);
					}
						
					tilesBuffers[0].Add(newTile);
				}
			}
		}

//		Debug.LogWarning("[PossibleMatchGenerator] After repopulating with tiles:");
//		for(int i = 0; i < tilesBuffers.Length; i++)
//		{
//			Debug.Log((TileColorType)i + " => " + tilesBuffers[i].Count);
//		}
		
		// Find a location on the board where to place the 3 tiles from the tile buffer 0 to create a possible match.
		if ( FindPossibleMatchLocation() ) 
		{
			// Create possible match on the board
			for(int i = 0; i < possibleMatchSlots.Count; i++)
			{	
				Match3Tile tempTile = possibleMatchSlots[i].Tile as Match3Tile;
				Match3BoardPiece targetPiece = tilesBuffers[0][i].BoardPiece as Match3BoardPiece;
				
				if (tempTile != targetPiece.Tile)
				{	
					possibleMatchSlots[i].Tile = tilesBuffers[0][i];
					targetPiece.Tile = tempTile;
					
					targetPiece.ResetTilePosition();
					possibleMatchSlots[i].ResetTilePosition();
				}
			}
			
			return true;
		}
		
		return false;
//		else 
//		{
//			Debug.LogWarning("No possible match location found!!!");
//		}
		
	}
	
	protected bool FindPossibleMatchLocation()
	{
		// Pick random tiles on the board and look up where there is a place to generate the possible match.
		while(normalTiles.Count > 0)
		{
			int randomIdx = Random.Range(0, normalTiles.Count);
			//TODO FLORIN - BUGGED! check all the board tiles that are user moveable not only normal tiles 
			Match3BoardPiece piece = normalTiles[randomIdx].BoardPiece as Match3BoardPiece;
			
			if ( LookupPossibleNeighborSlotsFor(piece) )
			{
				return true;
			}
			else {
				normalTiles.RemoveAt(randomIdx);
			}
		}

		return false;
	}
	
	protected bool LookupPossibleNeighborSlotsFor(Match3BoardPiece piece)
	{	
		int numNeighborDirections = (int)Match3BoardPiece.LinkType.Count;
		
		possibleMatchSlots.Clear();
		
		for(int i = 0; i < numNeighborDirections; i += 2)
		{
			// Count max 2 neighbors in each direction
			Match3BoardPiece neighborIterator = piece;
			
			for(int j = 0; j < 2; j++)
			{
				neighborIterator = neighborIterator.GetNeighbor( (Match3BoardPiece.LinkType)i );
				
				if (neighborIterator != null && neighborIterator.Tile != null && neighborIterator.Tile.GetType() == typeof(NormalTile) && 
					(!isBoardSetup || !(neighborIterator.Tile as NormalTile).IsTileIgnoredByAntiMatchSystems))
				{
					possibleMatchSlots.Add(neighborIterator);
					if (possibleMatchSlots.Count == 3) {
						return true;
					}
				}
				else {
					break;
				}
			}
		}

		return false;
	}

	protected void SortTilesBuffersCountDescending()
	{		
		System.Array.Sort(tilesBuffers, (tilesBuffer1, tilesBuffer2) => 
		{
			if (tilesBuffer1.Count < tilesBuffer2.Count) {
				return 1;
			}
			else if (tilesBuffer1.Count == tilesBuffer2.Count) {
				return 0;
			}
			else {
				return -1;
			}
		});
	}
	
}
