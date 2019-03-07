using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrollMagic : BasicItem
{
	public BasicItem[] eligibleItems;
	public int[] unlockLevels;
	public BasicItem theChosenOne;
	
	protected List<BasicItem> finalEligibleItems;
	
	public override string ItemName {
		get {
			return "TrollMagic";
		}
	}
	
	protected override void Awake ()
	{
		base.Awake();
		
		finalEligibleItems = new List<BasicItem>();
		/* 
		for (int i = 0; i < eligibleItems.Length; ++i) {
			if ((LoadLevelButton.lastUnlockedLevel >= unlockLevels[i] || MaleficentBlackboard.Instance.level >= unlockLevels[i]) && 
				(eligibleItems[i].GetType() != typeof(Snowball) || (Match3BoardRenderer.Instance.loseConditions is LoseMoves)) &&
				(eligibleItems[i].GetType() != typeof(Hourglass) || (Match3BoardRenderer.Instance.loseConditions is LoseTimer))) 
			{
				finalEligibleItems.Add(eligibleItems[i]);
//				Debug.Log("Eligible item " + (i + 1) + " : " + eligibleItems[i].ItemName);
			}
//			else {
//				Debug.Log("Not Eligible item " + (i + 1) + " : " + eligibleItems[i].ItemName + " level: " + LoadLevelButton.lastUnlockedLevel + " mylevel: " + unlockLevels[i]);
//			}
		}
		*/
	}

	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		
		SelectRandomItem();
	}
	
	protected void SelectRandomItem()
	{
		SoundManager.Instance.PlayOneShot("troll_magic_sfx");
		
		ActuallyUsingItem();
		
		Debug.Log("Choosing the one");
		theChosenOne = finalEligibleItems[Random.Range(0, finalEligibleItems.Count)];
		
		Debug.Log("The one: " + theChosenOne.name);
		
		SpawnEffect(effectPosition.position, effectPrefab);
		
		Debug.Log("spawned effect");
		
		Invoke("DoItem", destroyEffect.destroyTileTime);
		Invoke("DoDestroy", destroyEffect.lifeTime);
		
		Debug.Log("invoked the spirits");
	}	
}

