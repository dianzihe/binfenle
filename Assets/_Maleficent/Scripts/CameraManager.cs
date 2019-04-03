using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Camera))]
public class CameraManager : MonoBehaviour {
	
	public Camera target;
	
	public Camera[] targets;
	
	public float speed = 4.0f;
	
	public AnimationCurve interpolationCurve;
	
	private float t;
	
	private Camera previousTarget;
	private Rect previousRect;
	
	private Vector3 pos0;
	private Quaternion rot0;
	private Matrix4x4 projectionMatrix0;
	
	void Start() {
		//Disable all potential targets (not necessary, but helpful)
		foreach(Camera c in targets) {
			c.enabled = false;
		}
		
		MoveToTarget(target, 0.0001f);
	}
	
	public void MoveToTarget(Camera _target, float _time = 1.0f) {
		speed = 1.0f / _time;
		
		//Disable target camera
		_target.enabled = false;
		
		t = 0.0f;
		
		
		target = _target;
		transform.parent = _target.transform;
		previousRect = _target.rect;
		previousTarget = _target;
		
		pos0 = transform.localPosition;
		rot0 = transform.localRotation; //It's important to keep the rotation in local coordinates to avoid a gimbal lock
		projectionMatrix0 = GetComponent<Camera>().projectionMatrix;
		
		
		//Non interpolable values
		GetComponent<Camera>().transparencySortMode = _target.transparencySortMode;
		GetComponent<Camera>().cullingMask = _target.cullingMask;
		GetComponent<Camera>().depth = _target.depth;
	}
	
	// Update is called once per frame
	void Update () {
		if(target != previousTarget) {
			MoveToTarget(target);
		}
		
		Matrix4x4 m = GetComponent<Camera>().projectionMatrix;
		if(target != null) {
			if(t < 1.0f) {
				t =  Mathf.Clamp(t + speed * Time.deltaTime, 0.0f, 1.0f);
				
				float tSpring = interpolationCurve.Evaluate(t);
				
				transform.localPosition = Vector3.Lerp    (pos0, Vector3.zero, tSpring);
				transform.localRotation = Quaternion.Slerp(rot0, Quaternion.identity, tSpring);
				for(int i = 0; i < 4; ++i) {
					for(int j = 0; j < 4; ++j) {
						m[i, j] = Mathf.Lerp(projectionMatrix0[i, j], target.projectionMatrix[i, j], tSpring);
					}
				}
			} else {
				for(int i = 0; i < 4; ++i) {
					for(int j = 0; j < 4; ++j) {
						m[i, j] = target.projectionMatrix[i, j];
					}
				}
			}
			
		}
		GetComponent<Camera>().projectionMatrix = m;

		for(int key = (int)KeyCode.Alpha1; key < (int)KeyCode.Alpha9; ++key) {
			if(Input.GetKeyDown((KeyCode)key)) {
				int idx = key - (int)KeyCode.Alpha1;
				if(idx < targets.Length) {
					MoveToTarget(targets[idx]);
				}
			}
		}
	}
}
