using UnityEngine;
using System.Collections;

public class LoseReasonLabel : MonoBehaviour
{
	public Match3BoardGameLogic gameLogic;
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		//gameLogic.loseConditions.OnLoseChecked += UpdateReason;
	}
	
	public void UpdateReason()
	{
		//label.text = gameLogic.winConditions.GetLoseReason();
	}
}

