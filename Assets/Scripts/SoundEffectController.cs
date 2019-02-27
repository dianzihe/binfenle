using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEffectController : MonoBehaviour 
{	
	/// <summary>
	/// The number of play counts this sound effect has received in the current frame.
	/// At the end of the current frame we can play it with certain limits or delays.
	/// </summary>
	public int playPendingCount = 0;
	
	public SoundEffect sound;
	
	protected List<AudioSource> audioSourceBuffer;
		
	/// <summary>
	/// The index of the current available audio source on which the next sound play can be started.
	/// </summary>
	protected int audioSrcIdx = 0;
	
	protected float lastTimePlayed;
	protected WaitForEndOfFrame waitEndFrame;
	protected bool waitingForLateUpdate = false;


	void Awake()
	{
		waitEndFrame = new WaitForEndOfFrame();
	}

	// Use this for initialization
	void Start () { }
	
	public string Id 
	{
		get {
			return sound.id;
		}
		
		protected set {
			sound.id = value;
		}
	}
	
	public AudioClip SoundClip
	{
		get {
			return sound.audioClip;
		}
		set {
			for(int i = 0; i < audioSourceBuffer.Count; i++) {
				audioSourceBuffer[i].clip = value;
			}
		}
	}
	
	public AudioSource AudioSrc 
	{
		get {
			return audioSourceBuffer[0];
		}
	}
	
	public bool IsPlaying
	{
		get {
			return AudioSrc.isPlaying;
		}
	}
		
	public void InitComponent(SoundEffect sndEffect)
	{
		sound = sndEffect;
		
		Id = sndEffect.id;
		audioSourceBuffer = new List<AudioSource>(sound.maxAudioSources);
		
		// Create and setup corresponding AudioSource buffers list for this sound.
		for(int i = 0; i < sound.maxAudioSources; i++) {
			audioSourceBuffer.Add( CreateAudioSource(sound) );
		}
	}
	
	protected AudioSource CreateAudioSource(SoundEffect sndEffect)
	{
		AudioSource audioSrc = gameObject.AddComponent<AudioSource>();
		
		audioSrc.loop = sndEffect.loop;
		audioSrc.priority = sndEffect.priority;
		audioSrc.volume = sndEffect.volume;
		audioSrc.pitch = sndEffect.pitch;
		audioSrc.playOnAwake = false;
		
		audioSrc.clip = sndEffect.audioClip;
		
		return audioSrc;
	}
		
	protected void TryNewRandomClip()
	{
		//TODO: quick hack to support random sound effects
		if (sound.randomAudioClips != null && sound.randomAudioClips.Length > 0) {
			audioSourceBuffer[audioSrcIdx].clip = sound.randomAudioClips[Random.Range(0, sound.randomAudioClips.Length)];
//			Debug.LogWarning("playing random snd: " + audioSourceBuffer[audioSrcIdx].clip.name);
		}
	}
		
	/// <summary>
	/// Stop all currently playing audio sources for this sound effect.
	/// </summary>
	public void Mute()
	{
		for(int i = 0; i < audioSourceBuffer.Count; i++) 
		{
			if (audioSourceBuffer[i] != null) {
				audioSourceBuffer[i].Stop();
			}
		}
	}
	
	public void OnDisable()
	{
		Mute();
	}
	
	protected void UseNextAudioSource() {
		if (sound.delayUntilNextPlay > 0f) {
			lastTimePlayed = Time.time;
		}
		
		audioSrcIdx++;
		if (audioSrcIdx >= sound.maxAudioSources) {
			audioSrcIdx = 0;
		}
	}
	
	public bool CanPlay()
	{
		bool resul = true;

		if ( !enabled || (sound.delayUntilNextPlay > 0f && Time.time - lastTimePlayed < sound.delayUntilNextPlay) ) {
			resul = false;
		}

		return resul;
	}
		
	/// <summary>
	/// Play the current sound on the current available audio source. If the current audio source is already playing, it will be stopped first.
	/// </summary>
	public void Play()
	{
		if ( !CanPlay() ) {
			return;
		}
	
		TryNewRandomClip();
		
		AudioSource curAudioSrc = audioSourceBuffer[audioSrcIdx];

		if (curAudioSrc.isPlaying) {
			curAudioSrc.Stop();
		}
		
		curAudioSrc.Play();
		
		UseNextAudioSource();
	}
	
	public void PlayOneShot(AudioClip soundClip, float volumeScale = 1f)
	{
		if ( !CanPlay() ) {
			return;
		}
		
		if (sound.delayUntilNextPlay > 0f) {
			lastTimePlayed = Time.time;
		}

		//TODO: remove this hack "* AudioListener.volume" when Unity fixes their PlayOneShot audio source bug.
		AudioSrc.PlayOneShot(soundClip, volumeScale * AudioListener.volume);
	}

	/// <summary>
	/// Play the current sound on the current available audio source after the specified delay (in seconds).
	/// </summary>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	public void PlayDelayed(float delay)
	{
		if ( !CanPlay() ) {
			return;
		}
		
		TryNewRandomClip();
		
		AudioSource curAudioSrc = audioSourceBuffer[audioSrcIdx];
		
		if (curAudioSrc.isPlaying) {
			curAudioSrc.Stop();
		}
		
		curAudioSrc.PlayDelayed(delay);

		UseNextAudioSource();
	}

	/// <summary>
	/// Plays this sound effect in the LateUpdate, incrementing the <see cref="playPendingCount"/>
	/// </summary>
	public void PlayQueued()
	{
		if ( !CanPlay() ) {
			return;
		}
		
		playPendingCount++;
		
		if ( !waitingForLateUpdate ) {
			StartCoroutine( PlayInLateUpdate() );
		}
	}
	
	/// <summary>
	/// Stop this sound effect instance, thus stopping all audio source buffers.
	/// </summary>
	public void Stop() 
	{
		for(int i = 0; i < audioSourceBuffer.Count; i++) {
			audioSourceBuffer[i].Stop();
		}
	}
	
	protected IEnumerator PlayInLateUpdate()
	{
		waitingForLateUpdate = true;
		
		yield return waitEndFrame;
		
		waitingForLateUpdate = false;
		
//		Debug.Log("[SoundEffectController] " + Id + " -> play pending in frame: " + Time.frameCount + " -> " + playPendingCount);

		if (playPendingCount > 1)
		{
			for(int i = 0; i < playPendingCount && i < sound.maxAudioSources; i++) {
				PlayDelayed(i * sound.multiplePlayDelay);
			}
		}
		else {
			Play();
		}
		
		
		playPendingCount = 0;
	}
	
	/// <summary>
	/// Unity event raised when this component / gameobject is destroyed.
	/// </summary>
	void OnDestroy() 
	{
		// Destroy audio sources assigned to this sound effect.
		for(int i = 0; i < audioSourceBuffer.Count; i++) {
			Destroy(audioSourceBuffer[i]);
		}
	}
}
