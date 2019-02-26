// WARNING!!!!! DO NOT FORGET THIS DEFINE ENABLED BEFORE DOING A DEVICE BUILD!!!!!!!!!
//#define FORCE_DEVICE_INPUT
	
using UnityEngine;
using System.Collections;

/**
 * Handles cross-platform input. Allows using mouse input to simulate touches when not building for touch platform.
 */
public class CustomInput
{
    public class TouchInfo
    {
		public int fingerId = 0;
		public int beginFingerId = 0;
		public Vector2 deltaPosition = Vector2.zero;
		public float deltaTime = 0f;
        public Vector2 position = Vector2.zero;
		public Vector2 beginPosition = Vector2.zero;
		public float beginTime = 0f;
        public TouchPhase phase = TouchPhase.Stationary;
		
		public TouchInfo() { }
		
		public TouchInfo(TouchInfo otherTouch) {
			fingerId = otherTouch.fingerId;
			beginFingerId = otherTouch.beginFingerId;
			deltaPosition = otherTouch.deltaPosition;
			position = otherTouch.position;
			beginPosition = otherTouch.beginPosition;
			beginTime = otherTouch.beginTime;
			phase = otherTouch.phase;
		}
		
		public void PrintDebug() {
			Debug.Log("TouchInfo in frame: " + Time.frameCount + " fingerId = " + fingerId + " deltaPos = " + deltaPosition + " pos = " + position + 
			          "phase = " + phase + "beginTime = " + beginTime);
		}
    }
	
	private static ScreenOrientation screenOrient = ScreenOrientation.LandscapeLeft;
	// The deadzone applied on each acceleration axis
	public static Vector3 accelDeadzone = Vector3.zero;
	// Last acceration input that will be used for input calibration. (This amount will be substracted from future "inputAccel" values.)
	public static Vector3 baseAccel = Vector3.zero;
	// Last acceleration input read from the accelerometer.
	private static Vector3 inputAccel = Vector3.zero;
	
	public static bool mousePressed;
#if FORCE_DEVICE_INPUT || (!UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID))
#else
	public static int lastFrameUpdated;
	public static float lastFrameTime;
#endif
	
	public static TouchInfo[] touchesBuffer = new TouchInfo[5] {new TouchInfo(), new TouchInfo(), new TouchInfo(), new TouchInfo(), new TouchInfo() };
		
    public static int touchCount
    {
        get
        {
#if FORCE_DEVICE_INPUT || (!UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID))
			if (Input.touchCount == 0) {
				touchesBuffer[0].beginFingerId = -1;
			}
			return Input.touchCount;
#else
            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0) || mousePressed)
                return 1;
            else
                return 0;			
#endif
        }
    }

    public static TouchInfo GetTouch(int ID)
    {
#if FORCE_DEVICE_INPUT || (!UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID))
		touchesBuffer[ID].fingerId = Input.touches[ID].fingerId;
		touchesBuffer[ID].deltaPosition = Input.touches[ID].deltaPosition;
		touchesBuffer[ID].deltaTime = Input.touches[ID].deltaTime;
		touchesBuffer[ID].phase = Input.touches[ID].phase;
		touchesBuffer[ID].position = Input.touches[ID].position;
		
		if (touchesBuffer[ID].phase == TouchPhase.Began) {
			touchesBuffer[ID].beginPosition = touchesBuffer[ID].position;
			touchesBuffer[ID].beginFingerId = touchesBuffer[ID].fingerId;
			touchesBuffer[ID].beginTime = Time.time;
		}
#else
		if (lastFrameUpdated != Time.frameCount) {
			touchesBuffer[ID].position = Input.mousePosition;
			touchesBuffer[ID].fingerId = 0;
			
	        if (mousePressed) {
				touchesBuffer[ID].deltaTime = Time.time - lastFrameTime;
				touchesBuffer[ID].deltaPosition = new Vector2(Input.GetAxis("Mouse X") * (Mathf.Max(Screen.width, Screen.height) / 100.0f), Input.GetAxis("Mouse Y") * (Mathf.Min(Screen.width, Screen.height) / 100.0f));			
				if (touchesBuffer[ID].deltaPosition == Vector2.zero)
					touchesBuffer[ID].phase = TouchPhase.Stationary;
				else
					touchesBuffer[ID].phase = TouchPhase.Moved;
			} else if (Input.GetMouseButtonDown(0)) {
				mousePressed = true;
	            touchesBuffer[ID].phase = TouchPhase.Began;
				touchesBuffer[ID].beginPosition = touchesBuffer[ID].position;
				touchesBuffer[ID].beginFingerId = 0;
				touchesBuffer[ID].beginTime = Time.time;
			}
			
			if (Input.GetMouseButtonUp(0)) {
				mousePressed = false;
	            touchesBuffer[ID].phase = TouchPhase.Ended;
			}
			
			if (!mousePressed && touchesBuffer[ID].phase == TouchPhase.Began) {
				touchesBuffer[ID].phase = TouchPhase.Canceled;
			}
		}
		
		lastFrameUpdated = Time.frameCount;
		lastFrameTime = Time.time;
#endif
		
        return touchesBuffer[ID];
    }
	
	public static void UpdateScreenOrientation() {
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
		screenOrient = Screen.orientation;
#else
		if (Screen.width > Screen.height) {
			screenOrient = ScreenOrientation.LandscapeLeft;
		} else {
			screenOrient = ScreenOrientation.Portrait;
		}
#endif
		Debug.Log("screenOrient = " + screenOrient);	
	}
	
	public static void CalibrateAccelerometer() {
#if FORCE_DEVICE_INPUT || (!UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID))
		baseAccel = Input.acceleration;
		
		// Correct values based on the screen orientation
		switch(screenOrient) {
			case ScreenOrientation.LandscapeRight:
			case ScreenOrientation.LandscapeLeft: {
				Debug.Log("Screen running in landscape...");
				float temp = baseAccel.x;
				baseAccel.x = baseAccel.y;
				baseAccel.y = temp;
			}
			break;
		}
#else
		baseAccel = Vector3.zero;
#endif
	}
	
	public static Vector3 LastAcceleration {
		get {
#if FORCE_DEVICE_INPUT || (!UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID))
			inputAccel = Input.acceleration;
			
			// Correct values based on the screen orientation
			switch(screenOrient) {
				case ScreenOrientation.LandscapeLeft: {
					float temp = inputAccel.x;
					inputAccel.x = -inputAccel.y;
					inputAccel.y = temp;
				}
				break;
	
				case ScreenOrientation.LandscapeRight: {
					float temp = inputAccel.x;
					inputAccel.x = inputAccel.y;
					inputAccel.y = temp;
				}
				break;
				
				case ScreenOrientation.Portrait: {
					//default acceleration is correct
				}
				break;
				
				case ScreenOrientation.PortraitUpsideDown: {
					inputAccel.x = -inputAccel.x;
					inputAccel.y = -inputAccel.y;
				}
				break;
			}
	
			// Apply calibration filtering
			inputAccel -= baseAccel;
			
			// Apply deadzone filtering
			if (Mathf.Abs(inputAccel.x) <= accelDeadzone.x) {
				inputAccel.x = 0.0f;
			} else {
				inputAccel.x += -Mathf.Sign(inputAccel.x) * accelDeadzone.x;
			}
			if (Mathf.Abs(inputAccel.y) <= accelDeadzone.y) {
				inputAccel.y = 0.0f;
			} else {
				inputAccel.y += -Mathf.Sign(inputAccel.y) * accelDeadzone.y;
			}
			if (Mathf.Abs(inputAccel.z) <= accelDeadzone.z) {
				inputAccel.z = 0.0f;
			} else {
				inputAccel.z += -Mathf.Sign(inputAccel.z) * accelDeadzone.z;
			}
	
	
			return inputAccel;
#else
			float horizAxis = Input.GetAxis("Horizontal");
			float vertAxis = Input.GetAxis("Vertical");
			inputAccel.x = horizAxis;
			inputAccel.y = vertAxis;
			// Assume device is perpendicular to the ground.
			inputAccel.z = 0f;
			
			return inputAccel;
#endif
		}
	}
}
