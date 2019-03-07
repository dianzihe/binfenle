using UnityEngine;
using System.Collections;

public class ObjectiveHintController : MonoBehaviour {
	
	public GameObject effectMarkObjective;
	public float showNextHintDelay;
	
	public Vector3 positionLandscape;
	public Vector3 positionPortrait;

	public void Awake()
	{
		Debug.Log("[MarkObjectiveController] Registered to OnStableBoardCheckingStatusChanged...");
		Match3BoardGameLogic.OnStableBoardCheckingStatusChanged += OnStableBoardCheckingStatusChangedEvent;
		OrientationListener.Instance.OnOrientationChanged += ActionOnOrientationChanged;
		
		if(Screen.width > Screen.height)
		{
			ActionOnOrientationChanged(ScreenOrientation.Landscape);
		}
		else
		{
			ActionOnOrientationChanged(ScreenOrientation.Portrait);
		}
	}
	
	protected void OnStableBoardCheckingStatusChangedEvent(bool isCheckingBoardStable) 
	{
		// Don't handle this event if this component is disabled or the game is over!
		if ( !enabled || Match3BoardGameLogic.Instance.IsGameOver ) {
			return;
		}
			
//		Debug.Log("[ObjectiveHintController] OnStableBoardCheckingStatusChangedEvent...");
		
		if ( !isCheckingBoardStable )  {
			StartCoroutine("ShowHintAfterDelay", showNextHintDelay);
		}
		else if ( isCheckingBoardStable ) {
			StopCoroutine("ShowHintAfterDelay");
		    effectMarkObjective.SetActive(false);
			effectMarkObjective.GetComponent<Animation>().Stop();
		}
	}
	
	IEnumerator ShowHintAfterDelay(float delay) 
	{
		yield return new WaitForSeconds(delay);
		
		if (!Match3BoardGameLogic.Instance.IsGameOver)
		{
			effectMarkObjective.SetActive(true);
			effectMarkObjective.GetComponent<Animation>().Play();
		}
	}
	
	public void OnDestroy()
	{
		Match3BoardGameLogic.OnStableBoardCheckingStatusChanged -= OnStableBoardCheckingStatusChangedEvent;
	}
	
	public void ActionOnOrientationChanged(ScreenOrientation newOrientation) 
	{
		if(newOrientation == ScreenOrientation.Landscape)
		{
			transform.localPosition = positionLandscape;
		}
		else
		{
			transform.localPosition = positionPortrait;
		}
	}
}
