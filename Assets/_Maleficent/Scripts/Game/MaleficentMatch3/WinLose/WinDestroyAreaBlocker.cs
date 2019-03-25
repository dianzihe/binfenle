using UnityEngine;
using System.Collections;

public class WinDestroyAreaBlocker : WinScore {
	private int areaBlockersCount = 0;
	private int areaBlockersDestroyed = 0;


	#region LifeCycle
	protected override void Awake()
	{
		base.Awake();
		
		AreaBlockerTile.OnAreaBlockerTileInit += HandleOnAreaBlockerTileInit;
		AreaBlockerTile.OnAreaBlockerDestroyed += HandleOnAreaBlockerDestroyed;
	}
		
	protected override void OnDestroy () 
	{
		base.OnDestroy();
		AreaBlockerTile.OnAreaBlockerTileInit -= HandleOnAreaBlockerTileInit;
		AreaBlockerTile.OnAreaBlockerDestroyed -= HandleOnAreaBlockerDestroyed;
	}


	#endregion


	#region Public

	public int RemainigAreaBlockers () {
		return areaBlockersCount - areaBlockersDestroyed;
	}

	public bool AllAreaBlockersDestroyed () 
	{
		return areaBlockersCount == areaBlockersDestroyed;
	}
	
	#endregion

	#region EventHandlers
	
	private void HandleOnAreaBlockerTileInit (AreaBlockerTile obj)
	{
		areaBlockersCount++;
	}
	
	private void HandleOnAreaBlockerDestroyed (AreaBlockerTile obj)
	{
		areaBlockersDestroyed++;
		CheckWin();
	}
	#endregion

	#region Inherited
	
	public override float CalculateObjectiveProgress()
	{
		if (!AllAreaBlockersDestroyed()) {
			return (1f - areaBlockersDestroyed / (float)areaBlockersCount) * 0.9f;
		}
		
		return 0.9f + base.CalculateObjectiveProgress() * 0.1f;
	}
	
	public override bool Check()
	{
		bool allAreaBlockersDestroyed = AllAreaBlockersDestroyed();
		
		if (allAreaBlockersDestroyed) {
			UpdateMinimumWinRequirement();
		}
		
		return base.Check() && allAreaBlockersDestroyed;
	}
	
	public override string GetLoseReason()
	{
		if (AllAreaBlockersDestroyed()) {
			return base.GetLoseReason();
		}
		else {
			return Language.Get("DESTROY_AREA_BLOCKERS_LOSE");
		}
	}
	
	public override string GetObjectiveString()
	{
		return Language.Get("GAME_OBJECTIVE_DESTROY_AREA_BLOCKERS");
	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return Language.Get("MAP_OBJECTIVE_DESTROY_AREA_BLOCKERS");
	}
	
	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		return "DESTROY_AREA_BLOCKERS";
	}
	
	#endregion
}
