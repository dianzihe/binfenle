using UnityEngine;
using System.Collections;

public class RotateAccelerometer2 : MonoBehaviour {
	
	public Transform target;
	
	public float offset;
	public Vector2 leftLimit;
	public Vector2 rightLimit;

	private float previousX;
	private float t = 0.9f;

#if UNITY_EDITOR
	public float testAngle; //Fake accelerometer angle

	void Start () {
		testAngle = offset;
		
		Transform targetCopy = new GameObject(name + " pivot").transform;
		targetCopy.gameObject.hideFlags = HideFlags.NotEditable;
		targetCopy.position = target.position;
		targetCopy.rotation = Quaternion.LookRotation(target.position - new Vector3(transform.position.x, target.position.y, transform.position.z));
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

			float fovX = GetFovX((Screen.width * GetComponent<Camera>().rect.width) / (Screen.height * GetComponent<Camera>().rect.height), GetComponent<Camera>().fieldOfView);
			float min = Mathf.Atan2(leftLimit.x  - target.position.x, leftLimit.y  - target.position.z) * Mathf.Rad2Deg;
			float max = Mathf.Atan2(rightLimit.x - target.position.x, rightLimit.y - target.position.z) * Mathf.Rad2Deg;

			//Because the projection matrix is modified to add displacement we need to calculate the fov on each side
			float w = Mathf.Tan(fovX * 0.5f * Mathf.Deg2Rad); //Half screen width at d = 1.0f from camera
			float leftFov  = Mathf.Atan2((1.0f - GetComponent<Camera>().projectionMatrix.m02) * w, 1.0f) * Mathf.Rad2Deg;
			float rightFov = Mathf.Atan2((1.0f + GetComponent<Camera>().projectionMatrix.m02) * w, 1.0f) * Mathf.Rad2Deg; // fovX - leftFov seems to have precission issues, so better do it this way
			float leftLim  = GetAngleLimit(leftLimit,  leftFov);
			float rightLim = GetAngleLimit(rightLimit, rightFov);

			angle = Mathf.Clamp(angle, min + leftLim, max - rightLim);
			target.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
		//}
	}

	private float GetAngleLimit(Vector2 _limit, float _hFov) {
		float t = (new Vector2(target.position.x - transform.position.x, target.position.z - transform.position.z)).magnitude;
		float d = (new Vector2(_limit.x - target.position.x,             _limit.y - target.position.z)            ).magnitude;
		float h = t * Mathf.Sin(_hFov * Mathf.Deg2Rad);
		float alpha2 = Mathf.Asin(h / d) * Mathf.Rad2Deg;
		return 180.0f - (180.0f - (_hFov + alpha2));
	}

	public float GetFovX(float _aspectRatio, float _fovY) {
		return Mathf.Atan(_aspectRatio * Mathf.Tan(_fovY * Mathf.Deg2Rad / 2.0f)) * Mathf.Rad2Deg * 2.0f;
	}

	public void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(new Vector3(leftLimit.x, transform.position.y, leftLimit.y), 10.0f);
		Gizmos.DrawWireSphere(new Vector3(rightLimit.x, transform.position.y, rightLimit.y), 10.0f);
		Gizmos.DrawRay(transform.position, transform.forward * 10000.0f);

	}
}