using UnityEngine;
using System.Collections;

public class UICameraInputEnabler : MonoBehaviour 
{
	UICamera myCamera;
	LayerMask originalLayerMask;
	
	public bool InputEnabled {
		get {
			return myCamera.eventReceiverMask != 0;
		}
		set {
			if (value) {
				myCamera.eventReceiverMask = originalLayerMask;
			}
			else {
				originalLayerMask = myCamera.eventReceiverMask;
				myCamera.eventReceiverMask = 0;
			}
		}
	}
	
	void Awake()
	{
		myCamera = GetComponent<UICamera>();
		originalLayerMask = myCamera.eventReceiverMask;
	}
}
