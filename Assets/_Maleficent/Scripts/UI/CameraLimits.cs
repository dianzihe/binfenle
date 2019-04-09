using UnityEngine;
using System.Collections;

public class CameraLimits : MonoBehaviour {
	
	[System.Serializable]
	public class Limits{
		public float front;
		public float rear;
		public float left;
		public float right;
	}
	public Limits limits;
	public float height;
	public float springStrength = 0.2f;
	public static System.Action onLeftLimitReached;
	public static System.Action onRightLimitReached;
	public float threshold = 0.1f;
	public event System.Action onMovementLimited;
	[HideInInspector]
	public bool active = true;

	public enum LimitType
	{
		left,
		right,
		front,
		rear
	};

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Limits l = GetCameraLimits();
		Vector3 disp = Vector3.zero;

		if((l.front - l.rear) > (limits.front - limits.rear)) {
			disp.z = (limits.front + limits.rear) / 2.0f - (l.front + l.rear) / 2.0f;
		} else {
			if(l.front > limits.front)
				disp.z = limits.front - l.front; 
			else if(l.rear < limits.rear)
				disp.z = limits.rear - l.rear;
		}

		if((l.right - l.left) > (limits.right - limits.left)) {
			disp.x = (limits.right + limits.left) / 2.0f - (l.right + l.left) / 2.0f;
		} else {
			if(l.left < limits.left)
			{
				if(onLeftLimitReached != null)
					onLeftLimitReached();
				disp.x = limits.left - l.left;
			}
			else if(l.right > limits.right)
			{
				if(onRightLimitReached != null)
					onRightLimitReached();
				disp.x = limits.right - l.right;
			}
		}

		disp *= disp.magnitude * springStrength;

		if(active)
		{
			transform.Translate(disp, Space.World);
		
			if(onMovementLimited != null && disp != Vector3.zero) {
				onMovementLimited();
			}
		}
	}
	
	Limits GetCameraLimits() {
		Limits ret = new Limits();
		Plane p = new Plane(Vector3.up, new Vector3(0.0f, height, 0.0f));
		float d;
		
		Ray rFL = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.0f, 1.0f, 0.0f));
		p.Raycast(rFL, out d);
		ret.left = rFL.GetPoint(d).x;
		ret.front = rFL.GetPoint(d).z;
		
		Ray rFR = GetComponent<Camera>().ViewportPointToRay(new Vector3(1.0f, 1.0f, 0.0f));
		p.Raycast(rFR, out d);
		ret.right = rFR.GetPoint(d).x;
		
		Ray rRL = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.0f, 0.0f, 0.0f));
		p.Raycast(rRL, out d);
		ret.rear = rRL.GetPoint(d).z;
		
		return ret;
	}
	
	void DrawLimits(Limits _limits) {
		Vector3 fl = new Vector3(_limits.left, height, _limits.front);
		Vector3 fr = new Vector3(_limits.right, height, _limits.front);
		Vector3 rl = new Vector3(_limits.left, height, _limits.rear);
		Vector3 rr = new Vector3(_limits.right, height, _limits.rear);
		
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(fl, fr);
		Gizmos.DrawLine(fr, rr);
		Gizmos.DrawLine(rr, rl);
		Gizmos.DrawLine(rl, fl);
	}
	
	void OnDrawGizmos() {
		DrawLimits(limits);
		DrawLimits(GetCameraLimits());
	}

	public bool GetIsCloseToLimit(LimitType limitType,float threshold)
	{
		Limits l = GetCameraLimits();

		if( (limitType == LimitType.left && l.left < limits.left + threshold) ||
		   (limitType == LimitType.right && l.right > limits.right - threshold) ||
		   (limitType == LimitType.front && l.front > limits.front - threshold) ||
		   (limitType == LimitType.rear && l.rear < limits.rear + threshold) )
		{
			return true;
		}
		else
			return false;
	}

	public bool GetIsOutOfBounds(LimitType limitType)
	{
		Limits l = GetCameraLimits();

		if( (limitType == LimitType.left && l.left < limits.left - threshold) ||
		   (limitType == LimitType.right && l.right > limits.right + threshold) ||
		   (limitType == LimitType.front && l.front > limits.front + threshold) ||
		   (limitType == LimitType.rear && l.rear < limits.rear - threshold)
			)
		{
			return true;
		}
		else
			return false;
	}
}
