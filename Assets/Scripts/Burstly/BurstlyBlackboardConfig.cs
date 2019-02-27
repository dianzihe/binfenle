using UnityEngine;
using System.Collections;

public class BurstlyBlackboardConfig : MonoBehaviour {

	[System.Serializable]
	public class BurstlyPlatformConfig {
		public string pubId = "";
		public string pregameInterstitialZoneId = "";
		public string postgameInterstitialZondeId = "";
		public string bannerZoneId = "";
		public string xpromoTileZoneId = "";
	}

	public BurstlyPlatformConfig iosConfig;
	public BurstlyPlatformConfig googlePlayConfig;
	public BurstlyPlatformConfig amazonConfig;
}
