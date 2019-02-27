using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class BurstlyManager {

	private static IBurstlyBinding _burstlyBinding;
	public static IBurstlyBinding burstlyBinding {
		get {
			if (_burstlyBinding == null) {
#if (UNITY_IPHONE) && (!UNITY_EDITOR)
				_burstlyBinding = new IOSBurstlyBinding();
#elif (UNITY_ANDROID) && (!UNITY_EDITOR)
				_burstlyBinding = new GoolgePlayBurstlyBinding();
				//TODO: amazon
				//_burstlyBinding = new AmazonBurstlyBinding();
#else
				_burstlyBinding = new FakeBurstlyBinding();
#endif
			}
			return _burstlyBinding;
		}
	}

}

//public enum BurstlyManagerBannerAnchor 
//{
//	Bottom = 0x01,
//	Top = 0x2,
//	Left = 0x4,
//	Right = 0x8,
//	Center = 0xF
//}
//
//public class BurstlyManager {
//
//#if UNITY_IOS && !UNITY_EDITOR
//	[DllImport ("__Internal")]
//	private static extern void BurstlyManagerWrapper_configureBanner(string appId, string zoneId, float originX, float originY, float width, float height, int anchor);
//	[DllImport ("__Internal")]
//	private static extern void BurstlyManagerWrapper_showBanner(string zoneId);
//	[DllImport ("__Internal")]
//	private static extern void BurstlyManagerWrapper_hideBanner(string zoneId);
//	[DllImport ("__Internal")]
//	private static extern void BurstlyManagerWrapper_configureInterstitial(string appId, string zoneId, bool autocache);
//	[DllImport ("__Internal")]
//	private static extern void BurstlyManagerWrapper_showInterstitial(string zoneId);
//#endif
//
//
//	public static void ConfigureBanner(string appId, string zoneId, float originX, float originY, float width, float height, BurstlyManagerBannerAnchor anchor)
//	{
//#if UNITY_IOS && !UNITY_EDITOR
//		BurstlyManagerWrapper_configureBanner(appId, zoneId, originX, originY, width, height, (int)anchor);
//#endif
//
//#if UNITY_EDITOR
//		Debug.Log(string.Format("[BurstlyManager] Configure banner: {0}, {1}", appId, zoneId));
//#endif
//	}
//
//
//	public static void ShowBanner(string zoneId)
//	{
//#if UNITY_IOS && !UNITY_EDITOR
//		BurstlyManagerWrapper_showBanner(zoneId);
//#endif
//
//#if UNITY_EDITOR
//		Debug.Log(string.Format("[BurstlyManager] Show banner: {0}", zoneId));
//#endif
//	}
//
//
//	public static void HideBanner(string zoneId)
//	{
//#if UNITY_IOS && !UNITY_EDITOR
//		BurstlyManagerWrapper_hideBanner(zoneId);
//#endif
//		
//#if UNITY_EDITOR
//		Debug.Log(string.Format("[BurstlyManager] Hide banner: {0}", zoneId));
//#endif
//	}
//
//
//	public static void ConfigureInterstitial(string appId, string zoneId, bool autocache)
//	{
//#if UNITY_IOS && !UNITY_EDITOR
//		BurstlyManagerWrapper_configureInterstitial(appId, zoneId, autocache);
//#endif
//
//#if UNITY_EDITOR
//		Debug.Log(string.Format("[BurstlyManager] Configure Interstitial: {0}, {1}", appId, zoneId));
//#endif
//	}
//
//	public static void ShowInterstitial(string zoneId)
//	{
//#if UNITY_IOS && !UNITY_EDITOR
//		BurstlyManagerWrapper_showInterstitial(zoneId);
//#endif
//
//#if UNITY_EDITOR
//		Debug.Log(string.Format("[BurstlyManager] Show interstitial: {0}", zoneId));
//#endif
//	}
//
//		
//}
