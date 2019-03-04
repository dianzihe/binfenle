using UnityEngine;
using System.Collections;

//OBSOLETE CLASS
public class OfferPriceLabel : MonoBehaviour
{
	public UISprite icon;
	public UILabel packLabel;
	public UILabel itemsLabel;
	public UILabel movesLabel;
	public UILabel priceLabel;
	
	public int packIndex = 0;
	
	public ItemHolder item;
	public Match3BoardGameLogic boardLogic;
	
	protected int itemsCount;
	
	void Start () 
	{
		packLabel.gameObject.SetActive(false);
		
		if (packIndex == 0) {
			//packLabel.text = Language.Get("BASIC_PACK");
			priceLabel.text = Language.Get("PRICE_BASIC_ITEM_PACK").Replace("\\", "");
			//priceLabel.text = "Basic Pack - 0.99$";
		}
		else if (packIndex == 1) {
			//packLabel.text = Language.Get("GOOD_PACK");
			priceLabel.text = Language.Get("PRICE_GOOD_ITEM_PACK").Replace("\\", "");
			//priceLabel.text = "Good Deal - 1.99$";
		}
		else if (packIndex == 2) {
			//packLabel.text = Language.Get("BEST_PACK");
			priceLabel.text = Language.Get("PRICE_BEST_ITEM_PACK").Replace("\\", "");
			//priceLabel.text = "Best Deal - 2.99$";
		}
		
		BasicItem itemComponent = item.itemPrefab.GetComponent<BasicItem>();
		itemsCount = TweaksSystem.Instance.intValues["EndItemsPack" + packIndex];
		
		itemsLabel.text =  itemsCount.ToString();// + " " + ((itemsCount == 1) ? itemComponent.NameSingular : itemComponent.NamePlural);
		icon.spriteName = itemComponent.iconName;
		
//		itemsCount = TweaksSystem.Instance.intValues["EndIcePickPack" + packIndex];
//		itemsLabel.text = "+" + itemsCount + " " + ((itemsCount == 1) ? "Ice Pick" : "Ice Picks");

		movesLabel.text = "+" + boardLogic.loseConditions.GetOffer(packIndex) + " " + boardLogic.loseConditions.GetStringUnit();
	}
	
	void OnClick()
	{
		item.AddItems(itemsCount);
		boardLogic.AcceptOfferOnLose(packIndex);
	}
}

