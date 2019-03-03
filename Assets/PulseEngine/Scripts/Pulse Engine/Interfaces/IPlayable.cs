using UnityEngine;
using System.Collections;

//namespace PulseEngine {
	
/// <summary>
/// Playable entity.
/// </summary>
public interface IPlayable 
{
	/// <summary>
	/// Play this instance.
	/// </summary>
	void Play();
	
	/// <summary>
	/// Stop this instance.
	/// </summary>
	void Stop();
	
	/// <summary>
	/// Determines whether this instance is playing.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is playing; otherwise, <c>false</c>.
	/// </returns>
	bool IsPlaying();
}
	
//}
