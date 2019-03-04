using UnityEngine;
using System.Collections;

public class ItemHolder : MonoBehaviour
{
	public static bool usingItem = false;
	
	public event System.Action OnItemClick;
	
	public Match3BoardGameLogic boardLogic;
	public GameObject itemPrefab;
	public GameObject timeLevelItemPrefab;
	public int itemCount = 1;
	public Transform itemPosition;
	public Transform snowballPosition;
	public Transform shakeContainer;
	public UILabel countLabel;
	public ItemHolder twinItem;
	public UISprite itemBg;
	public string spriteOn;
	public string spriteOff;
	public string spriteSelected;
	public UISprite itemIcon;
	public UISprite itemCancelIcon;
	
	public PlayMakerFSM buyFSM;
	
	protected string idleAnimName = null;
	protected Vector3 initScale;
	
	BasicItem item;
	bool temporaryItem = false;
	GameObject oldPrefab;
	int oldCount;
	bool canceling = false;
	
	void Awake()
	{
		initScale = transform.parent.localScale;
		
		if (transform.parent.GetComponent<Animation>().clip != null) {
			idleAnimName = transform.parent.GetComponent<Animation>().clip.name;
		}
		
		itemCancelIcon.enabled = false;
		
		SetItem(itemPrefab);
	}
	
	void Start()
	{
		if (timeLevelItemPrefab != null && Match3BoardRenderer.Instance.loseConditions is LoseTimer) {
			SetItem(timeLevelItemPrefab);
		}
		
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
			return;
		}
			
		itemPrefab = prefab;
		
		if (itemBg == null) {
			itemBg = GetComponent<UISprite>();
		}
		
		if (itemPrefab.GetComponent<Snowball>() != null)
		{
			if (MaleficentBlackboard.Instance.level > 0 && MaleficentBlackboard.Instance.level < 7 && LoadLevelButton.lastUnlockedLevel <= 7)
			{
				Debug.LogWarning("NO SNOWBALL: " + LoadLevelButton.lastUnlockedLevel);
				countLabel.text = ""; // HACK BECAUSE OF STUPID RUSSIAN
				transform.parent.gameObject.SetActive(false);
				return;
			}
			
			itemCount = TokensSystem.Instance.snowballs;
		}
		else if (itemPrefab.GetComponent<Hourglass>() != null) 
		{
			itemCount = TokensSystem.Instance.hourglasses;
		}
		else if (itemPrefab.GetComponent<IcePick>() != null) 
		{
			if (MaleficentBlackboard.Instance.level > 0 && MaleficentBlackboard.Instance.level < 6 && LoadLevelButton.lastUnlockedLevel <= 6)
			{
				Debug.LogWarning("NO ICE PICKS: " + LoadLevelButton.lastUnlockedLevel);
				countLabel.text = ""; // HACK BECAUSE OF STUPID RUSSIAN
				transform.parent.gameObject.SetActive(false);
				return;
			}
			
			itemCount = TokensSystem.Instance.icePicks;
		}
		else {
			itemCount = TokensSystem.Instance.itemTokens;
		}
		
		UpdateCountLabels();
		UpdateItemPosition();
		UpdateIcon();
	}
	
	void UpdateCountLabels()
	{
		if (itemCount > 0) {
			countLabel.text = itemCount.ToString();
			countLabel.transform.localScale = new Vector3(35f, 35f, 1f);
		}
		else {
			countLabel.text = "+";
			countLabel.transform.localScale = new Vector3(42f, 42f, 1f);
		}
		
		itemBg.spriteName = (itemCount > 0 || temporaryItem) ? spriteOn : spriteOff;
	}
	
	public void UpdateItemPosition()
	{
		itemPosition = null;
		
		if (itemPrefab.GetComponent<Snowball>() != null || itemPrefab.GetComponent<Hourglass>() != null) {
			itemPosition = snowballPosition;
		}
		
		if (itemPrefab.GetComponent<TrollMagic>() != null) {
			itemPosition = transform;
		}
		
		if (itemPrefab.GetComponent<Carrot>() != null) {
			itemPosition = shakeContainer;
		}
	}
	
	public void UpdateIcon()
	{
		itemIcon.spriteName = itemPrefab.GetComponent<BasicItem>().iconName;
	}
	
	public void OnClick()
	{
		if (boardLogic.IsGameOver)
		{
			return;
		}
		
		if (OnItemClick != null) {
			OnItemClick();
		}
		
		ActionOnClick();
	}
	
	public void ActionOnClick()
	{
		if (!usingItem && itemPrefab != null && itemCount > 0) 
		{
			item = (GameObject.Instantiate(itemPrefab) as GameObject).GetComponent<BasicItem>();
			
			if (!item.CanBeUsed())
			{
				Destroy(item.gameObject);
				return;
			}
			
			if (itemPosition) {
				item.effectPosition = itemPosition;
			}
			twinItem.item = item;
			
			usingItem = true;
			itemCancelIcon.enabled = true;
			twinItem.itemCancelIcon.enabled = true;
			
			transform.parent.GetComponent<Animation>().Play("Gui_Powerup_Use");
			itemBg.spriteName = spriteSelected;
			twinItem.transform.parent.GetComponent<Animation>().Play("Gui_Powerup_Use");
			twinItem.itemBg.spriteName = spriteSelected;
			
			item.OnFinishUsingItem += FinishUsingItem;
			item.OnActuallyUsingItem += ActuallyUsingItem;
			item.StartUsingItem(boardLogic);
			
			SoundManager.Instance.Play("item_select_sfx");
		}
		else if (!usingItem && itemCount <= 0) 
		{
			if (!temporaryItem) {
				BuyItemHolder.SetSelectedItem(this);
				buyFSM.SendEvent("Buy");
			}
			
			SoundManager.Instance.Play("item_select_sfx");
		}
		else if (usingItem && item != null)
		{
			item.OnFinishUsingItem -= FinishUsingItem;
			item.OnActuallyUsingItem -= ActuallyUsingItem;
			item.CancelUsingItem();
			
			canceling = true;
			FinishUsingItem(item);

			SoundManager.Instance.Play("item_unselect_sfx");
		}
	}
	
	public void AddItems(int count)
	{
		Debug.Log("Add items: " + count);
		itemCount += count;
		twinItem.itemCount += count;
		UpdateCountLabels();
		twinItem.UpdateCountLabels();
		
		if (!temporaryItem) {
			if (itemPrefab.GetComponent<Snowball>() != null){
				TokensSystem.Instance.snowballs = itemCount;
			}
			else if (itemPrefab.GetComponent<Hourglass>() != null) {
				TokensSystem.Instance.hourglasses = itemCount;
			}
			else if (itemPrefab.GetComponent<IcePick>() != null) {
				TokensSystem.Instance.icePicks = itemCount;
			}
			else {
				TokensSystem.Instance.itemTokens = itemCount;
			}
			TokensSystem.Instance.SaveItems();
			Debug.Log("Saved items");
		}
	}
	
	void ActuallyUsingItem(BasicItem _item)
	{
		itemCancelIcon.enabled = false;
		twinItem.itemCancelIcon.enabled = false;
		AddItems(-1);
		
		item = null; //so we can't cancel it anymore
	}
	
	public void SetTemporaryItem(GameObject tempItem)
	{
		temporaryItem = true;
		oldPrefab = itemPrefab;
		oldCount = itemCount;
		
		itemPrefab = tempItem;
		itemCount = 1;
		UpdateItemPosition();
		UpdateIcon();
	}
	
	public void RestoreOriginalItem()
	{
		temporaryItem = false;
		itemPrefab = oldPrefab;
		itemCount = oldCount;
		
		UpdateItemPosition();
		UpdateIcon();
	}
	
	void FinishUsingItem(BasicItem _item) 
	{
		usingItem = false;
		itemCancelIcon.enabled = false;
		twinItem.itemCancelIcon.enabled = false;
	
		if (temporaryItem && !canceling) {
			RestoreOriginalItem();
			twinItem.RestoreOriginalItem();
		}
		
		if ((_item as TrollMagic) != null) {
			SetTemporaryItem((_item as TrollMagic).theChosenOne.gameObject);
			twinItem.SetTemporaryItem(itemPrefab);
		}
		
		transform.parent.localScale = initScale;
		if (idleAnimName != null) {
			transform.parent.GetComponent<Animation>().Play(idleAnimName);
			twinItem.transform.parent.GetComponent<Animation>().Play(idleAnimName);
		}
		else {
			transform.parent.GetComponent<Animation>().Stop();
			twinItem.transform.parent.GetComponent<Animation>().Stop();
		}
		
		UpdateCountLabels();
		twinItem.UpdateCountLabels();
		
		item = null;
		twinItem.item = null;
		
		canceling = false;
	}
	
	void OnDestroy()
	{
		usingItem = false;
	}
}

