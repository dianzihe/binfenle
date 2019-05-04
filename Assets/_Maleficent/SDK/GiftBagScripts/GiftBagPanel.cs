using UnityEngine;
using System.Collections;

public class GiftBagPanel : MonoBehaviour 
{
	public PlayMakerFSM fsm;
	private int m_nwhich=1;
	public GiftBagItem m_1,m_2;
	static GiftBagPanel m_instance=null;
	static public GiftBagPanel getInstance()
	{
		return m_instance;
	}
	void Awake()
	{
		m_instance = this;
	}
	public void ShowUI(int nUI)
	{
		GiftBagController.getInstance().ToggleWidgetCol(false);
	//	FirstChargePanel.getInstance().ColSetter(false);

		gameObject.SetActive (true);
		LoadUI ();
		if(nUI==1)
		{
			m_1.gameObject.SetActive(true);
			m_2.gameObject.SetActive(false);
		}
		else if(nUI==2)
		{
			m_1.gameObject.SetActive(false);
			m_2.gameObject.SetActive(true);
		}
	}
	public void HideUI()
	{
		m_1.gameObject.SetActive(false);
		m_2.gameObject.SetActive(false);
		GiftBagController.getInstance ().ShowOtherUI ();
	}
	void LoadUI()
	{
		if(m_1!=null)
		{
			m_1.m_lis.onClick=OnBuy;
			m_1.m_cls.onClick=OnCls;
		}
		if(m_2!=null)
		{
			m_2.m_lis.onClick=OnBuy;
			m_2.m_cls.onClick=OnCls;
		}
	}

	void OnCls(GameObject go)
	{
		fsm.SendEvent ("ShopClose");
		HideUI ();
	}
	void OnBuy(GameObject go)
	{
		GameObject go_par=go.transform.parent.gameObject;
		if(go_par!=null)
		{
			GameObject go_par_par=go_par.transform.parent.gameObject;
			if(go_par_par!=null)
			{
				GiftBagItem item=go_par_par.GetComponent<GiftBagItem>();
				if(item!=null)
				{
					switch(item.m_index)
					{
					case 0:
						m_nwhich=1;
						 //SDKPurcharseDelegate.getInstance().PurcharseProduct(6,ok,cancel,fail);
						//StartCoroutine(TestWaitForSomeSecond());

						break;
					case 1:
						m_nwhich=2;
						 //SDKPurcharseDelegate.getInstance().PurcharseProduct(7,ok,cancel,fail);
						//StartCoroutine(TestWaitForSomeSecond());
					
						break;
					}
				}
			}
		}
	}
	IEnumerator  TestWaitForSomeSecond()
	{
		yield return new WaitForSeconds(2.0f);
		ok("1");
	}
	void AddProperty(string str,int nCnt)
	{
		CostItemManager.mInstance.AddCount (nCnt, str);
	}
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	void cancel(string strID)
	{
		RemoveDelegate ();
	}
	void ok(string strID)
	{
		if(m_nwhich==1)
		{
			AddProperty("WingWindCost_Prop_Count", 2);
			AddProperty("Crow_Prop_Count", 3);
			AddProperty("YellowPixieDustCost_Prop_Count", 4);
			SetValue("Gift_5","1");
			HideUI ();
			ShowUI(2);
			//GiftBagController.getInstance().OtherUI(false);
		}
		else if(m_nwhich==2)
		{
			AddProperty("TheStaffCost_Prop_Count", 4);
			AddProperty("ThorwnCost_Prop_Count", 3);
			AddProperty("WolfHowlCost_Prop_Count", 5);
			SetValue("Gift_6","1");

			fsm.SendEvent ("ShopClose");
			GiftBagController.getInstance().Hide();
			gameObject.SetActive(false);
		}
		RemoveDelegate ();
		SDKTipsWindowController.getInstance().PopWindow("恭喜你,购买大礼包成功!",OKEvent,null);

	}
	void OKEvent()
	{
		}
	void SetValue(string str,string strValue)
	{
		PlayerPrefs.SetString (str,strValue);
	}
	void fail(string strID)
	{
		RemoveDelegate ();
	}
	void RemoveDelegate()
	{
		/* 
		InAppPurchasesSystem.OnPurchaseSuccess -= ok;
		InAppPurchasesSystem.OnPurchaseFail -= fail;;
		InAppPurchasesSystem.OnPurchaseCancel -= cancel;
		*/
	}
}
