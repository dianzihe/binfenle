using UnityEngine;
using System.Collections;

public class CameraOffset : MonoBehaviour {
	
	public Vector2 disp;

	private float previousAspect;
	private float previousFOV;
	private float previousNear;
	private float previousFar;

	float CameraAspect() {
		if(GetComponent<Camera>().enabled) {
			return GetComponent<Camera>().aspect;
		} else {
			return (GetComponent<Camera>().rect.width * Screen.width) / (GetComponent<Camera>().rect.height * Screen.height);
		}
	}

	// Use this for initialization
	void Start () {
		//originalProjection = camera.projectionMatrix;
		previousAspect = CameraAspect();
		previousFOV = GetComponent<Camera>().fieldOfView;
		previousNear = GetComponent<Camera>().nearClipPlane;
		previousFar = GetComponent<Camera>().farClipPlane;
	}

	void Update() {
		if(CameraAspect() != previousAspect || GetComponent<Camera>().fieldOfView != previousFOV || 
		   GetComponent<Camera>().nearClipPlane != previousNear || GetComponent<Camera>().farClipPlane != previousFar) {
			GetComponent<Camera>().ResetAspect();
			GetComponent<Camera>().ResetProjectionMatrix();

			previousAspect = CameraAspect();
			previousFOV = GetComponent<Camera>().fieldOfView;
			previousNear = GetComponent<Camera>().nearClipPlane;
			previousFar = GetComponent<Camera>().farClipPlane;
		}

		Matrix4x4 m = GetComponent<Camera>().projectionMatrix;
		m.m02 = disp.x;
		m.m12 = disp.y;
		GetComponent<Camera>().projectionMatrix = m;
	}

	void OnDrawGizmosSelected() {
		//if(camera.enabled) {
			Vector3[] nearPoints = {
				GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.0f, 0.0f, GetComponent<Camera>().nearClipPlane)),
				GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.0f, 1.0f, GetComponent<Camera>().nearClipPlane)),
				GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1.0f, 1.0f, GetComponent<Camera>().nearClipPlane)),
				GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1.0f, 0.0f, GetComponent<Camera>().nearClipPlane)),
			};

			Vector3[] farPoints = {
				GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.0f, 0.0f, GetComponent<Camera>().farClipPlane)),
				GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.0f, 1.0f, GetComponent<Camera>().farClipPlane)),
				GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1.0f, 1.0f, GetComponent<Camera>().farClipPlane)),
				GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1.0f, 0.0f, GetComponent<Camera>().farClipPlane)),
			};

			Gizmos.color = Color.white;
			Gizmos.DrawLine(nearPoints[0], nearPoints[1]);
			Gizmos.DrawLine(nearPoints[1], nearPoints[2]);
			Gizmos.DrawLine(nearPoints[2], nearPoints[3]);
			Gizmos.DrawLine(nearPoints[3], nearPoints[0]);

			Gizmos.DrawLine(farPoints[0], farPoints[1]);
			Gizmos.DrawLine(farPoints[1], farPoints[2]);
			Gizmos.DrawLine(farPoints[2], farPoints[3]);
			Gizmos.DrawLine(farPoints[3], farPoints[0]);

			Gizmos.DrawLine(nearPoints[0], farPoints[0]);
			Gizmos.DrawLine(nearPoints[1], farPoints[1]);
			Gizmos.DrawLine(nearPoints[2], farPoints[2]);
			Gizmos.DrawLine(nearPoints[3], farPoints[3]);
		//}
	}
}
