using UnityEngine;
using System.Collections;
using System.IO;


public abstract class AbstractTile : MonoBehaviour, ICustomSerializable {
	public delegate void TileEventHandler(AbstractTile sender);
	public delegate void TileEventMovingChanged(AbstractTile sender);
	public static event TileEventHandler OnTileTap;
	public static event TileEventHandler OnTileMovingChanged;
	public static event TileEventHandler OnBoardPieceChanged;
	
	// This tag is used for serialization when there is no tile on a board piece. This string is written to mark this.
	public const string EmptyTileTag = "EmptyTile";
		
	/// <summary>
	/// Indicates if this tile is moveable by user input. If it's not moveable it doesn't mean it will not be affected by gravity.
	/// </summary>
	[SerializeField]
	protected bool isUserMoveable = true;
	
	/// <summary>
	/// The number of points the player should receive when this tile is destroyed.
	/// </summary>
	[SerializeField]
	protected int numPoints = 0;

	/// <summary>
	/// The board piece this tile belongs to.
	/// </summary>
	private AbstractBoardPiece boardPiece;	
	
	/// <summary>
	/// Reference to the board data this tile belongs to.
	/// </summary>
	private BoardData boardData;
	
	/// <summary>
	/// Reference to the board renderer that draws this tile.
	/// </summary>
	private AbstractBoardRenderer boardRenderer;
		
	/// <summary>
	/// Indicates if this tile is desctructible by any means in the game. (by matching it or by another power)
	/// </summary>
	[SerializeField]
	private bool isDestructible = true;
	
	/// <summary>
	/// True if this tile is being animated in any way. (no other interaction upon it should be possible while this is true)
	/// </summary>
	private bool isMoving = false;
	
	/// <summary>
	/// True if the tile is destroying.
	/// </summary>
	private bool isDestroying = false;
	
	[System.NonSerialized]
	public Transform cachedTransform;
	
	[System.NonSerialized]
	public Rigidbody cachedRigidbody;
	
	[System.NonSerialized]
	public Collider cachedCollider;
	
	[System.NonSerialized]
	public Animation cachedAnimation;

	protected virtual void Awake() {
		cachedTransform = transform;
		cachedRigidbody = GetComponent<Rigidbody>();
		cachedCollider = GetComponent<Collider>();
		cachedAnimation = GetComponent<Animation>();
	}

	protected virtual void Start() {
		
	}

	protected virtual void Update() {
		
	}

	public virtual void InitComponent() {

	}
	
	public Vector3 WorldPosition {
		get {
			return cachedTransform.position;
		}
		set {
			cachedTransform.position = value;
		}
	}
	
	public Vector3 LocalPosition {
		get {
			return cachedTransform.localPosition;
		}
		set {
			cachedTransform.localPosition = value;
		}
	}
	
	public bool IsDestroying {
		get {
			return isDestroying;
		}
		set {
			isDestroying = value;
		}
	}
	
	public virtual bool IsUserMoveable {
		get {
			return isUserMoveable && enabled;
		}
		set {
			isUserMoveable = value;
		}
	}
	
	public AbstractBoardPiece BoardPieceRef {
		get {
			return boardPiece;
		}
		set {
			boardPiece = value;
		}
	}
		
	public AbstractBoardPiece BoardPiece {
		get {
			return boardPiece;
		}
		set {
			AbstractBoardPiece oldBoardPiece = boardPiece;
			boardPiece = value;
			
			if(boardPiece != oldBoardPiece) 
			{
				RaiseOnBoardPieceChanged();
			}
		}
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
	
	public int Points {
		get {
			return numPoints;
		}
		set {
			numPoints = value;
		}
	}
		
	public bool IsDestructible {
		get {
			return isDestructible;
		}
		set {
			isDestructible = value;
		}
	}
	
	public bool IsMoving {
		get {
			return isMoving;
		}
		set {
            bool lastValue = isMoving;
			isMoving = value;
			
			if (isMoving != lastValue) {
				RaiseEventTileMovingChanged();
			}
		}
	}
		
	/// <summary>
	/// Raises the event tile tap. This event should be raised when a tile is tapped.
	/// Useful if a tile should do any power-up effect or action when it is tapped.
	/// This method can be overriden by subclasses but should ALWAYS call this base implementation to correctly
	/// trigger any other methods registered to this event.
	/// </summary>
	public virtual void RaiseEventTileTap() {
		if (OnTileTap != null) {
			OnTileTap(this);
		}
	}
	
	public virtual void RaiseEventTileMovingChanged() {
		if (OnTileMovingChanged != null) {
			OnTileMovingChanged(this);
		}
	}
	
	public virtual void RaiseOnBoardPieceChanged() {
		if (OnBoardPieceChanged != null) {
			OnBoardPieceChanged(this);
		}
	}
		
	/// <summary>
	/// Determines whether this tile is adjacent to the specified targetBoardPiece.
	/// This method can be made virtual so other tiles can "fool" the game into thinking that a tile could "jump-switch" with
	/// another tile further from it.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this tile is adjacent to the specified targetBoardPiece; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='targetBoardPiece'>
	/// If set to <c>true</c> target board piece.
	/// </param>
//	public bool IsAdjacentTo(AbstractBoardPiece targetBoardPiece) {
//		return BoardPiece.IsAdjacentTo(targetBoardPiece);
//	}
	
	/// <summary>
	/// Determines whether this tile can move at the specified targetBoardPiece.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance can move at the specified targetBoardPiece; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='targetBoardPiece'>
	/// If set to <c>true</c> target board piece.
	/// </param>
	public virtual bool CanMoveAt(AbstractBoardPiece targetBoardPiece) {
		if (Board == null) {
			Debug.LogError("Tile " + name + " has no BoardData reference!");
		}
		
		if (targetBoardPiece == null) {
			return false;
		}
//		BoardCoord targetBoardPos = targetBoardPiece.BoardPosition;
//			
//		// Do board coordinates sanity check first.
//		if (targetBoardPos.row < 0 || targetBoardPos.col < 0 || targetBoardPos.row >= Board.NumRows || targetBoardPos.col >= Board.NumColumns) {
//			return false;
//		}
		
		bool canMove = !IsMoving && IsUserMoveable && BoardPiece.IsAdjacentTo(targetBoardPiece) && !IsDestroying;
		
		return canMove;
	}

	/// <summary>
	/// Called internally when the <see cref="Destroy"/> method is called on this tile right before actually destroying the tile.
	/// This method will raise the <see cref="OnTileDestroyed"/> event on it's parent <see cref="AbstractBoardPiece"/> (if it has one).
	/// and it can do all kinds of stuff before actually destroying the tile.
	/// </summary>
	/// <returns>
	/// If it returns <c>true</c> the destroy sequence will continue and the <see cref="PostDestroy"/> method will be called next.
	/// If it returns <c>false</c> the destroy sequence will stop and <see cref="PostDestroy"/> won't be called anymore. In this case
	/// the gameobject won't be destroyed and you could do various animations first and the manually destroy the game object.
	/// </returns>
	protected virtual bool PreDestroy() {
        IsDestroying = true;
		
		return true;
	}
	
	/// <summary>
	/// Does the actual destroying of the tile game object.
	/// </summary>
	protected virtual void TileDestroy(bool useEffect) {
        // Actual destroying the gameobject.
        if (BoardPiece) {
			BoardPiece.RemoveTile(this);
		}
		
		if (gameObject != null) {
//			if (BoardPiece) {
//				BoardPiece.Tile = null;
//			}
			Destroy(gameObject);
		}

    }

    /// <summary>
    /// Destroy this tile and remove it from it's parent board piece.
    /// Raises OnTileDestroyed event on it's parent board piece if it has one.
    /// </summary>
    public virtual void Destroy() {
        if ( IsDestructible && !IsDestroying && PreDestroy()) {
            TileDestroy(true);
		}

    }

    #region ICustomSerializable implementation
    public virtual void WriteToStream (BinaryWriter writeStream) {
		Debug.Log("[WriteToStream] Serializing tile " + name + ": ");
		
		Debug.Log("[WriteToStream] -> Tile ObjType: " + this.GetType().Name);
		writeStream.Write(this.GetType().Name);
		
		Debug.Log("[WriteToStream] -> IsMoveable: " + IsUserMoveable);
		writeStream.Write(IsUserMoveable);
		
		Debug.Log("[WriteToStream] -> Points: " + Points);
		writeStream.Write(Points);
		
		Debug.Log("[WriteToStream] -> IsDestructible: " + IsDestructible);
		writeStream.Write(IsDestructible);
		
	}

	public virtual void ReadFromStream (int fileVersion, BinaryReader readStream) {
		if (fileVersion == 1) {
			Debug.Log("[ReadFromStream] Deserialiazing tile " + name + ": ");
//			Debug.Log("[WriteToStream] -> ObjType: " + this.GetType().Name);
//			readStream.ReadString();			

			IsUserMoveable = readStream.ReadBoolean();
			Debug.Log("[ReadFromStream] -> IsMoveable: " + IsUserMoveable);
			
			Points = readStream.ReadInt32();
			Debug.Log("[ReadFromStream] -> Points: " + Points);

			IsDestructible = readStream.ReadBoolean();
			Debug.Log("[ReadFromStream] -> IsDestructible: " + IsDestructible);
		} else {
			throw new UnityException("[ReadFromStream] Reading tile from file version: " + fileVersion + " not supported!");
		}
	}
	#endregion
	
//	public override string ToString ()
//	{
//		return string.Format ("[" + this.GetType().ToString() + ": IsMoveable={0}, " +
//			"Points={1}, IsDestructible={2}, IsMoving={3}, BoardCoord={4}]", 
//			IsUserMoveable, Points, IsDestructible, IsMoving, BoardPiece != null ? BoardPiece.BoardPosition.ToString() : "");
//	}
	
	/// <summary>
	/// Destroy event raised by Unity.
	/// </summary>

	public virtual Object Thumbnail ()
	{
		return transform.Find("Model").GetComponent<Renderer>().sharedMaterial.mainTexture;
	}

	public virtual void OnDestroy() {
	}
}