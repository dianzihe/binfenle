 using UnityEngine;
using System.Collections;

public class ManaInAppHolder : InAppProductBehavior {

	public int manaPackIndex = 0;
	public UILabel priceLabel;
	public UILabel unitsLabel;
	public UILabel packLabel;
	public UIImageButton buyButton;
	public UISprite packIcon;
	public GameObject salesOffer;
	public UILabel saleLabel;
	public UILabel salePointsLabel;
	public UILabel saleExtraLabel;
	public string analyticsContext = "ingame";

	public PlayMakerFSM buyPanelFSM;
	private int manaUnits = 0;
	public int  getManaUnits()
	{
		return manaUnits;
	}

	private int saleManaUnits = 0;
	public int  getSalesManaUnits()
	{
		return saleManaUnits;
	}

	private string packName;

	void Start ()
	{
//		SetupPackUnits();
//
//		packName = Language.Get ("MANA_PACK_"+manaPackIndex+"_NAME");
//		packIcon.spriteName = string.Format("manapack{0}", manaPackIndex);
//		packIcon.MakePixelPerfect();
//
//		packLabel.text = packName;
//		unitsLabel.text = Language.Get("MANA_PACK_GET_POINTS").Replace("<points>", manaUnits.ToString());
//		//UpdatePriceLabel("-");
//
//		if(salesOffer != null) {
//			if (saleManaUnits > 0) {
//				salePointsLabel.text = "+" + saleManaUnits.ToString();
//				saleExtraLabel.text = Language.Get("MANA_PACK_EXTRA_POINTS");
//				saleLabel.text = Language.Get("SALE");
//			}else {
//				Destroy(salesOffer);
//			}
//
//		}

		InAppPurchaseId = (InAppPurchasesSystem.InAppPurchase)((int)InAppPurchasesSystem.InAppPurchase.ManaPack1 + manaPackIndex);
	}

	protected void UpdatePriceLabel (string price) 
	{
		priceLabel.text = price;
	}
	protected void InitManaUnits()
	{
		#if UNITY_ANDROID
		if (manaPackIndex == 0) 
		{
			manaUnits=40;
		}
		else if(manaPackIndex == 1) 
		{
			manaUnits=70;
		}
		else if(manaPackIndex == 2) 
		{
			manaUnits=100;
		}
		else if(manaPackIndex == 3) 
		{
			manaUnits=130;
		}
		else if(manaPackIndex == 4) 
		{
			manaUnits=200;
		}
		#endif
	}
	protected void SetupPackUnits ()
	{
		int indexForTweaksKey = manaPackIndex + 1;
		manaUnits = TweaksSystem.Instance.intValues["ManaPack" + indexForTweaksKey];
		InitManaUnits ();
		string isSaleKey = string.Format("IsSaleManaPack{0}", indexForTweaksKey);
		if (TweaksSystem.Instance.intValues.ContainsKey(isSaleKey)) {
			int saleInt = TweaksSystem.Instance.intValues[isSaleKey];
			if(saleInt == 1) {
				string salePointsKey = string.Format("SaleManaPack{0}", indexForTweaksKey);
				if(TweaksSystem.Instance.intValues.ContainsKey(salePointsKey)) {
					saleManaUnits = Mathf.Abs(TweaksSystem.Instance.intValues[salePointsKey]);
				}
			}
		}
	}

	protected override void OnProductUpdated (InAppProduct _product) 
	{
		if(_product != null) {
			UpdatePriceLabel(_product.formattedPrice);
		}
	}

	protected override bool CanPurchaseProduct (InAppProduct _product)
	{
		return true;
	}

	protected override string AnalyticsType()
	{
		return "magic";
	}

	protected override string AnalyticsContext ()
	{
		return analyticsContext;
	}
	void CancelBox()
	{

	}
	void AddFirstChargeGift()
	{
		CostItemManager.mInstance.AddCount(2,"Crow_Prop_Count");
		CostItemManager.mInstance.AddCount(3,"YellowPixieDustCost_Prop_Count");
		CostItemManager.mInstance.AddCount(4,"WolfHowlCost_Prop_Count");
	}
	protected override void OnPurchaseSuccess (InAppProduct _product) 
	{
		bool bFirstCharge=false;
		Debug.LogError("OnPurchaseSuccess:Begin...");
		ShopManaItem item=gameObject.GetComponent<ShopManaItem> ();
		if(item!=null)
		{
			int pointsToAdd = item.ManaPoint;
			if(InAppProductBehavior.IsFirstCharge()==true)
			{
				TokensSystem.Instance.AddMana(pointsToAdd*2);
				AddFirstChargeGift();
				if(FirstChargeContentPanel.getInstance()!=null&&FirstChargeContentPanel.getInstance().gameObject.activeSelf==true)
				{
					FirstChargeContentPanel.getInstance().ShowWindow(false);
				}

				InAppProductBehavior.SetFirstChargeFlag();
				bFirstCharge=true;
				if(FirstChargePanel.getInstance()!=null&&FirstChargePanel.getInstance().gameObject.activeSelf==true)
				{
					FirstChargePanel.getInstance().IsFirstChargeBtnVisiable();
				}
//				if(FirstChargePanel.getInstance()!=null)
//					FirstChargePanel.getInstance ().ClsFsm ();
			}
			else
			{
				TokensSystem.Instance.AddMana(pointsToAdd);
			}
		}
		if(bFirstCharge==true)
		{
			SDKTipsWindowController.getInstance ().PopWindow (Language.Get("FIRSTCHARGE_SUCCESS_TIPS") , CancelBox,null);
		}
		else		
		 SDKTipsWindowController.getInstance ().PopWindow ("购买成功", CancelBox,null);

		string context = AnalyticsContext();
		int level = 0;
		if (context == "ingame") 
		{
			if(MaleficentBlackboard.Instance!=null)
				level = MaleficentBlackboard.Instance.level;
			//SoundManager.Instance.PlayOneShot("mana_earn_sfx");
		}
		else
		{
			level = LoadLevelButton.lastUnlockedLevel;
		}

//#if !UNITY_EDITOR
//		if (_product != null) {
//			//AnalyticsBinding.LogInAppCurrencyAction(AnalyticsType(), pointsToAdd, _product.id, 1, context, "buy", TokensSystem.Instance.ManaPoints, _product.id, null, level);
//		}
//#endif

		if(buyPanelFSM != null) {
			buyPanelFSM.SendEvent("BuyFinished");
		}
		Debug.Log ("OnPurchaseSuccess:End...");

	

	}

	public bool IsOnSale ()
	{
		return saleManaUnits > 0;
	}
}
