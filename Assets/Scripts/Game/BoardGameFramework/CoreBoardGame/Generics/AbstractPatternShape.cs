using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// PatternShape
/// 
/// Stores a list of control points that define a shape on the grid. (Rectangle, Circle, Line, Cross, etc.)
/// The list of points is of type <see cref="BoardCoord"/>.
/// Subsequent calls to <see cref="GetNextPatternCoord"/> will return a <see cref="BoardCoord"/> pair that will describe the current pattern shape.
/// </summary>
public abstract class AbstractPatternShape {
			
	public abstract void ResetIterator();

	public abstract bool GetNextPatternCoord(out BoardCoord result);
}	
