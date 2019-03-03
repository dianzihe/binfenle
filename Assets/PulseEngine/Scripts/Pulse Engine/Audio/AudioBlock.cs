using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Audio block that may contain an audio file or have other audio block children.
/// </summary>
public class AudioBlock : PlayableBlock, ITimeline
{
	/// <summary>
	/// The audio file for this block.
	/// </summary>
	public string audioFile;
		
	/// <summary>
	/// The start delay for this audio. When Play() is called, the sound playing is delayed with this amount.
	/// </summary>
	public float startDelay = 0.5f;
	
	/// <summary>
	/// The audio source that will play this audio block.
	/// </summary>
	public GameObject audioSource;
	
	[System.NonSerialized]
	public SoundWrapper cachedSoundWrapper;
	[System.NonSerialized]
	public Component cachedAudio;
		
	/// <summary>
	/// Gets or sets the current time. Works directly on the cached audio source.
	/// </summary>
	/// <value>
	/// The current time.
	/// </value>
	public float CurrentTime {
		get {
			if (cachedAudio != null) {
				return cachedSoundWrapper.GetCurrentTime(cachedAudio);
			}
			else {
				return 0f;
			}
		}
		set {
			if (cachedAudio != null) {
				cachedSoundWrapper.SetCurrentTime(cachedAudio, value);
			}
			else {
				return;
			}
		}
	}
	
	/// <summary>
	/// Unity method.
	/// Awake this instance.
	/// </summary>
	public override void Awake()
	{
		base.Awake();
		
		cachedSoundWrapper = SoundWrapper.Instance;
		if (audioSource) {
			cachedAudio = cachedSoundWrapper.GetSoundComponent(audioSource);
		}
	}
	
	/// <summary>
	/// Plays the audio on this audio block and registers the narration setting changed notifier.
	/// </summary>
	public override void PlaySelf()
	{
		PlaySelfAudio(true);
	}
	
	/// <summary>
	///  Stops the audio on this audio block and unregisters the music setting changed notifier. 
	/// </summary>
	public override void StopSelf()
	{
		StopSelfAudio(true);
	}	
	
	/// <summary>
	/// Pauses the audio on this audio block.
	/// </summary>
	public override void PauseSelf()
	{
		if (cachedAudio != null) {
			cachedSoundWrapper.PauseSound(cachedAudio);
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
		if (cachedAudio != null) {
			return cachedSoundWrapper.IsPlaying(cachedAudio);
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
		if (cachedAudio != null) {
			return cachedSoundWrapper.IsPaused(cachedAudio);
		}
		
		return false;
	}
	
	/// <summary>
	/// The times length of the sound loaded on the cached audio source of this audio block.
	/// </summary>
	/// <returns>
	/// The time length.
	/// </returns>
	public float TimeLength()
	{
		if (cachedAudio) {
			return cachedSoundWrapper.GetTimeLength(cachedAudio);
		}
		
		return 0f;
	}
	
	/// <summary>
	/// Plays the audio on this audio block. If register listener is <c>true</c> it also registers the corresponding setting changed notifier.
	/// </summary>
	/// <param name='registerListener'>
	/// Register listener.
	/// </param>
	public void PlaySelfAudio(bool registerListener)
	{
		if (audioFile != null && cachedAudio != null) {
			if (registerListener) {
				RegisterSettingChanged();
			}
			
			if (IsSettingOn()) {
				cachedSoundWrapper.PlaySoundFile(cachedAudio, audioFile, startDelay);
			}
		}
	}
	
	/// <summary>
	/// Stops the audio on this audio block. If unregister listener is <c>true</c> it also unregisters the corresponding setting changed notifier.
	/// </summary>
	/// <param name='unregisterListener'>
	/// Unregister listener.
	/// </param>
	public void StopSelfAudio(bool unregisterListener)
	{
		if (cachedAudio != null) {
			if (unregisterListener) {
				UnregisterSettingChanged();
			}
			
			cachedSoundWrapper.StopSound(cachedAudio);
		}
	}
	
	/// <summary>
	/// Registers the sound setting changed notifier.
	/// </summary>
	protected virtual void RegisterSettingChanged()
	{
		GenericSettings.Instance.OnSoundChanged += OnSettingChanged;
	}
	
	/// <summary>
	/// Unregisters the sound setting changed notifier.
	/// </summary>
	protected virtual void UnregisterSettingChanged()
	{
		GenericSettings.Instance.OnSoundChanged -= OnSettingChanged;
	}
	
	/// <summary>
	/// Determines if the sound setting is on.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the sound setting is on, otherwise <c>false</c>.
	/// </returns>
	protected virtual bool IsSettingOn()
	{
		return GenericSettings.Instance.Sound;
	}
	
	/// <summary>
	/// Raised when the corresponding audio setting changes.
	/// </summary>
	/// <param name='musicOn'>
	/// Is music on.
	/// </param>
	public virtual void OnSettingChanged(bool settingOn)
	{
		if (!settingOn && IsPlayingSelf()) {
			StopSelfAudio(false);
		}
	}
}

//}
