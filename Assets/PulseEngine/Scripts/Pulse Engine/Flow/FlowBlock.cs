using UnityEngine;
using System.Collections;

/// <summary>
/// Flow block. Has an intro, idle and outro state, each with its corresponding animation block.
/// </summary>
public class FlowBlock : PlayableBlock
{
	/// <summary>
	/// The intro animation. It will be set as the current animation when the object awakes or the State changes to Intro.
	/// </summary>
	public AnimationBlock introAnimation;
	
	/// <summary>
	/// The idle animation. It will be set as the current animation when the State changes to Idle.
	/// </summary>
	public AnimationBlock idleAnimation;
	
	/// <summary>
	/// The outro animation. It will be set as the current animation the State changes to Outro.
	/// </summary>
	public AnimationBlock outroAnimation;
	
	/// <summary>
	/// The states that a flow block can have.
	/// </summary>
	public enum FlowState : int {
		Intro,
		Idle,
		Outro,
	}
	
	/// <summary>
	/// The current animation.
	/// </summary>
	protected AnimationBlock currentAnimation;
	
	/// <summary>
	/// The state of this flow block.
	/// </summary>
	protected FlowState state;
	
	/// <summary>
	/// Gets or sets the state for this flow block.
	/// </summary>
	/// <value>
	/// The state.
	/// </value>
	public FlowState State
	{
		get {
			return state;
		}
		set {
			state = value;
			if (state == FlowState.Intro) {
				currentAnimation = introAnimation;
			}
			else if (state == FlowState.Idle) {
				currentAnimation = idleAnimation;
			}
			else if (state == FlowState.Outro) {
				currentAnimation = outroAnimation;
			}
		}
	}
	
	/// <summary>
	/// Unity method.
	/// Awake this instance.
	/// </summary>
	public override void Awake ()
	{
		base.Awake ();
		
		State = FlowState.Intro;
	}
	
	/// <summary>
	/// Plays this flow block, by playing its current animation.
	/// </summary>
	public override void PlaySelf()
	{
		if (currentAnimation) {
			currentAnimation.Play();
		}
		OnPlayingFinished += OnAnimationFinished;
	}
	
	/// <summary>
	/// Stops this flow block, by stopping its current animation.
	/// </summary>
	public override void StopSelf()
	{
		if (currentAnimation) {
			currentAnimation.Stop();
		}
	}
	
	/// <summary>
	/// Pauses this flow block, by pausing its current animation.
	/// </summary>
	public override void PauseSelf()
	{
		if (currentAnimation) {
			currentAnimation.Pause();
		}
	}
	
	/// <summary>
	/// Determines whether this instance is playing, by checking if the current animation is playing.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is playing; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsPlayingSelf()
	{
		return currentAnimation != null && currentAnimation.IsPlaying();
	}
	
	/// <summary>
	/// Determines whether this instance is paused, by checking if the current animation is paused.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is paused; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsPausedSelf()
	{
		return currentAnimation != null && currentAnimation.IsPaused();
	}
	
	/// <summary>
	/// Called when the current animation finishes playing.
	/// </summary>
	public void OnAnimationFinished(PlayableBlock block)
	{
		// TODO: Remove if not useful
	}
	
	/// <summary>
	/// Advances the flow by going to the next state.
	/// </summary>
	public void AdvanceFlow()
	{
		if (State == FlowState.Intro) {
			State = FlowState.Idle;
		} else if (State == FlowState.Idle) {
			State = FlowState.Outro;
		}
	}
	
	/// <summary>
	/// Plays the animation for the new state.
	/// </summary>
	/// <param name='newState'>
	/// New state.
	/// </param>
	public virtual void PlayState(FlowState newState)
	{
		State = newState;
		Play();
	}
}

