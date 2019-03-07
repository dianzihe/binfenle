using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IceAxe : BasicItem
{
	public int destroyTiles = 3;
	
	public bool sequencial = true;
	public float stepTime = 1f;
	
	protected List<Match3BoardPiece> targetBoardPieces;
	
	
	public override string ItemName {
		get {
			return "IceAxe";
		}
	}

	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		if (BoardShuffleController.Instance.IsBoardReshuffling) {
			return;
		}
		
		base.StartUsingItem(_boardLogic);
		
//		TileSwitchInput.Instance.gameObject.SetActive(false);
		TileSwitchInput.Instance.DisableInput();
		
		targetBoardPieces = new List<Match3BoardPiece>(destroyTiles);
		
		SelectAndDestroyTiles();
	}
	
	public override void CancelUsingItem()
	{
//		TileSwitchInput.Instance.gameObject.SetActive(true);
		TileSwitchInput.Instance.EnableInput();
		
		base.CancelUsingItem();
	}
	
	public override bool CanBeUsed()
	{
		return !BoardShuffleController.Instance.IsBoardReshuffling;
	}
	
	protected void SelectAndDestroyTiles()
	{
		ActuallyUsingItem();
		
		List<Match3BoardPiece> allBoardPiecesWithTiles = new List<Match3BoardPiece>();
		List<LayeredBoardPiece> frostedBoardPieces = new List<LayeredBoardPiece>();
		
		boardLogic.boardData.ApplyActionToAll((boardPiece) => {
			Match3Tile tile = boardPiece.Tile as Match3Tile;
			if (tile != null && !tile.IsMoving && tile.IsDestructible && !tile.IsDestroying && !(tile as NormalTile).IsFrozen()) {
				allBoardPiecesWithTiles.Add(boardPiece as Match3BoardPiece);	
			}
			
			LayeredBoardPiece frostPiece = (boardPiece as LayeredBoardPiece);
			if (frostPiece != null && frostPiece.NumLayers > 0) {
				frostedBoardPieces.Add(frostPiece);
			}
		});
		
		while (targetBoardPieces.Count < destroyTiles) {
			int index;
			if (frostedBoardPieces.Count > 0) {
				index = Random.Range(0, frostedBoardPieces.Count);
				targetBoardPieces.Add(frostedBoardPieces[index]);
				frostedBoardPieces.RemoveAt(index);
			}
			else if (allBoardPiecesWithTiles.Count > 0) {
				index = Random.Range(0, allBoardPiecesWithTiles.Count);
				targetBoardPieces.Add(allBoardPiecesWithTiles[index]);
				allBoardPiecesWithTiles.RemoveAt(index);
			}
			else {
				break; // REALLY? THERE AREN'T 3 TILES ON THE BOARD? WHAT HAS THE WORLD COME TO?!?! CURSE YOU CANDY CRUSH SAGA!!!
			}
		}
		
		StartCoroutine(DestroyStuff());
	}
	
	protected IEnumerator DestroyStuff()
	{
		foreach (Match3BoardPiece boardPiece in targetBoardPieces) 
		{
			SpawnEffect(boardPiece.cachedTransform.position, effectPrefab);	
			if (sequencial) 
			{
				SoundManager.Instance.Play("iceaxe_sfx");
				
				StartCoroutine(DestroyThisStuffAfterWaiting(boardPiece));
				yield return new WaitForSeconds(stepTime);
			}
		}
		
		if (sequencial) {
			if (stepTime < destroyEffect.destroyTileTime) {
				Invoke("DoItem", destroyEffect.destroyTileTime - stepTime);
			} 
			else {
				DoItem();
			}
			if (stepTime < destroyEffect.lifeTime) {
				Invoke("DoDestroy", destroyEffect.lifeTime - stepTime);
			}
			else {
				DoDestroy();
			}
		}
		else {
			Invoke("DoItem", destroyEffect.destroyTileTime);
			Invoke("DoDestroy", destroyEffect.lifeTime);
		}
	}
	
	protected IEnumerator DestroyThisStuffAfterWaiting(Match3BoardPiece boardPiece)
	{
		yield return new WaitForSeconds(destroyEffect.destroyTileTime);
		DestroyThisStuff(boardPiece);
	}
	
	protected void DestroyThisStuff(Match3BoardPiece boardPiece)
	{
		LayeredBoardPiece frostPiece = (boardPiece as LayeredBoardPiece);
		if (frostPiece != null && frostPiece.NumLayers > 0 && frostPiece.Tile == null) {
			frostPiece.NumLayers--;
		}
	
		else if (boardPiece.Tile != null) {
			
			if(frostPiece != null && (frostPiece.Tile is SnowTile || frostPiece.Tile is LockedTile || frostPiece.Tile is FreezerTile))
			{
				frostPiece.NumLayers--;
			}
			
			boardPiece.Tile.Destroy();
		}
	}
	
	protected override void DoItem()
	{	
		if (!sequencial) {
			foreach (Match3BoardPiece boardPiece in targetBoardPieces) {
				//Spawn effect
				DestroyThisStuff(boardPiece);
			}
		}
		
//		TileSwitchInput.Instance.gameObject.SetActive(true);
		TileSwitchInput.Instance.EnableInput();
		base.DoItem();
	}
}
