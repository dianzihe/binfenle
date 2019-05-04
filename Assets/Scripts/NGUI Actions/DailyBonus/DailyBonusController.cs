using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DailyBonusController : MonoBehaviour
{
	public Transform presentsContainer;
	public Transform rewardsContainer;
	
	public Transform presentDestroyEffect;
	
	protected ItemPresent[] presents;
	
	public int selectionLimit = 1;
	
	public ItemReward selectedReward;
	
	public float postSelectionWaitTime = 1f;
	public float postUncoverAllWaitTime = 1f;
		
	public GameObject[] defaultRewardPrefabs;
	protected static List<GameObject> rewardPrefabs;
	protected static int[] chosenRewards;
	protected static bool alreadyChoseRewards = false;
	
	protected static List<int> commonRewardBag;
	
	public GameObject tapLabel;
	public GameObject continueButton;
	public PlayMakerFSM fsm;
	public UILabel wonLabel;
	
	public GameObject boardContainer;
	public GameObject connectionContainer;
	public UILabel connectionLabel;
	public UILabel serverLabel;
	
	protected static bool gaveReward = false;
	
	protected static bool sentAnalytics = false;
	
	protected System.DateTime serverTime;
	
	void Awake()
	{
		gaveReward = false;
		alreadyChoseRewards = false;
		
		boardContainer.SetActive(false);
		connectionLabel.text = Language.Get("CONNECT_SERVER");
		/* 
		HttpServerTime.OnServerResponse += TimeOnServerReceived;
		HttpServerTime.Instance.InitServerList(TweaksSystem.Instance.stringValues["TimeServers"]);
		HttpServerTime.Instance.GetServerTime();
		*/
		StartCoroutine("Connecting");
		
		// TODO TALIN - Remove these, temporary to keep showing daily rewards
//		int nowDays = (int)System.DateTime.Now.Subtract(LivesSystem.baseDate).TotalDays;
//		PlayerPrefs.SetInt("RewardTime", nowDays - 1);
//		PlayerPrefs.SetInt("AmISane", Sanity(nowDays - 1));
//		
		if (TweaksSystem.Instance.intValues["DailyReward"] == 0 || !CheckDate(System.DateTime.Now)) 
		{
			Destroy(gameObject);
			return;
		}
		
		fsm.SendEvent("Show");
	}

	void Start()
	{
		sentAnalytics = false;
		/*
		// while daily_bonus is disabled
		if (!sentAnalytics) {
			AnalyticsBinding.LogEventPageView("daily_bonus", "", "");
			sentAnalytics = true;
		} 
		else {
			sentAnalytics = false; //for next time it instantiates
		}
		*/
	}
	
	void TimeOnServerReceived (bool receivedTime)
	{
		//HttpServerTime.OnServerResponse -= TimeOnServerReceived;
		StopCoroutine("Connecting");
		
		if (!receivedTime) 
		{
			Debug.Log("Failed to get time");
			connectionLabel.text = Language.Get("CONNECT_FAILED");
			continueButton.SetActive(true);
			serverTime = System.DateTime.Now;
		}
		else {
			//serverTime = HttpServerTime.LastReceivedServerTime != null ? (System.DateTime)HttpServerTime.LastReceivedServerTime : System.DateTime.Now;
			
			if (CheckDate(serverTime, true)) {
				Debug.Log("Got time " + serverTime + " and showing reward");
				ShowRewards(true);
			}
			else {
				Debug.Log("Got time but reward was already awarded.");
				connectionLabel.text = "\n\n" + Language.Get("CONNECT_RECEIVED");
				serverLabel.gameObject.SetActive(false);
				serverLabel.text = "";
				continueButton.SetActive(true);
				
				int nowDays = (int)serverTime.Subtract(LivesSystem.baseDate).TotalDays;
				PlayerPrefs.SetInt("RewardTime", nowDays);
				PlayerPrefs.SetInt("AmISane", Sanity(nowDays));
			}
		}
	}
	
	IEnumerator Connecting()
	{
		string points = "\n ";
		
		while (true) 
		{
			connectionLabel.text = Language.Get("CONNECT_SERVER") + points;
			
			if (points.Length < 7) {
				points += ". ";
			}
			else {
				points = "\n ";
			}
			
			yield return new WaitForSeconds(0.3f);
			
			Debug.Log("connecting......");
		}
	}
	
	void ShowRewards(bool show)
	{
		if (show) 
		{
			tapLabel.SetActive(true);
			tapLabel.GetComponent<Animation>().Play();
			continueButton.SetActive(false);
			
			connectionContainer.SetActive(false);
			serverLabel.text = "";
			connectionLabel.text = "";
			boardContainer.SetActive(true);
			
			presents = new ItemPresent[presentsContainer.childCount];
			
			if (rewardPrefabs == null) {
				AdjustRewardsAndInitBag();
			}
			
			if (chosenRewards == null) {
				chosenRewards = new int[9];
			}
					
			List<int> rewardBag = new List<int>(commonRewardBag);
			for(int i = 0; i < presentsContainer.childCount; i++)
			{
				presents[i] = presentsContainer.GetChild(i).GetComponent<ItemPresent>();
				presents[i].index = i;
				
				int randomIndexFromBag = alreadyChoseRewards ? chosenRewards[i] : Random.Range(0, rewardBag.Count);
				if (!alreadyChoseRewards) {
					chosenRewards[i] = randomIndexFromBag;
				}
				
				Transform rewardItem = (Instantiate(rewardPrefabs[rewardBag[randomIndexFromBag]]) as GameObject).transform;
				
				Vector3 cachedLocalScale = rewardItem.localScale;
				
			    rewardItem.transform.parent = rewardsContainer;
				rewardItem.localPosition = presents[i].transform.localPosition + Vector3.forward * 12f;
				rewardItem.transform.localScale = cachedLocalScale;
				
				presents[i].rewardItem = rewardItem.GetComponent<ItemReward>();
				
				rewardBag.RemoveAt(randomIndexFromBag);
			}
			
			if (alreadyChoseRewards) 
			{
				foreach (GameObject prefab in rewardPrefabs)
				{
					Destroy(prefab);
				}
				
				rewardPrefabs.Clear();
				rewardPrefabs = null;
				chosenRewards = null;
				commonRewardBag.Clear();
				commonRewardBag = null;
				
				alreadyChoseRewards = false;
			}
			else {
				alreadyChoseRewards = true;
			}
			
			ItemPresent.OnPresentClicked += OnPresentClicked;
		}
	}
	
	/// <summary>
	/// Checks the date and returns true if the daily reward should appear.
	/// </summary>
	/// <returns>
	/// If the daily reward should appear or not.
	/// </returns>
	protected bool CheckDate(System.DateTime currentDate, bool fromServer = false)
	{
		// for the moment there are no daily rewards
		return false;
		/*
		// IMPORTANT: the code following next will be used in later versions
		int oldDays = PlayerPrefs.GetInt("RewardTime", 0);
		Debug.Log("DAYS: " + oldDays);
		int nowDays = (int)currentDate.Subtract(LivesSystem.baseDate).TotalDays;
		Debug.Log("NOW DAYS: " + nowDays);
		
		if (oldDays >= nowDays) 
		{
			if (fromServer) {
				PlayerPrefs.SetInt("RewardTime", nowDays);
				PlayerPrefs.SetInt("AmISane", Sanity(nowDays));
			}
			
			return false;
		}
		else if (oldDays > 0) {
			// check saved date sanity
			int oldDaysSanity = Sanity(oldDays);
			
			if (PlayerPrefs.GetInt("AmISane", 0) != oldDaysSanity) 
			{
				if (fromServer) {
					//corrupted date, possibly hacked
					PlayerPrefs.SetInt("RewardTime", nowDays);
					PlayerPrefs.SetInt("AmISane", Sanity(nowDays));
					
					return false;
				} else {
					//fix the hack by checking with the server
					return true;
				}
			}
			
			if (nowDays - oldDays < TweaksSystem.Instance.intValues["DailyReward"]) 
			{				
				return false;
			}
		}
		
		return true;
		*/
	}
	
	protected int Sanity(int days)
	{
		System.DateTime aDate = LivesSystem.baseDate.AddDays((double)days);
		return aDate.Year / 100 + aDate.Year % 100 + aDate.Month * 5 + aDate.Day / 2 + aDate.Day % 3;
	}
	
	
	
	protected void OnPresentClicked(ItemPresent sender)
	{
		selectedReward = presents[sender.index].rewardItem;
		presents[sender.index].rewardItem.LightBackground();
		
		if (!gaveReward)
		{
			gaveReward = true;
			
			int nowDays = (int)serverTime.Subtract(LivesSystem.baseDate).TotalDays;
			PlayerPrefs.SetInt("RewardTime", nowDays);
			PlayerPrefs.SetInt("AmISane", Sanity(nowDays));
			
			selectedReward.AwardItem();
			
			//AnalyticsBinding.LogEventGameAction("bonus", "receive_bonus", System.Enum.GetName(typeof(ItemReward.RewardType), selectedReward.rewardType),
			//	selectedReward.quantity.ToString(), -1);
		}
		
		ItemPresent.OnPresentClicked -= OnPresentClicked;
		StartCoroutine(FinishSelectionAndUncoverAll());
		
		tapLabel.SetActive(false);
		continueButton.SetActive(true);
	}
	
	protected IEnumerator FinishSelectionAndUncoverAll()
	{
		for(int i = 0; i < presents.Length; i++)
		{
			presents[i].GetComponent<Collider>().enabled = false;
		}
		
		yield return new WaitForSeconds(postSelectionWaitTime);
		
		List<int> things = new List<int>();
		for(int i = 0; i < presents.Length; i++)
		{
			if (!presents[i].uncovered) {
				things.Add(i);
			}
		}
		
		int count = things.Count;
		for(int i = 0; i < count; i++)
		{
			int index = Random.Range(0, things.Count);
			presents[things[index]].UncoverPresent();
			things.RemoveAt(index);
			yield return new WaitForSeconds(Random.Range(0f, 0.15f));
		}
		
		yield return new WaitForSeconds(postUncoverAllWaitTime);
		
		if (GetComponent<AudioSource>().clip != null) {
			NGUITools.PlaySound(GetComponent<AudioSource>().clip);
		}
		
		wonLabel.GetComponent<Animation>().Play("LabelFadeOut");
		selectedReward.ShowSelection();
		
		yield return new WaitForSeconds(wonLabel.GetComponent<Animation>()["LabelFadeOut"].length);
		
		wonLabel.text = Language.Get("DAILY_REWARD_WIN").Replace(" <number x item>", "") + "!";
		wonLabel.GetComponent<Animation>().Play("LabelFadeIn");
	}
	
	protected void AdjustRewardsAndInitBag()
	{
		commonRewardBag = new List<int>();
		rewardPrefabs = new List<GameObject>();
		Dictionary<string, int> items = new Dictionary<string, int>();
		
		for (int i = 0; i < defaultRewardPrefabs.Length; ++i)
		{
			items[System.Enum.GetName(typeof(ItemReward.RewardType), defaultRewardPrefabs[i].GetComponent<ItemReward>().rewardType)] = i;
		}
		
		Regex regex = new Regex("^Reward(?<type>[a-zA-Z]+)(?<quantity>[0-9]+)");
		
		foreach (string key in TweaksSystem.Instance.intValues.Keys) 
		{
			if (key.StartsWith("Reward")) 
			{
				Match match = regex.Match(key);

				if (match.Success)
		        {
					string type = match.Groups["type"].Value;
					int quantity = 0;
					
					if (int.TryParse(match.Groups["quantity"].Value, out quantity)) 
					{
						Debug.Log("Found reward with type: " + type + " quantity: " + quantity);
						
						// instantiate prefab, set quantity and add it to the bag
						GameObject rewardPrefab = GameObject.Instantiate(defaultRewardPrefabs[items[type]]) as GameObject;
						rewardPrefab.GetComponent<ItemReward>().quantity = quantity;
						rewardPrefabs.Add(rewardPrefab);
						
						for( int tempIndex = 0; tempIndex < TweaksSystem.Instance.intValues[key]; tempIndex++)
						{
							commonRewardBag.Add(rewardPrefabs.Count - 1);
						}
					}
				}
			}
		}
//		for(int prefabIndex = 0; prefabIndex < rewardPrefabs.Length; prefabIndex++)
//		{	
//			int count = TweaksSystem.Instance.intValues. [rewardPrefabs[prefabIndex].tweaksName];
//	
//			for( int tempIndex = 0; tempIndex < rewardPrefabs[prefabIndex].Value; tempIndex++)
//			{
//				rewardBag.Add(prefabIndex);
//			}
//		}
	}
	
	void OnDestroy()
	{
		//HttpServerTime.OnServerResponse -= TimeOnServerReceived;
	}
}
