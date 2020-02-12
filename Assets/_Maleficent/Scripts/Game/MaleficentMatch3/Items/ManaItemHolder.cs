using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManaItemHolder : MonoBehaviour, ISelectableItem 
{
	public UISprite m_AddSignIcon=null;
	bool m_bAdd=false;
	public static bool usingItem = false;
	private static Dictionary<System.Type, int> usages;

	public event System.Action<ISelectableItem> SelectionEvent;
	public event System.Action<ISelectableItem> EndSelectionEvent;

	public event System.Action OnItemClick;
	public event System.Action OnFinishUsingItem;
	public event System.Action OnCancelUsingItem;

	public string selectableItemName = "";
	public Match3BoardGameLogic boardLogic;
	public GameObject itemPrefab;
	public Transform itemPosition;
	public UILabel manaCostLabel;
	public ManaItemHolder twinItem;
	public ManaItem itemPrefabManaItem;
	public UISprite itemBg;
	public string spriteOn;
	public string spriteOff;
	public string spriteSelected;
	public UISprite itemIcon;
	public UISprite itemCancelIcon;
	//public PlayMakerFSM buyFSM;
	public Color colorCostLabelOn;
	public Color colorCostLabelOff;
	public bool ignoreInputs = false;

	[HideInInspector]
	public ManaPowerBallController manaPowerBall;
	
	private ManaItem item;
	private bool waitingForItemInvoke = false;

	public static bool FreeGiveAwayOfItem(ManaItem _manaItem)
	{
		bool res = _manaItem.JustBeenUnlocked() && !usages.ContainsKey(_manaItem.GetType());
		return res;
	}

	static bool TutorialInProgress()
	{
		bool res = false;
		/* 
		if(MaleficentTutorialsManager.Instance != null) {
			res = !MaleficentTutorialsManager.Instance.tutorialFinished;
		}
*/
		return res;
	}
	void Update()
	{
		UpdateCountLabels ();
		UpdateBGSprite ();
	}
	void Awake()
	{
		if (usages == null) {
			usages = new Dictionary<System.Type,int>();
		}
 
		itemCancelIcon.enabled = false;
		
		SetItem(itemPrefab);
		ManaItem.OnActuallyUsingAnyItem += HandleOnActuallyUsingAnyItem;
		TokensSystem.OnManaModified += HandleOnManaModified;
	
	}
 
	int PropertyIndex()
	{
		int nindex = -1;
		switch(itemPrefabManaItem.m_strPropertyName)
		{
		case "Crow_Prop_Count":
			nindex=1;
			break;
		case "Crow2nd_Prop_Count":
			nindex=6;
			break;
			
		case "TheStaffCost_Prop_Count":
			nindex=5;
			break;
			
		case "GreenMagicCost_Prop_Count":
			nindex=3;
			break;
			
		case "WingWindCost_Prop_Count":
			nindex=4;
			break;
			
		case "YellowPixieDustCost_Prop_Count":
			nindex=2;
			break;
			
		case "WolfHowlCost_Prop_Count":
			nindex=8;
			break;
			
		case "ThorwnCost_Prop_Count":
			nindex=7;
			break;
			
		}		 
		return nindex;
	}
	 
	void Start()
	{	
		boardLogic.loseConditions.OnLoseChecked += CancelItemOnLose; //to deactivate any selected items if the game is lost
	}

	void CancelItemOnLose()
	{
		if (usingItem && item != null && !item.IsRunning) {
			ActionOnClick();
		}
	}
	
	public void SetNoItem()
	{
		transform.parent.parent.gameObject.SetActive(false);
	}
	
	public void SetItem(GameObject prefab)
	{
		if (prefab == null) {
			transform.parent.parent.gameObject.SetActive(false);
			itemPrefabManaItem = null;
			return;
		}else {
			transform.parent.parent.gameObject.SetActive(true);
		}
		
		itemPrefab = prefab;
		itemPrefabManaItem = itemPrefab.GetComponent<ManaItem>();
		/*
		if (!itemPrefabManaItem.IsAvailable()) {
			manaCostLabel.text = "";
			transform.parent.gameObject.SetActive(false);
			return;
		}
		*/
		if (itemBg == null) {
			itemBg = GetComponent<UISprite>();
		}

		UpdateCountLabels();
		UpdateBGSprite();
		UpdateItemPosition();
		UpdateIcon();
	}

	public bool HasEnoughManaToUseItem () 
	{
		bool res = true;
		if(itemPrefabManaItem != null) {
			res =  itemPrefabManaItem.EnoughManaToInvoke();
			if(UseManaInTutrial()&&m_bAdd==false)
				res=true;

		}
		return res;
	}

	bool UseManaInTutrial()
	{
		bool bret = false;
		switch(itemPrefabManaItem.m_strPropertyName)
		{
		case "Crow_Prop_Count":
			if(MaleficentBlackboard.Instance.level==6)
				bret=true;
			break;
		case "Crow2nd_Prop_Count":
			if(MaleficentBlackboard.Instance.level==47)
				bret=true;
			break;

		case "TheStaffCost_Prop_Count":
			if(MaleficentBlackboard.Instance.level==32)
				bret=true;
			break;

		case "GreenMagicCost_Prop_Count":
			if(MaleficentBlackboard.Instance.level==17)
				bret=true;
			break;

		case "WingWindCost_Prop_Count":
			if(MaleficentBlackboard.Instance.level==8)
				bret=true;
			break;

		case "YellowPixieDustCost_Prop_Count":
			if(MaleficentBlackboard.Instance.level==7)
				bret=true;
			break;

		case "WolfHowlCost_Prop_Count":
			if(MaleficentBlackboard.Instance.level==92)
				bret=true;
			break;

		case "ThorwnCost_Prop_Count":
			if(MaleficentBlackboard.Instance.level==62)
				bret=true;
			break;

		}		 
		return bret;
	}
	void UpdateCountLabels()
	{
		manaCostLabel.text = "";
		/* 
		if(CostItemManager.getDict()!=null&&itemPrefab.GetComponent<ManaItem>()!=null)
		{
			manaCostLabel.text = CostItemManager.getDict()[itemPrefab.GetComponent<ManaItem>().m_strPropertyName].ToString();
		}
		else
		{
			manaCostLabel.text = "";
		}
		*/
	}

	void UpdateBGSprite ()
	{
		if(itemPrefabManaItem != null) 
		{
			string sprite = spriteOn;
			Color color = colorCostLabelOn;

			//if(!ManaItemHolder.FreeGiveAwayOfItem(itemPrefabManaItem) && 
			  if(!HasEnoughManaToUseItem()) 
			   {
					sprite = spriteOff;
					color = colorCostLabelOff;
				manaCostLabel.enabled=false;
				if(m_AddSignIcon!=null)
					m_AddSignIcon.alpha=255;
			}
			else
			{
				manaCostLabel.enabled=true;
				if(m_AddSignIcon!=null)
					m_AddSignIcon.alpha=0;
			}
	 

			itemBg.spriteName = sprite;
			manaCostLabel.color = color;

		}
	}

	bool CanBeCancelled () {
		return !TutorialInProgress() && item.canBeCancelled;
	}
	
	public void UpdateItemPosition()
	{
//		itemPosition = null;
		
//		if (itemPrefab.GetComponent<Snowball>() != null || itemPrefab.GetComponent<Hourglass>() != null) {
//			itemPosition = snowballPosition;
//		}
//		
//		if (itemPrefab.GetComponent<TrollMagic>() != null) {
//			itemPosition = transform;
//		}
//		
//		if (itemPrefab.GetComponent<Carrot>() != null) {
//			itemPosition = shakeContainer;
//		}
	}

	public string IconName () 
	{
		return itemPrefab.GetComponent<BasicItem>().iconName;
	}
	
	public void UpdateIcon()
	{
		itemIcon.spriteName = IconName();
	}
	
	public void OnClick()
	{
		if (boardLogic.IsGameOver || (manaPowerBall == null && ManaPowerBallController.isOpen) || ignoreInputs)
		{
			return;
		}

		if (SelectionEvent != null) {
			SelectionEvent(this);
		}
		
		if (OnItemClick != null) {
			OnItemClick();
		}
		
		ActionOnClick();
	}
	
	public void ActionOnClick()
	{
		if (!usingItem && itemPrefab != null && (HasEnoughManaToUseItem()))// || ManaItemHolder.FreeGiveAwayOfItem(itemPrefabManaItem)))
		{
			item = (GameObject.Instantiate(itemPrefab) as GameObject).GetComponent<ManaItem>();
			
			if (!item.CanBeUsed())
			{
				Destroy(item.gameObject);
				return;
			}
			
			if (itemPosition) {
				item.effectPosition = itemPosition;
			}
			twinItem.item = item;

			waitingForItemInvoke = false;
			usingItem = true;

			itemBg.spriteName = spriteSelected;
			twinItem.itemBg.spriteName = spriteSelected;

			if(item.canBeCancelled) {

				if(!TutorialInProgress()) {
					itemCancelIcon.enabled = true;
					twinItem.itemCancelIcon.enabled = true;
				}

				transform.parent.GetComponent<Animation>().Play("Gui_Powerup_Use");
				twinItem.transform.parent.GetComponent<Animation>().Play("Gui_Powerup_Use");
			}
			
			item.OnFinishUsingItem += FinishUsingItem;
			item.OnActuallyUsingItem += ActuallyUsingItem;
			item.OnCancelFinish = OnItemCancelFinish;
			item.StartUsingItem(boardLogic);

			//self
			//CostItemTipsPanel.getInstance().PopWindow(itemPrefabManaItem.m_strPropertyName);


			SoundManager.Instance.Play("item_select_sfx");
		}
		else if (!usingItem && !HasEnoughManaToUseItem() )//&& !ManaItemHolder.FreeGiveAwayOfItem(itemPrefabManaItem)) 
		{
			/* 
			buyFSM.SendEvent("Buy");

			ShopUINotifyer.getInstance().IndexUI=2	;
			ShopUINotifyer.getInstance().Notify();
			ShopPropertyPanel.getInstance().OnBuy(PropertyIndex());
			SoundManager.Instance.Play("item_select_sfx");
			*/
		}
		else if (usingItem && item != null && CanBeCancelled() && !waitingForItemInvoke)
		{
			item.OnFinishUsingItem -= FinishUsingItem;
			item.OnActuallyUsingItem -= ActuallyUsingItem;
			item.OnCancelFinish = null;

			item.CancelUsingItem();
			StopSelectAnimation();
			FinishUsingItem(item);
			
			SoundManager.Instance.Play("item_unselect_sfx");

			if(OnCancelUsingItem != null) {
				OnCancelUsingItem();
			}
			//CostItemTipsPanel.getInstance ().HideWindow ();
		}
	}

	public void AddItems(int count)
	{
//		UpdateCountLabels();
//		UpdateSprite();
//		twinItem.UpdateCountLabels();
	}

	void StopSelectAnimation()
	{
		if(item.canBeCancelled) {
			transform.parent.GetComponent<Animation>().Stop();
			twinItem.transform.parent.GetComponent<Animation>().Stop();

			transform.parent.localScale = Vector3.one;
			twinItem.transform.parent.localScale = Vector3.one;
		}
	}

	void OnItemCancelFinish()
	{

		waitingForItemInvoke = true;	
		if(CanBeCancelled()) {
			itemCancelIcon.enabled = false;
			twinItem.itemCancelIcon.enabled = false;
		}
	}
	
	void ActuallyUsingItem(BasicItem _item)
	{
//		Debug.Log(item.ItemName);
//		Recorder.RecoManaEvent(item.ItemName,item.ManaPointsCost);
//		using (AndroidJavaClass jc=new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity")) {
//			using(AndroidJavaObject jo=jc.GetStatic<AndroidJavaObject>("appdata"))
//			{
//				if(jo!=null)
//				{
//					jo.Call("use",item.ItemName,1,item.ManaPointsCost);
//					jo.Call("UseSum");
//				}
//			}
//		
//		}
		if(UseManaInTutrial()&&m_bAdd==false)
		{
			//CostItemManager.mInstance.AddCount(item.PropertyPointCost,item.m_strPropertyName);
			m_bAdd=true;
		}
		else
		{
			//CostItemManager.mInstance.SubstractCount(item.PropertyPointCost,item.m_strPropertyName);
		}

		//CostItemTipsPanel.getInstance ().HideWindow ();
		StopSelectAnimation();

		ManaItem manaItem = _item as ManaItem;
//		int cost = manaItem.ManaPointsCost;
//
//		if (! ManaItemHolder.FreeGiveAwayOfItem(manaItem)) {
//			TokensSystem.Instance.SubstractMana(cost);
//		}

		//AnalyticsBinding.LogEventGameAction(Match3BoardGameLogic.Instance.GetLevelType(), "use_powerup", _item.ItemName, 
		//                                    manaItem.ManaPointsCost.ToString(), MaleficentBlackboard.Instance.level);

		//AnalyticsBinding.LogInAppCurrencyAction("magic", -cost, null, 0, null, "use", TokensSystem.Instance.ManaPoints, manaItem.ItemName, null, MaleficentBlackboard.Instance.level);

		int accumUsages = 0;
		if(usages.ContainsKey(_item.GetType())) {
			accumUsages = usages[_item.GetType()];
		}

		accumUsages++;
		usages[_item.GetType()] = accumUsages;

		itemCancelIcon.enabled = false;
		twinItem.itemCancelIcon.enabled = false;
		if (manaPowerBall == null) {
			this.UpdateCountLabels();
			twinItem.UpdateCountLabels();
		}

		item = null; //so we can't cancel it anymore

		if (EndSelectionEvent != null) {
			EndSelectionEvent(this);
		}

		if(OnFinishUsingItem != null) {
			OnFinishUsingItem();
		}
	}

	void FinishUsingItem(BasicItem _item) 
	{
		usingItem = false;
		itemCancelIcon.enabled = false;
		twinItem.itemCancelIcon.enabled = false;

		UpdateBGSprite();
		twinItem.UpdateBGSprite();

		item = null;
		twinItem.item = null;
	}

	void HandleOnActuallyUsingAnyItem (BasicItem item)
	{	
		UpdateBGSprite();
	}

	void HandleOnManaModified (int amount)
	{
		UpdateBGSprite();
	}

	public static int GetNumUsedPowerUps()
	{
		int total = 0;
		foreach (KeyValuePair<System.Type, int> pair in usages)
			total += pair.Value;
		return total;
	}

	#region ISelectableItem
	
	public string SelectableItemName ()
	{
		return selectableItemName;
 	}
	
	public void EnableItem(bool enable)
	{
		ignoreInputs = !enable;
	}
	
	#endregion

	void OnDestroy()
	{
		ManaItem.OnActuallyUsingAnyItem -= HandleOnActuallyUsingAnyItem;
		TokensSystem.OnManaModified -= HandleOnManaModified;
		usingItem = false;
		usages.Clear();
	}

}
