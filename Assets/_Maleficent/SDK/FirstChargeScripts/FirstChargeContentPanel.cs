using UnityEngine;
using System.Collections;

public class FirstChargeContentPanel : MonoBehaviour
{
	public BoxCollider []WidgetCollider;
	public PlayMakerFSM m_shopFSM=null;
	public UIEventListener m_btnBuy=null,m_btnCls=null;
	static private FirstChargeContentPanel m_ins=null;
	static public FirstChargeContentPanel getInstance()
	{
		return m_ins;
	}
	void Awake()
	{
		m_ins=this;
		gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start () 
	{
		if(m_btnBuy!=null)
		{
			m_btnBuy.onClick=BuyEventDelegateFuc;
		}
		if(m_btnCls!=null)
		{
			m_btnCls.onClick=ClsEventDelegateFuc;
		}
	
	}

	void ClsEventDelegateFuc(GameObject go)
	{
		ShowWindow(false);
		if(FirstChargePanel.getInstance()!=null)
			FirstChargePanel.getInstance ().ClsFsm ();
	//	ToggleWindow(false);
	}

	void ToggleWindow(bool bTog)
	{
//		for(int i=0;i<WidgetCollider.Length;i++)
//		{
//			if(WidgetCollider[i]!=null)
//			{
//				WidgetCollider[i].enabled=bTog;
//			}
//		}
		ColliderManager.ToggleCollider (bTog);
	}

	void BuyEventDelegateFuc(GameObject go)
	{
	//	buyFSM.SendEvent("Buy");
		if(m_shopFSM!=null)
		{
			m_shopFSM.SendEvent("GameShopBegin");
		   ShopUINotifyer.getInstance().IndexUI=1;
		   ShopUINotifyer.getInstance().Notify();
		}
	}
	public void ShowWindow(bool bShow)
	{
		gameObject.SetActive(bShow);
		ToggleWindow(!bShow);
	}
	// Update is called once per frame
	void Update () 
	{
	
	}
}
