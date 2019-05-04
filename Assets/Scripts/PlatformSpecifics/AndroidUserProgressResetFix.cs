using UnityEngine;
using System.Collections;

public class AndroidUserProgressResetFix : MonoBehaviour {
	private string previousInstalledAppFlag = "previousInstalled";
		
	void Awake()
	{
		#if UNITY_ANDROID
		// Check if the app was previously installed but its PlayerPrefs are gonne. This could happen if
		// the user cleared the app data from Android Settings.
		if (PlayerPrefs.GetInt(previousInstalledAppFlag, 0) == 0)
		{
			PlayerPrefs.SetInt(previousInstalledAppFlag, 1);
			
			if ( System.IO.File.Exists(Application.persistentDataPath + "/" + UserManagerCloud.FILE_NAME_LOCAL) )
			{
				try {
					System.IO.File.Delete(Application.persistentDataPath + "/" + UserManagerCloud.FILE_NAME_LOCAL);
				}
				catch(System.Exception) {
					Debug.LogWarning("[AndroidUserProgressResetFix] User progress reset after clear data failed...");
				}
			}
		}
		#endif
		
		Destroy(this);
	}	
}
