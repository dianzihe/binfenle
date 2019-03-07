using UnityEngine;
using System.Collections;

public class IcePick : BasicItem
{
	protected Match3Tile tileToDestroy;
	protected AbstractBoardPiece selectedBoardPiece;
	
	public BoardPieceTouchController touchController;
	
	public override string ItemName {
		get {
			return "IcePick";
		}
	}
	
	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		
		TileSwitchInput.Instance.DeactivateDrag();
		touchController.StartInputController();
		touchController.OnNewBoardPieceSelected += OnNewBoardPieceSelected;
		
//		Match3Tile.OnTileTap += OnTileSelected;
	}
	
	public override void CancelUsingItem()
	{
//		Match3Tile.OnTileTap -= OnTileSelected;
		touchController.StopInputController();
		touchController.OnNewBoardPieceSelected -= OnNewBoardPieceSelected;
		
		TileSwitchInput.Instance.ActivateDrag();
		
		base.CancelUsingItem();
	}
	
	public void OnNewBoardPieceSelected(AbstractBoardPiece boardPiece, CustomInput.TouchInfo touchInfo)
	{
		if(BoardShuffleController.Instance.IsBoardReshuffling)
		{
			touchController.ClearLastSelection();
			return;
		}
		
		bool selectionSucces = false;
		
		selectedBoardPiece = boardPiece;
		tileToDestroy = boardPiece.Tile as Match3Tile;
		effectPosition = boardPiece.cachedTransform;

		//Decide wether this selection is icepick worthy or not
		if(boardPiece.Tile == null)
		{
			if(boardPiece is LayeredBoardPiece && (boardPiece as LayeredBoardPiece).NumLayers > 0 )
			{
				selectionSucces = true;
			}
		}
		else if (!tileToDestroy.IsMoving && tileToDestroy.IsDestructible && !tileToDestroy.IsDestroying && !(tileToDestroy as NormalTile).IsFrozen()) 
		{
			selectionSucces = true;
		}
		
		if(selectionSucces)
		{
			SoundManager.Instance.PlayOneShot("icepick_sfx");
			
			touchController.StopInputController();
			touchController.OnNewBoardPieceSelected -= OnNewBoardPieceSelected;
			
			StartItemEffects();
		}
	}
	
	protected override void DoItem()
	{
		if (tileToDestroy)
		{
			tileToDestroy.Destroy();
		}
		else if(selectedBoardPiece is LayeredBoardPiece)
		{
			(selectedBoardPiece as LayeredBoardPiece).NumLayers--;
		}
		
		TileSwitchInput.Instance.ActivateDrag();
		
		base.DoItem();
	}
	
	protected override void OnDestroy ()
	{
		base.OnDestroy ();
		
		touchController.StopInputController();
		touchController.OnNewBoardPieceSelected -= OnNewBoardPieceSelected;
	}
}

