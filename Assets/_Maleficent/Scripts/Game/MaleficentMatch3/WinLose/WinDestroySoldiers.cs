using UnityEngine;
using System.Collections;

public class WinDestroySoldiers : WinDestroyTiles {

	public int turnsBetweenWaves = 3;
	public SoldiersController soldiersController = null;

	protected override void Start ()
	{
		base.Start ();

		SoldiersController controller = Match3BoardGameLogic.Instance.gameObject.GetComponent<SoldiersController>();
		if(controller == null ) {
			soldiersController = Match3BoardGameLogic.Instance.gameObject.AddComponent<SoldiersController>();

			string key = "Level" + MaleficentBlackboard.Instance.level + "TurnsBetweenWaves";
			if (TweaksSystem.Instance.intValues.ContainsKey(key)) {
				turnsBetweenWaves = TweaksSystem.Instance.intValues[key];
			}

			soldiersController.turnsBetweenWaves = turnsBetweenWaves;
		}
	}

	public override string GetLoseReason()
	{
		LoseSoldiers loseSoldiers = Match3BoardGameLogic.Instance.loseConditions as LoseSoldiers;
		if (loseSoldiers.RemainingMoves > 0) {
			return Language.Get("LOSE_SOLDIERS_TARGET");
		}else {
			return base.GetLoseReason();
		}
	}
	
	public override string GetObjectiveString()
	{
		return Language.Get("GAME_OBJECTIVE_SOLDIERS");
	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return Language.Get("MAP_OBJECTIVE_SOLDIERS");
	}
	
	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		return "Soldiers";
	}
	
	protected override void OnDestroy() 
	{
		base.OnDestroy();
	}
}
