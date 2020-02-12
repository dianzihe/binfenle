using UnityEngine;
using System.Collections;

public class LoseLabel : MonoBehaviour
{
	public Match3BoardGameLogic gameLogic;
	public UILabel questionLabel;
	public UILabel itemsLabel;
	public UISprite itemsBg;
	public UISprite itemsIcon;
	public ManaItemHolder movesTimeItem;
	
	public PlayMakerFSM fsm;
	public PlayMakerFSM m_BuyFSM=null;
	public string acceptEvent = "AcceptOffer";
	public string acceptPovertyEvent = "AcceptOfferAsAPoorMF";
	
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		//label.text = gameLogic.loseConditions.GetLoseString();
		label.text = "dqdqdq";

		if (questionLabel != null) {
			if (gameLogic.loseConditions is LoseMoves)
			{
				ManaItem manaItem = movesTimeItem.itemPrefabManaItem;
				itemsIcon.spriteName = manaItem.iconName;
				itemsLabel.text = manaItem.ManaPointsCost.ToString();
			}
			else 
			{
				Debug.LogWarning("Unknow lose condition");
			}
		}
	}
	
	public void ShowOffer()
	{
		UpdateItem();
	}
	
	public void UpdateItem()
	{
		itemsBg.spriteName = movesTimeItem.HasEnoughManaToUseItem()?movesTimeItem.spriteOn:movesTimeItem.spriteOff;
	}
	
	public void AcceptOffer()
	{
		if (movesTimeItem.HasEnoughManaToUseItem())
		{
			movesTimeItem.ActionOnClick();
			//movesTimeItem.AddItems(-1);
			UpdateItem();
			
//			if (gameLogic.loseConditions is LoseMoves) {
//				(gameLogic.loseConditions as LoseMoves).RemainingMoves = movesTimeItem.itemPrefab.GetComponent<Snowball>().extraMoves;
//			}
//			else {
//				(gameLogic.loseConditions as LoseTimer).RemainingTime = movesTimeItem.itemPrefab.GetComponent<Hourglass>().extraTime;
//			}
			
			gameLogic.IsGameOver = false;
			gameLogic.TryCheckStableBoard();
			
			fsm.SendEvent(acceptEvent);
		}
		else 
		{
			if(m_BuyFSM!=null)
			{
				if(m_BuyFSM.ActiveStateName=="Out")
				{
					fsm.SendEvent(acceptPovertyEvent);
				}
				else if(m_BuyFSM.ActiveStateName=="Test End Buy")
				{
					m_BuyFSM.SendEvent("ChargeNotEnough");
				}

			}

		}
	}
}

