using UnityEngine;
using System.Collections;

//namespace PulseEngine {
	
/// <summary>
/// Timeline entity that can point to and modify its current time and has a total time length.
/// </summary>
public interface ITimeline
{
	/// <summary>
	/// Gets or sets the current time.
	/// </summary>
	/// <value>
	/// The current time.
	/// </value>
	float CurrentTime 
	{
		get;
		set;
	}
	
	/// <summary>
	/// The time length of this entity.
	/// </summary>
	/// <returns>
	/// The time length.
	/// </returns>
	float TimeLength();
}

//}
