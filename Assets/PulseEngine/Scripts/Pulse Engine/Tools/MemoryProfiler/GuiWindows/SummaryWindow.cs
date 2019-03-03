using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Draw the Debugger Window with the debugging info summary. Closing this window will display the Toggle Window again.
/// </summary>
public class SummaryWindow : CustomGUIWindow {
	protected MemoryProfiler parent;
	protected List<IProfilerResourcesGroup> listResourceGroups;
	protected Rect screenRect;
	protected uint totalGroupsMemory;
	
	public SummaryWindow(MemoryProfiler _parent, List<IProfilerResourcesGroup> _listResourceGroups) {
		windowId = this.GetInstanceID();
		windowDrawHandler = DrawWindowOverride;
		screenRect = new Rect(0f, 0f, Screen.width, Screen.height);

		parent = _parent;
		listResourceGroups = _listResourceGroups;
	}
		
	protected override void DrawWindowOverride(int windowId) {
		screenRect.width = Screen.width;
		screenRect.height = Screen.height;
		GUILayout.BeginVertical(); {
			// Draw the toggles for each group of debugged resources
			title = string.Format("Resources Total: {0}", MemoryProfiler.FormatBytesSize(totalGroupsMemory));

			totalGroupsMemory = 0;
			for(int i = 0; i < listResourceGroups.Count; i++) {
				GUILayout.BeginHorizontal(); {
					listResourceGroups[i].IsEnabled = GUILayout.Toggle(listResourceGroups[i].IsEnabled, listResourceGroups[i].GroupTypeName);
					
					// Draw debug info summary for this resources group
					if (listResourceGroups[i].IsEnabled) {
						listResourceGroups[i].UpdateProfilerInfo();
						uint groupMemSize = listResourceGroups[i].GroupMemSize;
						totalGroupsMemory += groupMemSize;
						
						// Check if the Details Window button is pressed for this resource group
						if ( GUILayout.Button( string.Format("({0} -> {1})", 
								listResourceGroups[i].NumResources, MemoryProfiler.FormatBytesSize(groupMemSize))) ) {
							// Pass the currently selected resource group to the Details Window
							parent.detailsWindow.userObject = listResourceGroups[i];
							parent.detailsWindow.isVisible = true;
							isVisible = false;
							break;
						}
					}
				}
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5f);
			}
			
			GUILayout.Label( string.Format("MonoHeapMemory:{0}", MemoryProfiler.FormatBytesSize(UnityEngine.Profiling.Profiler.usedHeapSize)) );
				
			GUILayout.Space(25f);
			
			if ( GUILayout.Button("Unload Unused Assets") ) {
				Resources.UnloadUnusedAssets();
			}
			
			GUILayout.Space(10f);
			
			if ( GUILayout.Button("Garbage Collect") ) {
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
			}			
			
			GUILayout.Space(10f);
			
			// If the Close button is pressed, go back to the toggle window
			if ( GUILayout.Button("Close") ) {
				isVisible = false;
				parent.toggleWindow.isVisible = true;
			}
		}
		GUILayout.EndVertical();
		
		GUI.DragWindow(screenRect);
	}
}
