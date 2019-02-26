using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Board data.
/// </summary>
public class BoardData : MonoBehaviour, ICustomSerializable {
//	public int debugMaxNumCols = 8;
//	public int debugMaxNumRows = 8;
		
	[SerializeField]
	protected int numColumns = 8;
	[SerializeField]
	protected int numRows = 8;
	
	protected AbstractBoardPiece[,] grid;
		
	[System.NonSerialized]
	public Transform cachedTransform;
	
	void Awake() {
		cachedTransform = transform;
		
		NumColumns = numColumns;
		NumRows = numRows;		
		
		InitComponent();
	}
	
//	void OnEnable() {
//		if (!isInitialized) {
//			isInitialized = true;
//		}
//	}
	
	// Use this for initialization
	void Start () {
	}
	
	public void InitComponent() {
		Debug.Log("[BoardData] InitComponent...");
		
		// Allocate grid.
		grid = new AbstractBoardPiece[NumRows, NumColumns];
	}
	
	public int NumColumns {
		get {
			return numColumns;
		}
		set {
			numColumns = value;
		}
	}
	
	public int NumRows {
		get {
			return numRows;
		}
		set {
			numRows = value;
		}
	}
	
	public BoardCoord GetBoardPosition(int row, int column) {
		return grid[row, column].BoardPosition;
	}
		
	public AbstractBoardPiece this[int row, int column] {
		get {
			return grid[row, column];
		}
		set {
			grid[row, column] = value;
			
			if (value != null) {
				value.Board = this;
				value.SetBoardPosition(row, column);
			}
		}
	}
	
	public AbstractBoardPiece this[BoardCoord boardPos] {
		get {
			return this[boardPos.row, boardPos.col];
		}
		set {
			this[boardPos.row, boardPos.col] = value;
		}
	}
	
	/// <summary>
	/// Find and mark the board pieces that are the border of the current board.
	/// </summary>
	public void MarkBoardBounds() {
		int halfNumCols = (int)(NumColumns * 0.5f);
		int halfNumRows = (int)(NumRows * 0.5f);
		bool foundLeftBorder = false;
		bool foundRightBorder = false;
		bool[] foundTopRowBorder = new bool[NumColumns];
		bool[] foundBottomRowBorder = new bool[NumColumns];
		
		for(int i = 0; i < NumColumns; i++) {
			foundTopRowBorder[i] = foundBottomRowBorder[i] = false;
		}
		
		// Determine the border pieces of the level in one sweep of the grid.
		// Horizontal search for border tiles.
		for(int rowIdx = 0; rowIdx < NumRows; rowIdx++) {
			// Assume we didn't find any new horizontal border pieces on the current row.
			foundLeftBorder = foundRightBorder = false;
			int bottomRowIdx = NumRows - rowIdx - 1;
//			Debug.Log("bottomRowIdx = " + bottomRowIdx);

			// Horizontal search columns on the current row for the first non-empty board piece and mark it as a border piece.
			for(int colIdx = 0; colIdx < NumColumns; colIdx++) {
				int rightColIdx = NumColumns - colIdx - 1;
				if (rightColIdx < halfNumCols) {
					rightColIdx = halfNumCols;
				}

				AbstractBoardPiece leftBoardPiece = grid[rowIdx, colIdx];
				AbstractBoardPiece rightBoardPiece = grid[rowIdx, rightColIdx];

				if (!foundLeftBorder && !leftBoardPiece.IsEmpty) {
					// Mark that we've found a new left border piece on the current column.
					leftBoardPiece.IsBorderPiece = true;
					foundLeftBorder = true;
				}

				if (!foundRightBorder && !rightBoardPiece.IsEmpty) {
					// Mark that we've found a right border piece on the current column.
					rightBoardPiece.IsBorderPiece = true;
					foundRightBorder = true;
				}

				if (!foundTopRowBorder[colIdx] && rowIdx <= halfNumRows && !leftBoardPiece.IsEmpty) {
					// Mark that we've found a new top border piece on the current column.
					foundTopRowBorder[colIdx] = true;
					leftBoardPiece.IsBorderPiece = true;
				}

				if (!foundBottomRowBorder[colIdx] && bottomRowIdx > halfNumRows && !leftBoardPiece.IsEmpty) {
					// Mark that we've found a new bottom border piece on the current column.
					foundBottomRowBorder[colIdx] = true;
					grid[bottomRowIdx, colIdx].IsBorderPiece = true;
				}
			}
		}
	}
	
	#region Utility methods
	public void SwitchTiles(AbstractTile src, AbstractTile dst) {
		AbstractBoardPiece dstBoardPiece = dst.BoardPiece;

		src.BoardPiece.Tile = dst;
		dstBoardPiece.Tile = src;
	}
		
//	public bool TryMoveTileTo(AbstractBoardPiece srcBoardPiece, AbstractBoardPiece dstBoardPiece) {
//		if (dstBoardPiece.Tile != null) {
//			return false;
//		}
//		
//		dstBoardPiece.Tile = srcBoardPiece.Tile;
//		srcBoardPiece.Tile = null;
//		return true;
//	}
		
	public void ApplyActionToColumn(int columnIdx, System.Action<AbstractBoardPiece> action) {
		if (action == null) {
			return;
		}
		
		for(int i = 0; i < NumRows; i++) {
			action(grid[i, columnIdx]);
		}
	}
	
	public void ApplyActionToAll(System.Action<AbstractBoardPiece> action) {
		if (action == null) {
			return;
		}
		
		for(int i = 0; i < NumRows; i++) {
			for(int j = 0; j < NumColumns; j++) {
				action(grid[i, j]);
			}
		}
	}
	
	/// <summary>
	/// Applies the cancellable action to all current board pieces until the action returns false.
	/// </summary>
	/// <param name='action'>
	/// Action. If it returns false it will cancel the board iteration. Otherwise it continues iterating through all the board pieces on the board.
	/// </param>
	public void ApplyCancellableActionToAll(System.Func<AbstractBoardPiece, bool> action) {
		if (action == null) {
			return;
		}
		
		for(int i = 0; i < NumRows; i++) {
			for(int j = 0; j < NumColumns; j++) {
				// If the action returns false, cancel the iteration.
				if ( !action(grid[i, j]) ) {
					return;
				}
			}
		}
	}	
	
	public void ApplyActionToRow(int rowIdx, System.Action<AbstractBoardPiece> action) {
		if (action == null) {
			return;
		}
		
		for(int i = 0; i < NumColumns; i++) {
			action(grid[rowIdx, i]);
		}
	}
	#endregion

	#region ICustomSerializable implementation
	public void WriteToStream (System.IO.BinaryWriter writeStream)
	{
		writeStream.Write(NumRows);
		writeStream.Write(NumColumns);
		
		for(int rowIdx = 0; rowIdx < NumRows; rowIdx++) {
			for(int colIdx = 0; colIdx < NumColumns; colIdx++) {
				// Write board piece class type
				AbstractBoardPiece boardPiece = grid[rowIdx, colIdx];
				if (boardPiece != null) {
					boardPiece.WriteToStream(writeStream);
				} else {
					writeStream.Write(AbstractBoardPiece.EmptyPieceTag);
				}
			}
		}

	}

	public void ReadFromStream (int fileVersion, System.IO.BinaryReader readStream)
	{
		throw new System.NotImplementedException ();
	}
	#endregion
	
	public override string ToString ()
	{
		System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
		
		strBuilder.AppendLine(string.Format("[BoardData: NumColumns={0}, NumRows={1}]", NumColumns, NumRows));
		for(int i = 0; i < NumRows; i++) {
			for(int j = 0; j < NumColumns; j++) {
				strBuilder.AppendLine( string.Format("grid[{0},{1}] = {2}", i, j, grid[i, j]) );
			}
		}
		
		return strBuilder.ToString();
	}
}

/// <summary>
/// Board Coordinate.
/// Stores the pair of (x,y) integers that represent a coordinate on the grid of the board.
/// </summary>
public struct BoardCoord {
	public int row;
	public int col;

	public BoardCoord(int _row, int _col) {
		row = _row;
		col = _col;
	}
	
	public BoardCoord(BoardCoord other) {
		row = other.row;
		col = other.col;
	}

	public static BoardCoord operator + (BoardCoord lhs, BoardCoord rhs) {
		return new BoardCoord(lhs.row + rhs.row, lhs.col + rhs.col);
	}

	public static BoardCoord operator - (BoardCoord lhs, BoardCoord rhs) {
		return new BoardCoord(lhs.row - rhs.row, lhs.col - rhs.col);
	}
	
	public static bool operator == (BoardCoord lhs, BoardCoord rhs) {
		return lhs.row == rhs.row && lhs.col == rhs.col;
	}
	
	public static bool operator != (BoardCoord lhs, BoardCoord rhs) {
		return !(lhs == rhs);
	}
		
	public bool IsValid(int numColumns, int numRows) {
		return (row >= 0 && row < numColumns && col >= 0 && col < numRows);
	}
		
	public void OffsetBy(BoardCoord offset) {
		row += offset.row;
		col += offset.col;
	}
	
	public void OffsetByAndClamp(BoardCoord offset, int clampToMaxRow, int clampToMaxColumn) {
		OffsetByAndClamp(offset.row, offset.col, clampToMaxRow, clampToMaxColumn);
	}
	
	public void OffsetByAndClamp(int rowOffset, int colOffset, int clampToMaxRow, int clampToMaxColumn) {
		row = Mathf.Clamp(row + rowOffset, 0, clampToMaxRow);
		col = Mathf.Clamp(col + colOffset, 0, clampToMaxColumn);
	}
	
	public override bool Equals (object obj)
	{
		if (obj == null || !(obj is BoardCoord)) {
			return false;
		}
		BoardCoord other = (BoardCoord)obj;
		return row == other.row && col == other.col;
	}

	public override int GetHashCode()
	{
		return row.GetHashCode() ^ col.GetHashCode();
	}
	
	public override string ToString () {
		return string.Format("[BoardCoord] ({0},{1})", row, col);
	}
	
	/// <summary>
	/// Parses the name of the coordinate from the name of a board piece which must use a valid format like: [3,4] BoardPiece.
	/// </summary>
	/// <returns>
	/// The parsed BoardCoord from the given board piece name.
	/// </returns>
	/// <param name='strName'>
	/// Board piece game object name.
	/// </param>
	public static BoardCoord ParseCoordFromString(string strName) {
		BoardCoord result = new BoardCoord(-1, -1);
		
		//TODO: this can be optimized with a regex expression (no time to implement this now)
		
		// Extract the board piece name from the gameobject name (TODO: note to self: use regex expression for faster and cleaner results)
		string[] strParts = strName.Split(',');
		if (strParts.Length != 2) {
			Debug.LogError("[BoardCoord] Invalid board piece naming format: " + strName);
		} else {
			string strPart1 = strParts[0];
			string strRow = strPart1.Substring(strPart1.IndexOf('[') + 1).Trim();
			
			string strPart2 = strParts[1];
			string strCol = strPart2.Substring(0, strPart2.IndexOf(']')).Trim();
			
			result.row = int.Parse(strRow);
			result.col = int.Parse(strCol);
		}
		
		return result;
	}
}
