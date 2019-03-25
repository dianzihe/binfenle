using UnityEngine;
using System.Collections;

public class WinDestroyTilesEater : WinDestroyTiles {
	
	[System.NonSerialized]
	public int nrOfEaterTiles;
	
	protected override void Awake()
	{
		base.Awake();

		destroyTiles[0].current = 0;
		destroyTiles[0].number = 0;

		// receives the event every time a eater tile is created
		EaterTile.OnEaterTileInit += ActionOnEaterTileInit;
	}
	
	protected void ActionOnEaterTileInit(EaterTile sender) 
	{
		// total number of eaters increases
		if(destroyTiles[0].type == null) {
			destroyTiles[0].type = sender;
		}
		destroyTiles[0].number++;
	}
	
	public override string GetLoseReason()
	{
		if (AllDestroyed()) {
			return base.GetLoseReason();
		}
		else {
			return Language.Get("LOSE_EATER");
		}
	}
	
	public override string GetObjectiveString()
	{
		return Language.Get("GAME_OBJECTIVE_EATER");
	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return Language.Get("MAP_OBJECTIVE_EATER");
	}
	
	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		return "Eater";
	}
	
	protected override void OnDestroy() 
	{
		EaterTile.OnEaterTileInit -= ActionOnEaterTileInit;

		base.OnDestroy();
	}
}