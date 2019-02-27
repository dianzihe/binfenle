using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_IOS
public class IOSBurstlyBinding : IBurstlyBinding {

	[DllImport ("__Internal")]
	private static extern void BurstlyManagerWrapper_configureBanner(string appId, string zoneId, float originX, float originY, float width, float height, int anchor);
	[DllImport ("__Internal")]
	private static extern void BurstlyManagerWrapper_showBanner(string zoneId);
	[DllImport ("__Internal")]
	private static extern void BurstlyManagerWrapper_hideBanner(string zoneId);
	[DllImport ("__Internal")]
	private static extern void BurstlyManagerWrapper_configureInterstitial(string appId, string zoneId, bool autocache);
	[DllImport ("__Internal")]
	private static extern void BurstlyManagerWrapper_showInterstitial(string zoneId);


	public IOSBurstlyBinding()
	{
		BurstlyBlackboardConfig platformConfig = Blackboard.Instance.GetComponent<BurstlyBlackboardConfig>();
		config = platformConfig.iosConfig;
	}


	public override void ConfigureBanner(string zoneId, float originX, float originY, float width, float height, BurstlyBindingBannerAnchor anchor)
	{
		BurstlyManagerWrapper_configureBanner(config.pubId, zoneId, originX, originY, width, height, (int)anchor);
	}


	public override void ShowBanner(string zoneId)
	{
		BurstlyManagerWrapper_showBanner(zoneId);
	}


	public override void HideBanner(string zoneId)
	{
		BurstlyManagerWrapper_hideBanner(zoneId);
	}
	
	public override void ConfigureInterstitial(string zoneId, bool autocache)
	{
		BurstlyManagerWrapper_configureInterstitial(config.pubId, zoneId, autocache);
	}


	public override  void ShowInterstitial(string zoneId)
	{
		BurstlyManagerWrapper_showInterstitial(zoneId);
	}
}
#endif
