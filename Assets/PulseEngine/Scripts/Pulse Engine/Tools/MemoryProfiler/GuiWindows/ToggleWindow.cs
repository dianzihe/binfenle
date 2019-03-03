using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Draws the Toggle Window which displays the draggable toggle switch
/// </summary>
public class ToggleWindow : CustomGUIWindow {
	protected MemoryProfiler parent;
	protected Rect screenRect;
	
	public ToggleWindow(MemoryProfiler _parent) {
		windowId = this.GetInstanceID();
		windowDrawHandler = DrawWindowOverride;
		screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
		
		parent = _parent;
	}
		
	protected override void DrawWindowOverride(int windowId) {
		screenRect.width = Screen.width;
		screenRect.height = Screen.height;
		
		GUILayout.BeginVertical(); {
			GUILayout.Space(10f);
			
			if ( GUILayout.Button("Memory Profiler") ) {
				parent.profilerWindow.isVisible = true;
				isVisible = false;
			}
			
			GUILayout.Space(10f);
		}
		
		GUILayout.EndVertical();

		GUI.DragWindow(screenRect);
	}
}
