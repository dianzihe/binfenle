using UnityEngine;
using System.Collections;
using System;

public class ShopBuyBoxPanel : MonoBehaviour 
{
	public GameObject m_tabGO=null,m_closeGO,m_comboGO=null;
	private static ShopBuyBoxPanel m_instance=null;
	public UISprite m_iconSpr=null;
	public UIEventListener m_Minus, m_Plus,m_buyBtn;
	public UILabel m_title, m_btnLab,m_cnt;
	private int m_count=1;
	public int m_nSinglePrice = -1;
	public UIEventListener m_close;
	int m_nWhich,m_UIType;

	public  static ShopBuyBoxPanel getInstance()
	{
		return m_instance;
	}
	void Awake()
	{
		m_instance = this;
		gameObject.SetActive (false);
	}
	public void HideUI()
	{
		gameObject.SetActive (false);
		EnableInput (true);
	}
	public void ShowUI(string strTitle,string strBtn,string strSprName,int nwhich,int nComboUI=1)
	{
		gameObject.SetActive (true);
		m_title.text = strTitle;
		m_btnLab.text = strBtn;
		m_iconSpr.spriteName = strSprName;
		m_cnt.text = "1";
		m_count = 1;
		m_nSinglePrice = int.Parse (strBtn);
		m_nWhich = nwhich;
		m_UIType = nComboUI;
		EnableInput (false);
	}
	void EnableInput(bool bInput)
	{
		BoxCollider [] col = m_tabGO.GetComponentsInChildren<BoxCollider> ();
		if(col!=null)
		{
			for(int i=0;i<col.Length;i++)
			{
				col[i].enabled=bInput;
			}
		}
		if(m_closeGO!=null)
		{
			m_closeGO.GetComponent<BoxCollider>().enabled=bInput;
		}



			BoxCollider [] col_Combo =m_comboGO.GetComponentsInChildren<BoxCollider> ();
			if(col_Combo!=null)
			{

				for(int i=0;i<col_Combo.Length;i++)
				{
					if(col_Combo[i]!=null)
						col_Combo[i].enabled=bInput;
				}

			}


	}
	void OnCloseUI(GameObject go)
	{
		HideUI ();
	}
	// Use this for initialization
	void Start () 
	{
		if(m_Minus!=null)
		{
			m_Minus.onClick=OnMinusClk;
		}
		if(m_close!=null)
		{
			m_close.onClick=OnCloseUI;
		}
		if(m_Plus!=null)
		{
			m_Plus.onClick=OnPlusClk;
		}
		if(m_buyBtn!=null)
		{
			m_buyBtn.onClick=OnBuyClk;
		}
	
	}
	void OnMinusClk(GameObject go)
	{
		if(m_count<=1)
		{
			return;
		}
		m_count--;
		m_cnt.text = m_count.ToString ();
		int total_pri = m_nSinglePrice * m_count;
		m_btnLab.text = total_pri.ToString();
	}
	void OnPlusClk(GameObject go)
	{

		m_count++;
		m_cnt.text = m_count.ToString ();
		int total_pri = m_nSinglePrice * m_count;
		m_btnLab.text = total_pri.ToString();

	}
	static void NotifyBuyIsSuccess(string strFsmName,string strEventName)
	{
		GameObject go= GameObject.Find (strFsmName);
		if(go!=null)
		{
			PlayMakerFSM fsm=go.GetComponent<PlayMakerFSM>();
			if(fsm!=null)
			{
				fsm.SendEvent(strEventName);
			}
		}
	}
	void OnBuyClk(GameObject go)
	{
		int ntotal_price = int.Parse (m_btnLab.text);
		if(m_UIType==1)
		{
			if(TokensSystem.Instance.ManaPoints>=ntotal_price)
			{
				TokensSystem.Instance.SubstractMana(ntotal_price);
				GiveGift(m_nWhich);

				SDKTipsWindowController.getInstance().PopWindow("恭喜你购买成功！",CancelBox,null);
				RecordProperty();
			}
			else
			{
				SDKTipsWindowController.getInstance().PopWindow("魔法值不够,请充值！",ClosePanel,null);
			}
		}
		else
		{
			if(TokensSystem.Instance.ManaPoints>=ntotal_price)
			{
				TokensSystem.Instance.SubstractMana(ntotal_price);
				GiveProperty(m_nWhich);

				SDKTipsWindowController.getInstance().PopWindow("恭喜你购买成功！",CancelBox,null);
				RecordProperty();
				using (AndroidJavaClass jc=new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity")) 
				{
					using(AndroidJavaObject jo=jc.GetStatic<AndroidJavaObject>("appdata"))
					{
						jo.Call("BuySum",ShopPropertyPanel.getInstance().LoadNameLab(m_nWhich),1,Convert.ToInt32(ShopPropertyPanel.getInstance().LoadPriceLab(m_nWhich)));
						jo.Call("buy");
					}
				}
			}
			else
			{
				SDKTipsWindowController.getInstance().PopWindow("魔法值不够,请充值！",ClosePanel,null);
			}
		}

	}

	void RecordProperty()
	{
		int ntotal_price = int.Parse (m_btnLab.text);
		GetUniqueOrderID();	
		Debug.Log(OrderManager.m_CurrentOrderID);

		TDGAItem.OnPurchase (m_title.text+OrderManager.m_CurrentOrderID,m_count,ntotal_price);


	}
	void GetUniqueOrderID()
	{
		string OrderID=RandomNumberGenerator.GetInstance().GetRandomKey;
		while(OrderManager.getInstance().AddOrderLst(OrderID)==false)
		{
			OrderID=RandomNumberGenerator.GetInstance().GetRandomKey;
		}
	}

	void GiveGift(int nwh)
	{
		switch(nwh)
		{
		case 1:
			AddProperty("Crow_Prop_Count",m_count*2);
			AddProperty("YellowPixieDustCost_Prop_Count",m_count*3);
			AddProperty("WolfHowlCost_Prop_Count",m_count*4);
			break;
		case 2:
			AddProperty("WingWindCost_Prop_Count",m_count*3);
			AddProperty("TheStaffCost_Prop_Count",m_count*4);
			AddProperty("YellowPixieDustCost_Prop_Count",m_count*5);
			break;
		case 3:
			AddProperty("ThorwnCost_Prop_Count",m_count*4);
			AddProperty("GreenMagicCost_Prop_Count",m_count*5);
			AddProperty("Crow2nd_Prop_Count",m_count*6);
			break;
		}
	}
	void GiveProperty(int nwh)
	{
		switch(nwh)
		{
		case 0:
			AddLives();
			break;
		case 1:
			AddProperty("Crow_Prop_Count",m_count);
			break;
		case 2:
			AddProperty("YellowPixieDustCost_Prop_Count",m_count);
			break;
		case 3:
			 
			AddProperty("GreenMagicCost_Prop_Count",m_count );
		 
			break;

		case 4:
			AddProperty("WingWindCost_Prop_Count",m_count);
			break;
		case 5:
			AddProperty("TheStaffCost_Prop_Count",m_count);
			break;
		case 6:
			
			AddProperty("Crow2nd_Prop_Count",m_count );
			
			break;
		case 7:
			
			AddProperty("ThorwnCost_Prop_Count",m_count );
			
			break;
		case 8:
			
			AddProperty("WolfHowlCost_Prop_Count",m_count );
			
			break;

		}
	}
	void AddProperty(string str,int nCnt)
	{
		CostItemManager.mInstance.AddCount (nCnt, str);
	}
	void AddLives()
	{
		PlayerPrefs.SetInt(LivesSystem.livesKey, 8);
		PlayerPrefs.Save();
		LivesSystem.instance.Lives = 8;
	}
	void CancelBox()
	{
		HideUI ();
		NotifyBuyIsSuccess("Buy Panel","BuyFinished");
	}
	void ClosePanel()
	{

		ShopUINotifyer.getInstance ().IndexUI = 1;
		ShopUINotifyer.getInstance ().Notify ();
		HideUI ();

	}
	// Update is called once per frame
	void Update () 
	{
	
	}
}
