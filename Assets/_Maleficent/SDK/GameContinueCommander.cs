using UnityEngine;
using System.Collections;

public class GameContinueCommander : MonoBehaviour 
{
	public LoseLabel titleLabel;
	public PlayMakerFSM fsm;

	int m_PurcharseID;
	// Use this for initialization
	void Start () 
	{
		m_PurcharseID = 8;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	void OnClick()
	{
		PlatBuy ();
	}

	void PlatBuy()
	{
	
	/* 
		InAppPurchasesSystem.OnPurchaseSuccess += HandleOnPurchaseSuccess;
		InAppPurchasesSystem.OnPurchaseFail += HandleOnPurchaseFail;
		InAppPurchasesSystem.OnPurchaseCancel += HandleOnPurchaseCancel;

		InAppPurchasesSystem.Instance.PurchaseProduct((InAppPurchasesSystem.InAppPurchase)(m_PurcharseID));
		*/
	//	StartCoroutine (TestWaitForSomeSecond ());

	}
	IEnumerator  TestWaitForSomeSecond()
	{
		yield return new WaitForSeconds(2.0f);
		HandleOnPurchaseSuccess("1");
	}
	void HandleOnPurchaseSuccess(string strID)
	{
		CostItemManager.mInstance.AddCount (1, "YellowPixieDustCost_Prop_Count");
		titleLabel.AcceptOffer ();
		titleLabel.UpdateItem ();
		fsm.SendEvent ("AcceptOffer");
		RemoveDelegate ();
	}
	void HandleOnPurchaseFail(string strID)
	{
		RemoveDelegate ();
	}
	void HandleOnPurchaseCancel(string strID)
	{
		RemoveDelegate ();
	}
	void RemoveDelegate()
	{
		/* 
		InAppPurchasesSystem.OnPurchaseSuccess -= HandleOnPurchaseSuccess;
		InAppPurchasesSystem.OnPurchaseFail -= HandleOnPurchaseFail;
		InAppPurchasesSystem.OnPurchaseCancel -= HandleOnPurchaseCancel;
		*/
	}

}
