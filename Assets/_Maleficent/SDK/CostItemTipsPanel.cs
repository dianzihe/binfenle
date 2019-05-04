using UnityEngine;
using System.Collections;

public class CostItemTipsPanel : MonoBehaviour
{
	public UILabel m_lab=null;
	private static CostItemTipsPanel m_Instance=null;
	public static CostItemTipsPanel getInstance()
	{
		return m_Instance;
	}
	// Use this for initialization
	void Awake()
	{
		m_Instance = this;
		m_Instance.gameObject.SetActive (false);
	}
	void Start () 
	{
	
	}
	public void PopWindow(string str)
	{
		m_lab.text = getText (str);
		if(m_lab.text!="")
			m_Instance.gameObject.SetActive (true);
	}
	public void HideWindow()
	{
		m_Instance.gameObject.SetActive (false);
	}
	string getText(string str)
	{
		string strtext = "";
		switch(str)
		{
		case "Crow_Prop_Count":
			strtext= "请在游戏中选择一个格子！";
			break;
		case "Crow2nd_Prop_Count":
			strtext= "请在游戏中选择两个格子！";
			break;
		case "TheStaffCost_Prop_Count":
			strtext= "请在游戏中划出一个方向！";
			break;
		case "GreenMagicCost_Prop_Count":
			strtext= "请在游戏中选择任意一个格子！";
			break;
		case "WingWindCost_Prop_Count":

			break;
		case "YellowPixieDustCost_Prop_Count":

			break;
		case "WolfHowlCost_Prop_Count":

			break;
		case "ThorwnCost_Prop_Count":
			strtext= "请在游戏中选择八个格子！" ;
			break;
		}
		return strtext;
	}
	// Update is called once per frame
	void Update () 
	{
	
	}
}
