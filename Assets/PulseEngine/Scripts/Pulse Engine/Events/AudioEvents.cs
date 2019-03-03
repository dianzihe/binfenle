using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//namespace PulseEngine {

/// <summary>
/// Audio animation events.
/// </summary>
public class AudioEvents : Events
{
	/// <summary>
	/// Animation event for playing a narration block.
	/// </summary>
	/// <param name='paramsString'>
	/// Parameters string. Parameters are given as pairs "key=value" separated by ", ".
	/// Required keys: 
	///  target - the target audio controller to execute the event.
	///  block - the name of the narration block game object
	/// </param>
	public void Event_PlayNarration(string paramsString)
	{
		Dictionary<string, string> values;
		
		AudioController audioController = GetTargetAndValues<AudioController>(paramsString, out values);
		
		if (audioController == null) {
			Debug.LogError("Event_PlayNarration(" + paramsString + ") does not contain target parameter or target not found. Ignoring event.");
			return;
		}
		
		if (!values.ContainsKey("block")) {
			Debug.LogError("Event_PlayNarration(" + paramsString + ") does not contain narration block name parameter. Ignoring event.");
			return;
		}
		
		audioController.PlayNarration(values["block"]);
	}
	
	/// <summary>
	/// Animation event for playing an audio block.
	/// </summary>
	/// <param name='paramsString'>
	/// Parameters string. Parameters are given as pairs "key=value" separated by ", ".
	/// Required keys: 
	///  target - the target audio controller to execute the event.
	///  block - the name of the audio block game object
	/// </param>
	public void Event_PlaySound(string paramsString)
	{
		Dictionary<string, string> values;
		
		AudioController audioController = GetTargetAndValues<AudioController>(paramsString, out values);
		
		if (audioController == null) {
			Debug.LogError("Event_PlaySound(" + paramsString + ") does not contain target parameter or target not found. Ignoring event.");
			return;
		}
		
		if (!values.ContainsKey("block")) {
			Debug.LogError("Event_PlaySound(" + paramsString + ") does not contain audio block name parameter. Ignoring event.");
			return;
		}
		
		audioController.PlaySound(values["block"]);
	}
	
	/// <summary>
	/// Animation event for playing a music block.
	/// </summary>
	/// <param name='paramsString'>
	/// Parameters string. Parameters are given as pairs "key=value" separated by ", ".
	/// Required keys: 
	///  target - the target audio controller to execute the event.
	///  block - the name of the music block game object
	/// </param>
	public void Event_PlayMusic(string paramsString)
	{
		Dictionary<string, string> values;
		
		AudioController audioController = GetTargetAndValues<AudioController>(paramsString, out values);
		
		if (audioController == null) {
			Debug.LogError("Event_PlaySound(" + paramsString + ") does not contain target parameter or target not found. Ignoring event.");
			return;
		}
		
		if (!values.ContainsKey("block")) {
			Debug.LogError("Event_PlaySound(" + paramsString + ") does not contain audio block name parameter. Ignoring event.");
			return;
		}
		
		audioController.PlayMusic(values["block"]);
	}
}

//}
