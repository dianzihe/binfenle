using UnityEngine;
using System.Collections;

public class RotateAccelerometer : MonoBehaviour {
	
	public Transform target;

	public float min;
	public float max;
	public float offset;
	private float previousX;
	
	private float t = 0.9f;
	
	// These two limits should be calculated... I just don't have time to do the maths :(
	// The limit is shomehow related to the FovX, linear... so just try to adjust that in the editor manually
	public float leftFactor;
	public float rightFactor;
#if UNITY_EDITOR
	public float testAngle; //Fake accelerometer angle

	void Start () {
		testAngle = offset;
		
		Transform targetCopy = new GameObject(name + " pivot").transform;
		targetCopy.gameObject.hideFlags = HideFlags.NotEditable;
		targetCopy.position = target.position;
		targetCopy.rotation = target.rotation;
		targetCopy.parent = target;
		transform.parent = targetCopy;
		target = targetCopy;
	}
#endif
	void Update () {
		//if(camera.enabled) {
			float accel = previousX * t + Input.acceleration.x * (1.0f - t);
			previousX = accel;

	#if UNITY_EDITOR
			float angle = testAngle;
	#else
			float angle = offset + accel * 20.0f;
	#endif
			float fovX = GetFovX(GetComponent<Camera>().aspect, GetComponent<Camera>().fieldOfView);
			angle = Mathf.Clamp(angle, min + fovX * leftFactor, max - fovX * rightFactor);
			
			target.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
		//}
	}

	public float GetFovX(float _aspectRatio, float _fovY) {
		return Mathf.Atan(_aspectRatio * Mathf.Tan(_fovY * Mathf.Deg2Rad / 2.0f)) * Mathf.Rad2Deg * 2.0f;
	}
}