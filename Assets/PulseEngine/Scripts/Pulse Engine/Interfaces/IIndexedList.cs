using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Entity that manages an indexed list of other entities. It can return or change the 
/// current index of said list, taking all actions needed for this change.
/// It can return the index count of said list.
/// </summary>
public interface IIndexedList
{
	/// <summary>
	/// Gets or sets the current index.
	/// </summary>
	/// <value>
	/// The current index.
	/// </value>
	int CurrentIndex 
	{
		get;
		set;
	}
	
	/// <summary>
	/// The count of indexes, as in the number of entities referenced by this indexed list.
	/// </summary>
	/// <returns>
	/// The count.
	/// </returns>
	int IndexCount();
}

//}
