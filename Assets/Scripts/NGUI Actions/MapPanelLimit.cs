using UnityEngine;
using System.Collections;

public class MapPanelLimit : MonoBehaviour
{
	//public MapPanelLimit twinMap;
	public UICamera guiCamera;
	
	[System.NonSerialized]
	public Transform contents;
	
	UIPanel myPanel;
	UIDraggablePanel myDragPanel;
	Transform cachedTransform;
	[System.NonSerialized]
	public float currentZoom;
	[System.NonSerialized]
	public float maxZoom;
	[System.NonSerialized]
	public float minZoom;
	float oldZoom;
	float zoomFactor;
	float initPanelZ;
	Vector2 screenCenter;
	float widthFactor;
	Vector3 screenOffset;
	
	// Use this for initialization
	void Awake ()
	{
		cachedTransform = transform;
		myPanel = GetComponent<UIPanel>();
		myDragPanel = GetComponent<UIDraggablePanel>();
		initPanelZ = myPanel.transform.localPosition.z;
		
		contents = transform.Find("Contents");
		currentZoom = 1.2f;
		oldZoom = currentZoom;
		maxZoom = 1.2f;
		zoomFactor = 0.1f;
	}
	
	void Start()
	{
		OrientationListener.Instance.OnOrientationChanged += OrientationChanged;
		UpdateLimits(false);
	}
	
	void OrientationChanged(ScreenOrientation newOrientation)
	{
		UpdateLimits(true);
	}
	
	void UpdateLimits(bool reposition)
	{
		screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
		
		Vector4 clip = myPanel.clipRange;
		if (Screen.width >= Screen.height) {
			clip.z = Screen.width * 768f / Screen.height;
			clip.w = 768f;
			widthFactor = 768f / Screen.height;
		}
		else {
			clip.z = 768f;
			clip.w = Screen.height * 768f / Screen.width;
			widthFactor = 768f / Screen.width;
		}
		// god pardon me for doing this
		clip.z += 250f;
		clip.w += 200f;
		
		myPanel.clipRange = clip;
		if (reposition) {
			myDragPanel.RestrictWithinBounds(true);
		}
		
		minZoom = 0.7f;
		float aspectRatio = (float)Screen.width / (float)Screen.height;
		float referenceAR = 4f / 3f;
		if (aspectRatio > referenceAR) {
			minZoom *= aspectRatio / referenceAR;
			if (currentZoom < minZoom) {
				SetMapZoomAndPosition(Vector3.zero, minZoom, false);
			}
		}
	}
	
	void Update()
	{
		if (!guiCamera.enabled) {
			//let the twin controll the beat
			return;
		}
		
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
		if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved) 
		{
			Vector2 curDist = Input.GetTouch(0).position - Input.GetTouch(1).position; //current distance between finger touches
			Vector2 oldDist = curDist - (Input.GetTouch(0).deltaPosition - Input.GetTouch(1).deltaPosition);
			float touchDelta = (curDist.magnitude - oldDist.magnitude) * Time.deltaTime;
			Vector2 screenPoint = Vector2.Lerp(Input.GetTouch(0).position, Input.GetTouch(1).position, 0.5f);
#else
		if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0f) {
			float touchDelta = Input.GetAxis("Mouse ScrollWheel");
			Vector2 screenPoint = Input.mousePosition;
#endif
			currentZoom = Mathf.Clamp(currentZoom + touchDelta * zoomFactor, minZoom, maxZoom);
			
			if (screenOffset.z == -1f) {
				screenPoint -= screenCenter;
				screenOffset = new Vector3(screenPoint.x * widthFactor, screenPoint.y * widthFactor, 0f);
			}
				
			SetMapZoomAndPosition(screenOffset, currentZoom);
		} 
		else {
			screenOffset.z = -1f;
		}
			
		//twinMap.SetPosition(cachedTransform.localPosition);
	}
		
	public void SetMapZoomAndPosition(Vector3 pivotScreenOffset, float newZoom, bool affectTwin = true)
	{

		if (newZoom == oldZoom) {
			return;
		}
		SetZoom(currentZoom);
		//twinMap.SetZoom(currentZoom);
			
		float factor = currentZoom / oldZoom;
		oldZoom = currentZoom;
			
		Vector3 newPos = (cachedTransform.localPosition - pivotScreenOffset) * factor + pivotScreenOffset;
		newPos.z = initPanelZ;
	
		SetPosition(newPos);
		//twinMap.SetPosition(newPos);
	}
		
	public void SetZoom(float newZoom)
	{
		currentZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
		contents.localScale = Vector3.one * currentZoom;
	}
		
	public void SetPosition(Vector3 newPos,bool isGlobal=false)
	{
		// if is global -> converts to local coords
		if(isGlobal)
			newPos = transform.InverseTransformPoint(newPos);
		contents.parent.localPosition = newPos;
		Vector4 panelClip = myPanel.clipRange;
		panelClip.x = -cachedTransform.localPosition.x;
		panelClip.y = -cachedTransform.localPosition.y;
		myPanel.clipRange = panelClip;
		
		myDragPanel.UpdateScrollbars(true);
		myDragPanel.RestrictWithinBounds(true);
	}

	public void SetGlobalPosition(Vector3 newPos)
	{
		contents.parent.position = newPos;
		Vector4 panelClip = myPanel.clipRange;
		panelClip.x = -cachedTransform.localPosition.x;
		panelClip.y = -cachedTransform.localPosition.y;
		myPanel.clipRange = panelClip;
		
		myDragPanel.UpdateScrollbars(true);
		myDragPanel.RestrictWithinBounds(true);
	}
}

