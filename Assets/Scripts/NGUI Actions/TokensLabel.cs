using UnityEngine;
using System.Collections;

public class TokensLabel : MonoBehaviour
{
	UILabel myLabel;
	
	void Awake()
	{
		myLabel = gameObject.GetComponent<UILabel>();
		myLabel.text = Language.Get("REMAINING_TOKENS").Replace("<SPACE>", " ") + ": " + TokensSystem.Instance.itemTokens.ToString();
	}
	
	public void UpdateStatus()
	{
		//myLabel.enabled = (MaleficentBlackboard.Instance.level >= 8);
		myLabel.enabled = false;
	}
}

