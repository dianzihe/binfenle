using UnityEngine;
using System.Collections;

public class DeviceInput
{
	/// <summary>
	/// TouchPhase doesn't include Inactive state, so I have created a new Enum and added it
	/// </summary>
	public enum Phase {
	 	Began,
	 	Moved,
	 	Stationary,
	 	Ended,
	 	Canceled,
		Inactive
	}
	
	/// <summary>
	///Touch input ID 
	/// </summary>
	public int fingerId;
	
	/// <summary>
	///Touch input Position in screen space.
	/// </summary>
	public Vector2 position;
	
	/// <summary>
	///Touch input Movement 
	/// </summary>
	public Vector2 deltaPosition;
	
	/// <summary>
	///Touch input state 
	/// </summary>
	public Phase phase = Phase.Inactive;
}
