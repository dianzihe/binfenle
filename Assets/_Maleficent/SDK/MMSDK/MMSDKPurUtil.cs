using UnityEngine;
using System.Collections;

public class MMSDKPurUtil : SDKUtilBase
{

	private static MMSDKPurUtil mInstance = null;
	public static MMSDKPurUtil getInstance()
	{
		if(mInstance==null)
		{
			mInstance=new MMSDKPurUtil();
		}
		return mInstance;
	}
	MMSDKPurUtil()
	{
	}
	public override void StartSDKSession (string strAppID, string strChannelID)
	{
		
		using (AndroidJavaClass jc = new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity"))
		{
			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("payWrapperMM"))
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

	static public void  GetSimState(string strPackName,string strObjectName)
	{
		using (AndroidJavaClass jc = new AndroidJavaClass(strPackName))
		{
			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>(strObjectName))
			{
				if(jo!=null)
				{
					jo.Call("Unity_Get_Sim_State"); 
					Debug.Log("Init is Success!");
				}
				else
					Debug.Log("Init is NULL!");
			}
		}
	}



	public override void BuyRequest (string strOrderID, string strOrderDesc, string nPrice, string strType, string nGetCount, string strWay)
	{
		int nIndex = -1;
		int.TryParse(strOrderID,out nIndex);
		using (AndroidJavaClass jc = new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity"))
		{
			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("payWrapperMM"))
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
	}

	public override void BuyResponse (string strOrderID)
	{

	}





}
