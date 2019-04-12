using UnityEngine;
using System.Collections;

public abstract class AbstractLevelTargetGUI : MonoBehaviour {

	private bool updatingGUI = false;
	private WaitForEndOfFrame waitEndFrame;
	protected AbstractWinCondition winCondition;
	public GameObject targetScore;
	public Match3BoardGameLogic gameLogic;


	// Use this for initialization
	protected virtual void Awake () {
		waitEndFrame = new WaitForEndOfFrame();
	}

	protected virtual void Start () {
		winCondition = gameLogic.winConditions;

		if(winCondition == null || winCondition.GetType() !=  WinConditionType()) {
			Destroy (gameObject);
		}else {
			if(targetScore != null) {
				targetScore.SetActive(false);
			}
		}

		TargetValuesChanged();
	}

	protected void TargetValuesChanged() 
	{
		StartCoroutine(UpdateValuesCoroutine());
	}

	
	private IEnumerator UpdateValuesCoroutine() 
	{
		if(!updatingGUI) {
			updatingGUI = true;
			yield return waitEndFrame;

			if(winCondition != null && MainTargetCompleted()) {
				if (targetScore != null) {
					targetScore.SetActive(true);
				}
				gameObject.SetActive(false);
			}else {
				UpdateGUI();
			}
			
			updatingGUI = false;
		}
	}
	
	protected abstract System.Type WinConditionType();
	protected abstract bool MainTargetCompleted();
	protected abstract void UpdateGUI();
}
