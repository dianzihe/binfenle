using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkingGameSDKUtil :SDKUtilBase
{

	static private TalkingGameSDKUtil mInstance=null;
	static public TalkingGameSDKUtil GetInstance()
	{
		if(mInstance==null)
		{
			mInstance=new TalkingGameSDKUtil();
		}
		return mInstance;
	}
	public override void StartSDKSession (string strAppID, string strChannelID)
	{
		//TalkingDataGA.AttachCurrentThread ();
		base.StartSDKSession (strAppID, strChannelID);
		Debug.Log ("StartSuccess:Beg");
		TalkingDataGA.OnStart (strAppID, strChannelID);
		Debug.Log ("StartSuccess:End");
	}

	public override void SetAccount ()
	{
		base.SetAccount ();
		TDGAAccount.SetAccount (TalkingDataGA.GetDeviceId ());
		Debug.Log ("SetAccountSuccess");
	}
	 

	public override void BuyResponse (string strOrderID)
	{
		base.BuyResponse (strOrderID);
 		TDGAVirtualCurrency.OnChargeSuccess (strOrderID);
		Debug.Log("BuyResponseSuccess!");
	}

	public override void EndSDKSession ()
	{
 
		base.EndSDKSession ();
		TalkingDataGA.OnEnd ();
		Debug.Log ("EndSuccess");
	}

	public override void LogError (string strErrorID, string strMessage, string strClass)
	{
 
		base.LogError (strErrorID, strMessage, strClass);
		Dictionary<string,object> dict = new Dictionary<string, object> ();
		dict.Add (strErrorID, strErrorID + strMessage + strClass);
		TalkingDataGA.OnEvent (strErrorID, dict);
		Debug.Log ("LogEventSuccess");
	}

}
