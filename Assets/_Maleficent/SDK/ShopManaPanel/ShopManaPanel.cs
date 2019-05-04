using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ShopManaPanel : MonoBehaviour 
{
	private static ShopManaPanel m_instance=null;
	public ShopManaItem m_tem=null;
	private const int m_ManaItemCount = 5;
	private static  Dictionary<int,ShopManaItem> m_dict=new Dictionary<int, ShopManaItem>();
	private   bool m_bCreate=false;
	public Transform m_parent=null;

	public  static ShopManaPanel getInstance()
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
	void CreateTemplateUI()
	{
		m_dict.Clear ();
		for(int i=0;i<5;i++)
		{
			ShopManaItem item=(ShopManaItem)Instantiate(m_tem);
			if(item!=null)
			{
				item.ManaPoint=LoadManaPoint(i);
				item.SetIndex(i);
				item.LoadLabel(LoadLabel(i));
				item.LoadNameLabel(LoadNameLabel(i));
				item.LoadPriceLabel(LoadPriceLab(i));
				string strSprName="manapack"+i.ToString();
				item.LoadSprite(strSprName);
				item.m_btnListener.onClick=OnManaBuy;
				m_dict.Add(i,item);
				item.gameObject.SetActive(true);
				item.gameObject.transform.parent=m_parent;
				item.transform.localPosition=m_tem.transform.localPosition;
				item.transform.localScale=new Vector3(1,1,1);
			}
			/* 
			ManaInAppHolder holder=item.gameObject.GetComponent<ManaInAppHolder>();
			if(holder!=null)
			{
				holder.manaPackIndex=i;
			}
			*/
		}
		m_bCreate = true;
	}
	void OnManaBuy(GameObject go)
	{
		GameObject go_par = go.transform.parent.gameObject;
		/* 
		ManaInAppHolder holder=go_par.GetComponent<ManaInAppHolder> ();
		if(holder!=null)
		{
			holder.PurchaseProduct();
		}
		*/
	}
	string LoadNameLabel(int i)
	{

		string retText = "";
		retText=Language.Get ("MANA_PACK_"+i.ToString()+"_NAME");
		return retText;
	
		 
	}
	int LoadManaPoint(int i)
	{
		int retText = 0;
		switch(i)
		{
		case 0:
			retText=40;
			break;
		case 1:
			retText=70;
			break;
		case 2:
			retText=100;
			break;
		case 3:
			retText=130;
			break;
		case 4:
			retText=200;
			break;
		}
		return retText;
	}
	string LoadPriceLab(int i)
	{
		string retText = "";
		switch(i)
		{
		case 0:
			retText="4元";
			break;
		case 1:
			retText="6元";
			break;
		case 2:
			retText="8元";
			break;
		case 3:
			retText="10元";
			break;
		case 4:
			retText="15元";
			break;
		}
		return retText;
	}
	string LoadLabel(int i)
	{
		string retText = "";
		 
		 
		switch(i)
		{
		case 1:
			retText="60魔法，额外赠送10魔法";
			break;
		case 2:
			retText="80魔法，额外赠送20魔法";
			break;
		case 0:
			retText="40魔法";
			break;
		case 3:
			retText="100魔法，额外赠送30魔法";
			break;
		case 4:
			retText="150魔法，额外赠送50魔法";
			break;
		}
		return retText;
		 
	}



	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
