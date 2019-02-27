using UnityEngine;
using System.Collections;

public class DMOAnalyticsBlackboardConfig : MonoBehaviour {

	[System.Serializable]
	public class DMOAnalyticsPlatformConfig {
		public string bundleId = ""; //TODO: autopopulate
		public string key;
		public string secret;
	}

	public DMOAnalyticsPlatformConfig iosConfig;
	public DMOAnalyticsPlatformConfig googlePlayConfig;
	public DMOAnalyticsPlatformConfig amazonConfig;
}
