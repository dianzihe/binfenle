using UnityEngine;
using System.Collections;

public class ManaItem : BasicItem {

	public System.Action OnCancelFinish;

	public int defaultManaPointsCost;
	public int unlockLevel;
	public string unlockTextKey;
	public bool canBeCancelled = true;
	public float useItemDelay = 1f;
	public string tweaksSystemCostId;
	public string animState = "Use Power Up";
	//self
	public string m_strPropertyCostID;
	public int m_ndefaultPropertyCost;
	static private int m_nPropertyCount;
	public string m_strPropertyName;
    /*
	public int PropertyCount
	{
        
		get
		{
			int count=0;
			if(CostItemManager.getDict()!=null)
			{
				if(CostItemManager.getDict().ContainsKey(m_strPropertyName))
				{
					count=CostItemManager.getDict()[m_strPropertyName];
				}
			}
			return count;
		}   
    }
 */



    public bool EnoughManaToInvoke ()
	{
		//return ManaPointsCost <= TokensSystem.Instance.ManaPoints;
		//return PropertyPointCost <= PropertyCount;
        return true;
	}


	//self 
	public int PropertyPointCost
	{
		get 
		{
			int cost = m_ndefaultPropertyCost;
			if (TweaksSystem.Instance.intValues.ContainsKey(m_strPropertyCostID)) 
			{
				cost = TweaksSystem.Instance.intValues[m_strPropertyCostID];
			}
			
			return cost;
		}
	}



	public int ManaPointsCost
	{
		get {
			int cost = defaultManaPointsCost;
			if (TweaksSystem.Instance.intValues.ContainsKey(tweaksSystemCostId)) {
				cost = TweaksSystem.Instance.intValues[tweaksSystemCostId];
			}
			
			return cost;
		}
	}

	public bool IsAvailable () 
	{
//		bool available = LoadLevelButton.lastUnlockedLevel >= unlockLevel;
		return true;
//
//#if UNITY_EDITOR
//		available = available || MaleficentBlackboard.Instance.unlockAllItems;
//#endif
		//return available;
	}

	public bool JustBeenUnlocked()
	{
	//	return MaleficentBlackboard.Instance.level == unlockLevel;
		return true;
	}

	public bool WillBeUnlocked() 
	{
		int showMessageLevel = unlockLevel - 1;
        bool willBeUnlocked = MaleficentBlackboard.Instance.level == showMessageLevel;
            //&& LoadLevelButton.lastUnlockedLevel <= showMessageLevel;
		return willBeUnlocked;
	}

	public override void StartItemEffects()
	{
        /*
		//Start animation
		PlayMakerFSM characterFSM = GameObject.FindObjectOfType< CharacterSpecialAnimations >().GetComponent< PlayMakerFSM >();
		characterFSM.SendEvent("PauseAnimation");
		characterFSM.SendEvent(animState);
        */
		if(OnCancelFinish != null) {
			OnCancelFinish();
		}
		/*
		//Wait a few seconds before using the item
		MaleficentTools.DoAfterSeconds(this, useItemDelay, () => {
			base.StartItemEffects();
		});
        */
	}
}
