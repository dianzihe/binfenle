using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//namespace PulseEngine {

/// <summary>
/// Animation events related to animations in general.
/// </summary>
public class AnimationEvents : Events
{
	/// <summary>
	/// Animation event for playing an animation block.
	/// </summary>
	/// <param name='paramsString'>
	/// Parameters string. Parameters are given as pairs "key=value" separated by ", ".
	/// Required keys: 
	///  target - the target animation controller to execute the event.
	///  block - the name of the animation game object
	/// </param>
	public void Event_PlayAnimation(string paramsString)
	{
		Dictionary<string, string> values;
		
		AnimationController animationController = GetTargetAndValues<AnimationController>(paramsString, out values);
		
		if (animationController == null) {
			Debug.LogError("Event_PlayAnimation(" + paramsString + ") does not contain target parameter or target not found. Ignoring event.");
			return;
		}
		
		if (!values.ContainsKey("block")) {
			Debug.LogError("Event_PlayAnimation(" + paramsString + ") does not contain animation block name parameter. Ignoring event.");
			return;
		}
		
		animationController.PlayAnimation(values["block"]);
	}
}

//}
