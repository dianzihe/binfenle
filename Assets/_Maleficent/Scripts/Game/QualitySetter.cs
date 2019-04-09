using UnityEngine;
using System.Collections;

public class QualitySetter : MonoBehaviour {

#if UNITY_EDITOR
	public int qualityTest;
#endif

	public static bool useHDAtlas;
	public static bool shouldUnloadMapAssets = true;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
		QualitySettings.SetQualityLevel(qualityTest);
		useHDAtlas = true;
#endif
		
#if UNITY_IPHONE && !UNITY_EDITOR
		if (iPhone.generation <= iPhoneGeneration.iPodTouch4Gen) {
			QualitySettings.SetQualityLevel(0);
			useHDAtlas = false;
		} else {
			QualitySettings.SetQualityLevel(3);
			useHDAtlas = true;
		}
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
#if AMAZON_ANDROID
		QualitySettings.SetQualityLevel(0);
		useHDAtlas = false;
#else
		QualitySettings.SetQualityLevel(3);
		useHDAtlas = false;
#endif
#endif

#if UNITY_WEBPLAYER && !UNITY_EDITOR
		QualitySettings.SetQualityLevel(3);
		useHDAtlas = true;
#endif
	}
}
