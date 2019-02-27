using UnityEngine;
using System.Collections;

public class FakeBurstlyBinding : IBurstlyBinding {

	public override void ConfigureBanner(string zoneId, float originX, float originY, float width, float height, BurstlyBindingBannerAnchor anchor)
	{
		Debug.Log("[FakeBurstlyBinding] ConfigureBanner: " + zoneId);
	}
	
	
	public override void ShowBanner(string zoneId)
	{
		Debug.Log("[FakeBurstlyBinding] ShowBanner: " + zoneId);
	}
	
	
	public override void HideBanner(string zoneId)
	{
		Debug.Log("[FakeBurstlyBinding] HideBanner: " + zoneId);
	}
	
	public override void ConfigureInterstitial(string zoneId, bool autocache)
	{
		Debug.Log("[FakeBurstlyBinding] ConfigureInterstitial: " + zoneId);
	}
	
	
	public override  void ShowInterstitial(string zoneId)
	{
		Debug.Log("[FakeBurstlyBinding] ShowInterstitial: " + zoneId);
	}
}
