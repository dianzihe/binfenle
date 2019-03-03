using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Text wrapper that handles the communication between the engine core and the used text library.
/// This class uses Unity's built-in text components. Extend and override the instance to use a custom text library.
/// </summary>
public class TextWrapper
{
	/// <summary>
	/// The singleton instance.
	/// </summary>
	protected static TextWrapper instance = null;
	
	/// <summary>
	/// Gets the singleton instance for this class.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static TextWrapper Instance {
		get {
			if (instance == null) {
				instance = new TextWrapper();
			}
			
			return instance;
		}
	}
	
	/// <summary>
	/// Gets the text component from the given game object. The wrapper is the only one who knows what component it needs to get. 
	/// This function is primarily used to cache the result on the caller.
	/// </summary>
	/// <returns>
	/// The text component.
	/// </returns>
	/// <param name='gameObj'>
	/// The game object from which to get the text component.
	/// </param>
	public virtual Component GetTextComponent(GameObject gameObj)
	{
		return gameObj.GetComponent<TextMesh>();
	}
	
	/// <summary>
	/// Gets the text from the given component.
	/// </summary>
	/// <returns>
	/// The text.
	/// </returns>
	/// <param name='textComponent'>
	/// Text component.
	/// </param>
	public virtual string GetText(Component textComponent)
	{
		return (textComponent as TextMesh).text;
	}
	
	/// <summary>
	/// Sets the text on the given component.
	/// </summary>
	/// <param name='textComponent'>
	/// Text component.
	/// </param>
	/// <param name='newText'>
	/// New text.
	/// </param>
	public virtual void SetText(Component textComponent, string newText)
	{
		(textComponent as TextMesh).text = newText;
	}
}

//}
