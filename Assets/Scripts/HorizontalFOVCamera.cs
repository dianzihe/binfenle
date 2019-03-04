using UnityEngine;
using System.Collections;

public class HorizontalFOVCamera : MonoBehaviour
{
	public bool deactivateOnPortrait = true;
	public bool deactivateOnLandscape = false;
	
	public bool keepFOVLandscape = false;
	public bool keepFOVPortrait = false;
	
	public float overrideFOVLandscape = 0f;
	public float overrideFOVPortrait = 0f;
	
	public Transform specificTransformLandscape;
	public Transform specificTransformPortrait;
	
	public Vector3 posOffsetLandscape = Vector3.zero;
	public Vector3 posOffsetPortrait = Vector3.zero;
	
	public float scalePercentLandscape = 1f;
	public float scalePercentPortrait = 1f;
	
	public float referenceAspectRatio = 4f / 3f;
	
	public Rect landscapeNormalizedViewPort = new Rect(0f, 0f, 1f, 1f);
	public Rect portraitNormalizedViewPort = new Rect(0f, 0f, 1f, 1f);
	
	private float initialFOV = 0;
	private float initialOrtoSize = 0;
	private Vector3 initialPos = Vector3.zero;
	private Quaternion initialRotation = Quaternion.identity;
	private Vector3 initialScale = Vector3.one;
	
	public float InitialFOV {
		get {
			if (overrideFOVPortrait != 0 && Screen.height > Screen.width) {
				return overrideFOVPortrait;
			}
			else if (overrideFOVLandscape != 0 && Screen.width >= Screen.height) {
				return overrideFOVLandscape;
			}
			else {
				return initialFOV;
			}
		}
	}
	
	public float InitialOrtoSize {
		get {
			return initialOrtoSize;
		}
	}
	
	// Use this for initialization
	void Awake ()
	{		
		if (GetComponent<Camera>().orthographic) {
			initialOrtoSize = GetComponent<Camera>().orthographicSize;
		}
		else {
			initialFOV = GetComponent<Camera>().fieldOfView;
		}
		initialPos = transform.localPosition;
		initialRotation = transform.localRotation;
		initialScale = transform.localScale;
		
		if (deactivateOnPortrait && Screen.height > Screen.width) {
			// Keep the default FOV
			return;
		}
		if (deactivateOnLandscape && Screen.width >= Screen.height) {
			// Keep the default FOV
			return;
		}
		
		UpdateCameraFOV();
	}
	
	void Start()
	{
		OrientationListener.Instance.OnOrientationChanged += OrientationChanged;
	}
		
	private void UpdateCameraFOV()
	{
		// Update position
		if (specificTransformLandscape != null && Screen.width >= Screen.height) {
			transform.position = specificTransformLandscape.position;
			transform.rotation = specificTransformLandscape.rotation;
			transform.localScale = specificTransformLandscape.localScale;
		} 
		else if (specificTransformPortrait != null && Screen.width < Screen.height) {
			transform.position = specificTransformPortrait.position;
			transform.rotation = specificTransformPortrait.rotation;
			transform.localScale = specificTransformPortrait.localScale;
		}
		else {
			transform.localPosition = initialPos + ((Screen.height > Screen.width) ? posOffsetPortrait : posOffsetLandscape);
			transform.localRotation = initialRotation;
			transform.localScale = initialScale;
		}
		
		// Update viewport
		float aspectRatio = Mathf.Min((float)Screen.width / (float)Screen.height, 1024f / 640f);		
		if (Screen.width >= Screen.height) {
			if (landscapeNormalizedViewPort != new Rect(0f, 0f, 1f, 1f)) {
				Rect tempRect = landscapeNormalizedViewPort;
				tempRect.width *= aspectRatio / referenceAspectRatio;
				GetComponent<Camera>().rect = tempRect;
				GetComponent<Camera>().ResetAspect();
				GetComponent<Camera>().ResetProjectionMatrix();
			}
		}
		else {
//			Debug.Log(portraitNormalizedViewPort);
			if (portraitNormalizedViewPort != new Rect(0f, 0f, 1f, 1f)) {
//				Debug.Log("I'm doing it, I'm shaking it, I'm hating it, I'm baking it!");
				GetComponent<Camera>().rect = portraitNormalizedViewPort;
				GetComponent<Camera>().ResetAspect();
				GetComponent<Camera>().ResetProjectionMatrix();
			}
		}
		
		if ((deactivateOnPortrait && Screen.height > Screen.width) || 
			(deactivateOnLandscape && Screen.width >= Screen.height)) 
		{
			// Set back default FOV
			if (GetComponent<Camera>().orthographic) {
				GetComponent<Camera>().orthographicSize = InitialOrtoSize;
			}
			else {
				GetComponent<Camera>().fieldOfView = InitialFOV;
			}
			
			return;
		}
		
		if (GetComponent<Camera>().orthographic) {
			GetComponent<Camera>().orthographicSize = InitialOrtoSize * referenceAspectRatio / aspectRatio * 
				((Screen.height > Screen.width) ? scalePercentPortrait : scalePercentLandscape);
		}
		else {
//			Debug.Log("AR: " + aspectRatio + " rAR: " + referenceAspectRatio);
			if (!Mathf.Approximately(aspectRatio, referenceAspectRatio)) {
//				Debug.Log("NEW FOV: " + GetNewFOV(InitialFOV));
				GetComponent<Camera>().fieldOfView = GetNewFOV(InitialFOV) * 
					((Screen.height > Screen.width) ? scalePercentPortrait : scalePercentLandscape);;
			} 
			else {
				GetComponent<Camera>().fieldOfView = InitialFOV;
			}
		}
	}
	
	private float GetNewFOV(float oldFOV) 
	{
		if ((keepFOVLandscape && Screen.width >= Screen.height) || (keepFOVPortrait && Screen.width < Screen.height))
		{
			return oldFOV;
		}
		else {
			float constant = referenceAspectRatio * Mathf.Tan(Mathf.Deg2Rad * oldFOV / 2f);
			return 2f * Mathf.Atan(constant * Screen.height / (float)Screen.width) * Mathf.Rad2Deg;
		}
	}
	
	void OrientationChanged(ScreenOrientation newOrientation)
	{
		UpdateCameraFOV();
	}
}

