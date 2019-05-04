using UnityEngine;
using System.Collections;

public class BuyGiftsOnStart : MonoBehaviour {
	public static BuyGiftsOnStart instanse=null;
	public UIEventListener closeenvet = null;
	public UIEventListener buyevent=null;
	public UIEventListener yesevent=null;
	public string sendEvent ="ShopClose";
	public PlayMakerFSM fsm;
	//public UIEventListener noevent=null;
	int TaoZSel;
	int CostMofa;
	public PlayMakerFSM buyFSM;
	bool isbuy=false;
	int showsel;
	public BuyGiftsOnStart GetInstanse()
	{
		return instanse;
	}
	// Use this for initialization
	void Start () {
		instanse = this;
		TaoZSel = Random.Range (1, 4);
		showsel = GiftIsShow.getinstanse ().showgift;
		
		if(showsel==2)
		{
			gameObject.SetActive(true);
			if (TaoZSel == 1) {
				transform.FindChild ("taozhuang1").gameObject.SetActive (true);
				transform.FindChild ("taozhuang2").gameObject.SetActive (false);
				transform.FindChild ("taozhuang3").gameObject.SetActive (false);
			} else if (TaoZSel == 2) {
				transform.FindChild ("taozhuang1").gameObject.SetActive (false);
				transform.FindChild ("taozhuang2").gameObject.SetActive (true);
				transform.FindChild ("taozhuang3").gameObject.SetActive (false);
			} else if (TaoZSel == 3) {
				transform.FindChild ("taozhuang1").gameObject.SetActive (false);
				transform.FindChild ("taozhuang2").gameObject.SetActive (false);
				transform.FindChild ("taozhuang3").gameObject.SetActive (true);
			} else {
				transform.FindChild ("taozhuang1").gameObject.SetActive (true);
				transform.FindChild ("taozhuang2").gameObject.SetActive (false);
				transform.FindChild ("taozhuang3").gameObject.SetActive (false);
			}
		}
		if(showsel==1)
		{
			gameObject.SetActive(false);
		}
		
		
		//print ("TaoZSel" + TaoZSel);
		
	}
	
	// Update is called once per frame
	void Update () {
		if (closeenvet != null) {
			closeenvet.onClick=OnCloseClick;	
		}
		if (buyevent != null) {
			buyevent.onClick=OnBuyClick;
		}
	}
	void OnCloseClick(GameObject go)
	{
		instanse.gameObject.SetActive (false);
	}
	public void Closeisuse()
	{
		closeenvet.gameObject.GetComponent<BoxCollider> ().enabled = true;
	}
	public void Closeunuse()
	{
		closeenvet.gameObject.GetComponent<BoxCollider> ().enabled = true;
	}
	void OnBuyClick(GameObject go)
	{
		string[] daojuname={"乌鸦魔法","恶魔乌鸦魔法","权杖魔法","绿雾魔法","风之翼魔法","金风魔法","狼魔法","荆棘丛生魔法"};
		string[] daoju={"Crow_Prop_Count","Crow2nd_Prop_Count","TheStaffCost_Prop_Count","GreenMagicCost_Prop_Count","WingWindCost_Prop_Count","YellowPixieDustCost_Prop_Count","WolfHowlCost_Prop_Count","ThorwnCost_Prop_Count"};
		//print("mofanum="+	TokensSystem.Instance.ManaPoints	);
		if (TaoZSel == 1) {
			if (TokensSystem.Instance.ManaPoints >= 260) {
				CostMofa = 260;
				string strDesc = Language.Get ("SUPPORT_BUY_TOOL");
				string strDest2 = strDesc + CostMofa + "魔法值，是否确认购买？";
				if (SDKTipsWindowController.getInstance () != null) {
					SDKTipsWindowController.getInstance ().PopWindow (strDest2, Hand_ChangeCode_OK_CallBack, Hand_ChangeCode_Cancel_CallBack);

				}

				if(isbuy){
					CostItemManager.mInstance.AddCount(2,daoju[1]);	
					CostItemManager.mInstance.AddCount(3,daoju[5]);	
					CostItemManager.mInstance.AddCount(4,daoju[6]);

					//	closeenvet.onClick=OnCloseClick;	

					//gameObject.SetActive(false);
				}
			}else{
				CostMofa = 260;
				string strDesc = Language.Get ("SUPPORT_BUY_TOOL");
				string strDest2 = strDesc + CostMofa + "魔法值，您当前的魔法值为"+TokensSystem.Instance.ManaPoints+",所持有的魔法值还不足，请至商城购买，是否确认购买？";
				if (SDKTipsWindowController.getInstance () != null) {
					SDKTipsWindowController.getInstance ().PopWindow (strDest2, Hand_SignOK_CallBack, Hand_ChangeCode_Cancel_CallBack);

				}

			}
			
		} else if (TaoZSel == 2) {
			if (TokensSystem.Instance.ManaPoints >= 350) {
				CostMofa = 350;
				string strDesc = Language.Get ("SUPPORT_BUY_TOOL");
				string strDest2 = strDesc + CostMofa + "魔法值，是否确认购买？";
				if (SDKTipsWindowController.getInstance () != null) {
					SDKTipsWindowController.getInstance ().PopWindow (strDest2, Hand_ChangeCode_OK_CallBack, Hand_ChangeCode_Cancel_CallBack);

				}

				if(isbuy){
					CostItemManager.mInstance.AddCount(3,daoju[4]);	
					CostItemManager.mInstance.AddCount(4,daoju[2]);	
					CostItemManager.mInstance.AddCount(5,daoju[5]);	

					//	closeenvet.onClick=OnCloseClick;	

				//	gameObject.SetActive(false);

				}
			}else{
				CostMofa = 350;
				string strDesc = Language.Get ("SUPPORT_BUY_TOOL");
				string strDest2 = strDesc + CostMofa + "魔法值，您当前的魔法值为"+TokensSystem.Instance.ManaPoints+",所持有的魔法值还不足，请至商城购买，是否确认购买？";
				if (SDKTipsWindowController.getInstance () != null) {
					SDKTipsWindowController.getInstance ().PopWindow (strDest2, Hand_SignOK_CallBack, Hand_ChangeCode_Cancel_CallBack);

				}

			}
		} else if (TaoZSel == 3) {
			if (TokensSystem.Instance.ManaPoints >= 420) {
				CostMofa = 420;
				string strDesc = Language.Get ("SUPPORT_BUY_TOOL");
				string strDest2 = strDesc + CostMofa + "魔法值，是否确认购买？";
				if (SDKTipsWindowController.getInstance () != null) {
					SDKTipsWindowController.getInstance ().PopWindow (strDest2, Hand_ChangeCode_OK_CallBack, Hand_ChangeCode_Cancel_CallBack);

				}

				if(isbuy){
					CostItemManager.mInstance.AddCount(4,daoju[7]);	
					CostItemManager.mInstance.AddCount(5,daoju[3]);	
					CostItemManager.mInstance.AddCount(6,daoju[0]);	

					//	closeenvet.onClick=OnCloseClick;	
					
					//gameObject.SetActive(false);

				}
			}else{
				CostMofa = 420;
				string strDesc = Language.Get ("SUPPORT_BUY_TOOL");
				string strDest2 = strDesc + CostMofa + "魔法值，您当前的魔法值为"+TokensSystem.Instance.ManaPoints+",所持有的魔法值还不足，请至商城购买，是否确认购买？";
				if (SDKTipsWindowController.getInstance () != null) {
					SDKTipsWindowController.getInstance ().PopWindow (strDest2, Hand_SignOK_CallBack, Hand_ChangeCode_Cancel_CallBack);

				}

			}
		} else {
			if (TokensSystem.Instance.ManaPoints >= 260) {
				CostMofa = 260;
				string strDesc = Language.Get ("SUPPORT_BUY_TOOL");
				string strDest2 = strDesc + CostMofa + "魔法值，是否确认购买？";
				if (SDKTipsWindowController.getInstance () != null) {
					SDKTipsWindowController.getInstance ().PopWindow (strDest2, Hand_ChangeCode_OK_CallBack, Hand_ChangeCode_Cancel_CallBack);

				}

				if(isbuy){
					CostItemManager.mInstance.AddCount(2,daoju[1]);	
					CostItemManager.mInstance.AddCount(3,daoju[5]);	
					CostItemManager.mInstance.AddCount(4,daoju[6]);	
			
					//	closeenvet.onClick=OnCloseClick;	
					
					//gameObject.SetActive(false);

				}
			}else{
				CostMofa = 260;
				string strDesc = Language.Get ("SUPPORT_BUY_TOOL");
				string strDest2 = strDesc + CostMofa + "魔法值，您当前的魔法值为"+TokensSystem.Instance.ManaPoints+",所持有的魔法值还不足，请至商城购买，是否确认购买？";
				if (SDKTipsWindowController.getInstance () != null) {
					SDKTipsWindowController.getInstance ().PopWindow (strDest2, Hand_SignOK_CallBack, Hand_ChangeCode_Cancel_CallBack);

				}

			}
		}
		
		
	}
	void Hand_SignOK_CallBack()
	{
		buyFSM.SendEvent("GameShopBegin");
		ShopUINotifyer.getInstance().IndexUI=1;
		ShopUINotifyer.getInstance().Notify();
	}
	void Hand_ChangeCode_OK_CallBack()
	{
		TokensSystem.Instance.SubstractMana (CostMofa);
		print ("lestmofa=" + TokensSystem.Instance.ManaPoints);
		if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;
		if(ExchangeUIFlag.getInstance().m_bShow==false)
		{
			return;
		}
		fsm.SendEvent(sendEvent);
		gameObject.SetActive(false);
		isbuy = true;
	}
	void Hand_ChangeCode_Cancel_CallBack()
	{
		
	}
	void closeGift(GameObject go)
	{
		transform.FindChild("Close Anchor").transform.FindChild("Close Button").GetComponent<NGuiEventsToPlaymakerFsmEvents>();
	}
}
