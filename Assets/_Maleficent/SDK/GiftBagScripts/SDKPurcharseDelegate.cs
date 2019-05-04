using UnityEngine;
using System.Collections;

public class SDKPurcharseDelegate 
{

	static SDKPurcharseDelegate  m_instance=null;
	static public SDKPurcharseDelegate  getInstance()
	{
		if(m_instance==null)
		{
			return (m_instance=new SDKPurcharseDelegate());
		}
		return m_instance;
	}
	void Awake()
	{
		m_instance = this;
	}



	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
/* 
	public void PurcharseProduct(int nID,InAppPurchasesSystem.ProductPurchased ok,InAppPurchasesSystem.ProductPurchased cancel,InAppPurchasesSystem.ProductPurchased fail)
	{

		InAppPurchasesSystem.OnPurchaseSuccess += ok;
		InAppPurchasesSystem.OnPurchaseFail+= fail;
		InAppPurchasesSystem.OnPurchaseCancel+= cancel;
		
		InAppPurchasesSystem.Instance.PurchaseProduct ((InAppPurchasesSystem.InAppPurchase)nID);
	}
	*/
}
