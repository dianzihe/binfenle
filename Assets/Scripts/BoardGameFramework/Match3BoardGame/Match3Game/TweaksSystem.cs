using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TweaksSystem : MonoBehaviour
{
	// NOT CHANGE because data of tweaks and user progress could be lost
//	public const string CRYPTO_KEY = "FROZEN12";

	protected static TweaksSystem instance;

	public Dictionary<string, int> intValues;
	public Dictionary<string, float> floatValues;
	public Dictionary<string, string> stringValues;

	public int OfferPrice {
		get {
			return 0;//intValues["OfferPrice"];
		}
	}

	public static TweaksSystem Instance {
		get {
			if (instance == null) {
				GameObject container = new GameObject("TweaksSystem");
				instance = container.AddComponent<TweaksSystem>();
				DontDestroyOnLoad(container);
			}

			return instance;
		}
	}

	public static Dictionary<string, int> GetDefaultIntValues()
	{
		return new Dictionary<string, int>() {

			{"Crow_Prop_Cost",1},
			{"Crow2nd_Prop_Cost",1},
			{"TheStaffCost_Prop_Cost",1},
			{"GreenMagicCost_Prop_Cost",1},
			{"WingWindCost_Prop_Cost",1},
			{"YellowPixieDustCost_Prop_Cost",1},
			{"WolfHowlCost_Prop_Cost",1},
			{"ThorwnCost_Prop_Cost",1},

			{"Crow_Prop_Count",0},
			{"Crow2nd_Prop_Count",0},
			{"TheStaffCost_Prop_Count",0},
			{"GreenMagicCost_Prop_Count",0},
			{"WingWindCost_Prop_Count",0},
			{"YellowPixieDustCost_Prop_Count",0},
			{"WolfHowlCost_Prop_Count",0},
			{"ThorwnCost_Prop_Count",0},


		  
      {"Level49Moves", 50},
      {"EnablePostgameInterstitial", 0},
      {"Level28Moves", 30},
      {"Level26Moves", 35},
      {"EnableBanner", 1},
      {"InitialMana", 0},
      {"Level71Moves", 45},
      {"Level74Moves", 40},
      {"DailyRewardRange_0_Days", 5},
      {"Level66Moves", 45},
      {"ManaPer2Stars", 2},
      {"Level37Destroy1", 3},
      {"Level57Moves", 40},
      {"Level34Destroy0", 3},
      {"LifeRefillTime", 1800},
      {"TheStaffCost", 40},
      {"ManaPack3", 750},
      {"ManaPack2", 250},
      {"ManaPack1", 100},
      {"Level38Moves", 45},
      {"Level34Moves", 50},
      {"ManaPack6", 9000},
      {"ManaPack5", 3500},
      {"ManaPack4", 1600},
      {"Level16Moves", 45},
      {"Level50Destroy0", 3},
      {"Level50Destroy1", 3},
      {"Level20Moves", 40},
      {"SaleManaPack1", 10},
      {"SaleManaPack2", 20},
      {"SaleManaPack3", 30},
      {"SaleManaPack4", 40},
      {"ManaPer3Stars", 3},
      {"TutorialsGiveMana", 1},
      {"MaxLives",5},
      {"IsSaleManaPack2", 0},
      {"IsSaleManaPack3", 0},
      {"IsSaleManaPack1", 0},
      {"IsSaleManaPack6", 0},
      {"Level36Moves", 35},
      {"IsSaleManaPack4", 0},
      {"ManaPer1Star", 1},
      {"Level56Moves", 40},
      {"Level60Moves", 35},
      {"GreenMagicCost", 60},
      {"Level58Moves", 35},
      {"SaleManaPack6", 60},
      {"MaxMultiplier", 10},
      {"DailyRewardRange_1_BonusAmount", 5},
      {"Level1Moves", 25},
      {"DailyRewardRange_0_BonusAmount", 5},
      {"Level45Moves", 40},
      {"WingWindCost", 40},
      {"YellowPixieDustCost", 50},
      {"Level31Moves", 30},
      {"Level59Moves", 35},
      {"Crow2ndCost", 60},
      {"Level34Destroy1", 3},
      {"DailyRewardRange_1_Days", 5},
      {"Level38Destroy1", 3},
      {"Level53Moves", 40},
      {"DailyRewardRange_2_Days", 5},
      {"DailyRewardRange_2_Amount", 3},
      {"GrowingThronsCost", 40},
      {"DailyRewardRange_2_BonusAmount", 5},
      {"Level54Moves", 45},
      {"DailyRewardRange_0_Amount", 3},
      {"DailyRewardRange_1_Amount", 3},
      {"EnableXpromo", 1},
      {"WolfHowlCost", 50},
      {"ManaEarnedRepeatedStars", 1},
      {"MultipliedScore", 50},
      {"DailyRewardsRanges", 3},
      {"Level55Moves", 45},
      {"Level37Destroy0", 3},
      {"DailyReward", 1},
      {"CrowCost", 40},
      {"Level49Destroy1", 3},
      {"Level13Star2", 40000},
      {"SaleManaPack5", 50},
      {"Level52Moves", 40},
      {"MovesScoreMultiplier", 2000},
      {"Level73Moves", 35},
      {"IsSaleManaPack5", 0},
      {"Level25Moves", 30},
		};
	}

	public static Dictionary<string, float> GetDefaultFloatValues()
	{
		return new Dictionary<string, float>() {
			
      {"MultiplierWait", 2.00f},
      {"ResyncTime", 60.00f},
		};
	}

	public static Dictionary<string, string> GetDefaultStringValues()
	{
		return new Dictionary<string, string>() {
			
      {"TimeServers", "http://www.google.com | http://www.wikipedia.org | http://www.w3schools.com"},
		};
	}

	void Awake()
	{
		intValues = GetDefaultIntValues();

		floatValues = GetDefaultFloatValues();

		stringValues = GetDefaultStringValues();
	}
}