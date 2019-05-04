using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class MMSDKManager : MonoBehaviour 
{
	public enum SIMTYPE
	{
		NONE=0,
		YIDONG,
		LIANTONG,
		DIANXIN,
		MDO
	};
	static public SIMTYPE  m_SimType = SIMTYPE.NONE;
	public static string m_strPayCode="";
	public static int m_nPayCodeIndex=-1;
	public static bool m_bSimReady=false;
	private static Dictionary<string,int > m_Dict=new Dictionary<string, int>();
	public static event Action<int> purchaseSucceededEvent;
	public static event Action<int> purchaseFailedEvent;
	public static event Action<int> purchaseCanceled;
	public static void InitMMDict()
	{
		m_Dict.Add("30000829358301",0);
		m_Dict.Add("30000829358302",1);
		m_Dict.Add("30000829358303",2);
		m_Dict.Add("30000829358304",3);
		m_Dict.Add("30000829358305",4);
		m_Dict.Add("30000829358306",5);
		m_Dict.Add("30000829358307",6);
		m_Dict.Add("30000829358308",7);
		m_Dict.Add("30000829358309",8);
	}
	public static void InitDXDict()
	{
		m_Dict.Add("5004827",0);
		m_Dict.Add("5004828",1);
		m_Dict.Add("5004829",2);
		m_Dict.Add("5004830",3);
		m_Dict.Add("5004831",4);
		m_Dict.Add("5004832",5);
		m_Dict.Add ("5016685", 6);
		m_Dict.Add ("5016686", 7);
		m_Dict.Add ("5016687", 8);
	}
//	public static void InitLTDict()
//	{				
//		m_Dict.Add ("140604039039", 0);
//		m_Dict.Add ("140604039041", 1);
//		m_Dict.Add ("140604039043", 2);
//		m_Dict.Add ("140604039045", 3);
//		m_Dict.Add ("140604039047", 4);
//		m_Dict.Add ("140604039049", 5);
//		m_Dict.Add ("140722046408", 6);
//		m_Dict.Add ("140722046409", 7);
//		m_Dict.Add ("140722046410", 8);
//	}
	public static void InitLTDict()
	{				
		m_Dict.Add ("001", 0);
		m_Dict.Add ("002", 1);
		m_Dict.Add ("003", 2);
		m_Dict.Add ("004", 3);
		m_Dict.Add ("005", 4);
		m_Dict.Add ("006", 5);
		m_Dict.Add ("007", 6);
		m_Dict.Add ("008", 7);
		m_Dict.Add ("009", 8);
	}
	public static void InitMDO()
	{
		m_Dict.Add("MAL001",0);
		m_Dict.Add("MAL002",1);
		m_Dict.Add("MAL003",2);
		m_Dict.Add("MAL004",3);
		m_Dict.Add("MAL005",4);
		m_Dict.Add("MAL006",5);
//		m_Dict.Add ("MAL007", 6);
//		m_Dict.Add ("MAL008", 7);
//		m_Dict.Add ("MAL009", 8);
	}

	// Use this for initialization
	void Start () 
	{
		MMSDKPurUtil.GetSimState ("com.disney.maleficentchina.CustomUnityPlayerActivity", "payWrapperMM");
		InitQHData ();
		InitQHPush ();
		//InitDXDict ();
		//DXSDKPurUtil.getInstance ().StartSDKSession ("", "");
		//InitMMDict ();
		//MMSDKPurUtil.getInstance ().StartSDKSession ("", "");
	//	InitLTDict ();
	//	LTSDKPurUtil.getinstance ().StartSDKSession ("", "");


	}

	void SwitchPlatInit()
	{
		switch(m_SimType)
		{
		case SIMTYPE.DIANXIN:
			InitDXDict ();
			DXSDKPurUtil.getInstance ().StartSDKSession ("", "");
			break;
		case SIMTYPE.LIANTONG:
			InitLTDict ();
			LTSDKPurUtil.getinstance ().StartSDKSession ("", "");
			break;
		case SIMTYPE.YIDONG:
			MMSDKPurUtil.getInstance ().StartSDKSession ("", "");
			InitMMDict ();
			break;
		case SIMTYPE.MDO:
			MDOSDKPurUtil.getInstance().StartSDKSession("","");
			InitMDO();
			break;
		case SIMTYPE.NONE:
			break;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	void InitQHData()
	{
		using (AndroidJavaClass jc=new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity")) {
			using(AndroidJavaObject jo=jc.GetStatic<AndroidJavaObject>("appdata"))
			{
				if(jo!=null)
				{
					jo.Call("Unity_Init");
				}
				else{
					Debug.Log("Inity is null");
				}
			}
		}
	}
	void InitQHPush()
	{
		using (AndroidJavaClass jc=new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity")) {
			using(AndroidJavaObject jo=jc.GetStatic<AndroidJavaObject>("apppush"))
			{
				if(jo!=null)
				{
					jo.Call("Unity_Init");
				}
			}
		}
	}
	void Handler_PaySucceed(string info)
	{
		Debug.LogError("Handle_PaySucceed");
		if (m_Dict.ContainsKey (info) == true) 
		{

			if(purchaseSucceededEvent!=null)
			{
				if(m_Dict[info]==5)
				{
					purchaseSucceededEvent(0);
				}
				else
				{
					purchaseSucceededEvent(m_Dict[info]+1);
				}

			}
		}
	}
	void Handler_GetSimState(string strResult)
	{
		if(strResult=="false")
		{
			m_bSimReady=false;
		}
		else if(strResult=="true")
		{
			m_bSimReady=true;
		}
	}

	void Handler_ShareResult(string strResult)
	{
		if(strResult=="false")
		{
			//NativeMessagesSystem.Instance.ShowMessage("消息","分享失败","确定"); 
		}
		else if(strResult=="true")
		{
			//NativeMessagesSystem.Instance.ShowMessage("消息","分享成功","确定");
		}
		if(ShareLoadingPanel.getInstance()!=null)
		{
			ShareLoadingPanel.getInstance().ShowLoadingPanel(false);
		}
	}

	void Handle_ShowLoadingUI(string str)
	{

		if(ShareLoadingPanel.getInstance()!=null)
		{
			ShareLoadingPanel.getInstance().ShowLoadingPanel(true);
		}
	}

	void Handler_GetSimType(string strType)
	{
		int nType = -1;
		int.TryParse (strType, out nType);
		m_SimType = (SIMTYPE)nType;
		SwitchPlatInit ();
	}
 
	void Handler_PayFailed(string info)
	{
		if (purchaseFailedEvent != null&&m_nPayCodeIndex!=-1) 
		{
			if(m_nPayCodeIndex==5)
			{
				purchaseFailedEvent(0);
			}
			else
			{
				purchaseFailedEvent(m_nPayCodeIndex+1);
			}
		}
	}

	void Handler_PayCancel(string info)
	{
		if (purchaseCanceled != null && m_nPayCodeIndex != -1) {
			if(m_nPayCodeIndex==5){
				purchaseCanceled(0);
			}		
			else{
				purchaseCanceled(m_nPayCodeIndex+1);
			}
		}
	}
}
