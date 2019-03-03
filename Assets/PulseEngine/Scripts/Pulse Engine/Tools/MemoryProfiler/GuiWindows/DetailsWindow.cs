using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Draws the Details window controls which displays a detailed list of all the resources from a selected resource group. (resource name and memory size)
/// The selected resource group is passed through the <see cref="userObject"/> member of the <see cref="CustomGUIWindow"/>
/// </summary>
public class DetailsWindow : CustomGUIWindow {
	protected MemoryProfiler parent;
	protected Rect screenRect;
	protected Vector2 detailsWinScroll = Vector2.zero;
		
	public DetailsWindow(MemoryProfiler _parent, string windowTitle) {
		windowId = this.GetInstanceID();
		windowDrawHandler = DrawWindowOverride;
		screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
		
		parent = _parent;
		title = windowTitle;
	}
		
	protected override void DrawWindowOverride(int windowId) {
		screenRect.width = Screen.width;
		screenRect.height = Screen.height;
		
		// Get the currently selected resources group to display details.
		IProfilerResourcesGroup currentGroup = userObject as IProfilerResourcesGroup;
		
		currentGroup.UpdateProfilerInfo();
		if (currentGroup.FilterSnapshotResources) {
			title = string.Format("{0} - (New) Total Memory: {1}", currentGroup.GroupTypeName, MemoryProfiler.FormatBytesSize(currentGroup.GroupMemSize));
		} else {
			title = string.Format("{0} - Total Memory: {1}", currentGroup.GroupTypeName, MemoryProfiler.FormatBytesSize(currentGroup.GroupMemSize));
		}
		
		GUILayout.BeginVertical(); {
			detailsWinScroll = GUILayout.BeginScrollView(detailsWinScroll, GUILayout.Width(Screen.width * 0.4f), GUILayout.Height(Screen.height * 0.75f)); {
				for(int i = 0; i < currentGroup.ResourcesInfo.Count; i++) {
					if (currentGroup.ResourcesInfo[i].Key.Length == 0) {
						// Render memory allocated resources (or resources that don't have a name) with a different color.
						Color lastColor = GUI.contentColor;
						GUI.contentColor = Color.green;
						GUILayout.Label(string.Format("{0} -> (MemAllocated)", MemoryProfiler.FormatBytesSize(currentGroup.ResourcesInfo[i].Value)) );

						GUI.contentColor = lastColor;
					} else {
						// Render disk loaded resources (or resources that have a name) with default GUI color
						GUILayout.Label( string.Format("{0} -> {1}", 
							MemoryProfiler.FormatBytesSize(currentGroup.ResourcesInfo[i].Value), currentGroup.ResourcesInfo[i].Key) );
					}
				}
			}
			GUILayout.EndScrollView();
			
			if ( GUILayout.Button("Show New MemAllocs") ) {
				currentGroup.CreateResourcesSnapshot();
				currentGroup.ResourcesInfo.Clear();
				currentGroup.FilterSnapshotResources = true;
			}

			GUILayout.Space(10f);
			
			if ( GUILayout.Button("Show All") ) {
				currentGroup.ClearResourcesSnapshot();
				currentGroup.FilterSnapshotResources = false;
			}
			
			GUILayout.Space(10f);
			
			if ( GUILayout.Button("Close") ) {
				isVisible = false;
				parent.profilerWindow.isVisible = true;
			}
		}
		GUILayout.EndVertical();	
		
		GUI.DragWindow(screenRect);
	}
}
