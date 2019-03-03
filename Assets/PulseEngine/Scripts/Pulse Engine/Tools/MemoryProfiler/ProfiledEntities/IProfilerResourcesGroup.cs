using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IProfilerResourcesGroup {	
	/// <summary>
	/// Gets or sets the name of this resources group type. 
	/// This is automatically set when the instance is created.
	/// </summary>
	/// <value>
	/// The name of the group type.
	/// </value>
	string GroupTypeName {
		get;
		set;
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this instance is enabled for updating its profiling info.
	/// </summary>
	/// <value>
	/// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
	/// </value>
	bool IsEnabled {
		get;
		set;
	}
	
	/// <summary>
	/// Gets the number resources since the last profiled info update done for this group.
	/// </summary>
	/// <value>
	/// The number resources.
	/// </value>
	int NumResources {
		get;
	}
	
	/// <summary>
	/// Gets the last gathered resources info.
	/// </summary>
	/// <value>
	/// The resources info.
	/// </value>
	List<KeyValuePair<string, uint>> ResourcesInfo {
		get;
	}
	
	
	/// <summary>
	/// Gets the memory size of the group since the last profiled info update.
	/// </summary>
	/// <value>
	/// The size of the group mem.
	/// </value>
	uint GroupMemSize {
		get;
	}
	
	/// <summary>
	/// Enables or disables snapshot resources filter. 
	/// </summary>
	/// <value>
	/// <c>true</c> if filter snapshot resources; otherwise, <c>false</c>.
	/// </value>
	bool FilterSnapshotResources {
		get;
		set;
	}
	
	/// <summary>
	/// Gets the last made resources snapshot.
	/// </summary>
	/// <value>
	/// The last resources snapshot. (a hash-set with instance ids of all the resources gathered at the moment of the snapshot)
	/// </value>
	HashSet<int> LastResourcesSnapshot {
		get;
	}
	
	/// <summary>
	/// Creates a snapshot with all the Instance IDs from all the currently gathered resources.
	/// You can access the snapshot hash-set by using the <see cref="LastResourcesSnapshot"/> property.
	/// </summary>
	void CreateResourcesSnapshot();
	
	/// <summary>
	/// Clears the last made resources snapshot.
	/// </summary>
	void ClearResourcesSnapshot();
	
	/// <summary>
	/// Updates the profiling info. (resources names, memory sizes, etc.)
	/// </summary>
	void UpdateProfilerInfo();
}