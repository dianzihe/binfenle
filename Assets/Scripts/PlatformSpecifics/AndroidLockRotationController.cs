using UnityEngine;
using System.Collections;

public class AndroidLockRotationController : MonoBehaviour {
	
	/// <summary>
	/// Handles the lock rotation accordingly. This method is only called from Android Native Code trough SendMessage().
	/// </summary>
	/// <param name='isLocked'>
	/// Is locked.
	/// </param>
	public void HandleLockRotation(string isRotationEnabled)
	{
		if(isRotationEnabled.Equals("0"))
		{
			Screen.orientation = ScreenOrientation.Portrait;
		}
		else
		{
			Screen.orientation = ScreenOrientation.AutoRotation;
		}
	}
}
