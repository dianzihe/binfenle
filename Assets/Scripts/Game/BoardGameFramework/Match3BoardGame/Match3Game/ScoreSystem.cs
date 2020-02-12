using UnityEngine;
using System.Collections;

public class ScoreSystem : MonoBehaviour
{
	public delegate void ScoreUpdated();
	
	protected static ScoreSystem instance;
	
	protected float multiplierWait = 0.5f;
	protected int maxMultiplier = 15;
	
	protected int score;
	protected int mana;
	public int multiplier;

	public int manaPer1Star = 1;
	public int manaPer2Stars = 2;
	public int manaPer3Stars = 3;
	public int manaPerRepeatedStars = 1;
	
	public event ScoreUpdated OnScoreUpdated;
	public event ScoreUpdated OnManaUpdated;
	
	protected float timeToWait = 0f;

	public int[] manaForStars = { 50, 150, 300};
	public int manaForReplay = 100;
	
	public int Score {
		get {
			return score;
		}
		set {
			score = value;
			if (OnScoreUpdated != null) {
				OnScoreUpdated();
			}
		}
	}

	public int Mana {
		get {
			return mana;
		}
		set {
			mana = value;
			if (OnManaUpdated != null) {
				OnManaUpdated();
			}
		}
	}
	
	public int Multiplier {
		get {
			return multiplier;
		}
	}
	
	public static ScoreSystem Instance {
		get {
//			if (instance == null) {
//				Debug.LogError("ScoreSystem hasn't been initialized");
//			}
			
			return instance;
		}
	}

	public void CalculateManaWon(int stars)
	{
		Mana = GetManaForStars(MaleficentBlackboard.Instance.level,stars,false);
	}

	/*
	// to simulate earning mana
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
			Debug.Log("1. given mana: "+GetManaForStars(1,3,false));
		if(Input.GetKeyDown(KeyCode.B))
			Debug.Log("1. given mana: "+GetManaForStars(1,2,false));
		if(Input.GetKeyDown(KeyCode.C))
			Debug.Log("2. given mana: "+GetManaForStars(1,1,false));
		if(Input.GetKeyDown(KeyCode.D))
			PlayerPrefs.DeleteAll();
	}
	*/

	int GetManaForStars(int levelIdx,int stars,bool hasGivenMana)
	{
		if(stars == 0)
			return 0;

		int starsForLevel = UserManagerCloud.Instance.GetStarsForLevel(levelIdx);

		if(stars > starsForLevel)
		{
			switch(stars)
			{
			case 3:
				return manaPer3Stars + GetManaForStars(levelIdx,2,true);
				break;
			case 2:
				return manaPer2Stars + GetManaForStars(levelIdx,1,true);
				break;
			case 1:
				return manaPer1Star;
				break;
			}
		}
		else
		{
			if(!hasGivenMana)
			{
				return manaPerRepeatedStars;
			}
		}
		return 0;
	}

	string GetKey(int levelIdx,int stars)
	{
		return "score_level"+levelIdx+"_"+stars+"stars";
	}

	public int ManaForReplay
	{
		get{
			if (TweaksSystem.Instance.intValues.ContainsKey("ManaEarnedReplay"))
				return TweaksSystem.Instance.intValues["ManaEarnedReplay"];
			return manaForReplay;
		}
	}

	public int GetPointsShortOfNextStar()
	{
		int points = 0;
		WinScore winCondition = GetComponent<Match3BoardGameLogic>().winConditions as WinScore;
		int stars = 0;
		
		if (score < winCondition.targetScore) {
			points = winCondition.targetScore-score;
		}
		else if (score < winCondition.targetScore2Stars) {
			points = winCondition.targetScore2Stars-score;
		}
		else if (score < winCondition.targetScore3Stars) {
			points = winCondition.targetScore3Stars-score;
		}
		
		return points;
	}

	public int GetStarsWon()
	{
		WinScore winCondition = GetComponent<Match3BoardGameLogic>().winConditions as WinScore;
		int stars = 0;
		
		if (score >= winCondition.targetScore3Stars) {
			stars = 3;
		}
		else if (score >= winCondition.targetScore2Stars) {
			stars = 2;
		}
		else if (score >= winCondition.targetScore) {
			stars = 1;
		}

		return stars;
	}

	void Awake() 
	{
		instance = this;
		
		Match3Tile.OnAnyTileDestroyed += OnAnyTileDestroyed;
		
		Reset();
	}

	void Start()
	{
		if (TweaksSystem.Instance.intValues.ContainsKey("ManaPer1Star"))
			manaPer1Star = TweaksSystem.Instance.intValues["ManaPer1Star"];
		if (TweaksSystem.Instance.intValues.ContainsKey("ManaPer2Stars"))
			manaPer2Stars = TweaksSystem.Instance.intValues["ManaPer2Stars"];
		if (TweaksSystem.Instance.intValues.ContainsKey("ManaPer3Stars"))
			manaPer3Stars = TweaksSystem.Instance.intValues["ManaPer3Stars"];
		if (TweaksSystem.Instance.intValues.ContainsKey("ManaEarnedRepeatedStars"))
			manaPerRepeatedStars = TweaksSystem.Instance.intValues["ManaEarnedRepeatedStars"];
	}

	void OnAnyTileDestroyed (Match3Tile tile)
	{
		timeToWait = multiplierWait;
	}
	
	public void Reset()
	{
		score = 0;
		multiplierWait = TweaksSystem.Instance.floatValues["MultiplierWait"];
		maxMultiplier = TweaksSystem.Instance.intValues["MaxMultiplier"];
		timeToWait = 0f;
		
		ResetMultiplier();
	}
	
	public void IncreaseMultiplier()
	{
		multiplier = Mathf.Min(multiplier + 1, maxMultiplier);
		
		StopAllCoroutines();
		StartCoroutine(WaitForMultiplierReset());
	}
	
	public void ResetMultiplier() 
	{
		multiplier = 1;
		
		StopAllCoroutines();
	}
	
	IEnumerator WaitForMultiplierReset()
	{
		timeToWait = multiplierWait;
		
		while (timeToWait > 0f) 
		{
			// done this way so that i can easily reset this time when 
			// a tile is destroyed, without restarting the coroutine
			timeToWait -= Time.deltaTime;
			
			yield return null;
		}
		
		multiplier = 1;
	}
	
	public string GetScoreString()
	{
		return ScoreSystem.FormatScore(score);
	}
	
	public int AddScore(int newScore, bool multiplied = true)
	{
		Logic.EventCenter.Log(LOG_LEVEL.WARN, "[ScoreSystem] AddScore ");
		int earnedPoints = newScore + (multiplied ? TweaksSystem.Instance.intValues["MultipliedScore"] * (multiplier - 1) : 0);
		Score += earnedPoints;
		return earnedPoints;
	}
	
	public static string FormatScore(int score)
	{
		return score >= 1000 ? score.ToString("0,0").Replace(",", Language.Get("SCORE_SEPARATOR").Replace("<SPACE>"," ")) : score.ToString();
	}
	
	void OnDestroy()
	{
		Match3Tile.OnAnyTileDestroyed -= OnAnyTileDestroyed;
	}
}

