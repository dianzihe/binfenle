using UnityEngine;
using System.Collections;

public class InputDisableOnLoad : MonoBehaviour
{
	public static InputDisableOnLoad instance;
	protected bool justLoaded = true;
	
	void Awake () 
	{
		instance = this;
		
		TemporaryDisable();
	}
	
	void TemporaryDisable()
	{
		NGuiEventsToPlaymakerFsmEvents.justLoaded = true;
		StartCoroutine(WaitEndFrame());
	}
	
	IEnumerator WaitEndFrame()
	{
		yield return new WaitForEndOfFrame();
		
		if (NGuiEventsToPlaymakerFsmEvents.justLoaded)
		{
			NGuiEventsToPlaymakerFsmEvents.justLoaded = false;
		}
	}

	void OnLevelWasLoaded()
	{
		TemporaryDisable();
	}
}

