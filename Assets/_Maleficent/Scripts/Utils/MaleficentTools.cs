using UnityEngine;
using System.Collections;

public class MaleficentTools : MonoBehaviour {

	public static bool IsDebugBuild {
		get {
#if !UNITY_EDITOR
		return Debug.isDebugBuild;
#else
		return true;
		//return false; //This is the only way to test IsDebugBuild = false on the editor
#endif
		}
	}

	public static Vector3 ConvertPositionBetweenLayers(Vector3 _position, string _currentLayer, string _desiredLayer, float _dist) {
		Camera[] cameras = GameObject.FindObjectsOfType< Camera >();
		Camera currentCamera = null, desiredCamera = null;

		//Search the cameras rendering these layers
		foreach(Camera cam in cameras) {
			if((cam.enabled && (cam.cullingMask & (1 << LayerMask.NameToLayer(_currentLayer))) != 0)) {
				if(currentCamera != null) {
					Debug.LogError("There are two cameras with layer " + _currentLayer);
				}
				
				currentCamera = cam;
			}
			if((cam.cullingMask & (1 << LayerMask.NameToLayer(_desiredLayer))) != 0) {
				if(desiredCamera != null) {
					Debug.LogError("There are two cameras with layer " + _desiredLayer);
				}
				
				desiredCamera = cam;
			}
		}

		Vector3 viewportPos = new Vector3();
		if(currentCamera != null)
			viewportPos = currentCamera.WorldToScreenPoint(_position);
		viewportPos.z = _dist;
		return desiredCamera != null ? desiredCamera.ScreenToWorldPoint(viewportPos): Vector3.zero;
	}

	public static void ChangeLayerRecursively(GameObject _go, string _layer) {
		_go.layer = LayerMask.NameToLayer(_layer);
		foreach(Transform t in _go.transform) {
			ChangeLayerRecursively(t.gameObject, _layer);
		}
	}

	private static IEnumerator DoOnNextFrame(System.Action _action) {
		yield return new WaitForEndOfFrame();
		_action();
	}
	
	public static void DoOnNextFrame(MonoBehaviour _go, System.Action _action) {
		_go.StartCoroutine(DoOnNextFrame(_action));
	}
	
	private static IEnumerator DoAfterSeconds(System.Action _action, float _seconds) {
		yield return new WaitForSeconds(_seconds);
		_action();
	}
	
	public static void DoAfterSeconds(MonoBehaviour _go, float _seconds, System.Action _action) {
		_go.StartCoroutine(DoAfterSeconds(_action, _seconds));
	}
	
	public static void DoAnim(MonoBehaviour _go, Animation _animation, string _animName, System.Action _onFinish) {
		_animation.CrossFadeQueued(_animName);
		DoAfterSeconds(_go, _animation[_animName].length, _onFinish);
	}
}
