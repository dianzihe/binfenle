using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Pausable entity.
/// </summary>
public interface IPausable
{
	/// <summary>
	/// Pause this instance.
	/// </summary>
	void Pause();
	
	/// <summary>
	/// Determines whether this instance is paused.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is paused; otherwise, <c>false</c>.
	/// </returns> 
	bool IsPaused();
}

//}
