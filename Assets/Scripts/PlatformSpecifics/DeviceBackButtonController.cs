using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
	
public class DeviceBackButtonController : MonoBehaviour {
	public delegate void OnBackBtnPressedDelegate(DeviceBackButtonController sender);
		
	public static event OnBackBtnPressedDelegate OnBackBtnPressed;
		
	public string fsmBroadcastEventName = "DeviceBack";
	
	protected bool justLoaded = true;
	
	// Use this for initialization
	void Start () 
	{
		// This script will be destroyed for iOS Platform because there is no Back button functionality.
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			Destroy(this);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Application.isLoadingLevel) {
			return;
		}
		
		if (justLoaded)
		{
			justLoaded = false;
			return;
		}
		
		if ( Input.GetKeyUp(KeyCode.Escape) )
		{
			if (OnBackBtnPressed != null) {
				OnBackBtnPressed(this);
			}

			PlayMakerFSM.BroadcastEvent(fsmBroadcastEventName);
		}
	}
	
	void OnLevelWasLoaded()
	{
		justLoaded = true;
	}
}
