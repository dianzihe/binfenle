using UnityEngine;
using System.Collections;

public class ManaPowerUpSelector : MonoBehaviour, ISelectableItem 
{
	public UISprite m_AddSignIcon=null;
	bool m_bAdd=false;
	public static event System.Action<ManaPowerUpSelector> OnPowerUpSelected;
	public event System.Action<ISelectableItem> SelectionEvent;
	public event System.Action<ISelectableItem> EndSelectionEvent;

	public UIImageButton button;
	public UILabel manaCostLabel;
	public UISprite itemIcon;
	public string notAvailableSprite;
	//public PlayMakerFSM buyFSM;
	public Color enoughManaLabelColor;
	public Color notEnoughLabelColor;
	public string selectableItemName = "";

	private ManaItem item;
	private string enoughManaBgSprite;
	private string notEnoughManaBgSprite;
	private bool ignoreInputs = false;

	public ManaItem Item {
		set {
			item = value;
			if (item != null) {
				itemIcon.enabled = true;
				itemIcon.spriteName = item.iconName;
				button.isEnabled = true;
				manaCostLabel.enabled = true;
				UpdateCostLabel();
				CheckAvailableMana();
			}else {
				itemIcon.enabled = false;
				button.disabledSprite = notAvailableSprite;
				button.isEnabled = false;
				manaCostLabel.enabled = false;
				manaCostLabel.text = "";
			}
		}
		get {
			return item;
		}
	}
	
	void Awake ()
	{
		ManaItem.OnActuallyUsingAnyItem += HandleOnActuallyUsingAnyItem;
		TokensSystem.OnManaModified += HandleOnManaModified;

		enoughManaBgSprite = button.normalSprite;
		notEnoughManaBgSprite = button.disabledSprite;


		Item = null;
	}
	
	void OnDestroy ()
	{
		ManaItem.OnActuallyUsingAnyItem -= HandleOnActuallyUsingAnyItem;
		TokensSystem.OnManaModified -= HandleOnManaModified;
	}
	int PropertyIndex()
	{
		int nindex = -1;
		switch(item.m_strPropertyName)
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
	void CheckAvailableMana()
	{
		if(item != null) {
			string spriteName = enoughManaBgSprite;
			Color tintColor = enoughManaLabelColor;

//			!ManaItemHolder.FreeGiveAwayOfItem(item) && 
			if(!item.EnoughManaToInvoke())
			{
				spriteName = notEnoughManaBgSprite;
				tintColor = notEnoughLabelColor;
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

 

			//button.UpdateNormalSprite(spriteName);
			//button.UpdateHoverSprite(spriteName);
			manaCostLabel.color = tintColor;
		}
	}

	void UpdateCostLabel()
	{
		manaCostLabel.text = "";
		/* 
		if(CostItemManager.getDict()!=null&&item!=null)
		{
			manaCostLabel.text = CostItemManager.getDict()[item.m_strPropertyName].ToString();
		}
		else
		{
			manaCostLabel.text = "";
		}
		*/
	}



	bool UseManaInTutrial()
	{
		bool bret = false;
		switch(item.m_strPropertyName)
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

	void OnClick ()
	{
		if(ignoreInputs) {
			return;
		}

		if(SelectionEvent != null) {
			SelectionEvent(this);
		}

		if(item.EnoughManaToInvoke()||(UseManaInTutrial()&&m_bAdd==false))
		/*|| ManaItemHolder.FreeGiveAwayOfItem(item))*/
		{
			if(OnPowerUpSelected != null) {
				OnPowerUpSelected(this);
			}
		}else {
			/* 
			buyFSM.SendEvent("Buy");

			ShopUINotifyer.getInstance().IndexUI=2;
			ShopUINotifyer.getInstance().Notify();
			ShopPropertyPanel.getInstance().OnBuy(PropertyIndex());
			*/
			SoundManager.Instance.Play("item_select_sfx");
		}
	}

	#region EventHandlers

	void HandleOnActuallyUsingAnyItem (BasicItem item)
	{
		CheckAvailableMana();
		UpdateCostLabel();
		if(UseManaInTutrial()&&m_bAdd==false)
		{
			//CostItemManager.mInstance.AddCount(item.PropertyPointCost,item.m_strPropertyName);
			m_bAdd=true;
		}
		 
		if (EndSelectionEvent != null) {
			EndSelectionEvent(this);
		}
	}
	void Update()
	{
		UpdateCostLabel ();
		CheckAvailableMana ();
	}
	void HandleOnManaModified (int amount)
	{
		CheckAvailableMana();
	}

	#endregion

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
}
