using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Recorder : MonoBehaviour {
	private static Recorder m_Instance=null;
	static public Recorder getInstance()
	{
		if(m_Instance==null)
		{
			return m_Instance=new Recorder();
		}
		return m_Instance;
	}
	void Awake()
	{
		m_Instance = this;
	}
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void RecoFailedEvent()
	{
	//	TDGAMission.OnFailed( MaleficentBlackboard.Instance.level.ToString(), "步数不足");

	}

	public void RecoSuccEvent()
	{
	//	TDGAMission.OnCompleted ( MaleficentBlackboard.Instance.level.ToString ());
		ShareToolManager.PicGenerator ();
	}
	public void goBackorSlideOut()
	{
		if(SinaSharePanel.getInstance()!=null)
		{
			if(SinaSharePanel.getInstance().gameObject.activeSelf==false)
			{
				SinaSharePanel.getInstance().m_fsm.SendEvent("SlidingOutEvent");
			}
			else
			{
				SinaSharePanel.getInstance().m_fsm.SendEvent("GoBackIn");
			}
		}
	}
	static public void RecoManaEvent(string strItem,int mNum)
	{
		string strKey="";
		strItem = getStr (strItem);
		strKey +=Language.Get("In_Level__Use")+strItem+Language.Get("In_Level__UseMana_Desc")+Language.Get("In_Level__Number")+MaleficentBlackboard.Instance.level.ToString();

		Dictionary<string,object> dict=new Dictionary<string, object>();
		dict.Add(strKey,1);
		TalkingDataGA.OnEvent(strKey,dict);

		TDGAItem.OnUse (strKey, mNum);
	}

	static private string getStr(string strEngName)
	{
		switch(strEngName)
		{
		case "Crow":
			return "乌鸦";
		case "Crow2nd":
			return "新的乌鸦";
		case "Green Magic":
			return "绿雾";
		case "GrowingThorns":
			return "荆棘丛生";
		case "The Staff":
			return "权杖";
		case "Wing":
			return "风之翼";
		case "YellowPixieDust":
			return "金风";
		}
		return strEngName;
	}

	static public void RecoLifeEvent()
	{
		string strKey="";
		strKey +="玩家购买"+Language.Get("In_Level__UseLife_Desc")+"次数";
		
		Dictionary<string,object> dict=new Dictionary<string, object>();
		dict.Add(strKey,1);
		TalkingDataGA.OnEvent(strKey,dict);
	}

}
