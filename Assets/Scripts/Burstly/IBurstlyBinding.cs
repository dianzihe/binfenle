using UnityEngine;
using System.Collections;

public enum BurstlyBindingBannerAnchor 
{
	Bottom = 0x01,
	Top = 0x2,
	Left = 0x4,
	Right = 0x8,
	Center = 0xF
}

public abstract class IBurstlyBinding : MonoBehaviour {

	protected string appId;
	public BurstlyBlackboardConfig.BurstlyPlatformConfig config;


	public abstract void ConfigureBanner(string zoneId, float originX, float originY, float width, float height, BurstlyBindingBannerAnchor anchor);
	public abstract void ShowBanner(string zoneId);
	public abstract void HideBanner(string zoneId);

	public abstract void ConfigureInterstitial(string zoneId, bool autocache);
	public abstract void ShowInterstitial(string zoneId);
}
