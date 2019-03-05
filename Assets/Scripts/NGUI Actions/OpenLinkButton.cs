using UnityEngine;
using System.Collections;

public class OpenLinkButton : MonoBehaviour
{
	//public static LinkLabel linkLabel;
	
	void OnClick()
	{
		/* 
		if (linkLabel) {
			linkLabel.OpenURL();
			linkLabel = null;
		}
		*/
	}
	
	void OnDestroy()
	{
		//linkLabel = null;
	}
}

