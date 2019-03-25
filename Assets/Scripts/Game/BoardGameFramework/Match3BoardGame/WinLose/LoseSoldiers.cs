using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoseSoldiers : LoseMoves 
{
	protected List<SoldierTile> soldiersReady;
	protected List<SoldierTile> soldiersWaiting;
	protected bool justMoved = false;
	protected bool soldierDestroyedInThisTurn = false;
	public float launchLoseDelay = 1.5f;
	public bool checkingLoose = false;
	
	protected override void Start() 
	{
		base.Start();
		soldiersReady = new List<SoldierTile>();
		soldiersWaiting = new List<SoldierTile>();

		Match3BoardGameLogic.OnStartGame += HandleOnStartGame;
		Match3BoardGameLogic.OnPostStableBoard += HandleOnPostStableBoard;
		Match3Tile.OnAnyTileDestroyed += HandleOnAnyTileDestroyed;
	}
	

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Match3BoardGameLogic.OnStartGame -= HandleOnStartGame;
		Match3BoardGameLogic.OnPostStableBoard -= HandleOnPostStableBoard;
		Match3Tile.OnAnyTileDestroyed -= HandleOnAnyTileDestroyed;
	}
	
	public override string GetLoseString()
	{
		if (RemainingMoves > 0) {
			return Language.Get("LOSE_SOLDIERS");
		}else {
			return base.GetLoseString();
		}
	}

	public override void NewMove ()
	{
		base.NewMove ();
		foreach (SoldierTile soldier in soldiersWaiting) {
			soldiersReady.Add(soldier);
		}
		soldiersWaiting.Clear();
		justMoved = true;
	}

	private IEnumerator CheckRaiseLose ()
	{
		if(!checkingLoose) {
			checkingLoose = true;

			yield return new WaitForEndOfFrame();
			while(BoardShuffleController.Instance.IsBoardReshuffling) {
				yield return null;
			}
			

			List<SoldierTile> soldiersToRemove = new List<SoldierTile>(soldiersReady.Count);
			
			foreach (SoldierTile soldier in soldiersReady) {
				Match3BoardPiece soldierPiece = soldier.BoardPiece as Match3BoardPiece;
				if (soldierPiece != null) {
					GrowingThornBoardPiece growingThornBoardPiece = soldierPiece.Top as GrowingThornBoardPiece;
					if (!growingThornBoardPiece.Burned) {
						soldier.PlayAttackAnimation();
						growingThornBoardPiece.Burned = true;
						soldiersToRemove.Add(soldier);
					}
				}
			}
			
			if (soldiersToRemove.Count > 0) {

				foreach(SoldierTile soldier in soldiersToRemove) {
					soldiersReady.Remove(soldier);
				}

				Match3BoardGameLogic.Instance.SetBoardEnabledState(false);
				TileSwitchInput.Instance.DisableInput();
				yield return new WaitForSeconds(launchLoseDelay);
				Match3BoardGameLogic.Instance.SetBoardEnabledState(true);
				TileSwitchInput.Instance.EnableInput();
				RaiseOnLoseChecked();
			}

			checkingLoose = false;
		}
	}


	#region EventHandlers

	void HandleOnStartGame ()
	{
		BoardData boardData = Match3BoardGameLogic.Instance.boardData;

		for (int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) {
			for (int rowIdx = 1; rowIdx < boardData.NumRows; rowIdx++) {
				Match3BoardPiece boardPiece = boardData[rowIdx, colIdx] as Match3BoardPiece;
				GrowingThornBoardPiece topBoardPiece = boardPiece.Top as GrowingThornBoardPiece;

				if (topBoardPiece != null) {
					boardPiece.OnTileChanged += HandleOnTileChanged;
					boardPiece.OnTileDestroyed += HandleOnTileDestroyed;
				}
			}
		}
	}
	
	void HandleOnTileChanged (AbstractBoardPiece sender, AbstractTile tile)
	{
		if (tile != null && tile.GetType() == typeof (SoldierTile)) {
			Match3BoardPiece bottomPiece = sender as Match3BoardPiece;
			GrowingThornBoardPiece growingThornBoardPiece = bottomPiece.Top as GrowingThornBoardPiece;
			if (!growingThornBoardPiece.Burned) {
				soldiersWaiting.Add(tile as SoldierTile);
			}
		}
	}

	void HandleOnTileDestroyed (AbstractBoardPiece sender, AbstractTile tile)
	{
		SoldierTile soldierTile = tile as SoldierTile;
		if (soldierTile != null) {
			soldiersReady.Remove(soldierTile);
			soldiersWaiting.Remove(soldierTile);
		}
	}

	void HandleOnAnyTileDestroyed (Match3Tile tile)
	{
		SoldierTile soldierTile = tile as SoldierTile;
		if (soldierTile != null) {
			soldierDestroyedInThisTurn = true;
		}
	}

	void HandleOnPostStableBoard ()
	{
		bool noSoldierDestroyed = !soldierDestroyedInThisTurn;
		bool playerJustMoved = justMoved;

		soldierDestroyedInThisTurn = false;
		justMoved = false;

		if (soldiersReady.Count > 0 && noSoldierDestroyed && playerJustMoved && !checkingLoose) {
			StartCoroutine(CheckRaiseLose());
		}
	}

	#endregion
}
	