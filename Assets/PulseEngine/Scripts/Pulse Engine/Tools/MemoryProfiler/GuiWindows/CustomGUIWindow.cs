using UnityEngine;
using System.Collections;

/// <summary>
/// Describes the common properties of a GUI window so you can easier manage multiple windows.
/// </summary>
public class CustomGUIWindow : UnityEngine.Object {	
	public int windowId;
	public string title = "";
	
	public Rect windowRect;
	public bool isVisible = false;
	
	public bool useAutoLayout = true;
	public GUILayoutOption[] extraLayoutOptions;
	
	// A reference to a user-defined object that can be passed around easier.
	public System.Object userObject;

	protected GUI.WindowFunction windowDrawHandler;
	
	public CustomGUIWindow() {
	}
		
	public CustomGUIWindow(GUI.WindowFunction _windowDrawHandler) : 
		this(_windowDrawHandler, "") {
	}
	
	public CustomGUIWindow(GUI.WindowFunction _windowDrawHandler, string windowTitle) : 
		this(new Rect(0f, 0f, 200f, 100f), _windowDrawHandler, windowTitle) {
	}
	
	public CustomGUIWindow(Rect windowRect, GUI.WindowFunction _windowDrawHandler, string windowTitle) {
		windowId = this.GetInstanceID();
		windowDrawHandler = _windowDrawHandler;
		title = windowTitle;
	}
	
	/// <summary>
	/// Draws the window if it's visible flag is true. 
	/// The window will be drawn with GUILayout if the useAutoLayout flag is true, or with simple GUI commands otherwise.
	/// </summary>
	public void DrawWindow() {
		if ( !isVisible ) {
			return;
		}
		
		if (windowDrawHandler != null) {
			if (useAutoLayout) {
				windowRect = GUILayout.Window(0, windowRect, windowDrawHandler, title, extraLayoutOptions);
			} else {
				windowRect = GUI.Window(0, windowRect, windowDrawHandler, title);
			}
		}
	}
	
	
	protected virtual void DrawWindowOverride(int windowId) {
		// to be implemented by subclasses
	}
}