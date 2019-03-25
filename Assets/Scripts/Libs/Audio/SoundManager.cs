using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour 
{
	protected const string internalSharedSndId = "_internal_shared";
		
	protected static SoundManager instance;
	
	protected Dictionary<string, SoundEffectController> soundsDictionary;
	
	[SerializeField]
	private List<SoundEffect> registeredSoundsList = new List<SoundEffect>();
	
	public int numAllocatedAudioSources;
	
	protected SoundEffectController sharedSndEffect;
	
	
	void Awake() 
	{
		instance = this;
		
		soundsDictionary = new Dictionary<string, SoundEffectController>();
		
		// Process the sounds list specified in edit mode and create the corresponding sound dictionary.
		for(int i = 0; i < registeredSoundsList.Count; i++) {
			AddSound(registeredSoundsList[i], true);
		}
		
		// Create and cache the shared sound effect used internally for all OneShot sound effects.
		AddSound(new SoundEffect(null, internalSharedSndId), true);
		sharedSndEffect = this[internalSharedSndId];
	}
	
	public static SoundManager Instance {
		get {
			return instance;
		}
	}

	// Use this for initialization
//	void Start () 
//	{
//		
//	}
	
	public SoundEffectController this[string soundId] {
		get {
			SoundEffectController sndController = null;
			soundsDictionary.TryGetValue(soundId, out sndController);
			return sndController;
		}
	}
	
	public void RegisterSound(SoundEffect sndEffect, bool checkForDuplicate = true) 
	{
		if (checkForDuplicate && registeredSoundsList.Contains(sndEffect))
		{
			return;
		}
		
		registeredSoundsList.Add(sndEffect);
		
		AddSound(registeredSoundsList[registeredSoundsList.Count-1], true);
	}
	
	public void UnregisterSound(string sndId)
	{
		for(int i = 0; i < registeredSoundsList.Count; i++)
		{
			if (registeredSoundsList[i].id == sndId) {
				registeredSoundsList.RemoveAt(i);
				break;
			}
		}
	}

	public void Play(string soundId)
	{
		SoundEffectController sndController = this[soundId];

		if (sndController != null) {
			sndController.Play();
		}
		else {
			Debug.LogWarning("[SoundManager] Can't find sound with id: " + soundId + " for Play!");
		}
	}
	
	public void PlayQueued(string soundId)
	{
		SoundEffectController sndController = this[soundId];
		
		if (sndController != null) {
			sndController.PlayQueued();
		}
		else {
			Debug.LogWarning("[SoundManager] Can't find sound with id: " + soundId + " for PlayQueued!");
		}
	}
	
	public void Stop(string soundId)
	{
		SoundEffectController sndController = this[soundId];
		
		if (sndController != null) {
			sndController.Stop();
		}
		else {
			Debug.LogWarning("[SoundManager] Can't find sound with id: " + soundId + " for Stop!");
		}		
	}
		
	/// <summary>
	/// Gets sound effect with the specified id and plays it using OneShot on the internally shared AudioSource.
	/// The sound effect must be registered on the SoundManager.
	/// </summary>
	/// <param name='soundId'>
	/// Sound identifier.
	/// </param>
	/// <param name='volumeScale'>
	/// Volume scale.
	/// </param>
	public void PlayOneShot(string soundId)
	{
		SoundEffectController sndEffect = this[soundId];

		if (sndEffect != null) {
			PlayOneShot(sndEffect);
		}
		else {
			Debug.LogWarning("[SoundManager] Can't find sound with id: " + soundId + " for PlayOneShot!");
		}
	}
	
	public void PlayOneShot(string soundId, float volumeScale)
	{
		SoundEffectController sndEffect = this[soundId];

		if (sndEffect != null) {
			PlayOneShot(sndEffect, volumeScale);
		}
		else {
			Debug.LogWarning("[SoundManager] Can't find sound with id: " + soundId + " for PlayOneShot!");
		}		
	}
	
	public void PlayOneShot(SoundEffectController soundEffect) 
	{
		PlayOneShot(soundEffect.SoundClip, soundEffect.sound.volume);
	}
	
	public void PlayOneShot(SoundEffectController soundEffect, float volumeScale)
	{
		PlayOneShot(soundEffect.SoundClip, volumeScale);
	}
	
	public void PlayOneShot(AudioClip soundClip, float volumeScale = 1f)
	{
		if(sharedSndEffect != null){
			sharedSndEffect.PlayOneShot(soundClip, volumeScale);
		}
	}
	
	void OnEnable()
	{
		SetMuteStateForAll(false);
	}
	
	void OnDisable()
	{
		Debug.Log("[SoundManager] Disabling SoundManager...");
		SetMuteStateForAll(true);
	}
	
	public void SetMuteStateForAll(bool mute) 
	{
		foreach(KeyValuePair<string, SoundEffectController> snd in soundsDictionary)
		{
			if (snd.Value != null) {
				snd.Value.enabled = !mute;
			}
		}
	}
	
	public bool AddSound(SoundEffect soundEffect, bool checkForDuplicate = false)
	{
		if (checkForDuplicate)
		{
			if ( soundsDictionary.ContainsKey(soundEffect.id) ) {
				// The given sound effect has an id already registered in the sound manager.
				return false;
			}
		}
			
		SoundEffectController sndController = gameObject.AddComponent<SoundEffectController>();
		
		soundEffect.ResetDefaults();
		sndController.InitComponent(soundEffect);
		
		soundsDictionary.Add(sndController.Id, sndController);
		
		numAllocatedAudioSources += soundEffect.maxAudioSources;
			
		// Succesfully created and registered the given sound effect.
		return true;
	}
	
	public void RemoveSound(string soundId) 
	{
		SoundEffectController sndController;
		
		soundsDictionary.TryGetValue(soundId, out sndController);
		
		if (sndController != null) 
		{
			numAllocatedAudioSources -= sndController.sound.maxAudioSources;
			
			Destroy(sndController);
			soundsDictionary.Remove(soundId);
		}
	}
	
}