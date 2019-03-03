using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Text block that may contain a Text Mesh component or have other text block children.
/// </summary>
public class TextBlock : PlayableBlock
{
	public const float DEFAULT_ANIMATION_TIME = -1f;
	
	public string animFadeIn = "Text_Fade_In";
	public string animFadeOut = "Text_Fade_Out";
	
	/// <summary>
	/// The current fade animation to play, is playing or has played.
	/// </summary>
	protected string currentAnimation;
	
	/// <summary>
	/// The animation speed before pausing it. Used for resuming when calling PlaySelf().
	/// </summary>
	protected float animSpeedBeforePause = 1f;
	
	[System.NonSerialized]
	public AnimationWrapper cachedAnimWrapper;
	[System.NonSerialized]
	public RenderWrapper cachedRenderWrapper;
	[System.NonSerialized]
	public Component cachedAnimation;
	[System.NonSerialized]
	public Component cachedRender;
		
	/// <summary>
	/// Unity method. 
	/// Awake this instance.
	/// </summary>
	public override void Awake()
	{
		base.Awake();
		
		if (children.Count == 0) {
			cachedAnimWrapper = AnimationWrapper.Instance;
			cachedAnimation = cachedAnimWrapper.GetAnimationComponent(gameObject);
			
			cachedRenderWrapper = RenderWrapper.Instance;
			cachedRender = cachedRenderWrapper.GetRenderComponent(gameObject);
			
			if (cachedRender) {
				cachedRenderWrapper.ChangeAlpha(cachedRender, 0f);
			}
			else {
				Debug.LogWarning("TextBlock: " + name + " doesn't have a renderer!");
			}
		}
	}
	
	/// <summary>
	/// Fades in this text block or its children based on the parse type.
	/// </summary>
	/// <param name='animationTime'>
	/// Animation time. If negative, use the default animation time.
	/// </param>
	public void FadeIn(float animationTime = DEFAULT_ANIMATION_TIME)
	{
		currentAnimation = animFadeIn;
		SendEvents("FadeIn");

		if (children.Count == 0) {
			SetActiveBlocks();
			PlayFadeAnimation(animationTime);
			return;
		}
		
		// ParseType.All starts all children at once, while other parse types only start 
		// one children and eventually continue after it has finished playing
		while (FadeInNextChild(animationTime) && parseType == ParseType.All) { }
	}
	
	/// <summary>
	/// Fades out this text block and all its children.
	/// </summary>
	/// <param name='animationTime'>
	/// Animation time. If negative, use the default animation time.
	/// </param>
	public void FadeOut(float animationTime = DEFAULT_ANIMATION_TIME)
	{
		currentAnimation = animFadeOut;
		SendEvents("FadeOut");

		if (children.Count == 0) {
			SetActiveBlocks();
			PlayFadeAnimation(animationTime);
			return;
		}
		
		foreach (TextBlock block in children) 
		{
			block.FadeOut(animationTime);
		}
	}
	
	/// <summary>
	/// Fades the next child.
	/// </summary>
	/// <returns>
	/// <c>true</c> if there are more children to be faded, otherwise <c>false</c>.
	/// </returns>
	public bool FadeInNextChild(float animationTime)
	{
		SetActiveBlocks();
		
		continueParse = Parse<PlayableBlock>(out playingChild);
		(playingChild as TextBlock).FadeIn(animationTime);
		if (parseType != ParseType.All || !continueParse) {
			playingChild.OnPlayingFinished += OnChildPlayingFinished;
		}
		
		return continueParse;
	}
		
	/// <summary>
	/// Plays the current fade animation with the specified time.
	/// </summary>
	/// <param name='animationTime'>
	/// Animation time.
	/// </param>
	protected void PlayFadeAnimation(float animationTime)
	{
		if (cachedAnimation != null) {
			if ((currentAnimation == animFadeIn && cachedRenderWrapper.GetAlpha(cachedRender) < 0.1f) ||
				(currentAnimation == animFadeOut && cachedRenderWrapper.GetAlpha(cachedRender) > 0.9f))
			{
				SetSpeedBasedOnDuration(currentAnimation, animationTime);
				cachedAnimWrapper.PlayAnimation(cachedAnimation, currentAnimation);
			}
		}
		
		StartCoroutine("CheckPlaying");
	}
	
	/// <summary>
	/// Sets the speed of the given animation based on the time you want it to last.
	/// </summary>
	/// <param name='animationName'>
	/// Animation name.
	/// </param>
	/// <param name='animationTime'>
	/// Animation time. If negative, use the default animation time.
	/// </param>
	protected void SetSpeedBasedOnDuration(string animationName, float animationTime)
	{
		if (animationTime > 0f) {
			cachedAnimWrapper.SetAnimationSpeed(cachedAnimation, animationName, 
				cachedAnimWrapper.GetAnimationLength(cachedAnimation, animationName) / animationTime);
		} 
		else if (animationTime == 0f) {
			cachedAnimWrapper.SetAnimationSpeed(cachedAnimation, animationName, 
				cachedAnimWrapper.GetAnimationLength(cachedAnimation, animationName) * 10000f);
		} 
		else {
			cachedAnimWrapper.SetAnimationSpeed(cachedAnimation, animationName, 1f);
		}
	}
	
	/// <summary>
	/// Resumes self, shouldn't be called for another reason on a text block.
	/// </summary>
	public override void PlaySelf()
	{
		if (IsPausedSelf()) {
			cachedAnimWrapper.PlayAnimation(cachedAnimation, currentAnimation, animSpeedBeforePause);
		}
	}
	
	/// <summary>
	/// Stops the current fade animation.
	/// </summary>
	public override void StopSelf()
	{
		if (cachedAnimation) {
			cachedAnimWrapper.StopAnimation(cachedAnimation);
		}
	}
	
	/// <summary>
	/// Pauses the current fade animation.
	/// </summary>
	public override void PauseSelf()
	{
		if (cachedAnimation) {
			animSpeedBeforePause = cachedAnimWrapper.PauseAnimation(cachedAnimation);
		} 
	}
	
	/// <summary>
	/// Determines whether this instance is playing.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is playing; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsPlayingSelf()
	{
		return cachedAnimation != null && cachedAnimWrapper.IsAnimationPlaying(cachedAnimation);
	}
	
	/// <summary>
	/// Determines whether this instance is paused.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is paused; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsPausedSelf()
	{
		return cachedAnimation != null && cachedAnimWrapper.IsAnimationPaused(cachedAnimation);
	}
}

//}
