using UnityEngine;
using System.Collections;

public class LoseMoves : AbstractLoseCondition 
{
	[SerializeField]
	private int startMoves;
	
	[SerializeField]
	private int remainingMoves;
	
	[SerializeField]
	private int offerMoves;
	
	protected bool won = false;
	
	public int StartMoves {
		get {
			return startMoves;
		}
		set {
			startMoves = value;
		}
	}
	
	public int RemainingMoves {
		get {
			return remainingMoves;
		}
		set {
			remainingMoves = value;
		}
	}
	
	protected override void Awake() 
	{
		base.Awake();
		
		string key = "Level" + MaleficentBlackboard.Instance.level + "Moves";
		if (TweaksSystem.Instance.intValues.ContainsKey(key)) {
			startMoves = TweaksSystem.Instance.intValues[key];
		}
		remainingMoves = startMoves;
	}
	
	public override bool Check ()
	{
		// the game is lost when there are no remaining moves
		return !won && remainingMoves <= 0;
	}
	
	public override string GetString()
	{
		return Language.Get("GAME_MOVES");
	}
	
	public override string GetStringUnit()
	{
		return Language.Get("GAME_MOVES");
	}
	
	public override string GetStringValue()
	{
		return remainingMoves.ToString();
	}
	
	public override string GetLoseString()
	{
		return Language.Get("LOSE_MOVES");
		//return "Out of moves!";
	}
	
	public override void NewMove()
	{
		base.NewMove();
		
		if (!paused) 
		{
			remainingMoves--;
			
			if (remainingMoves < 0) {
				remainingMoves = 0;
			}
			
			if (Check()) {
				RaiseOnLoseChecked();
			}
		}
	}
	
	//OBSOLETE
	public override int GetOffer(int packIndex)
	{
		return 0;//TweaksSystem.Instance.intValues["EndMovesPack" + packIndex];
	}
		
	public override void AcceptOffer(int packIndex)
	{
		remainingMoves += GetOffer(packIndex);
	}
	
	public override void DoWin()
	{
		won = true;
		
//		ScoreSystem.Instance.AddScore(remainingMoves * TweaksSystem.Instance.intValues["MovesScoreMultiplier"], false);
//		CoinsSystem.Instance.AddCoins(Mathf.FloorToInt(ScoreSystem.Instance.Score * TweaksSystem.Instance.floatValues["ScoreSilverMultiplier"]), 
//			Mathf.FloorToInt(remainingMoves * TweaksSystem.Instance.floatValues["MovesGoldMultiplier"]));
		
		base.DoWin();
	}
}
	