using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Localization wrapper that handles the communication between the engine core and the used localization library.
/// Extend and override the instance to use a custom localization library.
/// </summary>
public class LocalizationWrapper
{
	/// <summary>
	/// The singleton instance.
	/// </summary>
	protected static LocalizationWrapper instance = null;
	
	/// <summary>
	/// Gets the singleton instance for this class.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static LocalizationWrapper Instance {
		get {
			if (instance == null) {
				instance = new LocalizationWrapper();
			}
			
			return instance;
		}
	}
	
	/// <summary>
	/// Gets the localization component from the given game object. The wrapper is the only one who knows what component it needs to get. 
	/// This function is primarily used to cache the result on the caller.
	/// </summary>
	/// <returns>
	/// The localization component.
	/// </returns>
	/// <param name='gameObj'>
	/// The game object from which to get the localization component.
	/// </param>
	public virtual Component GetLocalizationComponent(GameObject gameObj)
	{
		return gameObj.GetComponent<TextMesh>();
	}
}

//}
