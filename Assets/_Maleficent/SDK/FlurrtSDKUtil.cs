using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FlurrtSDKUtil :SDKUtilBase
{
	#if UNITY_ANDROID
	private string FLURRY_API = "8SP2TYDMY23CH8JTY4FQ";
	#elif UNITY_IPHONE
	private string FLURRY_API = "643YRY5F5J6V7D79239H";
	#else
	private string FLURRY_API = "x";
	#endif
	static private FlurrtSDKUtil mInstance=null;
	static public FlurrtSDKUtil GetInstance()
	{
		if(mInstance==null)
		{
			mInstance=new FlurrtSDKUtil();
		}
		return mInstance;
	}
	public override void StartSDKSession(string strAppID, string strChannelID)
	{
		//FlurryAgent.Instance.onStartSession(FLURRY_API);
		Debug.Log ("StartSuccess");
	}

 
	public override void BuyRequest (string strOrderID, string strOrderDesc, string nPrice, string strType, string nGetCount, string strWay)
	{
		string strEvent = "Buy Request"+strOrderDesc+nPrice+strType+nGetCount+strWay;
		//FlurryAgent.Instance.logEvent(strEvent);
	}

	public override void BuyResponse (string strOrderID)
	{

		string strEvent = "Buy Response"+"Success";
		//FlurryAgent.Instance.logEvent (strEvent);

	}

	public override void EndSDKSession ()
	{
		base.EndSDKSession ();
		//FlurryAgent.Instance.onEndSession ();
		Debug.Log ("EndSuccess");
	}

	public override void LogError(string strErrorID,string strMessage,string strClass)
	{
		base.LogError (strErrorID, strMessage, strClass);
		//FlurryAgent.Instance.onError(strErrorID, strMessage, strClass);
		Debug.Log ("EndSuccess");
	}

}
