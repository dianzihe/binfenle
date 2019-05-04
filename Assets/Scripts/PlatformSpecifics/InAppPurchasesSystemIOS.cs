using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InAppPurchasesSystemIOS : InAppPurchasesSystem
{	
#if UNITY_IPHONE	
	protected List<StoreKitProduct> productList;
#endif

	protected override void Awake() 
	{
		base.Awake();

#if UNITY_IPHONE
		StoreKitManager.productListReceivedEvent += OnProductListReceived;
		StoreKitManager.productListRequestFailedEvent += OnProductListRequestFailedEvent;

		StoreKitUtilsBinding.Instance.OnProductLocaleReceived += ProductLocaleReceived;
//		StoreKitUtilsBinding.Instance.RequestProductInformation(GetPurchaseId(InAppPurchase.Lives));
#endif
	}


	void ProductLocaleReceived (string _locale)
	{
		locale = _locale;
	}
		
	protected override void PlatformRequestProductList(string[] products)
	{
#if UNITY_IPHONE
		if(!requestingProducts) {
			requestingProducts = true;
			StoreKitBinding.requestProductData(products);
		}
#endif
	}
	
#if UNITY_IPHONE	
	protected void OnProductListReceived(List<StoreKitProduct> list)
	{
		if (list != null && list.Count > 0) 
		{
			Debug.Log("Product list received with " + list.Count + " items.");
			
			productList = list;
			receivedProductList = true;
		}
		requestingProducts = false;
	}

	protected void OnProductListRequestFailedEvent (string error)
	{
		Debug.LogError("Error retrieving products list "+ error);
		requestingProducts = false;
	}
	
	public override InAppProduct GetProduct(string productId)
	{
		if (productList == null) {
			return base.GetProduct(productId);
		}
		
		foreach (StoreKitProduct product in productList) 
		{
			if (product.productIdentifier == productId) 
			{
				InAppProduct myProduct = new InAppProduct();
				
				myProduct.id = productId;
				myProduct.price = product.price;
				myProduct.currencyCode = product.currencyCode;
				myProduct.formattedPrice = product.formattedPrice;
				
				return myProduct;
			}
		}
		
		return base.GetProduct(productId);
	}
#endif
		
	public override void PurchaseProduct(InAppPurchase purchase)
	{
#if UNITY_EDITOR
		base.PurchaseProduct(purchase);
#elif UNITY_IPHONE
		//if(MaleficentTools.IsDebugBuild) {
		//	base.PurchaseProduct(purchase);
		//} else 
		if (StoreKitBinding.canMakePayments()) 
		{			
			purchasingProduct = purchase;
			
			RequestProductList();
			
			StoreKitManager.productPurchaseAwaitingConfirmationEvent += OnProductAwaitingVerification;
			StoreKitManager.purchaseFailedEvent += OnProductFailed;
			StoreKitManager.purchaseCancelledEvent += OnProductCanceled;
			
			Debug.Log("Purchasing product: " + productIds[(int)purchasingProduct]);
			StoreKitBinding.purchaseProduct(GetPurchaseId(purchase), 1);
		}
		else {
			OnInAppDisabled(purchase);
		}
#endif
	}
	
#if UNITY_IPHONE
	protected void OnProductAwaitingVerification(StoreKitTransaction transaction)
	{
		StoreKitManager.productPurchaseAwaitingConfirmationEvent -= OnProductAwaitingVerification;
		
		StoreKitManager.purchaseSuccessfulEvent += OnVerificationSuccess;
		
		OnProductAwaitingVerification();
	}
#endif		

#if UNITY_IPHONE	
	protected void OnVerificationSuccess(StoreKitTransaction transaction)
	{
		StoreKitManager.purchaseSuccessfulEvent -= OnVerificationSuccess;
		StoreKitManager.purchaseFailedEvent -= OnProductFailed;
		StoreKitManager.purchaseCancelledEvent -= OnProductCanceled;
		
		Debug.Log("Checking transaction: " + transaction.productIdentifier + " equal to " + GetPurchaseId(purchasingProduct));
		if (transaction.productIdentifier == GetPurchaseId(purchasingProduct)) 
		{
			OnVerificationSuccess();
		}
		else {
			OnProductCanceled("Verification error");
		}
	}
#endif
	
	protected override void OnProductFailed(string error)
	{
#if UNITY_IPHONE
		StoreKitManager.productPurchaseAwaitingConfirmationEvent -= OnProductAwaitingVerification;
		StoreKitManager.purchaseSuccessfulEvent -= OnVerificationSuccess;
		StoreKitManager.purchaseFailedEvent -= OnProductFailed;
		StoreKitManager.purchaseCancelledEvent -= OnProductCanceled;
#endif
		
		base.OnProductFailed(error);
	}
	
	protected override void OnProductCanceled(string error)
	{
#if UNITY_IPHONE
		StoreKitManager.productPurchaseAwaitingConfirmationEvent -= OnProductAwaitingVerification;
		StoreKitManager.purchaseSuccessfulEvent -= OnVerificationSuccess;
		StoreKitManager.purchaseFailedEvent -= OnProductFailed;
		StoreKitManager.purchaseCancelledEvent -= OnProductCanceled;
#endif
		
		base.OnProductCanceled(error);
	}
}

