using UnityEngine;
using System.Collections;

public class LinkLabel : MonoBehaviour 
{
	public string link;
	public PlayMakerFSM confirmFsm;
	public string confirmEvent = "Confirm";
	
	void OnClick() 
	{
		OpenLinkButton.linkLabel = this;
		confirmFsm.SendEvent(confirmEvent);
	}
	
	public void OpenURL()
	{
		Application.OpenURL(link);
	}
}
