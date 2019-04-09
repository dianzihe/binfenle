using UnityEngine;
using Holoville.HOTween;
using System.Collections;
using System.Collections.Generic;

public class CameraMovement : MonoBehaviour {
	
	Vector3 cachedPosition;
	Quaternion cachedRotation;
	float speed = 0.6f;

	public Transform landscapeGizmo;
	public Transform portraitGizmo;

	public static event System.Action onSwipeDone;
	public static bool moving = false;
	public CameraLimits.Limits landscapeLimits;
	public CameraLimits.Limits portraitLimitsL;
	public CameraLimits.Limits portraitLimitsR;

	enum Orientation
	{
		portraitLeft,
		portraitRight,
		landscape,
		None
	};

	Orientation currentOrientation;

	void Awake()
	{
		cachedPosition = transform.position;
		cachedRotation = this.transform.rotation;
	}

	void Start ()
	{
		OrientationListener.Instance.OnOrientationChanged += OrientationChanged;
		//BookUIControl.OnSwipe += ActionOnSwipe;
		currentOrientation = Orientation.None;
	}

	void OnDestroy()
	{
		if(OrientationListener.IsInstantiated()) {
			OrientationListener.Instance.OnOrientationChanged -= OrientationChanged;
		}
	}

	public void RefreshCameraLimits()
	{
		OrientationChanged(Screen.width>Screen.height?ScreenOrientation.Landscape:ScreenOrientation.Portrait,false);
	}

	void OrientationChanged(ScreenOrientation newOrientation) {
		OrientationChanged(newOrientation, true);
	}

	void OrientationChanged(ScreenOrientation newOrientation, bool changePosition)
	{
		// if the camera is moving we do not set a new camera position
		if(moving)
			return;

		if(newOrientation == ScreenOrientation.Portrait)
		{
			if(transform.position.x > 0)
			{
				if(changePosition)
					GetComponent<Camera>().transform.position = new Vector3(-portraitGizmo.position.x,portraitGizmo.position.y,portraitGizmo.position.z);
				SetPortraitLimitsR();
			}
			else
			{
				if(changePosition)
					GetComponent<Camera>().transform.position = portraitGizmo.position;
				SetPortraitLimitsL();
			}
		}
		if(newOrientation == ScreenOrientation.Landscape)
		{
			if(changePosition)
				GetComponent<Camera>().transform.position = landscapeGizmo.position;
			currentOrientation = Orientation.landscape;
			GetComponent<CameraLimits>().limits = landscapeLimits;
		}
	}

	IEnumerator SetPortraitLimitsL()
	{
		currentOrientation = Orientation.portraitLeft;
		GetComponent<CameraLimits>().limits = landscapeLimits; //portraitLimitsL;

		yield return null;
	}

	IEnumerator SetPortraitLimitsR()
	{
		currentOrientation = Orientation.portraitRight;
		GetComponent<CameraLimits>().limits = landscapeLimits; //portraitLimitsR;

		yield return null;
	}

	#region Camera movement algorithms
	public void GoToStartPosition (System.Action onEndAction) {
		Vector3 dstPos = (Screen.height > Screen.width)?portraitGizmo.position:landscapeGizmo.position;
		float duration = Vector3.Magnitude(transform.position - dstPos) * speed;
		Move(dstPos,duration,onEndAction);
	}

	public void GoToLevelButton(LoadLevelButton levelButton,bool move,System.Action actionOnEnd = null)
	{
		Ray ray = GetComponent<Camera>().ViewportPointToRay (new Vector3(0.5f,0.5f,0f));
		RaycastHit hit;
		if (Physics.Raycast (ray,out hit))
		{
			// to put the button a little downwards
			Vector3 targetPos = new Vector3(levelButton.transform.position.x,levelButton.transform.position.y,levelButton.transform.position.z+0.5f);

			float offsetX = targetPos.x - hit.point.x;
			float offsetY = targetPos.y - hit.point.y;

			Vector3 intermediateCamPosition = new Vector3(transform.position.x+offsetX,transform.position.y+offsetY,transform.position.z);
			Vector3 ButtonIntCamVector = intermediateCamPosition - targetPos;
			Vector3 dstPos = (targetPos + Vector3.Normalize(ButtonIntCamVector) * 1.7f);

			// xavi, to do: duration depending on the distance
			if(move)
				Move(dstPos,1f,actionOnEnd);
			else
				transform.position = dstPos;
		}
	}

	void Move(Vector3 dstPos,float duration,System.Action actionOnEnd = null)
	{
		// disable camera limits
		GetComponent<CameraLimits>().enabled = false;

		HOTween.To(this.transform,duration, new TweenParms()
		           .Prop("position",dstPos)
		           .Ease(EaseType.EaseInOutSine)
		           .OnStart(() => {
					moving = true;
			})
		           .OnComplete(() => {
					moving = false;

					// check if the orientation has changed while moving the camera
					Orientation previousOrientation = currentOrientation;
					if(previousOrientation == Orientation.None)
						OrientationChanged(Screen.width >Screen.height?ScreenOrientation.Landscape:ScreenOrientation.Portrait,false);
					else if(previousOrientation == Orientation.landscape && Screen.height > Screen.width)
						OrientationChanged(ScreenOrientation.Portrait,false);
					else if(((previousOrientation == Orientation.portraitLeft || previousOrientation == Orientation.portraitRight)
			         && Screen.width >Screen.height))
						OrientationChanged(ScreenOrientation.Landscape,false);

					// enable cam limits
					GetComponent<CameraLimits>().enabled = true;

					// call the action sent by param
					if(actionOnEnd != null)
						actionOnEnd();
			})
		           );

	}
	#endregion
}
