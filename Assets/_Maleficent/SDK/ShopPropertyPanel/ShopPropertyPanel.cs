using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ShopPropertyPanel : MonoBehaviour 
{
	private static ShopPropertyPanel m_instance=null;
	public ShopPropertyItem m_tem=null;
	private const int m_ManaItemCount = 9;
	private static  Dictionary<int,ShopPropertyItem> m_dict=new Dictionary<int, ShopPropertyItem>();
	private   bool m_bCreate=false;
	public Transform m_parent=null;

	
	public  static ShopPropertyPanel getInstance()
	{
		return m_instance;
	}
	void Awake()
	{
		m_instance = this;
		m_tem.gameObject.SetActive (false);
	}
	public void HideUI()
	{
		gameObject.SetActive (false);
	}
	public void ShowUI()
	{
		gameObject.SetActive (true);
		if (m_bCreate == false)
			CreateTemplateUI ();
		ResetPanel ();
	}
	void ResetPanel()
	{
		GameObject go= m_parent.gameObject.transform.parent.gameObject;
		if(go!=null)
		{
			UIPanel panelInstance=go.GetComponent<UIPanel>();
			if(panelInstance!=null)
			{
				panelInstance.clipRange=new Vector4(0,-45,640,722);
			}
			go.transform.position=new Vector3(0,0,0);
		}
	}
	public string LoadPriceLab(int i)
	{
		string retText = "";
		switch(i)
		{
		case 0:
			retText="298";
			break;
		case 1:
			retText="30";
			break;
		case 2:
			retText="40";
			break;
		case 3:
			retText="40";
			break;
		case 4:
			retText="50";
			break;
		case 5:
			retText="50";
			break;
		case 6:
			retText="30";
			break;
		case 7:
			retText="50";
			break;
		case 8:
			retText="40";
			break;
		}
		return retText;
	}
	public string LoadNameLab(int i)
	{
		string retText = "";
		switch(i)
		{
		case 0:
			retText="生命上限";
			break;
		case 1:
			retText="乌鸦魔法";
			break;
		case 2:
			retText="金风魔法";
			break;
		case 3:
			retText="绿雾魔法";
			break;
		case 4:
			retText="风之翼魔法";
			break;
		case 5:
			retText="权杖魔法";
			break;
		case 6:
			retText="恶魔乌鸦魔法";
			break;
		case 7:
			retText="荆棘丛生魔法";
			break;
		case 8:
			retText="狼魔法";
			break;
		}
		return retText;
	}
	string LoadDescLab(int i)
	{
		string retText = "";
		switch(i)
		{
		case 0:
			retText="补满生命并增加3点生命值上限";
			break;
		case 1:
			retText="摧毁所选择的任一宝石";
			break;
		case 2:
			retText="步数+5";
			break;
		case 3:
			retText="摧毁所选择的任一颜色的所有宝石";
			break;
		case 4:
			retText="召唤一场风暴，随机摧毁多个宝石";
			break;
		case 5:
			retText="摧毁所选择位置横向或者竖向的所有宝石";
			break;
		case 6:
			retText="对换两个宝石的位置";
			break;
		case 7:
			retText="摧毁所选择的8个位置相邻的宝石";
			break;
		case 8:
			retText="重置所有宝石的位置";
			break;
		}
		return retText;
	}
	string LoadSpr(int i)
	{
		string retText = "";
		switch(i)
		{
		case 0:
			retText="menu_lives_icon";
			break;
		case 1:
			retText="powerups_crow";
			break;
		case 2:
			retText="powerups_yellowdust";
			break;
		case 3:
			retText="powerups_greenpower";
			break;
		case 4:
			retText="powerups_wind";
			break;
		case 5:
			retText="powerups_staff";
			break;
		case 6:
			retText="powerups_crow2";
			break;
		case 7:
			retText="powerups_thorns";
			break;
		case 8:
			retText="powerups_wolf";
			break;
		}
		return retText;
	}
	void CreateTemplateUI()
	{
		m_dict.Clear ();
		for(int i=0;i<9;i++)
		{
			ShopPropertyItem item=(ShopPropertyItem)Instantiate(m_tem);
			if(item!=null)
			{
//				item.SetIndex(i);
//				item.LoadLabel(LoadLabel(i));
//				string strSprName="manapack"+i.ToString();
//				item.LoadSprite(strSprName);
				item.SetIndex(i);
				item.LoadSprite( LoadSpr(i));
				item.LoadLabel(item.m_costLab,LoadPriceLab(i));
				item.LoadLabel(item.m_labName,LoadNameLab(i));
				item.LoadLabel(item.m_lab,LoadDescLab(i));
				item.m_btnListener.onClick=OnManaBuy;
				m_dict.Add(i,item);
				item.gameObject.SetActive(true);
				item.gameObject.transform.parent=m_parent;
				item.transform.localPosition=m_tem.transform.localPosition;
				item.transform.localScale=new Vector3(1,1,1);
			}
		}
		m_bCreate = true;
	}
	void OnManaBuy(GameObject go)
	{
		GameObject go_par = go.transform.parent.gameObject;
		ShopPropertyItem item =go_par.GetComponent<ShopPropertyItem> ();

		if(item!=null)
		{
			ShopBuyBoxPanel.getInstance().ShowUI(item.m_labName.text,item.m_costLab.text,LoadSpr(item.m_nIndex),item.m_nIndex,2);
		}
	}
	public void OnBuy(int i)
	{
		ShopBuyBoxPanel.getInstance ().ShowUI (LoadNameLab (i), LoadPriceLab (i), LoadSpr (i), i, 2);
	}
//	string LoadLabel(int i)
//	{
//		string retText = "";
//		switch(i)
//		{
//		case 0:
//			retText="Item_1_Label";
//			break;
//		case 1:
//			retText="Item_2_Label";
//			break;
//		case 2:
//			retText="Item_3_Label";
//			break;
//		case 3:
//			retText="Item_4_Label";
//			break;
//		case 4:
//			retText="Item_5_Label";
//			break;
//		}
//		return retText;
//	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
