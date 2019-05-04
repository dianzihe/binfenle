using UnityEngine;
using System.Collections;

public class SDKUtilBase 
{

	private string m_strAppID,m_strChannelID;

	// Use this for initialization
	void Start () 
	{
	
	}
	public virtual void StartSDKSession(string strAppID,string strChannelID)
	{
		m_strAppID = strAppID;
		m_strChannelID = strChannelID;
	}

	public virtual void EndSDKSession()
	{

	}

	public virtual void LogError(string strErrorID,string strMessage,string strClass)
	{

	}
	public virtual void SetAccount()
	{

	}
	public virtual void BuyRequest(string strOrderID,string strOrderDesc,string nPrice,string strType,string nGetCount,string strWay)
	{

	}
	public virtual void BuyResponse(string strOrderID)
	{

	}


}
