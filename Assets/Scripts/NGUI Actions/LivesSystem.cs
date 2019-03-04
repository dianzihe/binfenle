using UnityEngine;
using System.Collections;

public class LivesSystem : MonoBehaviour 
{
	public static System.DateTime baseDate = new System.DateTime(2013, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
	
	public static LivesSystem instance;
	
	public static string livesKey = "Lives";
	public static string livesTimeKey = "LivesTime";
	
	public static int lives;
	public static int maxLives = 8;
	public static int lifeRefillTime = 60;//1800;
	
	public static event System.Action OnLivesUpdate;
	
	public static long waitTime = 0;
	
	
	public int Lives {
		get {
			return lives;
		}
		set {
			lives = value;
			
			if (OnLivesUpdate != null) {
				OnLivesUpdate();
			}
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		instance = this;
		
		lifeRefillTime = TweaksSystem.Instance.intValues["LifeRefillTime"];
		maxLives = TweaksSystem.Instance.intValues["MaxLives"];
		
		//TODO TALIN: very easy to modify this property by the user if it's stored in playerprefs (save to binary file?)
		Lives = PlayerPrefs.GetInt(livesKey, maxLives);
		long time = TimeSeconds();
		waitTime = System.Math.Min(long.Parse(PlayerPrefs.GetString(livesTimeKey, time.ToString())), time);

		if(OnLivesUpdate != null)
			OnLivesUpdate();
	}
	
	public static long TimeSeconds()
	{
		return (long)System.DateTime.Now.Subtract(baseDate).TotalSeconds;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (lives >= maxLives) {
			return;
		}
		
		long time = TimeSeconds();
		if (time - waitTime >= lifeRefillTime) {
			while (lives < maxLives && time - waitTime >= lifeRefillTime) {
				waitTime += lifeRefillTime;
				Lives++;
			}
		}
	}
	
	public static string GetTimerString()
	{
		long time = lifeRefillTime - (TimeSeconds() - waitTime);
		long minutes = (time / 60);
		long seconds = (time % 60);
		return "" + minutes.ToString("00") + ":" + seconds.ToString("00");
	}
	
	public static void SaveLivesAndNotify(int _lives, long _waitTime, bool notifications = true)
	{
		PlayerPrefs.SetInt(livesKey, _lives);
		PlayerPrefs.SetString(livesTimeKey, _waitTime.ToString());
		
		if (notifications) 
		{
		//	NativeMessagesSystem.CancelNotifications(Language.Get("LIVES_NOTIFICATION_MESSAGE"));
			
			if (_lives < maxLives) {
				long showTime = lifeRefillTime * (maxLives - _lives) - (TimeSeconds() - _waitTime);
		//		NativeMessagesSystem.ScheduleNotification(Language.Get("LIVES_NOTIFICATION_TITLE"), Language.Get("LIVES_NOTIFICATION_MESSAGE"), showTime);
			}
		}
	}
	
	void OnDestroy()
	{
		Lives = PlayerPrefs.GetInt(livesKey, maxLives);
		SaveLivesAndNotify(lives, waitTime);
		PlayerPrefs.Save();
		instance = null;
	}
}
