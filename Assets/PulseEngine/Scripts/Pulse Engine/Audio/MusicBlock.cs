using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Music block that may contain a music or have other music block children.
/// </summary>
public class MusicBlock : AudioBlock
{	
	public override void Awake ()
	{
		if (audioSource == null) {
			audioSource = Camera.main.gameObject;
		}
		
		base.Awake();
	}
	
	/// <summary>
	/// Registers the music setting changed notifier.
	/// </summary>
	protected override void RegisterSettingChanged()
	{
		GenericSettings.Instance.OnMusicChanged += OnSettingChanged;
	}
	
	/// <summary>
	/// Unregisters the music setting changed notifier.
	/// </summary>
	protected override void UnregisterSettingChanged()
	{
		GenericSettings.Instance.OnMusicChanged -= OnSettingChanged;
	}
	
	/// <summary>
	/// Determines if the music setting is on.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the music setting is on, otherwise <c>false</c>.
	/// </returns>
	protected override bool IsSettingOn()
	{
		return GenericSettings.Instance.Music;
	}
	
	/// <summary>
	/// Raised when the corresponding audio setting changes.
	/// </summary>
	/// <param name='musicOn'>
	/// Is music on.
	/// </param>
	public override void OnSettingChanged(bool settingOn)
	{
		base.OnSettingChanged(settingOn);
		
		if (settingOn && !IsPlayingSelf()) {
			PlaySelfAudio(false);
		}
	}
	
	/// <summary>
	/// Determines whether this instance is looped. A music block is always considered looped.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is looped self; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsLoopedSelf()
	{
		return true;
	}
}

//}
