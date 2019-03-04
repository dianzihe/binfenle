using UnityEngine;
using System.Collections;
/*
public class FacebookConnectButton : MonoBehaviour 
{
	protected static bool dirty = false;
	
	public UILabel label;
	
	// Use this for initialization
	void Start () 
	{
		if (FacebookManager.Instance == null) {
			return;
		}
		
		FacebookManager.Instance.OnFacebookStatusChangedEvent += OnFacebookStatusChanged;
		
		if (!dirty) 
		{
			dirty = true;
			FacebookManager.Instance.Init();
		}
		
		UpdateText();
	}

	void OnFacebookStatusChanged(FacebookEventId eventId)
	{
		Debug.Log("[FacebookConnectButton] OnFacebookStatusChanged: " + eventId);
		dirty = false;
		UpdateText();
	}

	void UpdateText() 
	{
		if (FacebookManager.Instance.IsUserLogged) {
			label.text = Language.Get("FACEBOOK_LOGOUT");
		}
		else {
			label.text = Language.Get("FACEBOOK_LOGIN");
		}
	}
	
	void OnClick()
	{
		if (dirty || label.alpha < 1f) {
			return;
		}
		
		dirty = true;
		
		if (FacebookManager.Instance.IsUserLogged) {
			FacebookManager.Instance.Logout();
		}
		else {
			FacebookManager.Instance.Login();
		}
	}
	
	void OnDestroy()
	{
		dirty = false;
		
		if (FacebookManager.Instance) 
		{
			FacebookManager.Instance.OnFacebookStatusChangedEvent -= OnFacebookStatusChanged;
		}
	}
}
*/
