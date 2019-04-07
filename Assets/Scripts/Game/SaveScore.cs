using UnityEngine;
using System.Collections;

public class SaveScore : MonoBehaviour 
{
	public Match3BoardGameLogic boardLogic;

	public void SyncScores()
	{
		int stars = ScoreSystem.Instance.GetStarsWon();
		int score = ScoreSystem.Instance.Score;

		if (stars > 0) {
			ScoreSystem.Instance.CalculateManaWon(stars);

			LoadLevelButton.newUnlockedLevel = (MaleficentBlackboard.Instance.level >= LoadLevelButton.lastUnlockedLevel);


			int prevStars = UserManagerCloud.Instance.GetStarsForLevel(MaleficentBlackboard.Instance.level);

			string type = "";
			if (stars > prevStars) {
				type = stars.ToString();
			}else {
				type = "repeat";
			}


			UserManagerCloud.Instance.SetScoreForLevel(MaleficentBlackboard.Instance.level, score, stars);

			// mana earned for stars
			int mana = ScoreSystem.Instance.Mana;
			TokensSystem.Instance.AddMana(mana);

			//AnalyticsBinding.LogInAppCurrencyAction("magic", mana, null, 0, null, "earn", TokensSystem.Instance.ManaPoints, type, null, MaleficentBlackboard.Instance.level);

			/*AnalyticsBinding.LogEventGameAction(Match3BoardGameLogic.Instance.GetLevelType(), "earn_mana", ScoreSystem.Instance.Mana.ToString(), 
			                                    "at end of level", MaleficentBlackboard.Instance.level);*/



		}
		else {
			//Debug.LogError("SYNCING");
			UserManagerCloud.Instance.LoadUserFromCloud();
		}
	}
}
