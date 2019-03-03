using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//namespace PulseEngine {
	
/// <summary>
/// Text animation events.
/// </summary>
public class TextEvents : Events
{		
	/// <summary>
	/// Animation event for fading in a text block.
	/// </summary>
	/// <param name='paramsString'>
	/// Parameters string. Parameters are given as pairs "key=value" separated by ", ".
	/// Required keys: 
	///  target - the target text controller to execute the event.
	///  block - the name of the text game object
	/// Optional keys:
	///  time - how much time the fade in animation should last
	/// </param>
	public void Event_FadeInText(string paramsString)
	{
		Dictionary<string, string> values;
		
		TextController textController = GetTargetAndValues<TextController>(paramsString, out values);
		
		if (textController == null) {
			Debug.LogError("Event_FadeInText(" + paramsString + ") does not contain target parameter or target not found. Ignoring event.");
			return;
		}
		
		if (!values.ContainsKey("block")) {
			Debug.LogError("Event_FadeInText(" + paramsString + ") does not contain text block name parameter. Ignoring event.");
			return;
		}
		
		float time = TextBlock.DEFAULT_ANIMATION_TIME;
		if (values.ContainsKey("time")) {
			if (!float.TryParse(values["time"], out time)) {
				Debug.LogWarning("Event_FadeInText(" + paramsString + ") has bad formatting for the time parameter. Ignoring time parameter.");
				time = TextBlock.DEFAULT_ANIMATION_TIME;
			}
		} 

		textController.FadeInText(values["block"], time);
	}
	
	/// <summary>
	/// Animation event for fading in a text block.
	/// </summary>
	/// <param name='paramsString'>
	/// Parameters string. Parameters are given as pairs "key=value" separated by ", ".
	/// Required keys: 
	///  target - the target text controller to execute the event.
	///  block - the name of the text game object
	/// Optional keys:
	///  time - how much time the fade in animation should last
	/// </param>
	public void Event_FadeOutText(string paramsString)
	{
		Dictionary<string, string> values;
		
		TextController textController = GetTargetAndValues<TextController>(paramsString, out values);
		
		if (textController == null) {
			Debug.LogError("Event_FadeOutText(" + paramsString + ") does not contain target parameter or target not found. Ignoring event.");
			return;
		}
		
		if (!values.ContainsKey("block")) {
			Debug.LogError("Event_FadeOutText(" + paramsString + ") does not contain text block name parameter. Ignoring event.");
			return;
		}
		
		float time = TextBlock.DEFAULT_ANIMATION_TIME;
		if (values.ContainsKey("time")) {
			if (!float.TryParse(values["time"], out time)) {
				Debug.LogWarning("Event_FadeOutText(" + paramsString + ") has bad formatting for the time parameter. Ignoring time parameter.");
				time = TextBlock.DEFAULT_ANIMATION_TIME;
			}
		} 

		textController.FadeOutText(values["block"], time);
	}
}

//}
