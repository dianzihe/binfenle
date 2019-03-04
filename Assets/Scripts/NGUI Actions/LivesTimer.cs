using UnityEngine;
using System.Collections;

public class LivesTimer : MonoBehaviour 
{
	public UISprite livesTexture;
	protected UILabel myLabel;
	
	void Awake() 
	{
		myLabel = GetComponent<UILabel>();
		LivesSystem.OnLivesUpdate += UpdateLives;
	}
	
	void UpdateLives()
	{
		myLabel.enabled = (LivesSystem.lives < LivesSystem.maxLives);

		StopAllCoroutines();
		
		if (LivesSystem.lives < LivesSystem.maxLives) {
			StartCoroutine(UpdateTimer());
		}
	}
	
	IEnumerator UpdateTimer()
	{
		WaitForSeconds waiter = new WaitForSeconds(0.05f);
		
		while (LivesSystem.lives < LivesSystem.maxLives) {
			myLabel.text = LivesSystem.GetTimerString();
			yield return waiter;
		}
	}
	
	void OnDestroy()
	{
		LivesSystem.OnLivesUpdate -= UpdateLives;
	}
}
