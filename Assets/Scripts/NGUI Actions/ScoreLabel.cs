using UnityEngine;
using System.Collections;

public class ScoreLabel : MonoBehaviour
{
	public string separator = "\n";
	UILabel label;
	public int index = 0;
	
	protected int actualScore = 0;
	protected int currentScore = 0;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		UpdateScore();
		
		if (index != 0) {
			ScoreSystem.Instance.OnScoreUpdated += UpdateScore;
		}
		
		if (index == 1) {
			label.text = ScoreSystem.FormatScore(currentScore);
			StartCoroutine(ScoreUpdater());
		}
	}
	
	void UpdateScore () 
	{
		if (index == 0) {
			label.text = Language.Get("GAME_SCORE").Replace("<SPACE>"," ").TrimEnd();
			//label.text = "Score";
		} 
		else if (index == 1) {
			actualScore = ScoreSystem.Instance.Score;
			//label.text = ScoreSystem.Instance.GetScoreString();
		}
		else if (index == 10) {
			label.text = Language.Get("GAME_SCORE").Replace("<SPACE>"," ") + separator + ScoreSystem.Instance.GetScoreString();
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
				
				label.text = ScoreSystem.FormatScore(currentScore);
				
				yield return new WaitForSeconds(0.08f);
			}
			else {
				yield return null;
			}
		}
	}
}

