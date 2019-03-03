using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Playable block that can be played and paused or have other playable block children. 
/// Abstract class, extend with specific behavior.
/// </summary>
public abstract class PlayableBlock : Block, IPlayable, IPausable
{
	/// <summary>
	/// The playing child animated block.
	/// </summary>
	protected PlayableBlock playingChild;

	/// <summary>
	/// Tells if this block should continue parsing its children after the playing child has finished.
	/// </summary>
	protected bool continueParse = false;
	
	#region Abstract Methods
	/// <summary>
	/// Plays self. Abstract method, override this with specific behavior.
	/// </summary>
	public abstract void PlaySelf();
	
	/// <summary>
	/// Stops self. Abstract method, override this with specific behavior.
	/// </summary>
	public abstract void StopSelf();
	
	/// <summary>
	/// Pauses self. Abstract method, override this with specific behavior.
	/// </summary>
	public abstract void PauseSelf();
	
	/// <summary>
	/// Determines whether this instance is playing.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is playing; otherwise, <c>false</c>.
	/// </returns>
	public abstract bool IsPlayingSelf();
	
	/// <summary>
	/// Determines whether this instance is paused.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is paused; otherwise, <c>false</c>.
	/// </returns>
	public abstract bool IsPausedSelf();
	#endregion Abstract Methods
	
	public delegate void PlayingFinishedEventHandler(PlayableBlock block);
	
	/// <summary>
	/// Occurs when the block finishes playing.
	/// </summary>
	public event PlayingFinishedEventHandler OnPlayingFinished;
	
	/// <summary>
	/// Plays the block if it doesn't have children, otherwise plays its children based on the parse type.
	/// </summary>
	public void Play()
	{
		if (children.Count == 0) {
			continueParse = false;
			SetActiveBlocks();
			SendEvents("Play");
			
			PlaySelf();
			StartCoroutine("CheckPlaying");
			
			return;
		}
		
		// ParseType.All starts all children at once, while other parse types only start 
		// one children and eventually continue after it has finished playing
		while (PlayNextChild() && parseType == ParseType.All) { }
	}
	
	/// <summary>
	/// Stops the block and all its children.
	/// </summary>
	public void Stop()
	{
		if (children.Count == 0) {
			StopCoroutine("CheckPlaying");
			StopSelf();
			
			return;
		}
		
		foreach (PlayableBlock block in children) 
		{
			block.Stop();
		}
		
		continueParse = false;
	}
	
	/// <summary>
	/// Pauses the block and all its children.
	/// </summary>
	public void Pause()
	{
		if (children.Count == 0) {
			PauseSelf();
			return;
		}
		
		foreach (PlayableBlock block in children) 
		{
			block.Pause();
		}
	}
	
	/// <summary>
	/// Determines whether this instance or its children are playing.
	/// </summary>
	/// <returns>
	/// <c>true</c> if it is playing; otherwise, <c>false</c>.
	/// </returns>
	public bool IsPlaying()
	{
		if (children.Count == 0) {
			return IsPlayingSelf();
		}
		
		foreach (PlayableBlock block in children) 
		{
			if (block.IsPlaying()) {
				return true;
			}
		}
		
		return continueParse;
	}
	
	/// <summary>
	/// Determines whether this instance or its children are paused.
	/// </summary>
	/// <returns>
	/// <c>true</c> if it is paused; otherwise, <c>false</c>.
	/// </returns>
	public bool IsPaused()
	{
		if (children.Count == 0) {
			return IsPausedSelf();
		}
		
		foreach (PlayableBlock block in children) 
		{
			if (block.IsPaused()) {
				return true;
			}
		}
		
		return false;
	}
	
	/// <summary>
	/// Determines whether this instance or its playing child is looped.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is looped; otherwise, <c>false</c>.
	/// </returns>
	public bool IsLooped()
	{
		if (children.Count == 0) {
			return IsPlayingSelf() && IsLoopedSelf();
		}
		
		foreach (PlayableBlock block in children) 
		{
			if (block.IsLooped()) {
				return true;
			}
		}
		
		return false;
	}
	
	/// <summary>
	/// Determines whether this instance is looped. Defaults to false.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is looped self; otherwise, <c>false</c>.
	/// </returns>
	public virtual bool IsLoopedSelf()
	{
		return false;
	}
	
	/// <summary>
	/// Checks if this instance is playing and raises an event when finished.
	/// </summary>
	protected IEnumerator CheckPlaying()
	{
		while (true) {
			yield return null;
			
			if (!IsPlayingSelf() && !IsPausedSelf()) {
				if (Time.timeScale <= 0.0001f) {
					Debug.LogWarning("Check playing in pause: " + name);
				}
				if (OnPlayingFinished != null) {
					OnPlayingFinished(this);
				}
				
				yield break;
			}
		}
	}
	
	/// <summary>
	/// Should be called when the playing child finishes playing.
	/// </summary>
	protected void OnChildPlayingFinished(PlayableBlock block)
	{
		if (continueParse) {
			PlayNextChild();
		} else {
			if (OnPlayingFinished != null) {
				OnPlayingFinished(this);
			}
			SendEvents("OnPlayingFinished");
		}
	}
	
	/// <summary>
	/// Plays the next child.
	/// </summary>
	/// <returns>
	/// <c>true</c> if there are more children to be played, otherwise <c>false</c>.
	/// </returns>
	public bool PlayNextChild()
	{
		SetActiveBlocks();
		
		continueParse = Parse<PlayableBlock>(out playingChild);
		playingChild.Play();
		if (parseType != ParseType.All || !continueParse) {
			playingChild.OnPlayingFinished += OnChildPlayingFinished;
		}
		
		return continueParse;
	}
}

//}
