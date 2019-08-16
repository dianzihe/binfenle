using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif 

public abstract class AbstractBoardPiece: MonoBehaviour, ICustomSerializable {
	public delegate void BoardPieceTileHandler(AbstractBoardPiece sender, AbstractTile tile);
		
	// This tag is used for serialization when there is no board piece on a board grid position. This string tag is written to mark this.
	public const string EmptyPieceTag = "EmptyBoardPiece";
	
	private event BoardPieceTileHandler onTileDestroyed;
	private event BoardPieceTileHandler onTileChanged;
		
	/// <summary>
	/// Reference to the tile piece that is positioned on this BoardPiece
	/// </summary>
	[SerializeField]
	protected AbstractTile tile;
	
	/// <summary>
	/// The board position of this board piece.
	/// </summary>
	private BoardCoord boardPosition;
	
	/// <summary>
	/// Reference to the board data that this board piece belongs to.
	/// </summary>
	private BoardData boardData;
	
	/// <summary>
	/// Indicates if this board piece is empty in which case no tile should be connected to it.
	/// </summary>
	[SerializeField]
	private bool isEmpty = false;

	/// <summary>
	/// Indicates if this board piece is on the border of the board. A border piece cannot be at the same time set as "IsEmpty".
	/// </summary>
	[SerializeField]
	private bool isBorderPiece;

    [System.NonSerialized]
    public Transform cachedTransform;

	/// <summary>
	/// Reference to the board renderer that draws the tile for this boardPiece.
	/// </summary>
	private AbstractBoardRenderer boardRenderer;
	
	public virtual void Awake() {
        //System.Console.WriteLine("AbstractBoardPiece->awake");
        if (null == transform) {
            cachedTransform = Instantiate(transform) as Transform;
            //System.Console.WriteLine("AbstractBoardPiece->awake->new transform");
        }
        else
            cachedTransform = transform;
        // Apply values currently set on the prefab.
        IsEmpty = isEmpty;
		IsBorderPiece = isBorderPiece;
	}
	
	public virtual void Start() {
        //System.Console.WriteLine("AbstractBoardPiece->start");

    }

    public virtual void InitComponent(AbstractBoardRenderer _boardRenderer) {
//		Board = owner;
		BoardPosition = new BoardCoord(-1, -1);
		BoardRenderer = _boardRenderer;

        //System.Console.WriteLine("AbstractBoardPiece->InitComponent->" + name);
        if (null == cachedTransform)
        {
            cachedTransform = Instantiate(transform) as Transform;
        }
        // Parent this board piece to the owner's transform.
        cachedTransform.parent = BoardRenderer.cachedTransform;
	}
	
	public virtual bool IsEmpty {
		get {
			return isEmpty;
		}
		set {
			isEmpty = value;
		}
	}	

	public virtual bool IsBorderPiece {
		get {
			return isBorderPiece;
		}
		set {
			isBorderPiece = value;
			
			// A border piece can't be an empty piece at the same time.
			if (isBorderPiece) {
				IsEmpty = false;
			}
		}
	}		
	
	/// <summary>
	/// Determines whether this board piece is on the same row/column with the other specified board piece.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is collinear with the specified other; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='other'>
	/// If set to <c>true</c> other.
	/// </param>
	public bool IsCollinearWith(AbstractBoardPiece other)
	{
		return BoardPosition.row == other.BoardPosition.row || BoardPosition.col == other.BoardPosition.col;
	}
	
	/// <summary>
	/// Gets or sets the tile reference directly without triggering any game related events.
	/// This property is meant ONLY for board initialization when tiles can get destroyed and re-assigned multiple times.
	/// </summary>
	/// <value>
	/// The tile reference.
	/// </value>
	public AbstractTile TileRef {
		get {
			return tile;
		}
		set {
			tile = value;
			
			if (tile != null)
			{
				tile.BoardPieceRef = this;
			}
				
		}
	}

	/// <summary>
	/// Gets or sets the tile. 
	/// When setting the tile it will also set corresponding properties on the new tile.
	/// </summary>
	/// <value>
	/// The tile.
	/// </value>
	public virtual AbstractTile Tile {
		get {
			return tile;
		}
		set {
            tile = value;

			if (tile != null) {
				tile.BoardPiece = this;
				tile.Board = Board;
            }
			
			RaiseTileChangedEvent(tile);
        }
	}

	/// <summary>
	/// Resets the tile position to the same local position as the owner board piece.
	/// </summary>
	public void ResetTilePosition() {
		if (Tile != null) {
			// Position the new tile in the corresponding board piece slot.
			Tile.cachedTransform.localPosition = LocalPosition;
		}
	}
	
	/// <summary>
	/// Gets or sets the board position (grid position [row, column]) of this board piece.
	/// </summary>
	/// <value>
	/// The board position.
	/// </value>
	public BoardCoord BoardPosition {
		get {
			return boardPosition;
		}
		set {
			boardPosition = value;
		}
	}
	
	/// <summary>
	/// Faster than creating a new BoardCoord everytime we need to set a new value. 
	/// This method is preferred when setting the board position multiple times per frame instead of using the <see cref="BoardPosition"/>  setter method.
	/// </summary>
	/// <param name='row'>
	/// Row.
	/// </param>
	/// <param name='column'>
	/// Column.
	/// </param>
	public void SetBoardPosition(int row, int column) {
		boardPosition.row = row;
		boardPosition.col = column;
	}	
	
	public Vector3 LocalPosition {
		get {
			return cachedTransform.localPosition;
		}
		set {
			cachedTransform.localPosition = value;
			if (Tile != null) {
				Tile.LocalPosition = value;
			}
		}
	}
	
	/// <summary>
	/// Determines whether this board piece is adjacent only in the 4 directions (top, bottom, left, right) to the specified target board piece.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is adjacent to the specified targetBoardPiece; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='targetBoardPiece'>
	/// If set to <c>true</c> target board piece.
	/// </param>
	public bool IsAdjacentTo(AbstractBoardPiece targetBoardPiece) 
	{
		if (targetBoardPiece == null) {
			return false;
		}
		
		BoardCoord offset = BoardPosition - targetBoardPiece.BoardPosition;
		offset.row = Mathf.Abs(offset.row);
		offset.col = Mathf.Abs(offset.col);
		
		return offset.row == 0 && offset.col == 1 || 
			   offset.row == 1 && offset.col == 0;
	}
	
	/// <summary>
	/// Determines whether this board piece is adjacent in any of the 8 directions to the specified target board piece.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is area adjacent to the specified targetBoardPiece; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='targetBoardPiece'>
	/// If set to <c>true</c> target board piece.
	/// </param>
	public bool IsAreaAdjacentTo(AbstractBoardPiece targetBoardPiece)
	{
		if (targetBoardPiece == null) {
			return false;
		}
		
		BoardCoord offset = BoardPosition - targetBoardPiece.BoardPosition;
		offset.row = Mathf.Abs(offset.row);
		offset.col = Mathf.Abs(offset.col);
		
		return offset.row == 0 && offset.col == 1 || 
			   offset.row == 1 && offset.col == 0 ||
			   offset.row == 1 && offset.col == 1;
	}
		
	public BoardData Board {
		get {
			return boardData;
		}
		set {
			boardData = value;
		}		
	}

	public AbstractBoardRenderer BoardRenderer {
		get {
			return boardRenderer;
		}
		set {
			boardRenderer = value;
		}
	}

	public event BoardPieceTileHandler OnTileDestroyed {
		add {
			onTileDestroyed += value;
		}
		
		remove {
			onTileDestroyed -= value;
		}
	}
	
	public event BoardPieceTileHandler OnTileChanged {
		add {
			onTileChanged += value;
		}
		remove {
			onTileChanged -= value;
		}
	}
	
	public void MoveTileTo(AbstractBoardPiece dstBoardPiece) {
		dstBoardPiece.Tile = Tile;
		Tile = null;
	}
	
	/// <summary>
	/// Raises the tile changed event whenever the board piece <see cref="Tile"/> property is changed.
	/// </summary>
	/// <param name='newTile'>
	/// Changed tile.
	/// </param>
	public virtual void RaiseTileChangedEvent(AbstractTile newTile) {
        if (onTileChanged != null) {
			onTileChanged(this, newTile);
		}
	}
	
	/// <summary>
	/// Raises the event OnTileDestroyed. This event is usually called by the <see cref="Match3Tile.Destroy()"/> method.
	/// </summary>
	/// <param name='destroyedTile'>
	/// Destroyed tile.
	/// </param>
	public virtual void RaiseEventTileDestroyed(AbstractTile destroyedTile) {
		if (onTileDestroyed != null) {
			onTileDestroyed(this, destroyedTile);
		}
	}
	
	public void RemoveTile(AbstractTile destroyedTile) {
		if (destroyedTile == Tile) {
			Tile = null;
		}
	}
	
	public void PrintDebug(bool debugPrivateMembers = false) {
		//TODO: self reflection based debug to display all fields and values (easier than to keep extending
		// and keeping the debug print in sync with the evolving framework code.
	}
	
	#region ICustomSerializable implementation
	public virtual void WriteToStream (BinaryWriter writeStream) {
		Debug.Log("[WriteToStream] Serializing board piece: ");
		
		Debug.Log("[WriteToStream] -> Board piece ObjType: " + this.GetType().Name);
		writeStream.Write(this.GetType().Name);
		
		// Write board tile information if there is a tile on this board piece.
		if (Tile != null) {			
			Tile.WriteToStream(writeStream);
		} else {
			// No Tile is on this board piece.
			writeStream.Write(AbstractTile.EmptyTileTag);
		}
		
		// Write other board piece related information in subclassed implementations.
	}

	public virtual void ReadFromStream (int fileVersion, BinaryReader readStream) {
		if (fileVersion == 1) {
			throw new System.NotImplementedException();
		} else {
			throw new UnityException("[ReadFromStream] Reading board piece from file version: " + fileVersion + " not supported!");
		}
	}
	#endregion
	
	public override string ToString ()
	{
		return string.Format ("[AbstractBoardPiece: Tile={0}, BoardPosition={1}, LocalPosition={2}", Tile, BoardPosition, LocalPosition);
	}
}
