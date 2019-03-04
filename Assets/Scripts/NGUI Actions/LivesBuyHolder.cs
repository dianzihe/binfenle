using UnityEngine;
using System.Collections;

public class LivesBuyHolder : InAppProductBehavior {

	public UILabel priceLabel;
	public UILabel unitsLabel;
	public UILabel packLabel;
	public UIImageButton buyButton;
	public PlayMakerFSM buyLivesPanelFSM;
	public PlayMakerFSM buyLivesButtonFSM;
	
	private string packName;
	
	void Start ()
	{
		packName = Language.Get ("LIVES_PACK_NAME");
		
		packLabel.text = packName;
		unitsLabel.text = LivesSystem.maxLives.ToString();
		//priceLabel.text = "-";
		
		InAppPurchaseId = (InAppPurchasesSystem.InAppPurchase)((int)InAppPurchasesSystem.InAppPurchase.Lives);
	}

	protected override void OnProductUpdated (InAppProduct _product) 
	{
		if(_product != null) {
			priceLabel.text = _product.formattedPrice;
		}
	}
	
	protected override bool CanPurchaseProduct (InAppProduct _product)
	{
		return (buyLivesPanelFSM == null || buyLivesPanelFSM.ActiveStateName == "In");
		
	}

	protected override string AnalyticsType ()
	{
		return ("lives_refill");

	}
	
	protected override void OnPurchaseSuccess (InAppProduct _product) 
	{
		PlayerPrefs.SetInt(LivesSystem.livesKey, LivesSystem.maxLives);
		PlayerPrefs.Save();
		LivesSystem.instance.Lives = LivesSystem.maxLives;
		
		if(buyLivesButtonFSM != null) {
			buyLivesButtonFSM.SendEvent("BuyLives");
		}
	}

}
