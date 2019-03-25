using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

/// <summary>
/// SliderTile
/// 
/// This tile is moveable to any border directly
/// </summary>
public class SliderTile : NormalTile
{	
	[HideInInspector]
	public bool hasToStop;
	private bool isStarted = false;
	private bool isDragging = false;

	public override bool IsUserMoveable
	{
		get {
			return isUserMoveable && enabled && isDragging && TappedFirst;
		}
		set {
			isUserMoveable = value;
		}
	}

	protected override void Start ()
	{
		base.Start();
		Match3BoardGameLogic.OnTryingToMoveTile += OnTryingToMoveTile;
	}

	protected override void Update()
	{
		base.Update();
		if(Input.GetMouseButton(0))
			isDragging = true;
		if(Input.GetMouseButtonUp(0))
			isDragging = false;
	}
	
	public void OnTryingToMoveTile(AbstractTile tile,TileMoveDirection direction)
	{
		if(tile as Match3Tile != this)
			return;

		// check whether is possible to move
		if(!isStarted && isDragging)
		{
			isStarted = true;
			hasToStop = false;
			// gets swipeDirection
			Match3BoardPiece.LinkType linkType = Match3BoardPiece.GetLinkTypeFromMoveDirection(direction);
			StartCoroutine(DoDisplacements(linkType));

			// counts as one move
			Match3BoardGameLogic.Instance.loseConditions.NewMove();
		}
	}

	// returns the target board piece to go, it may be a border piece, an empty one, or a special tile
	private Match3BoardPiece GetDstBoardPiece(Match3BoardPiece.LinkType linkType)
	{
		Match3BoardPiece dstBoardPiece = BoardPiece as Match3BoardPiece;
		BoardCoord curPos = BoardPiece.BoardPosition;
		BoardCoord offset = Match3BoardPiece.linkOffsets[(int)linkType];
		curPos.OffsetBy(offset);

		while(IsIntoBoard(curPos) && IsValid(Board[curPos] as Match3BoardPiece))
		{
			dstBoardPiece = Board[curPos] as Match3BoardPiece;
			curPos.OffsetBy(offset);
		}

		if(dstBoardPiece != BoardPiece)
		{
			//Debug.Log("A: "+BoardPiece.BoardPosition.row+","+BoardPiece.BoardPosition.col+
			//          " -> B: "+dstBoardPiece.BoardPosition.row+","+dstBoardPiece.BoardPosition.col);
			//Debug.Log("aux color: "+(dstBoardPiece.Tile as Match3Tile).TileColor);
			return dstBoardPiece;
		}
		else
			return null;
	}

	// returns if the coord by param is a valid position in the board
	private bool IsIntoBoard(BoardCoord coord)
	{
		return !(coord.row >= Board.NumRows || coord.col >= Board.NumColumns || coord.row < 0 || coord.col < 0);
	}

	// returns if the current boardpiece is a good one to continue checking, otherwise search is finished
	private bool IsValid(Match3BoardPiece boardPiece)
	{
		return (boardPiece != null &&
			   !(boardPiece.IsEmpty) &&
		       (boardPiece.Tile.IsUserMoveable) &&
		       !(boardPiece.Tile is SliderTile)
		     //  !(boardPiece.IsBlocked)
			   );
	}

	//private void DoDisplacements(Match3BoardPiece dstBoardPiece,Match3BoardPiece.LinkType linkType)
	private IEnumerator DoDisplacements(Match3BoardPiece.LinkType linkType)
	{
		// goes moving all pieces from the dst to our tile
		BoardCoord offset = Match3BoardPiece.linkOffsets[(int)linkType];
		BoardCoord nextPos = BoardPiece.BoardPosition;
		nextPos.OffsetBy(offset);
		Match3Tile dstTile;

		while(IsIntoBoard(nextPos))
		{
			dstTile = Board[nextPos].Tile as Match3Tile;

			/*
			// in case there's a switch in front -> it stops
			if(dstTile == null || (dstTile!= null && 
			 	(dstTile.IsTileSwitching || dstTile.IsMoving || !IsValid(dstTile.BoardPiece as Match3BoardPiece))))
			{
				Finish ();
				yield break;
			}
			*/

			if(dstTile == null)
				yield break;

			while(dstTile.IsTileSwitching || dstTile.IsMoving)
			{
				yield return null;
			}

			dstTile = Board[nextPos].Tile as Match3Tile;
			//Debug.Log(id+"("+BoardPiece.BoardPosition.row+","+BoardPiece.BoardPosition.col+") -> ("+nextPos.row+","+nextPos.col+")");
			if(!IsValid(dstTile.BoardPiece as Match3BoardPiece))
			{
				Finish();
				yield break;
			}

			this.IsTileSwitching = true;
			dstTile.IsTileSwitching = true;

			Match3BoardGameLogic.Instance.BoardAnimations.SwitchTilesAnim(false,this,dstTile,
				(_sender, _srcTile, _dstTile) => 
				{
					Match3BoardGameLogic.Instance.boardData.SwitchTiles(this, dstTile);
					this.IsTileSwitching = false;
					dstTile.IsTileSwitching = false;
					Match3BoardGameLogic.Instance.matchesFinder.FindMatches();
					Match3BoardGameLogic.Instance.DestroyLastFoundMatches();
				}
			);
			nextPos.OffsetBy(offset);

			while(IsTileSwitching)
				yield return null;
		}

		isStarted = false;
		yield return null;
	}

	private void Finish()
	{
		hasToStop = false;
		isStarted = false;
	}

	/*
// DO NOT delete this method, it will be useful if we want to change the way it's displaced
	private void DoDisplacements(Match3BoardPiece dstBoardPiece,Match3BoardPiece.LinkType linkType)
	{
		// goes moving all pieces from the dst to our tile
		BoardCoord offset = Match3BoardPiece.linkOffsets[(int)linkType];
		BoardCoord prevPos = dstBoardPiece.BoardPosition;
		BoardCoord curPos = dstBoardPiece.BoardPosition;
		AbstractTile tileToSet = dstBoardPiece.Tile;
		curPos.OffsetBy(offset);
		do
		{
			Match3BoardGameLogic.Instance.BoardAnimations.MoveTileAnim(tileToSet as Match3Tile,Board[curPos].LocalPosition);
			tileToSet = Match3BoardGameLogic.Instance.boardData.ChangeBoardPieceTile(Board[curPos],tileToSet);
			prevPos.OffsetBy(offset);
			curPos.OffsetBy(offset);
		}while(Board[prevPos] != BoardPiece);
		
		// last movement, our own tile
		Match3BoardGameLogic.Instance.BoardAnimations.MoveTileAnim(this,dstBoardPiece.LocalPosition);
		Match3BoardGameLogic.Instance.boardData.ChangeBoardPieceTile(dstBoardPiece,this);
	}
	*/
}
