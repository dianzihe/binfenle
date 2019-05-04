using UnityEngine;
using System.Collections;

public class DXSDKPurUtil : SDKUtilBase
{

	private static DXSDKPurUtil mInstance = null;
	public static DXSDKPurUtil getInstance()
	{
		if(mInstance==null)
		{
			mInstance=new DXSDKPurUtil();
		}
		return mInstance;
	}
	DXSDKPurUtil()
	{
	}


	public override void StartSDKSession (string strAppID, string strChannelID)
	{
		
		using (AndroidJavaClass jc = new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity"))
		{
			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("payWrapperDX"))
			{
				if(jo!=null)
				{
					jo.Call("Unity_Init"); 
					Debug.Log("Init is Success!");
				}
				else
					Debug.Log("Init is NULL!");
			}
		}
	}


	public override void BuyRequest (string strOrderID, string strOrderDesc, string nPrice, string strType, string nGetCount, string strWay)
	{
		Debug.Log ("Buy Begin:");
		int nIndex = -1;
		int.TryParse(strOrderID,out nIndex);
		using (AndroidJavaClass jc = new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity"))
		{
			if(jc!=null)
			{
				using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("payWrapperDX"))
				{
					if(jo!=null)
					{
						jo.Call("Unity_Order",nIndex); 
						 
						Debug.Log("Purcharsr is Success!");
					}
					else
						Debug.Log("Purcharsr is NULL!");
				}
			}
			else
			{
				Debug.Log ("jc is null!");
			}
		}
	}

	public override void BuyResponse (string strOrderID)
	{

	}





}
