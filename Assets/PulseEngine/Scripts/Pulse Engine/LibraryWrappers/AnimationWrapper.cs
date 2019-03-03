using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Animation wrapper that handles the communication between the engine core and the used animation library.
/// This class uses Unity's built-in animation components. Extend and override the instance to use a custom animation library.
/// </summary>
public class AnimationWrapper
{
	/// <summary>
	/// The singleton instance.
	/// </summary>
	protected static AnimationWrapper instance = null;
	
	/// <summary>
	/// Gets the singleton instance for this class.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static AnimationWrapper Instance {
		get {
			if (instance == null) {
				instance = new AnimationWrapper();
			}
			
			return instance;
		}
	}
	
	/// <summary>
	/// Gets the animation component from the given game object. The wrapper is the only one who knows what component it needs to get. 
	/// This function is primarily used to cache the result on the caller.
	/// </summary>
	/// <returns>
	/// The animation component.
	/// </returns>
	/// <param name='gameObj'>
	/// The game object from which to get the animation component.
	/// </param>
	public virtual Component GetAnimationComponent(GameObject gameObj)
	{
		return gameObj.GetComponent<Animation>();
	}
	
	/// <summary>
	/// Plays the animation with the give name on the given component. If it is paused, it resumes it.
	/// </summary>
	/// <param name='animationComponent'>
	/// Animation component.
	/// </param>
	/// <param name='animationName'>
	/// Animation name.
	/// </param>
	/// <param name='speedBeforePause'>
	/// If the animation is paused, this speed will be set for it before it is resumed.
	/// </param>
	public virtual void PlayAnimation(Component animationComponent, string animationName, float speedBeforePause = 1f)
	{
		Animation anim = (animationComponent as Animation);
		
		if (anim.IsPlaying(animationName) && anim[animationName].normalizedSpeed == 0f) {
			anim[animationName].normalizedSpeed = speedBeforePause;
		}
		
		anim.Play(animationName);
	}
	
	/// <summary>
	/// Gets the speed of the animation with the given name from the given component.
	/// </summary>
	/// <returns>
	/// The animation speed.
	/// </returns>
	/// <param name='animationComponent'>
	/// Animation component.
	/// </param>
	/// <param name='animationName'>
	/// Animation name.
	/// </param>
	public virtual float GetAnimationSpeed(Component animationComponent, string animationName)
	{
		return (animationComponent as Animation)[animationName].normalizedSpeed;
	}
	
	/// <summary>
	/// Sets the speed of the animation with the given name from the given component.
	/// </summary>
	/// <param name='animationComponent'>
	/// Animation component.
	/// </param>
	/// <param name='animationName'>
	/// Animation name.
	/// </param>
	/// <param name='animationSpeed'>
	/// Animation speed.
	/// </param>
	public virtual void SetAnimationSpeed(Component animationComponent, string animationName, float animationSpeed)
	{
		(animationComponent as Animation)[animationName].normalizedSpeed = animationSpeed;
	}
	
	/// <summary>
	/// Gets the length of the animation with the given name from the given component.
	/// </summary>
	/// <returns>
	/// The animation length.
	/// </returns>
	/// <param name='animationComponent'>
	/// Animation component.
	/// </param>
	/// <param name='animationName'>
	/// Animation name.
	/// </param>
	/// <param name='ignoreSpeed'>
	/// Tells if the animation speed should be ignored when computing the length of the animation. Defaults to <c>true</c>.
	/// </param>
	public virtual float GetAnimationLength(Component animationComponent, string animationName, bool ignoreSpeed = true)
	{
		if (ignoreSpeed) {
			return (animationComponent as Animation)[animationName].length;
		}
		else {
			AnimationState anim = (animationComponent as Animation)[animationName];
			return anim.length * anim.normalizedSpeed;
		}
	}
	
	/// <summary>
	/// Stop any animation on the given component.
	/// </summary>
	/// <param name='animationComponent'>
	/// Animation component.
	/// </param>
	public virtual void StopAnimation(Component animationComponent)
	{
		(animationComponent as Animation).Stop();
	}
	
	/// <summary>
	/// Pauses the animation on the given component.
	/// </summary>
	/// <returns>
	/// The animation speed before pause.
	/// </returns>
	/// <param name='animationComponent'>
	/// Animation component.
	/// </param>
	public virtual float PauseAnimation(Component animationComponent)
	{
		Animation anim = (animationComponent as Animation);
		float speed = 1f;
		
		if (anim.isPlaying) {
			speed = anim[anim.clip.name].normalizedSpeed;
			anim[anim.clip.name].normalizedSpeed = 0f;
		}
		
		return speed;
	}
	
	/// <summary>
	/// Determines whether there is an animation playing on the specified animation component.
	/// </summary>
	/// <returns>
	/// <c>true</c> if there is an animation playing on the specified animation component; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='animationComponent'>
	/// Animation component.
	/// </param>
	public virtual bool IsAnimationPlaying(Component animationComponent)
	{
		return (animationComponent as Animation).isPlaying;
	}
	
	/// <summary>
	/// Determines whether the current animation on the specified animation component is paused.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the current animation on the specified animation component is paused; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='animationComponent'>
	/// Animation component.
	/// </param>
	public virtual bool IsAnimationPaused(Component animationComponent)
	{
		Animation anim = (animationComponent as Animation);
		return anim.isPlaying && anim[anim.clip.name].normalizedSpeed == 0f;
	}
	
	/// <summary>
	/// Determines whether the current animation on the specified animation component is looped or runs forever.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the current animation on the specified animation component is looped or runs forever; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='animationComponent'>
	/// If set to <c>true</c> animation component.
	/// </param>
	public virtual bool IsAnimationLooped(Component animationComponent)
	{
		Animation anim = (animationComponent as Animation);
		if (anim.clip != null) {
			WrapMode wrapMode = anim.clip.wrapMode;
			return wrapMode == WrapMode.Loop || wrapMode == WrapMode.ClampForever || wrapMode == WrapMode.PingPong;
		}
		
		return false;
	}
}

//}
