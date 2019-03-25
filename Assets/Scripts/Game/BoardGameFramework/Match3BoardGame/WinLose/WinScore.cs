using UnityEngine;
using System.Collections;
	
public class WinScore : AbstractWinCondition 
{
	public delegate void NewStarReached(int count);

	public static event NewStarReached OnNewStarReached;
	
	public static int bestScore = 0;
	
	public int targetScore;
	public int targetScore2Stars;
	public int targetScore3Stars;
	
	private int starsReached;

	protected int oldScore = 0;
	
	
	protected override void Awake()
	{
		for (int i = 1; i <= 3; ++i) {
			string key = "Level" + MaleficentBlackboard.Instance.level + "Star" + i;
			if (TweaksSystem.Instance.intValues.ContainsKey(key)) {
				if (i == 1) {
					targetScore = TweaksSystem.Instance.intValues[key];
				}
				else if (i == 2) {
					targetScore2Stars = TweaksSystem.Instance.intValues[key];
				}
				else if (i == 3) {
					targetScore3Stars = TweaksSystem.Instance.intValues[key];
				}
			}
		}
		
		base.Awake();
	}
	
	protected override void Start()
	{
		ScoreSystem.Instance.OnScoreUpdated += OnScoreUpdated;
		bestScore = UserManagerCloud.Instance.GetScoreForLevel(MaleficentBlackboard.Instance.level);//PlayerPrefs.GetInt("BestScore", 0);
		
		if (this.GetType() == typeof(WinScore)) {
			Match3BoardGameLogic.Instance.loseConditions.OnLoseChecked += OnScoreUpdated;
		}
		
		base.Start();
	}
	
	public int StarsReached {
		get {
			return starsReached;
		}
		set {
			starsReached = value;
		}
	}

	/// <summary>
	/// Calculates and returns the objective completion percentage (a value from 0f to 1f).
	/// The result represents the percentage reached until the level can be completed with the minimum requirements.
	/// Each extending class should calculate this progress accordingly.
	/// </summary>
	/// <value>
	/// The objective progress.
	/// </value>
	public override float CalculateObjectiveProgress()
	{		
		return (float)ScoreSystem.Instance.Score / targetScore;
	}
	
	protected override void UpdateMinimumWinRequirement()
	{
		// Make sure we update this property only once per level
		if (CachedRemainingPlayAmount >= 0) {
			return;
		}

		if (ScoreSystem.Instance.Score >= targetScore) 
		{
			if (Match3BoardGameLogic.Instance.loseConditions is LoseMoves) {
				// Cache the remaining moves
				CachedRemainingPlayAmount = (Match3BoardGameLogic.Instance.loseConditions as LoseMoves).RemainingMoves;
			}
			else if (Match3BoardGameLogic.Instance.loseConditions is LoseTimer) {
				// Cache the remaining seconds
				CachedRemainingPlayAmount = Mathf.CeilToInt((Match3BoardGameLogic.Instance.loseConditions as LoseTimer).RemainingTime);
			}
		}
	}
	
	public override bool Check()
	{	
		if (this.GetType() == typeof(WinScore)) 
		{
			UpdateMinimumWinRequirement();
			
			return ScoreSystem.Instance.Score >= targetScore && Match3BoardGameLogic.Instance.loseConditions.Check();
		}
		else {
			return ScoreSystem.Instance.Score >= targetScore;
		}
	}
	
	protected void OnScoreUpdated()
	{
		int newScore = ScoreSystem.Instance.Score;
		
		if (OnNewStarReached != null) {
			if (oldScore < targetScore && targetScore <= newScore) {
				StarsReached = 1;
				OnNewStarReached(1);
			}
			if (oldScore < targetScore2Stars && targetScore2Stars <= newScore) {
				StarsReached = 2;
				OnNewStarReached(2);
			}
			if (oldScore < targetScore3Stars && targetScore3Stars <= newScore) {
				StarsReached = 3;
				OnNewStarReached(3);
			}
		}
		
		oldScore = newScore;
		bestScore = Mathf.Max(bestScore, newScore);
	
		CheckWin();
	}
	
	protected void CheckWin()
	{
		if (Check()) 
		{
			if (!paused) {
				RaiseOnWinChecked();
				paused = true;
			}
		}
	}
	
	public override string GetLoseReason()
	{
		return Language.Get("LOSE_SCORE");
		//return "You didn't reach\nthe target score.";
	}
	
	public override string GetObjectiveString()
	{
		if (Match3BoardGameLogic.Instance.loseConditions is LoseTimer) {
			return Language.Get("GAME_OBJECTIVE_SCORE_TIME");
		}
		else {
			return Language.Get("GAME_OBJECTIVE_SCORE");
			//return "You have to reach\nthe target score.";
		}
	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		if (loseConditions is LoseTimer) {
			return Language.Get("MAP_OBJECTIVE_SCORE_TIME");
		}
		else {
			return Language.Get("MAP_OBJECTIVE_SCORE");
		}
	}

	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		if (loseConditions is LoseTimer) {
			return "Clock";
		}
		else {
			return "Score";
		}
	}
	
	protected virtual void OnDestroy() { }
}
