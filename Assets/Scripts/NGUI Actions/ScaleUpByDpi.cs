using UnityEngine;
using System.Collections;

public class ScaleUpByDpi : MonoBehaviour 
{
	static float referenceWidth = 1024f;
	static float referenceDpi = 130f; // iPad1 / iPad2 dpi
	public float minScaleFactor = 1.0f;
	public float maxScaleFactor = 1.3f;
	
	protected float dpiScaleFactor;
	protected Vector3 originalScale;
	protected Transform cachedTransform;
	protected float screenWidth = 0f;
	
	void Awake() 
	{
#if UNITY_EDITOR
		dpiScaleFactor = 1f;
#elif UNITY_IPHONE
		dpiScaleFactor = Screen.dpi / referenceDpi;
#elif UNITY_ANDROID
		dpiScaleFactor = Screen.dpi / referenceDpi;
#else
		dpiScaleFactor = 1f;
#endif
		cachedTransform = transform;
		originalScale = cachedTransform.localScale;
		
		UpdateScale(ScreenOrientation.Landscape);
	}
	
	void Start()
	{
		OrientationListener.Instance.OnOrientationChanged += UpdateScale;
	}
	
	void UpdateScale(ScreenOrientation orientation)
	{	
		if (screenWidth == Mathf.Max(Screen.width, Screen.height)) {
			return;
		}
		
		screenWidth = Mathf.Max(Screen.width, Screen.height);
		float myScaleFactor = screenWidth / referenceWidth;
		cachedTransform.localScale = originalScale * Mathf.Clamp(dpiScaleFactor / myScaleFactor, minScaleFactor, maxScaleFactor);
	}
}
