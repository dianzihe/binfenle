using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundEffect
{
	public string id;
	public AudioClip audioClip;
	
	//TODO:quick hack to support random audio clip playing for same sound effect
	public AudioClip[] randomAudioClips;
	
	/// <summary>
	/// The max audio sources. You can also set this to zero in which case this sound effect won't allocate it's own AudioSource buffer
	/// and could only be used with the sound managers PlayOneShot methods. (useful for reducing the number of allocated AudioSources but with less control
	/// over the sound effect).
	/// </summary>
	public int maxAudioSources = 1;
	
	public bool loop = false;
	public int priority = 128;
	public float volume = 1f;
	public float pitch = 1f;
	public float pan = 0f;
	
	/// <summary>
	/// The delay when playing the same sound in the same frame.
	/// </summary>
	public float multiplePlayDelay = 0.05f;
	
	public float delayUntilNextPlay = 0f;

	[System.NonSerialized]
	public float defaultVolume = 1f;
	
	[System.NonSerialized]
	public float defaultPitch = 1f;
	
	[System.NonSerialized]
	public float defaultPan = 0f;

	
	public SoundEffect()
	{
		maxAudioSources = 1;

		priority = 128;
		volume = 1f;
		pitch = 1f;
		pan = 0f;
	}

	public SoundEffect(AudioClip _audioClip) : this() 
	{
		audioClip = _audioClip;
		
		if (audioClip != null) {
			id = audioClip.name;
		}
	}
	
	public SoundEffect(AudioClip _audioClip, string _id) : this(_audioClip)
	{
		id = _id;
	}
	
	public SoundEffect(AudioClip _audioClip, string _id, int _maxAudioSources) : this(_audioClip, _id)
	{
		maxAudioSources = _maxAudioSources;
	}
	
	public void ResetDefaults()
	{
		defaultVolume = volume;
		defaultPitch = pitch;
		defaultPan = pan;
	}
	
	public override bool Equals (object obj)
	{
		SoundEffect snd = obj as SoundEffect;
		
		if (snd == null)
		{
			return false;
		}
		
		return snd.id == id;
	}

	public override int GetHashCode()
	{
		return id.GetHashCode();
	}
}
