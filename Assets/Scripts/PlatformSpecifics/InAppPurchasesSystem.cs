using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Prime31;

public class InAppPurchasesSystem : MonoBehaviour
{
	//protected static bool fakePurchases = false; //TODO TALIN - set this to false to actually process purchases
	
	public static string locale = "en_US";
	
	protected static InAppPurchasesSystem instance;

	protected bool requestingProducts = false;

#if AMAZON_ANDROID
	protected static string[] productIds = new string[] {
		"2209679",
		"2209680",
		"2209681",
		"2209682",
		"2209683",
		"2209684",
		"2209685"
	};
#else
	protected static string[] productIds = new string[] {
		"lives",
		"potion",
		"potionset",
		"potionpile",
		"demijohn",
		"demijohnset",
		"cauldron",
		"1",
		"2",
		"3",
		"4",
		"5"
	};
#endif

	protected static string generamobileId = "com.generamobile.maleficenttest.";
	protected static string disneyId = "disney.maleficentfreefall.china.";
		
	public enum InAppPurchase {
		Lives = 0,
		ManaPack1,
		ManaPack2,
		ManaPack3,
		ManaPack4,
		ManaPack5,
		ManaPack6,
		ManaPack7,
		ManaPack8,
		ManaPack9
	}
	
	public delegate void ProductPurchased(string id);
	
	public static event ProductPurchased OnPurchaseSuccess;
	public static event ProductPurchased OnPurchaseFail;
	public static event ProductPurchased OnPurchaseCancel;
	public static string m_strOrderID="";
	
	public bool showDialogs = true;
	public bool logAnalytics = true;
	
	protected bool receivedProductList = false;
	
	protected string prefix = disneyId;
	
	protected InAppPurchase purchasingProduct;
		
	public static InAppPurchasesSystem Instance {
		get {
			if (instance == null) {
				GameObject container = new GameObject("InAppPurchasesSystem");
			
#if UNITY_IPHONE
				instance = container.AddComponent<InAppPurchasesSystemIOS>();
#elif UNITY_ANDROID
		#if AMAZON_ANDROID
				instance = container.AddComponent<InAppPurchasesSystemAmazon>();
		#else
				instance = container.AddComponent<InAppPurchasesSystemAndroid>();
		#endif
#else
				instance = container.AddComponent<InAppPurchasesSystem>();
#endif
				 
				DontDestroyOnLoad(container);
			}
			
			return instance;
		}
	}
	
	protected virtual void Awake() 
	{
		if(MaleficentTools.IsDebugBuild) {
			prefix = disneyId;
		}else {
			prefix = disneyId;
		}

		StartCoroutine(RequestProductListCoroutine());
	}

	protected IEnumerator RequestProductListCoroutine ()
	{
		while(!receivedProductList) {
			RequestProductList();
			if(receivedProductList) {
				yield return null;
			}else {
				yield return new WaitForSeconds(30);
			}
		}
	}
		
	protected void RequestProductList()
	{
		if (receivedProductList) {
			return;
		}
		
		PlatformRequestProductList(GetProducts());
	}
	
	protected string[] GetProducts()
	{
		string[] products = new string[productIds.Length];
		for (int i = 0; i < products.Length; ++i) {
			products[i] = GetPurchaseId((InAppPurchase)i);
		}
		
		return products;
	}
	
	protected virtual void PlatformRequestProductList(string[] products)
	{
		receivedProductList = true;
	}
		
	public virtual string GetPurchaseId(InAppPurchase purchase)
	{
		return prefix + productIds[(int)purchase];
	}
	
	public virtual InAppProduct GetProduct(string productId)
	{
		return null;
	}
	
	public virtual void PurchaseProduct(InAppPurchase purchase)
	{
		purchasingProduct = purchase;
		//TalkingGameSDKUtil.GetInstance ().BuyRequest ();
		if (OnPurchaseSuccess != null) {
			OnPurchaseSuccess(GetPurchaseId(purchasingProduct));
		}
	}
	
	protected void OnInAppDisabled(InAppPurchase purchase)
	{
		if (OnPurchaseFail != null) {
			OnPurchaseFail(GetPurchaseId(purchase));
		}
		
		ShowInAppDisabledWindow();
	}
	
	protected void OnProductAwaitingVerification()
	{
		Debug.Log("Purchase successful: " + productIds[(int)purchasingProduct]);
		ShowWaitingStoreWindow();
	}
	
	protected void OnVerificationSuccess()
	{
		Debug.Log("Purchase and verification successful: " + productIds[(int)purchasingProduct]);
		if (OnPurchaseSuccess != null) {
			OnPurchaseSuccess(GetPurchaseId(purchasingProduct));
		}
		
		ShowPurchasedWindow();
	}

	public void HandleMMSuccessEvent(int nIndex)
	{
		Debug.LogError( "MM_Success_Event");

		if (OnPurchaseSuccess != null)
			OnPurchaseSuccess (InAppPurchasesSystem.instance.GetPurchaseId((InAppPurchase)nIndex));

		MMSDKManager.purchaseSucceededEvent-=HandleMMSuccessEvent;
	}
	public void HandleLTCancelEvent(int nIndex)
	{
		if (OnPurchaseCancel != null) {
			OnPurchaseCancel(InAppPurchasesSystem.instance.GetPurchaseId((InAppPurchase)nIndex));		
		}
		MMSDKManager.purchaseCanceled -= HandleLTCancelEvent;
	}

	public void HandleMMFailedEvent(int nIndex)
	{
		if (OnPurchaseFail != null)
			OnPurchaseFail(InAppPurchasesSystem.instance.GetPurchaseId((InAppPurchase)nIndex));
	
		MMSDKManager.purchaseSucceededEvent-=HandleMMFailedEvent;
	}
	
	protected virtual void OnProductFailed(string error)
	{
		Debug.Log("Purchase failed: " + productIds[(int)purchasingProduct] + " Error: " + error);
		if (OnPurchaseFail != null) {
			OnPurchaseFail(GetPurchaseId(purchasingProduct));
		}
		
	
	}
	
	protected virtual void OnProductCanceled(string error)
	{
		Debug.Log("Purchase canceled: " + productIds[(int)purchasingProduct] + " Error: " + error);
		if (OnPurchaseCancel != null) {
			OnPurchaseCancel(GetPurchaseId(purchasingProduct));
		}
	}
		
	protected void ShowWaitingStoreWindow()
	{
		if (showDialogs) {
			//NativeMessagesSystem.Instance.ShowMessage(Language.Get("STORE_WAITING_TITLE"), 
			//	Language.Get("STORE_WAITING_TEXT"), "");
		}
	}
	
	protected void ShowInAppDisabledWindow()
	{
		if (showDialogs) {
			//NativeMessagesSystem.Instance.ShowMessage(Language.Get("STORE_DISABLED_TITLE"), 
			//	Language.Get("STORE_DISABLED_TEXT"), Language.Get("BUTTON_OK"));
		}
	}
	
	protected void ShowPurchasedWindow()
	{
		if (showDialogs) {
			//NativeMessagesSystem.Instance.ShowMessage(Language.Get("STORE_PURCHASED_TITLE"), 
			//	Language.Get("STORE_PURCHASED_TEXT"), Language.Get("BUTTON_OK"));
		}
	}
	
	protected void ShowFailedPurchaseWindow()
	{
		if (showDialogs) {
			//NativeMessagesSystem.Instance.ShowMessage(Language.Get("STORE_FAILED_TITLE"), 
			//	Language.Get("STORE_FAILED_TEXT"), Language.Get("BUTTON_OK"));
		}
	}
}

public class InAppProduct 
{
	public string id;
	public string currencyCode;
	public string currencyType;
	public string price;
	public string formattedPrice;
	public string strDesc;
}