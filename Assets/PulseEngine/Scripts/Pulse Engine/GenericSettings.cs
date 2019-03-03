using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Generic settings and behaviours for the application. Override the instance if you want to change settings and behaviors or add new ones.
/// </summary>
public class GenericSettings : MonoBehaviour
{
	protected static GenericSettings instance = null;
	
	/// <summary>
	/// Tells if an instance object has already been created so it doesn't create a new one on destroy.
	/// </summary>
	public static bool createdInstance = false;
	
	[SerializeField]
	protected bool narration = true;
	
	[SerializeField]
	protected bool useRecording = false;
	
	[SerializeField]
	protected bool autoplay = false;
	
	[SerializeField]
	protected bool arrows = false;
	
	[SerializeField]	
	protected bool music = true;
	
	[SerializeField]
	protected bool sound = true;
	
	[SerializeField]
	protected bool tutorials = false;
	
	#region Events Delegates
	public delegate void NarrationChangedEventHandler(bool on);
	public delegate void UseRecordingChangedEventHandler(bool on);
	public delegate void AutoplayChangedEventHandler(bool on);
	public delegate void ArrowsChangedEventHandler(bool on);
	public delegate void MusicChangedEventHandler(bool on);
	public delegate void SoundChangedEventHandler(bool on);
	public delegate void TutorialsChangedEventHandler(bool on);
	#endregion Events Delegates
	
	/// <summary>
	/// Occurs when the narration setting changes.
	/// </summary>
	public event NarrationChangedEventHandler OnNarrationChanged;
	
	/// <summary>
	/// Occurs when the use recording setting changes.
	/// </summary>
	public event UseRecordingChangedEventHandler OnUseRecordingChanged;
	
	/// <summary>
	/// Occurs when the autoplay setting changes.
	/// </summary>
	public event AutoplayChangedEventHandler OnAutoplayChanged;
	
	/// <summary>
	/// Occurs when the arrows setting changes.
	/// </summary>
	public event ArrowsChangedEventHandler OnArrowsChanged;
	
	/// <summary>
	/// Occurs when the music setting changes.
	/// </summary>
	public event MusicChangedEventHandler OnMusicChanged;
	
	/// <summary>
	/// Occurs when the sound setting changes.
	/// </summary>
	public event SoundChangedEventHandler OnSoundChanged;

	/// <summary>
	/// Occurs when the tutorials setting changes.
	/// </summary>
	public event SoundChangedEventHandler OnTutorialsChanged;
	
	/// <summary>
	/// Gets the instance of this singleton.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static GenericSettings Instance {
		get {
			if (instance == null && !createdInstance) {
				GameObject settingsObj = new GameObject("Settings");
				DontDestroyOnLoad(settingsObj);
				instance = settingsObj.AddComponent<GenericSettings>();
			}
			
			return instance;
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether the narration setting is on for this application.
	/// </summary>
	/// <value>
	/// <c>true</c> if narration is on; otherwise, <c>false</c>.
	/// </value>
	public virtual bool Narration {
		get {
			return narration;
		}
		set {
			if (narration != value) {
				narration = value;
				if (OnNarrationChanged != null) {
					OnNarrationChanged(narration);
				}
			}
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether the use recording setting is on for this application.
	/// </summary>
	/// <value>
	/// <c>true</c> if use recording is on; otherwise, <c>false</c>.
	/// </value>
	public virtual bool UseRecording {
		get {
			return useRecording;
		}
		set {
			if (useRecording != value) {
				useRecording = value;
				if (OnUseRecordingChanged != null) {
					OnUseRecordingChanged(useRecording);
				}
			}
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether the autoplay setting is on for this application.
	/// </summary>
	/// <value>
	/// <c>true</c> if autoplay is on; otherwise, <c>false</c>.
	/// </value>
	[SerializeField]
	public virtual bool Autoplay
	{
		get {
			return autoplay;
		}
		set {
			if (autoplay != value) {
				autoplay = value;
				if (OnAutoplayChanged != null) {
					OnAutoplayChanged(autoplay);
				}
			}
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether the arrows setting is on for this application.
	/// </summary>
	/// <value>
	/// <c>true</c> if arrows are on; otherwise, <c>false</c>.
	/// </value>
	public virtual bool Arrows
	{
		get {
			return arrows;
		}
		set {
			if (arrows != value) {
				arrows = value;
				if (OnArrowsChanged != null) {
					OnArrowsChanged(arrows);
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether the music setting is on for this application.
	/// </summary>
	/// <value>
	/// <c>true</c> if music is on; otherwise, <c>false</c>.
	/// </value>
	public virtual bool Music
	{
		get {
			return music;
		}
		set {
			if (music != value) {
				music = value;
				if (OnMusicChanged != null) {
					OnMusicChanged(music);
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether the sound setting is on for this application.
	/// </summary>
	/// <value>
	/// <c>true</c> if sound is on; otherwise, <c>false</c>.
	/// </value>
	public virtual bool Sound
	{
		get {
			return sound;
		}
		set {
			if (sound != value) {
				sound = value;
				if (OnSoundChanged != null) {
					OnSoundChanged(sound);
				}
			}
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether the tutorials setting is on for this application.
	/// </summary>
	/// <value>
	/// <c>true</c> if sound is on; otherwise, <c>false</c>.
	/// </value>
	public virtual bool Tutorials
	{
		get {
			return tutorials;
		}
		set {
			if (tutorials != value) {
				tutorials = value;
				if (OnTutorialsChanged != null) {
					OnTutorialsChanged(sound);
				}
			}
		}
	}
	
	/// <summary>
	/// Unity method.
	/// Awake this instance.
	/// </summary>
	protected virtual void Awake()
	{
		if (instance == null) {
			instance = this;
			createdInstance = true;
		}
		
		// Initialize the wrappers you want to use
		AnimationWrapper.Instance.GetType();
		RenderWrapper.Instance.GetType();
		SoundWrapper.Instance.GetType();
		TextWrapper.Instance.GetType();
		LocalizationWrapper.Instance.GetType();
	}
	
	/// <summary>
	/// Should the play narration if use recording fails. Override this behavior in ProjectSettings if needed.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the app should play narration if use recording fails; otherwise <c>false</c>.
	/// </returns>
	public virtual bool ShouldPlayNarrationIfUseRecordingFails()
	{
		return true;
	}
}

//}
