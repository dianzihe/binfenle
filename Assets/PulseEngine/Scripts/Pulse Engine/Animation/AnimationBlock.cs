using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Animation block that can animate a source or have other animation block children.
/// </summary>
public class AnimationBlock : PlayableBlock
{
	/// <summary>
	/// The animation file. It will be played by this block on the animation source.
	/// </summary>
	public AnimationClip animationFile;
	
	/// <summary>
	/// The start delay for this animation. When Play() is called, the animation start is delayed with this amount.
	/// </summary>
	public float startDelay = 0f;
	
	/// <summary>
	/// The animation source that will play this animation block.
	/// </summary>
	public GameObject animationSource;
	
	[System.NonSerialized]
	public AnimationWrapper cachedAnimWrapper;
	[System.NonSerialized]
	public Component cachedAnimation;
	
	/// <summary>
	/// The animation speed before pausing it. Used for resuming when calling PlaySelf().
	/// </summary>
	protected float animSpeedBeforePause = 1f;
		
	/// <summary>
	/// Unity method.
	/// Awake this instance.
	/// </summary>
	public override void Awake()
	{
		base.Awake();
		
		cachedAnimWrapper = AnimationWrapper.Instance;
		if (animationSource) {
			cachedAnimation = cachedAnimWrapper.GetAnimationComponent(animationSource);
		}
	}
	
	/// <summary>
	/// Plays the animation on self.
	/// </summary>
	public override void PlaySelf()
	{
		if (animationFile != null && cachedAnimation != null) {
			if (startDelay <= 0f) {
				PlayAnimation();
			} else {
				Invoke("PlayAnimation", startDelay);
			}
		}
	}
	
	/// <summary>
	/// Stops the animation on self.
	/// </summary>
	public override void StopSelf()
	{
		if (cachedAnimation != null) {
			CancelInvoke("PlayAnimation");
			cachedAnimWrapper.StopAnimation(cachedAnimation);
			animSpeedBeforePause = 1f;
		}
	}
	
	/// <summary>
	/// Pauses the animation on self.
	/// </summary>
	public override void PauseSelf()
	{
		if (cachedAnimation != null) {
			CancelInvoke("PlayAnimation");
			animSpeedBeforePause = cachedAnimWrapper.PauseAnimation(cachedAnimation);
		}
	}
	
	/// <summary>
	/// Determines whether this instance is playing.
	/// </summary>
	/// <returns>
	/// <c>true</c> if it is playing; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsPlayingSelf()
	{
		if (cachedAnimation != null) {
			return IsInvoking("PlayAnimation") || cachedAnimWrapper.IsAnimationPlaying(cachedAnimation);
		}
		return false;
	}
	
	/// <summary>
	/// Determines whether this instance is paused.
	/// </summary>
	/// <returns>
	/// <c>true</c> if it is paused; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsPausedSelf()
	{
		if (cachedAnimation != null) {
			return !IsInvoking("PlayAnimation") && cachedAnimWrapper.IsAnimationPaused(cachedAnimation);
		}
		return false;
	}
	
	/// <summary>
	/// Determines whether this instance is looped, based on its animation type.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is looped self; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsLoopedSelf()
	{
		if (cachedAnimation != null) {
			cachedAnimWrapper.IsAnimationLooped(cachedAnimation);
		}
		
		return false;
	}
	
	/// <summary>
	/// Actually plays the animation file on the animation source. Invoke this method with the start delay if needed.
	/// </summary>
	protected void PlayAnimation()
	{
		cachedAnimWrapper.PlayAnimation(cachedAnimation, animationFile.name, animSpeedBeforePause);
	}
}

//}
