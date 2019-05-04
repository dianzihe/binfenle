using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Prime31;
using System.Text.RegularExpressions;

public class InAppPurchasesSystemAndroid : InAppPurchasesSystem
{
	protected static string generaKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvdYX0BkpmwQw/qGIczXrmXfSZjr/Uaa/8x5bGLdc68gp6mO5TZe1ZWGoHfbbpF184EdsX50EBPFlM71mxzFkRg0yXVMffIbFeALfLzqXHCSt3RD2bsP2JxTdUVjz7uNmLgBn6uIVyi5fKwmkYDZq5ELkoj3QKNZj183afutY0IACC/aM0XEl4pqPifxMzEXE5pXy1FPLDhS6qIlieHd98mKALCRZFXuAwd+b2bWtFku+n5ng34B3jvLYu0U+fKpbtcvGT1zils0XY9tYqbyxGnXYQKaUIPWWhsAODVG58dY13+FEdd4ynhEolISzo8qEAQeXmVf4GfjcFkSSwE1FFwIDAQAB";
	
	protected static string disneyKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAi89GZ5Y5t1CtNmvpEQWQ/7ZL+qDwz9lwjwUhQmu10xtTUptDJ+x/kODg1CNMIHOMF8ZpdTFLZ0vtfQzYsKDPixEGHM1OD+WBcqOQHjPkcwdtA6XE8DAiVYKU67flxkK1gwmHB89oA3QuDD/183PTHONJbDtfkkODPIk4QGdFahMbzUHSMfUgNFTWhZHMvENEZ1SaiqRa9ZpXEVyskbN/5FRAb0d3V4BbT/E5PugCvxIsQCKOdhLF+ECrMU0S9XtKewDRU6UGTLolLlr5eETJ29Ix40Az6YSJoUmALM8mDFQr9bTD+4T3uYMepcyuHMUhFFwVQ9iuXdrIvk2DQLElWwIDAQAB";
	
	protected static string[] productExtraIds = new string[] {
		"2209605",
		"2209606",
		"2209607",
		"2209608",
		"2209609",
		"2209610",
		"2209611",
		"1",
		"2",
		"3",
		"4",
		"5"
	};


	
	protected string key = disneyKey;
	
	protected bool billingSupported = false;
	protected bool waitingForInit = false;
	
#if UNITY_ANDROID	
	protected List<GoogleSkuInfo> productList;
#endif
	
	protected Dictionary<string, string> currencies;
	
	protected override void Awake() 
	{
		if (MaleficentTools.IsDebugBuild) 
		{
			key = disneyKey;
		}
		else {
			key = disneyKey;
		}
		
		currencies = new Dictionary<string, string>();
		currencies.Add("$", "USD");
		currencies.Add("Lek", "ALL");
//		currencies.Add("ƒ", "AWG");
//		currencies.Add("ман", "AZN");
		currencies.Add("KM", "BAM");
		currencies.Add("лв", "BGN");
		currencies.Add("лв.", "BGN");
		currencies.Add("$b", "BOB");
		currencies.Add("R$", "BRL");
		currencies.Add("BZ$", "BZD");
		currencies.Add("fr.", "CHF");
//		currencies.Add("¥", "JPY");
//		currencies.Add("�?", "CRC");
		currencies.Add("Din.", "CSD");
		currencies.Add("Kč", "CZK");
		currencies.Add("kr.", "DKK");
		currencies.Add("kr", "SEK");
		currencies.Add("RD$", "DOP");
		//currencies.Add("�?","EUR");
		currencies.Add("£", "GBP");
		currencies.Add("Lari", "GEL");
		currencies.Add("Q", "GTQ");
		currencies.Add("HK$", "HKD");
		currencies.Add("L.", "HNL");
		currencies.Add("L", "HNL");
		currencies.Add("kn", "HRK");
		currencies.Add("Ft", "HUF");
		currencies.Add("Rp", "IDR");
		currencies.Add("J$", "JMD");
		currencies.Add("S", "KES");
//		currencies.Add("сом", "KGS");
//		currencies.Add("�?", "KRW");
//		currencies.Add("Т", "KZT");
//		Debug.Log(currencies["�?"]);
//		currencies.Add("�?", "LAK"); eur
		currencies.Add("Lt", "LTL");
		currencies.Add("Ls", "LVL");
		currencies.Add("ден.", "MKD");
//		currencies.Add("ден", "MKD"); kgs
//		currencies.Add("�?", "MNT"); eur
//		currencies.Add("�?", "MUR"); eur
		currencies.Add("RM", "MYR");
		//Debug.Log(currencies["N"]); //// the given key was not present in the dictionary
		//currencies.Add("N", "NIO");
		currencies.Add("C$", "NIO");
		currencies.Add("B/.", "PAB");
		currencies.Add("S/.", "PEN");
		currencies.Add("PhP", "PHP");
//		Debug.Log(currencies["�?"]);
//		currencies.Add("�?", "PHP");  eur
		currencies.Add("Rs", "PKR");
		currencies.Add("zł", "PLN");
		currencies.Add("Gs", "PYG");
		currencies.Add("lei", "RON");
//		Debug.Log(currencies["Дин."]);
//		currencies.Add("Дин.", "RSD"); mkd
		currencies.Add("р.", "RUB");
//		Debug.Log(currencies["руб"]);
		currencies.Add("руб", "RUB");
		currencies.Add("т.р.", "TJS");
		currencies.Add("m.", "TMT");
		currencies.Add("TL", "TRY");
		currencies.Add("TT$", "TTD");
		currencies.Add("NT$", "TWD");
//		Debug.Log(currencies["�?"]);
//		currencies.Add("�?", "UAH"); eur
		currencies.Add("$U", "UYU");
		currencies.Add("Bs.", "VEF");
		currencies.Add("Bs", "VEF");
//		Debug.Log(currencies["�?"]);
//		currencies.Add("�?", "VND"); eur
		currencies.Add("R", "ZAR");
		currencies.Add("Z$", "ZWL");
//		};
				
		base.Awake();
	}
	
	protected override void PlatformRequestProductList(string[] products)
	{
#if UNITY_ANDROID
		if(!waitingForInit && !billingSupported) {
			waitingForInit = true;

			GoogleIABManager.billingSupportedEvent += OnBillingSupported;
			GoogleIABManager.billingNotSupportedEvent += OnBillingNotSupported;

			GoogleIAB.init(key);
		}else if (billingSupported) {
			QueryInventory();
		}
#endif
	}
	
#if UNITY_ANDROID
	protected void OnBillingSupported()
	{
		GoogleIABManager.billingSupportedEvent -= OnBillingSupported;
		GoogleIABManager.billingNotSupportedEvent -= OnBillingNotSupported;

		waitingForInit = false;
		billingSupported = true;
		
		QueryInventory();
	}
	
	protected void OnBillingNotSupported(string error)
	{
		GoogleIABManager.billingSupportedEvent -= OnBillingSupported;
		GoogleIABManager.billingNotSupportedEvent -= OnBillingNotSupported;

		waitingForInit = false;
		billingSupported = false;
	}
	
	protected void QueryInventory()
	{
		if(!requestingProducts) {
			requestingProducts = true;

			GoogleIABManager.queryInventorySucceededEvent += OnQueryInventorySucceeded;
			GoogleIABManager.queryInventoryFailedEvent += OnQueryInventoryFailed;
			
			GoogleIAB.queryInventory(GetProducts());
		}
	}
	
	protected void OnQueryInventorySucceeded(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		GoogleIABManager.queryInventorySucceededEvent -= OnQueryInventorySucceeded;
		GoogleIABManager.queryInventoryFailedEvent -= OnQueryInventoryFailed;
		
		if (skus != null && skus.Count > 0) 
		{	
			productList = skus;
			receivedProductList = true;
		}
		
		if (purchases != null && purchases.Count > 0) 
		{
			foreach (GooglePurchase purchase in purchases) 
			{
				if (purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased) {
					GoogleIAB.consumeProduct(purchase.productId);
				}
			}
		} 

		requestingProducts = false;
	}
	
	protected void OnQueryInventoryFailed(string error)
	{
		GoogleIABManager.queryInventorySucceededEvent -= OnQueryInventorySucceeded;
		GoogleIABManager.queryInventoryFailedEvent -= OnQueryInventoryFailed;

		requestingProducts = false;
	}
	
	public override InAppProduct GetProduct(string productId)
	{
		if (productList == null) {
			return base.GetProduct(productId);
		}
		
		foreach (GoogleSkuInfo product in productList) 
		{
			if (product.productId == productId)
			{
				InAppProduct myProduct = new InAppProduct();
				
				myProduct.id = productId;
				myProduct.formattedPrice = product.price;

		        Regex regex = new Regex("(?<price>([0-9]*[.,]?[0-9]+)+)");
		        Match match = regex.Match(product.price);

		        if (match.Success)
		        {
		            myProduct.price = match.Groups["price"].Value;
					myProduct.currencyCode = product.price.Replace(myProduct.price ,"").Trim();
					if (currencies.ContainsKey(myProduct.currencyCode)) {
						myProduct.currencyCode = currencies[myProduct.currencyCode];
					}
					else if (myProduct.currencyCode.Length != 3) {
						myProduct.currencyCode = "NUL"; //fallback
					}
		        }
				else {
					myProduct.price = product.price;
					myProduct.currencyCode = "NUL"; //fallback
				}
				
				return myProduct;
			}
		}
		
		return base.GetProduct(productId);
	}
#endif
	void RemoveEvent()
	{
//		if(MMSDKManager.purchaseSucceededEvent!=null)
			MMSDKManager.purchaseSucceededEvent-=HandleMMSuccessEvent;
//		if(MMSDKManager.purchaseFailedEvent!=null)
			MMSDKManager.purchaseFailedEvent-=HandleMMFailedEvent;
//		if(MMSDKManager.purchaseCanceled!=null)
			MMSDKManager.purchaseCanceled -= HandleLTCancelEvent;
	}
	
	public override string GetPurchaseId(InAppPurchase purchase)
	{
		return prefix + productIds[(int)purchase] + productExtraIds[(int)purchase];
	}

	private void MMSDKPurchaseProduct(InAppPurchase purchase)
	{
		RemoveEvent ();
		int nIndex = (int)purchase;
		MMSDKPurUtil.getInstance ().BuyRequest (nIndex.ToString (), "", "", "", "", "");
		MMSDKManager.purchaseSucceededEvent+=HandleMMSuccessEvent;
		MMSDKManager.purchaseFailedEvent+=HandleMMFailedEvent;
	}
	private void LTSDKPurchaseProduct(InAppPurchase purchase)
	{
		RemoveEvent ();
		int nIndex = (int)purchase;
		LTSDKPurUtil.getinstance ().BuyRequest (nIndex.ToString (), "", "", "", "", "");
		MMSDKManager.purchaseSucceededEvent+=HandleMMSuccessEvent;
		MMSDKManager.purchaseFailedEvent+=HandleMMFailedEvent;
		MMSDKManager.purchaseCanceled += HandleLTCancelEvent;
		
	}
	private void DXSDKPurchaseProduct(InAppPurchase purchase)
	{
		RemoveEvent ();
		int nIndex = (int)purchase;
		DXSDKPurUtil.getInstance ().BuyRequest (nIndex.ToString (), "", "", "", "", "");
		MMSDKManager.purchaseSucceededEvent+=HandleMMSuccessEvent;
		MMSDKManager.purchaseFailedEvent+=HandleMMFailedEvent;
	}
	private void MDOSDKPurchaseProduct(InAppPurchase purchase)
	{
		RemoveEvent ();
		int nIndex = (int)purchase;
		MDOSDKPurUtil.getInstance ().BuyRequest (nIndex.ToString (), "", "", "", "", "");
		MMSDKManager.purchaseSucceededEvent+=HandleMMSuccessEvent;
		MMSDKManager.purchaseFailedEvent+=HandleMMFailedEvent;
		MMSDKManager.purchaseCanceled += HandleLTCancelEvent;
	}
	void SwitchPlatBuy(InAppPurchase purchase)
	{
		switch(MMSDKManager.m_SimType)
		{
		case MMSDKManager.SIMTYPE.DIANXIN:
			DXSDKPurchaseProduct(purchase);
			break;
		case MMSDKManager.SIMTYPE.LIANTONG:
			LTSDKPurchaseProduct(purchase);
			break;
		case MMSDKManager.SIMTYPE.YIDONG:
			MMSDKPurchaseProduct(purchase);
			break;
		case MMSDKManager.SIMTYPE.MDO:
			MDOSDKPurchaseProduct(purchase);
			break;
		case MMSDKManager.SIMTYPE.NONE:
			break;
		}
	}

	public override void PurchaseProduct(InAppPurchase purchase)
	{
		SwitchPlatBuy (purchase);
		//DXSDKPurchaseProduct (purchase);
		//MMSDKPurchaseProduct (purchase);
		//LTSDKPurchaseProduct (purchase);
		#if UNITY_EDITOR
		base.PurchaseProduct(purchase);
		#elif UNITY_ANDROID
		/*
		//if (fakePurchases) {
		//	base.PurchaseProduct(purchase);
		//} else 
		if (!billingSupported) 
		{
			RequestProductList(); //try to check again
			
			OnInAppDisabled(purchase);
		}
		else {
			purchasingProduct = purchase;
			
			RequestProductList();
			
			GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += OnProductAwaitingVerificationAndroid;
			GoogleIABManager.purchaseSucceededEvent += OnVerificationSuccess;
			GoogleIABManager.purchaseFailedEvent += OnProductFailed;

			Debug.Log("Purchasing product: " + GetPurchaseId(purchase));
			GoogleIAB.purchaseProduct(GetPurchaseId(purchase));
		}
		*/
	#endif
	}

	protected void OnProductAwaitingVerificationAndroid(string purchaseData, string signature)
	{
#if UNITY_ANDROID
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= OnProductAwaitingVerificationAndroid;
#endif				
		OnProductAwaitingVerification();
	}


#if UNITY_ANDROID
	protected void OnVerificationSuccess(GooglePurchase purchase)
	{
#if UNITY_ANDROID
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= OnProductAwaitingVerificationAndroid;
		GoogleIABManager.purchaseSucceededEvent -= OnVerificationSuccess;
		GoogleIABManager.purchaseFailedEvent -= OnProductFailed;
		
		GoogleIAB.consumeProduct(purchase.productId);
#endif				
		if (purchase.productId == GetPurchaseId(purchasingProduct)) 
		{
			OnVerificationSuccess();
		}
		else {
			OnProductCanceled("Verification error");
		}

		TalkingGameSDKUtil.GetInstance ().BuyResponse(InAppPurchasesSystem.m_strOrderID);
		FlurrtSDKUtil.GetInstance ().BuyResponse(InAppPurchasesSystem.m_strOrderID);
	}
#endif




	 
	
	protected override void OnProductFailed(string error)
	{
		if (error.ToLower().Contains("user canceled") || error.ToLower().Contains("user cancelled"))
		{
			OnProductCanceled(error);
			return;
		}
		
#if UNITY_ANDROID
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= OnProductAwaitingVerificationAndroid;
		GoogleIABManager.purchaseSucceededEvent -= OnVerificationSuccess;
		GoogleIABManager.purchaseFailedEvent -= OnProductFailed;
#endif
		
		base.OnProductFailed(error);
	}
	
	protected override void OnProductCanceled(string error)
	{
#if UNITY_ANDROID
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= OnProductAwaitingVerificationAndroid;
		GoogleIABManager.purchaseSucceededEvent -= OnVerificationSuccess;
		GoogleIABManager.purchaseFailedEvent -= OnProductFailed;
#endif
		
		base.OnProductCanceled(error);
	}
	
	/*protected void OnLevelWasLoaded()
	{
#if UNITY_ANDROID
		QueryInventory();
#endif
	}*/
}
