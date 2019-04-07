using UnityEngine;
using System.Collections;

public class CustomAnimationEvents : MonoBehaviour {
	
	[System.NonSerialized]
	public Transform cachedTransform;
	private Transform child;
	
	void Awake() {
		cachedTransform = transform;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	void SetActive(bool active){
		gameObject.SetActive(active);
		
	}
	void EnableChild(string childName){
		child = cachedTransform.Find(childName);
		if (child == null) {
			Debug.LogError(childName + " not found!!!");
		}
		child.gameObject.SetActive(true);
	}
	
	void DisableChild(string childName) {
		child = cachedTransform.Find(childName);
		if (child == null) {
			Debug.LogError(childName + " not found!!!");
		}
		child.gameObject.SetActive(false);
	}
}
