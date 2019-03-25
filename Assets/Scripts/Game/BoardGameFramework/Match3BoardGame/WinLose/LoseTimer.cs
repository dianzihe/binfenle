using UnityEngine;
using System.Collections;

public class LoseTimer : AbstractLoseCondition 
{
	public static event System.Action<LoseTimer> OnRemainingTimeChanged;
	
	public float startTime;
	protected float remainingTime;
//	protected float offerTime;
	
	protected bool movedTile = false;
	protected bool cachedPause = false;
	
	protected SoundEffectController sndCountdown;
	
	[System.NonSerialized]
	public bool pendingTimeUpdate = false;
	
	protected override void Awake() 
	{
		base.Awake();
		
		string key = "Level" + MaleficentBlackboard.Instance.level + "Time";
		if (TweaksSystem.Instance.floatValues.ContainsKey(key)) {
			startTime = TweaksSystem.Instance.floatValues[key];
		}
		RemainingTime = startTime;

		// Cache the timer sound effect
		sndCountdown = SoundManager.Instance["timer_sfx"];

		LoseTimer.OnRemainingTimeChanged += OnRemainingTimeChangedEvent;
	}
	
	protected override void Start () {
		Match3BoardGameLogic.Instance.BoardAnimations.OnTilesSwitchAnimFinished += TileMoved;
		BasicItem.OnStartUsingAnyItem += OnStartUsingAnyItem;
		base.Start();
	}

	public float StartTime {
		get {
			return startTime;
		}
		set {
			startTime = value;
		}
	}
	
	public float RemainingTime {
		get {
			return remainingTime;
		}
		set {
			remainingTime = value;
			if (OnRemainingTimeChanged != null) {
				OnRemainingTimeChanged(this);
			}
		}
	}

	public void TileMoved(AbstractBoardAnimations sender, AbstractTile srcTile, AbstractTile dstTile) 
	{
		StartTimer();
	}
	
	void OnStartUsingAnyItem (BasicItem item)
	{
		StartTimer();
	}
	
	void StartTimer()
	{
		movedTile = true;
		Match3BoardGameLogic.Instance.BoardAnimations.OnTilesSwitchAnimFinished -= TileMoved;
		BasicItem.OnStartUsingAnyItem -= OnStartUsingAnyItem;
		
		Pause(cachedPause);
	}
	
	public override bool Check ()
	{
		// the game is lost when there are no remaining moves
		return RemainingTime <= 0f && !pendingTimeUpdate;
	}
	
	public override string GetString()
	{
		return Language.Get("GAME_TIME");
	}
	
	public override string GetStringUnit()
	{
		return Language.Get("GAME_SECONDS");
	}
	
	public override string GetStringValue()
	{
		return Mathf.CeilToInt(RemainingTime).ToString() + " ";
	}
	
	public override string GetLoseString()
	{
		return Language.Get("LOSE_TIME");
		//return "Out of time!";
	}
	
	protected void OnRemainingTimeChangedEvent(LoseTimer sender)
	{
		if (RemainingTime <= 5f && RemainingTime > 0f && !sndCountdown.IsPlaying) {
			sndCountdown.Play();
		} 
		else if (RemainingTime > 5f && sndCountdown.IsPlaying) {
			sndCountdown.Stop();
		}
	}
	
	protected void Update()
	{
		if (!paused && RemainingTime > 0f) {
			RemainingTime -= Time.deltaTime;

			if (Check()) {
				sndCountdown.Stop();
				LoseTimer.OnRemainingTimeChanged -= OnRemainingTimeChangedEvent;
				
				RaiseOnLoseChecked();
			}
		}
	}
	
	//OBSOLETE
	public override int GetOffer(int packIndex)
	{
		return 0;//TweaksSystem.Instance.intValues["EndTimePack" + packIndex];
	}
	
	public override void AcceptOffer(int packIndex)
	{
		RemainingTime += GetOffer(packIndex);
	}
	
//	public override void DoWin()
//	{
//		//ScoreSystem.Instance.AddScore((int)RemainingTime * TweaksSystem.Instance.intValues["MovesScoreMultiplier"], false);
//		CoinsSystem.Instance.AddCoins(Mathf.FloorToInt(ScoreSystem.Instance.Score * TweaksSystem.Instance.floatValues["ScoreSilverMultiplier"]), 
//			Mathf.FloorToInt(Mathf.CeilToInt(RemainingTime) * TweaksSystem.Instance.floatValues["TimeGoldMultiplier"]));
//		
//		base.DoWin();
//	}
	
	public override void Pause(bool pause) 
	{
		cachedPause = pause;
		
		if (movedTile || pause) {
			base.Pause(pause);
		}
	}
	
	protected override void OnDestroy ()
	{
		base.OnDestroy ();
		
		LoseTimer.OnRemainingTimeChanged -= OnRemainingTimeChangedEvent;
		Match3BoardGameLogic.Instance.BoardAnimations.OnTilesSwitchAnimFinished -= TileMoved;
		BasicItem.OnStartUsingAnyItem -= OnStartUsingAnyItem;
	}
}
