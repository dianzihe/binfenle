using UnityEngine;
using System.Collections;

public class GoolgePlayBurstlyBinding : IBurstlyBinding {

	public override void ConfigureBanner(string zoneId, float originX, float originY, float width, float height, BurstlyBindingBannerAnchor anchor) {
	}

	public override void ShowBanner(string zoneId) {
	}

	public override void HideBanner(string zoneId) {
	}
	
	public override void ConfigureInterstitial(string zoneId, bool autocache) {
	}

	public override void ShowInterstitial(string zoneId) {
	}
}
