using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Sound wrapper that handles the communication between the engine core and the used sound library.
/// This class uses Unity's built-in audio components. Extend and override the instance to use a custom sound library.
/// </summary>
public class SoundWrapper
{
	/// <summary>
	/// The singleton instance.
	/// </summary>
	protected static SoundWrapper instance = null;
	
	/// <summary>
	/// Gets the singleton instance for this class.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static SoundWrapper Instance {
		get {
			if (instance == null) {
				instance = new SoundWrapper();
			}
			
			return instance;
		}
	}
	
	/// <summary>
	/// Gets the sound component from the given game object. The wrapper is the only one who knows what component it needs to get. 
	/// This function is primarily used to cache the result on the caller.
	/// </summary>
	/// <returns>
	/// The sound component.
	/// </returns>
	/// <param name='gameObj'>
	/// The game object from which to get the sound component.
	/// </param>
	public virtual Component GetSoundComponent(GameObject gameObj)
	{
		return gameObj.GetComponent<AudioSource>();
	}
	
	/// <summary>
	/// Plays the sound from the given component with the given delay.
	/// </summary>
	/// <param name='soundComponent'>
	/// Sound component.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	public virtual void PlaySound(Component soundComponent, float delay = 0f)
	{
		(soundComponent as AudioSource).Play((ulong)(44100 * delay));
	}
	
	/// <summary>
	/// Plays the sound file with the given delay, loading it on the given component.
	/// </summary>
	/// <param name='soundComponent'>
	/// Sound component.
	/// </param>
	/// <param name='fileName'>
	/// File name.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	public virtual void PlaySoundFile(Component soundComponent, string fileName, float delay = 0f)
	{
		(soundComponent as AudioSource).clip = Resources.Load(fileName, typeof(AudioClip)) as AudioClip;
		(soundComponent as AudioSource).Play((ulong)(44100 * delay));
	}
	
	/// <summary>
	/// Stops the sound from the given component.
	/// </summary>
	/// <param name='soundComponent'>
	/// Sound component.
	/// </param>
	public virtual void StopSound(Component soundComponent)
	{
		(soundComponent as AudioSource).Stop();
	}
	
	/// <summary>
	/// Pauses the sound from the given component.
	/// </summary>
	/// <param name='soundComponent'>
	/// Sound component.
	/// </param>
	public virtual void PauseSound(Component soundComponent)
	{
		if (IsPlaying(soundComponent)) {
			(soundComponent as AudioSource).Pause();
		}
	}
	
	/// <summary>
	/// Determines whether the sound on the specified component is playing.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the sound is playing; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='soundComponent'>
	/// The sound component.
	/// </param>
	public virtual bool IsPlaying(Component soundComponent)
	{
		return (soundComponent as AudioSource).isPlaying;
	}
	
	/// <summary>
	/// Determines whether the sound on the specified component is paused.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the sound is paused; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='soundComponent'>
	/// The sound component.
	/// </param>
	public virtual bool IsPaused(Component soundComponent)
	{
		return !IsPlaying(soundComponent) && (soundComponent as AudioSource).timeSamples > 0;
	}
	
	/// <summary>
	/// Gets the current time for the sound on the given component.
	/// </summary>
	/// <returns>
	/// The current time.
	/// </returns>
	/// <param name='soundComponent'>
	/// Sound component.
	/// </param>
	public virtual float GetCurrentTime(Component soundComponent)
	{
		AudioSource audio = (soundComponent as AudioSource);
		return (audio.clip == null ? 0f : (float)audio.timeSamples / (float)audio.clip.frequency);
	}
	
	/// <summary>
	/// Sets the current time on the given sound component, if it has a sound loaded.
	/// </summary>
	/// <param name='soundComponent'>
	/// Sound component.
	/// </param>
	/// <param name='newTime'>
	/// New time.
	/// </param>
	public virtual void SetCurrentTime(Component soundComponent, float newTime)
	{
		AudioSource audio = (soundComponent as AudioSource);
		if (audio.clip != null) {
			audio.timeSamples = (int)(newTime * audio.clip.frequency);
		}
	}
	
	/// <summary>
	/// Gets the time length of the sound on the given component.
	/// </summary>
	/// <returns>
	/// The time length.
	/// </returns>
	/// <param name='soundComponent'>
	/// Sound component.
	/// </param>
	public virtual float GetTimeLength(Component soundComponent)
	{
		AudioSource audio = (soundComponent as AudioSource);
		return audio.clip == null ? 0f : (float)audio.timeSamples / (float)audio.clip.frequency;
	}
}

//}
