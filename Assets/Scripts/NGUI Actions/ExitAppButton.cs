using UnityEngine;
using System.Collections;

public class ExitAppButton : MonoBehaviour 
{
	void OnClick()
	{
#if !UNITY_IPHONE
		Application.Quit();
#endif
	}
}
