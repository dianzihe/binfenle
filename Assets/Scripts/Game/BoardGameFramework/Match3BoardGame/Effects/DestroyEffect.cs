using UnityEngine;
using System.Collections;

public class DestroyEffect : MonoBehaviour
{
	public float lifeTime;
	public float destroyTileTime;
	
	[System.NonSerialized]
	public System.Action<DestroyEffect> OnEffectFinished;

	[System.NonSerialized]
	public Transform cachedTransform;
	
	protected virtual void Awake() {
		cachedTransform = transform;
	}
	
	protected virtual void Start() { }
	
	public virtual void InitComponent() { }
}

