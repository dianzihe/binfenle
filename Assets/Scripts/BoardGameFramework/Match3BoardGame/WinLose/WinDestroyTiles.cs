using UnityEngine;
using System.Collections;

public class WinDestroyTiles : WinScore
{
	public DestroyTilesPair[] destroyTiles;


	protected override void Awake()
	{
		for (int i = 0; i < destroyTiles.Length; ++i) {
			string key = "Level" + MaleficentBlackboard.Instance.level + "Destroy" + i;
			if (TweaksSystem.Instance.intValues.ContainsKey(key)) {
				destroyTiles[i].number = TweaksSystem.Instance.intValues[key];
			}
		}
		
		base.Awake();
	}
	
	protected override void Start()
	{
		Match3Tile.OnAnyTileDestroyed += OnTileDestroyed;
		
		base.Start();
	}
	
	public override float CalculateObjectiveProgress()
	{
		float partialProgressScore = base.CalculateObjectiveProgress();

		float totalPartialProgressTiles = 0f;
		int totalTilesToDestroy = 0;
		int totalTilesDestroyed = 0;
		
		for(int i = 0; i < destroyTiles.Length; i++) {
			int clampedTilesDestroyed = Mathf.Clamp(destroyTiles[i].current, 0, destroyTiles[i].number);
			totalTilesDestroyed += clampedTilesDestroyed;
			totalTilesToDestroy += destroyTiles[i].number;
			totalPartialProgressTiles += (float)clampedTilesDestroyed / destroyTiles[i].number;
		}		
		totalPartialProgressTiles /= destroyTiles.Length;
		
//		Debug.LogWarning("totalPartialProgressTiles = " + totalPartialProgressTiles);
//		Debug.LogWarning("partialProgressScore = " + partialProgressScore);
		
		// If the first objective (destroy target tiles) has not been completed, don't calculate the score objective.
		if (totalTilesDestroyed != totalTilesToDestroy) {
			return totalPartialProgressTiles * 0.9f;
		}

		return 0.9f + partialProgressScore * 0.1f;
	}
		
	public override bool Check()
	{
		bool allDestroyed = AllDestroyed();
		
		if (allDestroyed) {
			UpdateMinimumWinRequirement();
		}
		
		return base.Check() && allDestroyed;
	}
	
	protected virtual void OnTileDestroyed(Match3Tile tile)
	{
		foreach (DestroyTilesPair pair in destroyTiles) 
		{
			if (pair.type.GetType().IsInstanceOfType(tile) && tile.TileColor == pair.type.TileColor) {
				pair.current++;
			}
		}
		
		CheckWin();
	}
	
	protected bool AllDestroyed()
	{
		foreach (DestroyTilesPair pair in destroyTiles) 
		{
			if (pair.current < pair.number) {
				return false;
			}
		}
		
		return true;
	}
	
	public override string GetLoseReason()
	{
		if (AllDestroyed()) {
			return base.GetLoseReason();
		}
		else {
			return Language.Get("LOSE_TARGET_TILES");
			//return "You didn't destroy\nall target tiles.";
		}
	}
	
	public override string GetObjectiveString()
	{
		return Language.Get("GAME_OBJECTIVE_TARGET_TILES");
		//return "You have to destroy\nthe indicated amount\nof tiles.";
	}

	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return Language.Get("MAP_OBJECTIVE_TARGET_TILES");
		//return "You have to destroy\nthe indicated amount\nof tiles.";
	}

	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		return "Elimination";
	}
	
	protected override void OnDestroy()
	{
		Match3Tile.OnAnyTileDestroyed -= OnTileDestroyed;
		base.OnDestroy();
	}
}

[System.Serializable]
public class DestroyTilesPair
{
	public Match3Tile type;
	public int number;
	public int current = 0;
}
