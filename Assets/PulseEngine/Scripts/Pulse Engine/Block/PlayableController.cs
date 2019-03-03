using UnityEngine;
using System.Collections;

/// <summary>
/// Playable controller interface. 
/// Tells if a controller has any blocks playing and raises an event when all blocks finish playing.
/// </summary>
public interface PlayableController
{
	/// <summary>
	/// Occurs when the controlled blocks finish playing.
	/// </summary>
	event PlayableControllerDelegates.PlayingFinishedEventHandler OnPlayingFinished;
	
	/// <summary>
	/// Determines whether this instance has any blocks playing.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance has any blocks playing; otherwise, <c>false</c>.
	/// </returns>
	bool IsPlaying();
}

public class PlayableControllerDelegates
{
	public delegate void PlayingFinishedEventHandler(PlayableController playableController);
}

