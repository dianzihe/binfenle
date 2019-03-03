using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Render wrapper that handles the communication between the engine core and the used rendering library.
/// This class uses Unity's built-in rendering components. Extend and override the instance to use a custom rendering library.
/// </summary>
public class RenderWrapper
{
	/// <summary>
	/// The singleton instance.
	/// </summary>
	protected static RenderWrapper instance = null;
	
	/// <summary>
	/// Gets the singleton instance for this class.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static RenderWrapper Instance {
		get {
			if (instance == null) {
				instance = new RenderWrapper();
			}
			
			return instance;
		}
	}
	
	/// <summary>
	/// Gets the render component from the given game object. The wrapper is the only one who knows what component it needs to get. 
	/// This function is primarily used to cache the result on the caller.
	/// </summary>
	/// <returns>
	/// The render component.
	/// </returns>
	/// <param name='gameObj'>
	/// The game object from which to get the render component.
	/// </param>
	public virtual Component GetRenderComponent(GameObject gameObj)
	{
		return gameObj.GetComponent<Renderer>();
	}
	
	/// <summary>
	/// Changes the color for the given render component, if supported.
	/// </summary>
	/// <param name='renderComponent'>
	/// Render component.
	/// </param>
	/// <param name='newColor'>
	/// New color.
	/// </param>
	public virtual void ChangeColor(Component renderComponent, Color newColor)
	{
		(renderComponent as Renderer).material.color = newColor;
	}
	
	/// <summary>
	/// Gets the color of the given render component.
	/// </summary>
	/// <returns>
	/// The color.
	/// </returns>
	/// <param name='renderComponent'>
	/// Render component.
	/// </param>
	public virtual Color GetColor(Component renderComponent)
	{
		return (renderComponent as Renderer).material.color;
	}
	
	/// <summary>
	/// Changes the alpha for the given render component, if supported.
	/// </summary>
	/// <param name='renderComponent'>
	/// Render component.
	/// </param>
	/// <param name='newAlpha'>
	/// New alpha value.
	/// </param>
	public virtual void ChangeAlpha(Component renderComponent, float newAlpha)
	{
		Color color = GetColor(renderComponent);
		color.a = newAlpha;
		ChangeColor(renderComponent, color);
	}
	
	/// <summary>
	/// Gets the alpha of the given render component.
	/// </summary>
	/// <returns>
	/// The alpha value.
	/// </returns>
	/// <param name='renderComponent'>
	/// Render component.
	/// </param>
	public virtual float GetAlpha(Component renderComponent)
	{
		return GetColor(renderComponent).a;
	}
	
	/// <summary>
	/// Enables or disables the given render component.
	/// </summary>
	/// <param name='renderComponent'>
	/// Render component.
	/// </param>
	/// <param name='enabled'>
	/// Tells if the render component should be enabled or disabled.
	/// </param>
	public virtual void SetEnabled(Component renderComponent, bool enabled)
	{
		(renderComponent as Renderer).enabled = enabled;
	}
}

//}
