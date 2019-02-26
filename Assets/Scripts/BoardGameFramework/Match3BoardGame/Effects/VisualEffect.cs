using UnityEngine;
using System.Collections;

//WIP: Controller for other visual only effects that doesn't destroy anything.
public abstract class VisualEffect : MonoBehaviour {
	[System.NonSerialized]
	public Transform cachedTransform;
		
	protected virtual void Awake() {
		cachedTransform = transform;
	}
	
	protected virtual void Start() {
	}
		
	public virtual void InitComponent() {
	}
}
