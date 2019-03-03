using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Encapsulates the profiler info required for a group of resources of the same type (Textures, Meshes, Animations, etc.).
/// The profiler info is related to: number of resources, resources name, memory size per resource, etc.
/// </summary>
public class ProfilerResourcesGroup<T> : IProfilerResourcesGroup where T:UnityEngine.Object {
	protected bool isEnabled = true;
	protected string typeName;
	
	// The frame number when the profiler info was last updated for this resources group.
	protected int lastFrameProfilerInfoUpdated = -1;
	
	// The number of resources that this group contains.
	protected int numResources;
	
	// A snapshot of the resources instance IDs
	protected HashSet<int> snapshotResourcesIds = new HashSet<int>();
	protected bool filterSnapshotResources = false;
	
	protected List<KeyValuePair<string, uint>> listResourcesInfo = new List<KeyValuePair<string, uint>>();
	// The total memory occupied by this group of resources
	protected uint totalMemSize;
	
	public ProfilerResourcesGroup() {
		typeName = typeof(T).Name;
	}

	private void InternalUpdateProfilerInfo(bool createSnapshot, bool filterOutSnapshot) {
		// Check if we've called this method twice this frame
		if (Time.frameCount == lastFrameProfilerInfoUpdated) {
			return;
		}
		
		lastFrameProfilerInfoUpdated = Time.frameCount;
		T[] listResources = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];
		
		// Check if we need to create a snapshot of the currently gathered resources.
		if (createSnapshot) {
			snapshotResourcesIds.Clear();
			for(int i = 0; i < listResources.Length; i++) {
				snapshotResourcesIds.Add( listResources[i].GetInstanceID() );
			}
			return;
		}
		
		numResources = listResources.Length;
		
		// Update resources profiler info: names and memory sizes
		listResourcesInfo.Clear();
		totalMemSize = 0;
		for(int i = 0; i < listResources.Length; i++) {
			// If we have to filter-out the contents of the current resources snapshot,
			// this means that all the resources found that have the Instance ID already present in the current snapshot, will not be gathered in the profiler lists.
			if ( !filterOutSnapshot || (filterOutSnapshot && !snapshotResourcesIds.Contains( listResources[i].GetInstanceID() )) ) {
				uint memSize = (uint)UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(listResources[i]);
				totalMemSize += memSize;
				listResourcesInfo.Add( new KeyValuePair<string, uint>(listResources[i].name.Trim(), memSize) );
			}
		}
	}

	#region IProfilerResourcesGroup implementation	
	public string GroupTypeName {
		get {
			return typeName;
		}
		set {
			typeName = value;
		}
	}
		
	public bool IsEnabled {
		get {
			return isEnabled;
		}
		set {
			isEnabled = value;
		}
	}

	public int NumResources {
		get {
			return numResources;
		}
	}
	
	public List<KeyValuePair<string, uint>> ResourcesInfo {
		get {
			return listResourcesInfo;
		}
	}

	public uint GroupMemSize {
		get {
			return totalMemSize;
		}
	}
	
	/// <summary>
	/// Enables or disables the snapshot resources filter. 
	/// </summary>
	/// <value>
	/// <c>true</c> if filter snapshot resources; otherwise, <c>false</c>.
	/// </value>
	public bool FilterSnapshotResources {
		get {
			return filterSnapshotResources;
 		}
		set {
			filterSnapshotResources = value;
		}
	}

	/// <summary>
	/// Gets the last made resources snapshot.
	/// </summary>
	/// <value>
	/// The last resources snapshot. (a hash-set with instance ids of all the resources gathered at the moment of the snapshot)
	/// </value>
	public HashSet<int> LastResourcesSnapshot {
		get {
			return snapshotResourcesIds;
		}
	}
	
	/// <summary>
	/// Creates a snapshot with all the Instance IDs from all the currently gathered resources.
	/// You can access the snapshot hash-set by using the <see cref="LastResourcesSnapshot"/> property.
	/// </summary>
	public void CreateResourcesSnapshot() {
		lastFrameProfilerInfoUpdated = -1;
		InternalUpdateProfilerInfo(true, false);
	}
	
	/// <summary>
	/// Clears the last made resources snapshot.
	/// </summary>
	public void ClearResourcesSnapshot() {
		snapshotResourcesIds.Clear();
	}

	public void UpdateProfilerInfo() {
		InternalUpdateProfilerInfo(false, filterSnapshotResources);
	}
		
	#endregion
}