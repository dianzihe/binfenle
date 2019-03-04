using UnityEngine;
using System.Collections;

public class ScoreProgressBar : MonoBehaviour
{
	public Match3BoardGameLogic gameLogic;
	
	public float step0 = 0.033f;
	public float step1 = 0.3f;
	public float step2 = 0.6f;
	public float step3 = 0.9f;
	public float step4 = 1.0f;

	UISprite sprite;
	WinScore winCondition;
	
	protected int actualScore = 0;
	protected int currentScore = 0;

	void Start () 
	{
		sprite = GetComponent<UISprite>();
		winCondition = gameLogic.winConditions as WinScore;
		
		UpdateProgressBar();

		ScoreSystem.Instance.OnScoreUpdated += UpdateProgress;
		
		StartCoroutine(ScoreUpdater());
	}
	
	void UpdateProgress()
	{
		actualScore = ScoreSystem.Instance.Score;
	}
	
	void UpdateProgressBar()
	{
		if (ScoreSystem.Instance.Score <= winCondition.targetScore) {
			sprite.fillAmount = Mathf.Lerp(step0, step1, (float)currentScore / (float)winCondition.targetScore);
		} 
		else if (ScoreSystem.Instance.Score <= winCondition.targetScore2Stars) {
			sprite.fillAmount = Mathf.Lerp(step1, step2, 
				(float)(currentScore - winCondition.targetScore) / (float)(winCondition.targetScore2Stars - winCondition.targetScore));
		}
		else if (ScoreSystem.Instance.Score <= winCondition.targetScore3Stars) {
			sprite.fillAmount = Mathf.Lerp(step2, step3, 
				(float)(currentScore - winCondition.targetScore2Stars) / (float)(winCondition.targetScore3Stars - winCondition.targetScore2Stars));
		}
		else {
			sprite.fillAmount = Mathf.Lerp(step3, step4, 
				(float)(currentScore - winCondition.targetScore3Stars) / ((float)winCondition.targetScore3Stars + 1000f));
		}
	}
	
	IEnumerator ScoreUpdater()
	{
		while (true) 
		{
			if (currentScore < actualScore) {
				if (actualScore - currentScore > 1300) {
					currentScore += 1000;
				}
				else if (actualScore - currentScore > 130) {
					currentScore += 100;
				}
				else {
					currentScore += Mathf.Min(10, actualScore - currentScore);
				}
				
				UpdateProgressBar();
				
				yield return new WaitForSeconds(0.08f);
			}
			else {
				yield return null;
			}
		}
	}
}

