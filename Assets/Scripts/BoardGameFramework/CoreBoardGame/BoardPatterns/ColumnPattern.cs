using UnityEngine;
using System.Collections;


/// <summary>
/// ColumnPattern
/// Describes the coordinates for column on a board.
/// </summary>
public class ColumnPattern : AbstractPatternShape {
	protected int startRow;
	protected int endRow;
	protected int columnIdx;
	
	protected int iteratorIdx;
	
	public ColumnPattern(int _columnIdx, int _startRow, int _endRow) {
		columnIdx = _columnIdx;
		startRow = _startRow;
		endRow = _endRow;
		ResetIterator();
	}

	public ColumnPattern(int columnIdx, int columnHeight) : this(columnIdx, 0, columnHeight - 1) {  }
		

	#region implemented abstract members of Frozen.PatternShape
	/// <summary>
	/// Resets the iterator for this pattern shape. 
	/// Calling <see cref="GetNextPatternCoord"/> afterwards will start returning points for this pattern from the start.
	/// </summary>
	public override void ResetIterator () {
		iteratorIdx = startRow;
	}

	/// <summary>
	/// Gets the next pattern coordinate for this pattern shape.
	/// It will return null when there are no more points available for this pattern.
	/// </summary>
	/// <returns>
	/// The next pattern coordinate.
	/// </returns>
	public override bool GetNextPatternCoord(out BoardCoord result) {
		result.row = 0;
		result.col = 0;
		if ( (startRow <= endRow && iteratorIdx <= endRow) || (startRow > endRow && iteratorIdx >= startRow) ) {
			result.row = (startRow < endRow) ? iteratorIdx++ : iteratorIdx--;
			result.col = columnIdx;
//			result = new BoardCoord(startRow < endRow ? iteratorIdx++ : iteratorIdx--, columnIdx);
			return true;
		}

		return false;
	}
	#endregion
}
