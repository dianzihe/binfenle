using UnityEngine;
using System.Collections;

public class AreaBlockerTile : NormalTile {

	public static event System.Action<AreaBlockerTile> OnAreaBlockerTileInit;
	public static event System.Action<AreaBlockerTile> OnAreaBlockerDestroyed;

	public int movesCount;
	private int movesAccum;
	private bool idle = false;

	#region LyfeCycle

	protected override void Awake () 
	{
		base.Awake();
	}

	public override void InitComponent ()
	{
		base.InitComponent ();
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove += HandleOnNewMove;
		Match3BoardGameLogic.OnPostStableBoard += HandleOnPostStableBoard;

		if(OnAreaBlockerTileInit != null) {
			OnAreaBlockerTileInit(this);
		}
	}

	public override void OnDestroy ()
	{
		base.OnDestroy ();
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove -= HandleOnNewMove;
		Match3BoardGameLogic.OnPostStableBoard -= HandleOnPostStableBoard;
		RegisterNeighborBoardPieces(false);
	}

	#endregion
	
	#region Private

	private void SpawnLockedTiles () 
	{
		Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
		Match3BoardPiece boardPieceIterator;
		
		for(int i = 0; i < match3BoardPiece.neighbors.Length; i++) {
			
			boardPieceIterator = match3BoardPiece.neighbors[i];
			
			if(boardPieceIterator != null) {
				NormalTile prevTile = boardPieceIterator.Tile as NormalTile;
				
				if (prevTile != null && (prevTile.GetType() != typeof(LockedTile) && prevTile.TileColor != TileColorType.None)) {
					Match3BoardRenderer.Instance.SpawnSpecificTileAt(boardPieceIterator.BoardPosition, typeof(LockedTile), prevTile.TileColor, false);
					if(prevTile != null) {
						prevTile.DisableTileLogic();
						Destroy(prevTile.gameObject);
					}
				}
			}
		}
		
		Match3BoardGameLogic.Instance.IsBoardStable = false;
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
	
	
	private void RegisterNeighborBoardPieces(bool subscribe) 
	{
		Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
		Match3BoardPiece boardPieceIterator;
		
		for(int i = 0; i < match3BoardPiece.neighbors.Length; i+=2) {
			
			boardPieceIterator = match3BoardPiece.neighbors[i];
			
			if(boardPieceIterator != null) {
				if(subscribe) {
					boardPieceIterator.OnTileDestroyed += HandleOnNeighborDestroyed;
				} else {
					boardPieceIterator.OnTileDestroyed -= HandleOnNeighborDestroyed;
				}
			}
		}
	}
	
	private bool IsAdjacentMatched () 
	{
		bool res = false;
		Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
		Match3BoardPiece boardPieceIterator;
		
		for(int i = 0; i < match3BoardPiece.neighbors.Length; i+=2) {
			boardPieceIterator = match3BoardPiece.neighbors[i];

			if (boardPieceIterator != null) {
				NormalTile prevTile = boardPieceIterator.Tile as NormalTile;
				if (prevTile != null && prevTile.IsMatched) {
					res = true;
					break;
				}
			}
		}
		
		return res;
	}

	#endregion
	
	#region EventHandlers

	private void HandleOnNewMove ()
	{
		if(!idle && !IsDestroying && !IsAdjacentMatched()) {
			movesAccum ++;

			if (movesCount == movesAccum) {
				idle = true;
				movesAccum = 0;
				SpawnLockedTiles();
			}
		}
	}


	private void HandleOnNeighborDestroyed (AbstractBoardPiece sender, AbstractTile neighbor)
	{
		if (neighbor != null && (neighbor as NormalTile).IsMatched) {
			if (neighbor.GetType() != typeof(LockedTile)) {

				if(OnAreaBlockerDestroyed != null) {
					OnAreaBlockerDestroyed(this);
				}

				TileDestroy(false);
			}
		}
	}

	private void HandleOnPostStableBoard ()
	{
		if(idle) {
			bool reactivate = false;

			Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
			Match3BoardPiece boardPieceIterator;
			for(int i = 0; i < match3BoardPiece.neighbors.Length; i++) {
				boardPieceIterator = match3BoardPiece.neighbors[i];

				if(boardPieceIterator != null) {
					NormalTile tile = boardPieceIterator.Tile as NormalTile;

					if (tile != null && tile.GetType() != typeof(LockedTile)) {
						reactivate = true;
						break;
					}
				}
			}

			idle = !reactivate;
		}
	}

	#endregion

	#region Inherited
	public override void InitAfterAttachedToBoard ()
	{
		base.InitAfterAttachedToBoard ();
		RegisterNeighborBoardPieces(true);
	}
	#endregion
}
