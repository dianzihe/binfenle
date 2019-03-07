using UnityEngine;
using System.Collections;

public class Align : MonoBehaviour 
{
	public enum AlignType {
		None,
		Center,
		Top,
		Bottom,
		Left,
		Right,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}
	
	public Camera renderCamera;
	public float referenceAspectH = 4f / 3f;
	public float referenceAspectV = 3f / 4f;
	
	public AlignType alignH = AlignType.None;
	public AlignType alignV = AlignType.None;
	
	Transform xForm;
	Vector3 initialPos;
	
	// Use this for initialization
	void Awake() 
	{
		xForm = transform;
		initialPos = xForm.position;
	}
	
	void Start()
	{
		Realign();
		
		OrientationListener.Instance.OnOrientationChanged += OrientationChanged;
	}
	
	void Realign()
	{
		if (Screen.width >= Screen.height) {
			if (alignH != AlignType.None) {
				Realign(referenceAspectH, alignH);
			}
		} 
		else {
			if (alignV != AlignType.None) {
				Realign(referenceAspectV, alignV);
			}
		}
	}
	
	void Realign(float referenceAspectRatio, AlignType alignment) 
	{			
		Vector3 newPos;
		xForm.position = initialPos;
		
//		float viewRatio = 1f;
//		HorizontalFOVCamera viewChanger = renderCamera.GetComponent<HorizontalFOVCamera>();
//		if (viewChanger != null) {
//			if (renderCamera.orthographic) {
//				viewRatio = renderCamera.orthographicSize / viewChanger.InitialOrtoSize;
//			}
//			else {
//				viewRatio = renderCamera.fov / viewChanger.InitialFOV;
//			}
//		}
		
		if (alignment == AlignType.Left || alignment == AlignType.BottomLeft || alignment == AlignType.TopLeft ||
			alignment == AlignType.Right || alignment == AlignType.BottomRight || alignment == AlignType.TopRight) 
		{
			float originalX = Screen.height * referenceAspectRatio / Screen.width;
			Vector3 offset = renderCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)) - renderCamera.ViewportToWorldPoint(new Vector3(originalX, 0f, 0f));
			newPos = xForm.position;
			if (alignment == AlignType.Left || alignment == AlignType.BottomLeft || alignment == AlignType.TopLeft) {
				newPos.x = initialPos.x - offset.x / 2f;
			} else {
				newPos.x = initialPos.x + offset.x / 2f;
			}
			xForm.position = newPos;
		}
		
		if (alignment == AlignType.Top || alignment == AlignType.BottomLeft || alignment == AlignType.TopLeft ||
			alignment == AlignType.Bottom || alignment == AlignType.BottomRight || alignment == AlignType.TopRight) 
		{
			float originalY = Screen.width / referenceAspectRatio / Screen.height;
			Vector3 offset = renderCamera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)) - renderCamera.ViewportToWorldPoint(new Vector3(0f, originalY, 0f));
//			Debug.Log(offset);
			newPos = xForm.position;
			if (alignH == AlignType.Top || alignH == AlignType.TopLeft || alignH == AlignType.TopRight) {
				newPos.y = initialPos.y + offset.y / 2f;
			} else {
				newPos.y = initialPos.y - offset.y / 2f;
			}
			xForm.position = newPos;
		}
	}
	
	void OrientationChanged(ScreenOrientation newOrientation)
	{
		Realign();
	}
}
