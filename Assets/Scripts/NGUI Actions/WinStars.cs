using UnityEngine;
using System.Collections;

public class WinStars : MonoBehaviour 
{
	public Match3BoardGameLogic boardLogic;
	
	public GameObject star1;
	public GameObject star2;
	public GameObject star3;
	public GameObject manaLabel;
	public GameObject manaAmountLabel;
	//public GameObject character;

	int starsCount = 0;
	
	protected WinScore winCondition;
	
	void Start () 
	{
		//winCondition = boardLogic.winConditions as WinScore;

		UpdateStars(0);
		manaLabel.SetActive(false);
		manaAmountLabel.SetActive(false);
		WinScore.OnNewStarReached += UpdateStars;
	}
	
	void UpdateStars(int count) 
	{
		starsCount = count;
	}
	
	public void AnimateStars()
	{
		StartCoroutine(CoolStarsAnimation());
	}
	
	IEnumerator CoolStarsAnimation()
	{
		GameObject[] stars = new GameObject[] { star1, star2, star3 };
		
		yield return new WaitForSeconds(0.2f);
		
		for (int i = 0; i < stars.Length; ++i) {
			if (starsCount >= i + 1) {
				stars[i].SetActive(true);
				stars[i].GetComponent<Animation>().Play();
				
				yield return new WaitForSeconds(0.1f);
				SoundManager.Instance.PlayOneShot("star" + (i + 1) + "_sfx");
				
				if (i < stars.Length - 1) {
					yield return new WaitForSeconds(0.4f);
				}
			}
		}
		yield return new WaitForSeconds(0.5f);
		manaLabel.SetActive(true);
		//SoundManager.Instance.PlayOneShot("star3_sfx");
		yield return new WaitForSeconds(0.4f);
		SoundManager.Instance.PlayOneShot("mana_earn_sfx");
		manaAmountLabel.SetActive(true);
	}
	
	void OnDestroy()
	{
		WinScore.OnNewStarReached -= UpdateStars;
	}
}
