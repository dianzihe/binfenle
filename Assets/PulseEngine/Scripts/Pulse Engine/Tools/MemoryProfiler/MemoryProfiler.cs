using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// To use the MemoryProfiler just place this script on a GameObject in your scene.
/// Memory profiler main renderer for all debugger windows.
/// TODO:
/// Future features: 
/// 	- add a toggle button to enable log flushing instead of on-screen display of debugging info
/// 	- enlarge the the scroll bars from the GUIStyle
/// 	- setting to sort objects based on their memory size
/// </summary>
public class MemoryProfiler : MonoBehaviour {	
	
	protected static MemoryProfiler instance;
	
	protected List<IProfilerResourcesGroup> listResourceGroups = new List<IProfilerResourcesGroup>();
	protected GUIStyle guiStyle;
	
	protected Rect screenRect;
	
	[System.NonSerialized]
	public CustomGUIWindow toggleWindow;
	[System.NonSerialized]
	public CustomGUIWindow profilerWindow;
	[System.NonSerialized]
	public CustomGUIWindow detailsWindow;
	
	
	public static MemoryProfiler Instance {
		get {
			return instance;
		}
	}
	
	/// <summary>
	/// Formats the size of the bytes.
	/// </summary>
	/// <returns>
	/// The bytes size.
	/// </returns>
	/// <param name='bytes'>
	/// Bytes.
	/// </param>
	public static string FormatBytesSize(uint bytes) {
		string strFormat;
		if (bytes >= 1073741824) {
			strFormat = string.Format("{0:##.##} GB", decimal.Divide(bytes, 1073741824));
		} else if (bytes >= 1048576) {
			strFormat = string.Format("{0:##.##} MB", decimal.Divide(bytes, 1048576));
		} else if (bytes >= 1024) {
			strFormat = string.Format("{0:##.##} KB", decimal.Divide(bytes, 1024));
		} else if (bytes > 0 & bytes < 1024) {
			strFormat = string.Format("{0:##.##} B", bytes);
		} else {
			strFormat = "0 B";
		}
		
		return strFormat;
	}

	public void Awake() {
		screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
		
		if(instance == null) {
	       instance = this;
	    }
	    else {
	       Destroy(this);
	    }
		
		DontDestroyOnLoad(gameObject);

		// Setup Custom GUI Windows
		toggleWindow = new ToggleWindow(this);
		toggleWindow.isVisible = true;
		profilerWindow = new SummaryWindow(this, listResourceGroups);
		detailsWindow = new DetailsWindow(this, "Memory Details");

		// Setup the resources groups that you want to have in the profiler at run-time
		listResourceGroups.Add( new ProfilerResourcesGroup<Texture>() );
		listResourceGroups.Add( new ProfilerResourcesGroup<AudioClip>() );
		listResourceGroups.Add( new ProfilerResourcesGroup<Mesh>() );
		listResourceGroups.Add( new ProfilerResourcesGroup<Material>() );
		listResourceGroups.Add( new ProfilerResourcesGroup<GameObject>() );
	}
	
	void OnGUI() {
		screenRect.width = Screen.width;
		screenRect.height = Screen.height;

		// Copy the default label skin, change the color and the alignement
		if( guiStyle == null ) {
			guiStyle = new GUIStyle( GUI.skin.label );
			guiStyle.normal.textColor = Color.white;
			guiStyle.alignment = TextAnchor.MiddleCenter;
		}

		// Render debugger related windows (if visible).
		toggleWindow.DrawWindow();
		profilerWindow.DrawWindow();
		detailsWindow.DrawWindow();
	}		
}