using UnityEngine;
using System.Collections;

public class TimeMovesLabel : MonoBehaviour 
{
	public Match3BoardGameLogic gameLogic;
	public UILabel secondsLabel;
	UILabel label;
	public int index;
	
	protected int actualValue = 0;
	protected int currentValue = 0;
	protected bool loseIsMoves = false;
	protected LoseMoves loseMoves;
	protected LoseTimer loseTimer;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		
		loseIsMoves = (gameLogic.loseConditions is LoseMoves);
		loseMoves = (gameLogic.loseConditions as LoseMoves);
		loseTimer = (gameLogic.loseConditions as LoseTimer);
		
		if (index == 1) {
			secondsLabel.gameObject.SetActive(gameLogic.loseConditions is LoseTimer);
		}
		UpdateText();
		
		if (index == 1) {
			label.text = ScoreSystem.FormatScore(currentValue);
			StartCoroutine(ValueUpdater());
		}
	}
	
	void UpdateText()
	{
		if (index == 0) {
			label.text = gameLogic.loseConditions.GetString();
		}
		else if (index == 1) {
			if (loseIsMoves) {
				actualValue = loseMoves.RemainingMoves;
			} 
			else {
//				actualValue = Mathf.CeilToInt(loseTimer.RemainingTime);
//				Debug.Log(loseTimer.RemainingTime);
			}
//			label.text = gameLogic.loseConditions.GetStringValue();
		}
	}

	void Update () 
	{
		UpdateText();
	}
	
	IEnumerator ValueUpdater()
	{
		while (true) 
		{
			if (currentValue < actualValue) 
			{
				if (actualValue - currentValue > 130) {
					currentValue += 100;
				}
				else if (actualValue - currentValue > 13) {
					currentValue += 10;
				}
				else {
					currentValue += actualValue > currentValue ? 1 : -1;
				}

				currentValue = Mathf.Max(0, currentValue);
				label.text = currentValue.ToString() + (loseIsMoves ? "" : " ");
				
				yield return new WaitForSeconds(0.08f);
			}
			else 
			{
				currentValue = actualValue;
				currentValue = Mathf.Max(0, currentValue);
				label.text = currentValue.ToString() + (loseIsMoves ? "" : " ");
			
				yield return null;
			}
		}
	}
}
