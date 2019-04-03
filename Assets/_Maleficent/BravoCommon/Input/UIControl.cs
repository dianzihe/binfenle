using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIControl : MonoBehaviour
{
	//touches sorted in the order they were pressed
	[HideInInspector]public List< int >touchesIds  = new List< int >(InputHandler.MAX_TOUCHES);
	
	//For movement the collision plane where we are moving and the relativeHitPosition
	[HideInInspector]public Plane collisionPlane = new Plane(new Vector3(), 0.0f);
	[HideInInspector]public Vector3 relativeHitPosition;
	
	void Start()
	{
		if(gameObject.GetComponent< Collider >() == null)
			Debug.LogWarning("No collider found for " + gameObject.name);
	}
	
#region Methods to Overload
	/// <summary>
	/// Method called by device Touch Down input
	/// </summary>
	public virtual void OnTouchDown(BravoInputManager.TouchCollision _collisionInfo) {}
	
	/// <summary>
	/// Method called by device Touch Moved input
	/// </summary>
	public virtual void OnTouchMoved(BravoInputManager.TouchCollision _collisionInfo) {}
	
	/// <summary>
	/// Method called by device Touch Up input
	/// </summary>
	public virtual void OnTouchUp(BravoInputManager.TouchCollision _collisionInfo){}
	
	/// <summary>
	/// Method called by device Touch Cancelled input
	/// </summary>
	public virtual void OnTouchCancelled() {}
#endregion
}
