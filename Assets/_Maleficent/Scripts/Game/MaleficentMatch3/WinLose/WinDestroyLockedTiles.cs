using UnityEngine;
using System.Collections;

public class WinDestroyLockedTiles : WinDestroyTiles {

	protected override void Awake () {
		base.Awake();

		destroyTiles[0].current = 0;
		destroyTiles[0].number = 0;

		LockedTile.OnLockedTileInit += HandleOnLockedTileInit;
		LockedTile.OnLockedTileDestroyed += HandleOnLockedTileDestroyed;
	}
	
	protected override void OnDestroy () {
		base.OnDestroy();

		LockedTile.OnLockedTileInit -= HandleOnLockedTileInit;
		LockedTile.OnLockedTileDestroyed -= HandleOnLockedTileDestroyed;
	}

	protected override void OnTileDestroyed(Match3Tile tile)
	{

	}

	#region EventHandlers
	void HandleOnLockedTileInit (LockedTile obj)
	{
		destroyTiles[0].number++;
	}

	void HandleOnLockedTileDestroyed (LockedTile obj)
	{
		destroyTiles[0].current++;
		CheckWin();
	}
	#endregion

	public override string GetLoseReason()
	{
		if (AllDestroyed()) {
			return base.GetLoseReason();
		}
		else {
			return Language.Get("LOSE_LOCKED_TILES");
		}
	}
	
	public override string GetObjectiveString()
	{
		return Language.Get("GAME_OBJECTIVE_LOCKED_TILES");

	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return Language.Get("MAP_OBJECTIVE_LOCKED_TILES");
	}
	
	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		return "LockedTile";
	}
	
}
