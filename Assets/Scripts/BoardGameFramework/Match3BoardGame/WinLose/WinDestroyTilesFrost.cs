using UnityEngine;
using System.Collections;

public class WinDestroyTilesFrost : WinScore {
	
	public float frostedBoardPiecesCount;
	protected float totalFrostedBoardPieces;
	
	protected override void Awake()
	{
		base.Awake();
		frostedBoardPiecesCount = 0;
		
		LayeredBoardPiece.OnLayeredBoardPieceInit += ActionOnFrostCreate;
		//LayeredBoardPiece.OnAllLayersDestroyed += ActionOnFrostDissapear;
		LayeredBoardPiece.OnNumLayersDecreased += ActionOnFrostDissapear;
	}
	
	public override float CalculateObjectiveProgress()
	{
		if (frostedBoardPiecesCount != totalFrostedBoardPieces) {
			return (1f - frostedBoardPiecesCount / totalFrostedBoardPieces) * 0.9f;
		}

		return 0.9f + base.CalculateObjectiveProgress() * 0.1f;
	}
	
	public override bool Check()
	{
		bool frostDestroyed = FrostDestroyed();
		
		if (frostDestroyed) {
			UpdateMinimumWinRequirement();
		}

		return base.Check() && frostDestroyed;
	}
	
	protected bool FrostDestroyed()
	{
		return frostedBoardPiecesCount == 0 ? true : false;
	}
	
	protected void ActionOnFrostCreate(LayeredBoardPiece boardPiece) 
	{
		frostedBoardPiecesCount += boardPiece.NumLayers;
		totalFrostedBoardPieces = frostedBoardPiecesCount;
	}
	
	protected void ActionOnFrostDissapear(LayeredBoardPiece boardPiece) 
	{
		frostedBoardPiecesCount--;
		
		CheckWin();
	}
	
	public override string GetLoseReason()
	{
		if (FrostDestroyed()) {
			return base.GetLoseReason();
		}
		else {
			return Language.Get("LOSE_FROST");
			//return "You didn't clear all\nthe frost on the board.";
		}
	}
	
	public override string GetObjectiveString()
	{
		return Language.Get("GAME_OBJECTIVE_FROST");
		//return "You have to clear\nall the frost on the\nboard.";
	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return Language.Get("MAP_OBJECTIVE_FROST");
	}
	
	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		return "Frost";
	}
	
	protected override void OnDestroy() 
	{
		LayeredBoardPiece.OnLayeredBoardPieceInit -= ActionOnFrostCreate;
		LayeredBoardPiece.OnNumLayersDecreased -= ActionOnFrostDissapear;
		base.OnDestroy();
	}
}
