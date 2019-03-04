using UnityEngine;
using System.Collections;

public class TutorialLabel : MonoBehaviour
{
	public GameObject[] items;
	public AudioClip playSoundIn;
	public AudioClip playSoundOut;
	
	protected UILabel myLabel;
	protected GameObject daddy;
	protected ItemHolder designatedItem;
	
	void Start()
	{
		TutorialsSystem tutorialSystem = TutorialsSystem.Instance;
		if (tutorialSystem == null) {
			Debug.LogWarning("No tutorial system");
			return;
		}
		
		myLabel = GetComponent<UILabel>();
		myLabel.text = Language.Get(tutorialSystem.textKey);
		
		if (PlayerPrefs.GetInt(tutorialSystem.textKey, 0) != 0 || 
			MaleficentBlackboard.Instance.level < LoadLevelButton.lastUnlockedLevel) 
		{ 
			//already shown tutorial
			myLabel.text = myLabel.text.Replace("\n<TOKEN>", "");
			myLabel.text = myLabel.text.Replace("<TOKEN>", "");
			myLabel.text = myLabel.text.Replace("\n<FREE_ICE_PICKS>", "");
			myLabel.text = myLabel.text.Replace("<FREE_ICE_PICKS>", "");
			myLabel.text = myLabel.text.Replace("\n<FREE_SNOWBALLS>", "");
			myLabel.text = myLabel.text.Replace("<FREE_SNOWBALLS>", "");
			myLabel.text = myLabel.text.Replace("\n<FREE_HOURGLASSES>", "");
			myLabel.text = myLabel.text.Replace("<FREE_HOURGLASSES>", "");
		}
		else {
			myLabel.text = myLabel.text.Replace("<TOKEN>", Language.Get("TUTORIAL_ITEM_TOKENS"));
			myLabel.text = myLabel.text.Replace("<FREE_ICE_PICKS>", Language.Get("TUTORIAL_FREE_ICE_PICKS"));
			myLabel.text = myLabel.text.Replace("<FREE_SNOWBALLS>", Language.Get("TUTORIAL_FREE_SNOWBALLS"));
			myLabel.text = myLabel.text.Replace("<FREE_HOURGLASSES>", Language.Get("TUTORIAL_FREE_HOURGLASSES"));
		}
		
		daddy = transform.parent.gameObject;
		daddy.transform.parent.localPosition = tutorialSystem.messagePosition;
		
		TutorialsSystem.OnTutorialShow += ShowTutorial;
		
		tutorialSystem.animationTime = daddy.GetComponent<Animation>()["ScaleDown"].length + 0.01f;
		
		if (tutorialSystem.itemIdx >= 0 && tutorialSystem.itemIdx < items.Length) 
		{
			if (tutorialSystem.highlightObjects[0] == null) {
				tutorialSystem.highlightObjects[0] = items[tutorialSystem.itemIdx];
			}
			else {
				tutorialSystem.highlightObjects[1] = items[tutorialSystem.itemIdx];
			}
			
			StartCoroutine(WaitForSetItem());
		}
	}
	
	IEnumerator WaitForSetItem()
	{
		yield return null;
		
		designatedItem = items[TutorialsSystem.Instance.itemIdx].GetComponentsInChildren<ItemHolder>(true)[0];
		TutorialsSystem.Instance.GiveFreeItems(designatedItem);
	}
	
	void ActivateItems(bool activate)
	{
		for (int i = 0; i < items.Length; ++i) {
			if (i != TutorialsSystem.Instance.itemIdx) {
				if (items[i].activeInHierarchy) {
					items[i].GetComponentInChildren<BoxCollider>().enabled = activate;
				}
			}
		}
	}
	
	public void ShowTutorial()
	{
		TutorialsSystem.OnTutorialShow -= ShowTutorial;
		
		if (TutorialsSystem.Instance.justTap) {
			myLabel.text = myLabel.text + "\n" + Language.Get("TUTORIAL_TAP");
		}
		
		ActivateItems(false);
		
		daddy.GetComponent<Animation>().Play("ScaleUp");
		if (playSoundIn != null) {
			NGUITools.PlaySound(playSoundIn);
		}
				
		if (TutorialsSystem.Instance.itemIdx >= 0) 
		{
			designatedItem.OnItemClick += HideTutorial;
			TutorialsSystem.OnTutorialHide += HideTutorial;
		}
		else {
			TutorialsSystem.OnTutorialHide += HideTutorial;
		}
	}
	
	void HideTutorial()
	{
		if (designatedItem != null) {
			designatedItem.OnItemClick -= HideTutorial;
		}
		
		ActivateItems(true);
		
		if (TutorialsSystem.Instance.itemIdx >= 0) {
			TutorialsSystem.Instance.disableTutorial = true;
		}
		
		daddy.GetComponent<Animation>().Play("ScaleDown");
		if (playSoundOut != null) {
			NGUITools.PlaySound(playSoundOut);
		}
		
		StartCoroutine(WaitForAnimation());
	}
	
	IEnumerator WaitForAnimation()
	{
		yield return new WaitForSeconds(daddy.GetComponent<Animation>()["ScaleDown"].length + 0.01f);
		
		Destroy(daddy);
	}
	
	void OnDestroy()
	{
		TutorialsSystem.OnTutorialShow -= ShowTutorial;
		TutorialsSystem.OnTutorialHide -= HideTutorial;
	}
}
