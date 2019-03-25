using UnityEngine;
using System.Collections;

public class StubbornTile : NormalTile {

	public int movesForDestroying = 1;
	public float timeForDestroying = 10f;

	public override void RaiseEventTileTap()
	{
		base.RaiseEventTileTap();
		StubbornDestroy();
	}

	protected void StubbornDestroy () 
	{

		AbstractLoseCondition loseCondition =  Match3BoardGameLogic.Instance.loseConditions;

		System.Type loseConditionType = loseCondition.GetType();

		if(loseConditionType == typeof(LoseMoves)) {
			LoseMoves loseMoves = loseCondition as LoseMoves;
			for (int i = 0; i <= movesForDestroying; i++) {
				loseMoves.NewMove();
			}

		}else if(loseConditionType == typeof(LoseTimer)) {
			LoseTimer loseTimer = loseCondition as LoseTimer;
			loseTimer.RemainingTime -= timeForDestroying;
		}else {
			//Nothing happens ... ??
		}


		Destroy();
	}

	protected override void TileDestroy (bool useEffect)
	{
		base.TileDestroy (false);
		//TODO: Efectos de destruccion
	}
}
