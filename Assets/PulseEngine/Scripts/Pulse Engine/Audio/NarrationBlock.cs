using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Narration block that may contain a narration or have other narration block children.
/// </summary>
public class NarrationBlock : AudioBlock
{	
	/// <summary>
	/// Registers the narration setting changed notifier.
	/// </summary>
	protected override void RegisterSettingChanged()
	{
		GenericSettings.Instance.OnNarrationChanged += OnSettingChanged;
	}
	
	/// <summary>
	/// Unregisters the narration setting changed notifier.
	/// </summary>
	protected override void UnregisterSettingChanged()
	{
		GenericSettings.Instance.OnNarrationChanged -= OnSettingChanged;
	}
	
	/// <summary>
	/// Determines if the narration setting is on.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the narration setting is on, otherwise <c>false</c>.
	/// </returns>
	protected override bool IsSettingOn()
	{
		return GenericSettings.Instance.Narration;
	}
}

//}
