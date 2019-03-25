using UnityEngine;
using System.Collections;

public class WinDestroyGrowTargets : WinDestroyTiles {
	
	[System.NonSerialized]
	public int nrOfSnowTiles;
	
	protected override void Awake()
	{
		base.Awake();
		
		destroyTiles[0].current = 0;
		destroyTiles[0].number = 0;

		GrowingTargetTile.OnGrowTargetTileInit += ActionOnGrowTargetTileInit;
		GrowingTargetTile.OnGrowTargetWinCondition += ActionOnGrowTargetWinCondition;
	}
	
	protected void ActionOnGrowTargetTileInit(GrowingTargetTile sender) 
	{
		destroyTiles[0].number ++;
	}
	
	protected void ActionOnGrowTargetWinCondition(GrowingTargetTile sender) 
	{
		destroyTiles[0].current++;
		
		CheckWin();
	}
	
	public override string GetLoseReason()
	{
		if (AllDestroyed()) {
			return base.GetLoseReason();
		}
		else {
			return Language.Get("LOSE_GROW_TARGETS");
		}
	}
	
	public override string GetObjectiveString()
	{
		return Language.Get("GAME_OBJECTIVE_GROW_TARGETS");
	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return Language.Get("MAP_OBJECTIVE_GROW_TARGETS");
	}
	
	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		return "GROW_TARGETS";
	}
	
	protected override void OnDestroy() 
	{
		GrowingTargetTile.OnGrowTargetTileInit -= ActionOnGrowTargetTileInit;
		GrowingTargetTile.OnGrowTargetWinCondition -= ActionOnGrowTargetWinCondition;
		base.OnDestroy();
	}
}
