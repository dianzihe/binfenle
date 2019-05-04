using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class InAppProductBehavior : MonoBehaviour {
	protected static bool purchaseInProcess = false; 
	
	protected bool purchasing = false;
	private InAppPurchasesSystem.InAppPurchase inAppPurchaseId;
	private string inAppPurchaseStrId;
	public string getAppPurchaseID()
	{
		return inAppPurchaseStrId;
	}

	private InAppProduct product;

	protected void OnDestroy ()
	{
		if(purchasing) {
			ClosePurchase();
		}else {
			StopCoroutine("UpdateProductCoroutine");
		}
	}

	public InAppPurchasesSystem.InAppPurchase InAppPurchaseId
	{
		get {
			return inAppPurchaseId;
		}set {
			if(!purchasing) {
				inAppPurchaseId = value;
				UpdateProduct();
			}
		}
	}

	public InAppProduct Product
	{
		get {
			return product;
		}
	}
	int []priceArray=new int[]{4,6,8,10,15,6};
	public void PurchaseProduct ()
	{

		if(!purchaseInProcess && CanPurchaseProduct(product)) 
		{
			purchaseInProcess = true;
			purchasing = true;
			InAppPurchasesSystem.OnPurchaseSuccess += HandleOnPurchaseSuccess;
			InAppPurchasesSystem.OnPurchaseFail += HandleOnPurchaseFail;
			InAppPurchasesSystem.OnPurchaseCancel += HandleOnPurchaseCancel;

	 	//InAppPurchasesSystem.Instance.PurchaseProduct(InAppPurchaseId);
			 ShowMsgBox();
		}
	}

	public static bool IsFirstCharge()
	{
		int nChargeFlag=0;
		nChargeFlag=PlayerPrefs.GetInt("FirstCharge_Already_Ready",-1);
		if(nChargeFlag==-1)
		{
			return true;
		}
		else if(nChargeFlag==1)
		{
			return false;
		}
		return true;
	}


	
	public static void SetFirstChargeFlag()
	{
		PlayerPrefs.SetInt("FirstCharge_Already_Ready",1);
		PlayerPrefs.Save();
	}

	void GetUniqueOrderID()
	{
		string OrderID=RandomNumberGenerator.GetInstance().GetRandomKey;
		while(OrderManager.getInstance().AddOrderLst(OrderID)==false)
		{
			OrderID=RandomNumberGenerator.GetInstance().GetRandomKey;
		}
	}
	void ShowMsgBox()
	{
		if(MMSDKManager.m_bSimReady==true)
		{
			if(MMSDKManager.m_SimType==MMSDKManager.SIMTYPE.NONE)
			{
				string strDesc=Language.Get ("SIM_TYPE_STORE_FAILED_TEXT");
				Debug.Log("STRDESC");
				Debug.Log(strDesc);
				if(SDKTipsWindowController.getInstance()!=null)
				{
					SDKTipsWindowController.getInstance().PopWindow(strDesc,Handle_NoSimState_CallBack,Handle_NoSimState_CallBack);
				}
				return;
			}
			ManaInAppHolder holder=gameObject.GetComponent<ManaInAppHolder>();
			if (holder != null) 
			{
				string strDesc=Language.Get ("BUYMANA_BITTON_0" + (holder.manaPackIndex+1).ToString());
				Debug.Log("STRDESC");
				Debug.Log(strDesc);
				if(SDKTipsWindowController.getInstance()!=null)
				{
					SDKTipsWindowController.getInstance().PopWindow(strDesc,HandleOKCallback,HandleCancelCallback);
				}
			}
			else
			{
				string strDesc=Language.Get ("BUYHEART_BITTON_01");
				Debug.Log("STRDESC");
				Debug.Log(strDesc);
				if(SDKTipsWindowController.getInstance()!=null)
				{
					SDKTipsWindowController.getInstance().PopWindow(strDesc,HandleOKCallback,HandleCancelCallback);
				}
			}
		}
		else
		{
			string strDesc=Language.Get ("STORE_FAILED_TEXT");
			Debug.Log("STRDESC");
			Debug.Log(strDesc);
			if(SDKTipsWindowController.getInstance()!=null)
			{
				SDKTipsWindowController.getInstance().PopWindow(strDesc,Handle_NoSimState_CallBack,Handle_NoSimState_CallBack);
			}
		}
	}

	void Handle_NoSimState_CallBack()
	{
		ClosePurchase();
	}

	void HandleOKCallback()
	{
		SwitchPlatBuy ();
		//MMSDKBuy ();
		//LTSDKBuy ();
	//	
	}


	void SwitchPlatBuy()
	{
		switch(MMSDKManager.m_SimType)
		{
		case MMSDKManager.SIMTYPE.DIANXIN:
			DXSDKBuy ();
			break;
		case MMSDKManager.SIMTYPE.LIANTONG:
			LTSDKBuy();
			break;
		case MMSDKManager.SIMTYPE.YIDONG:
			MMSDKBuy ();
			break;
		case MMSDKManager.SIMTYPE.NONE:
			return;
		}

	}
	string getMainWayStr ()
	{
		switch(MMSDKManager.m_SimType)
		{
		case MMSDKManager.SIMTYPE.DIANXIN:
			return "爱游戏";
		case MMSDKManager.SIMTYPE.LIANTONG:
			return "沃商店";
		case MMSDKManager.SIMTYPE.YIDONG:
			return "移动MM";
		case MMSDKManager.SIMTYPE.NONE:
			return "";
		}
		return "";
	}
	void RecordBuyEvent()
	{
		string strChannel="酷我音乐";
		string strMainChargeWay = getMainWayStr ();
		strChannel += strMainChargeWay;
		InAppPurchasesSystem.m_strOrderID=inAppPurchaseStrId;
		ShopManaItem holder=gameObject.GetComponent<ShopManaItem>();
		ManaInAppHolder ex = gameObject.GetComponent<ManaInAppHolder> ();
		if(holder!=null&&ex!=null)
		{
			GetUniqueOrderID();
			Debug.Log(OrderManager.m_CurrentOrderID);
			FlurrtSDKUtil.GetInstance().BuyRequest(OrderManager.m_CurrentOrderID, holder.m_lab.text,holder.m_priceLab.text,"CNY",holder.ManaPoint.ToString(),strChannel);
			TDGAVirtualCurrency.OnChargeRequest(OrderManager.m_CurrentOrderID,holder.m_lab.text,priceArray[ex.manaPackIndex],"CNY",holder.ManaPoint,strChannel);

			TalkingGameSDKUtil.GetInstance().BuyResponse(OrderManager.m_CurrentOrderID);
			FlurrtSDKUtil.GetInstance().BuyResponse(OrderManager.m_CurrentOrderID);
			Debug.Log("Charge Request Begin!");
		}
//		else
//		{
//			LivesBuyHolder live_holder=gameObject.GetComponent<LivesBuyHolder>();
//			if(live_holder!=null)
//			{
//				GetUniqueOrderID();
//				FlurrtSDKUtil.GetInstance().BuyRequest(OrderManager.m_CurrentOrderID,live_holder.packLabel.text,live_holder.priceLabel.text,"CNY","5",strChannel);
//				TDGAVirtualCurrency.OnChargeRequest (OrderManager.m_CurrentOrderID,live_holder.packLabel.text,priceArray[5],"CNY",6.0,strChannel);
//
//				TalkingGameSDKUtil.GetInstance().BuyResponse(OrderManager.m_CurrentOrderID);
//				Recorder.RecoLifeEvent();
//				FlurrtSDKUtil.GetInstance().BuyResponse(OrderManager.m_CurrentOrderID);
//				Debug.Log("Charge Request End!");
//			}
//		}

	}
	void HandleCancelCallback()
	{
		ClosePurchase();
	}
	void DXSDKBuy()
	{
		#if UNITY_ANDROID
		ManaInAppHolder holder=gameObject.GetComponent<ManaInAppHolder>();
		if(holder!=null)
		{
			InAppPurchasesSystem.Instance.PurchaseProduct((InAppPurchasesSystem.InAppPurchase)(holder.manaPackIndex));
			MMSDKManager.m_nPayCodeIndex=holder.manaPackIndex;
		}
		else
		{
			InAppPurchasesSystem.Instance.PurchaseProduct((InAppPurchasesSystem.InAppPurchase)(5));
			MMSDKManager.m_nPayCodeIndex=5;
		}
		#elif !UNITY_ANDROID
		InAppPurchasesSystem.Instance.PurchaseProduct(inAppPurchaseId);
		#endif
	}

	void MMSDKBuy()
	{
		#if UNITY_ANDROID
		ManaInAppHolder holder=gameObject.GetComponent<ManaInAppHolder>();
		if(holder!=null)
		{
			InAppPurchasesSystem.Instance.PurchaseProduct((InAppPurchasesSystem.InAppPurchase)(holder.manaPackIndex));
			MMSDKManager.m_nPayCodeIndex=holder.manaPackIndex;
		}
		else
		{
			InAppPurchasesSystem.Instance.PurchaseProduct((InAppPurchasesSystem.InAppPurchase)(5));
			MMSDKManager.m_nPayCodeIndex=5;
		}
		#elif !UNITY_ANDROID
			InAppPurchasesSystem.Instance.PurchaseProduct(inAppPurchaseId);
		#endif
	}
	void LTSDKBuy()
	{
		
		#if UNITY_ANDROID
		ManaInAppHolder holder=gameObject.GetComponent<ManaInAppHolder>();
		if(holder!=null)
		{
			InAppPurchasesSystem.Instance.PurchaseProduct((InAppPurchasesSystem.InAppPurchase)(holder.manaPackIndex));
			MMSDKManager.m_nPayCodeIndex=holder.manaPackIndex;
		}
		else
		{
			InAppPurchasesSystem.Instance.PurchaseProduct((InAppPurchasesSystem.InAppPurchase)(5));
			MMSDKManager.m_nPayCodeIndex=5;
		}
		#elif !UNITY_ANDROID
		InAppPurchasesSystem.Instance.PurchaseProduct(inAppPurchaseId);
		#endif
	}
	protected void UpdateProduct ()
	{
		StopCoroutine("UpdateProductCoroutine");

		inAppPurchaseStrId = InAppPurchasesSystem.Instance.GetPurchaseId(inAppPurchaseId);
#if !UNITY_EDITOR
		StartCoroutine(UpdateProductCoroutine());
#endif
	}

	protected void ClosePurchase ()
	{
		if(purchasing) {
			purchasing = false;
			purchaseInProcess = false;

			InAppPurchasesSystem.OnPurchaseSuccess -= HandleOnPurchaseSuccess;
			InAppPurchasesSystem.OnPurchaseFail -= HandleOnPurchaseFail;
			InAppPurchasesSystem.OnPurchaseCancel -= HandleOnPurchaseCancel;
		}
	}

	public IEnumerator UpdateProductCoroutine()
	{
		while(product == null) {
			product = InAppPurchasesSystem.Instance.GetProduct(inAppPurchaseStrId);
			if(product == null) {
				yield return new WaitForSeconds(3f);
			}
		}

		OnProductUpdated(product);
	}

	#region Hooks

	protected abstract bool CanPurchaseProduct (InAppProduct _product);
	protected abstract string AnalyticsType();

	protected virtual string AnalyticsSubtype()
	{
		return "";
	}

	protected virtual string AnalyticsContext()
	{
		return "";
	}
	
	protected virtual void OnProductUpdated (InAppProduct _product) 
	{
		
	}
	
	protected virtual void OnPurchaseSuccess (InAppProduct _product) 
	{
		
	}
	
	protected virtual void OnPurchaseCancel (InAppProduct _product) 
	{
		
	}
	
	protected virtual void OnOnPurchaseFail (InAppProduct _product) 
	{
		
	}

	#endregion


	#region Event Handlers

	protected void HandleOnPurchaseCancel (string _id)
	{
		Debug.Log ("HandleOnPurchaseCancel:Begin...");
		if(string.Equals(_id, inAppPurchaseStrId)) 
		{
			ClosePurchase();
			OnPurchaseCancel(product);
			Dictionary<string,object> dict=new Dictionary<string, object>();
			dict.Add("Charge Failed or Cancel!","Event:User Cancel");
			TalkingDataGA.OnEvent("Charge Failed or Cancel!",dict); 
			Debug.Log("Cancel");
			FlurrtSDKUtil.GetInstance().LogError("Charge Failed or Cancel!","Event:User Cancel","");
		}
		Debug.Log ("HandleOnPurchaseCancel:Begin...");
	}

	protected void HandleOnPurchaseFail (string _id)
	{
		Debug.Log ("HandleOnPurchaseFail:Begin...");
		if(string.Equals(_id, inAppPurchaseStrId)) {
			ClosePurchase();
			OnOnPurchaseFail(product);
			Dictionary<string,object> dict=new Dictionary<string, object>();
			dict.Add("Charge Failed or Cancel!","Event:Charge Failed");
			TalkingDataGA.OnEvent("Charge Failed or Cancel!",dict); 
			FlurrtSDKUtil.GetInstance().LogError("Charge Failed or Cancel!","Event:Charge Failed","");
		}
		Debug.Log ("HandleOnPurchaseFail:End...");
	}

	protected void HandleOnPurchaseSuccess (string _id)
	{
		Debug.Log ("HandleOnPurchaseSuccess:Begin...");

		if(string.Equals(_id, inAppPurchaseStrId)) 
		{
			ClosePurchase();
			OnPurchaseSuccess(product);
			UpdateAnalytics(product);
			RecordBuyEvent ();

		}
		Debug.Log ("HandleOnPurchaseSuccess:End...");
	}

	protected void UpdateAnalytics(InAppProduct _product)
	{
		Debug.Log ("UpdateAnalytics:Begin...");
		if (_product != null) 
		{
			string analyticsType = AnalyticsType();
			string analyticsSubtype = AnalyticsSubtype();
			string analyticsContext = AnalyticsContext();

			float price;
			if (!float.TryParse(_product.price , out price))
			{
				price = 0.99f;
			}
			
			//AnalyticsBinding.LogEventPaymentAction(product.currencyCode, InAppPurchasesSystem.locale, -price, _product.id, 1, analyticsType, analyticsSubtype, analyticsContext, LoadLevelButton.lastUnlockedLevel);
		}
		Debug.Log ("UpdateAnalytics:End...");
	}
	#endregion
}
