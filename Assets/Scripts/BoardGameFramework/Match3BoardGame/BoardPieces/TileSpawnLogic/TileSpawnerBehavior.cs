using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TileSpawnerBehavior
/// 
/// Describes the tile spawning behavior for a board piece.
/// This behavior can be attached on any <see cref="Match3BoardPiece"/> to mark it as being a tile spawner board piece.
/// This behavior will set the "IsTileSpawner" property of the Match3BoardPiece to true OnAwake which will register the tile in the 
/// <see cref="Match3BoardGameLogic"/>'s "tileSpawners" list of board pieces in the current level.
/// 
/// This behavior registers to the board piece's "OnTileChanged" event. When this event is triggered it will check if the new tile is 
/// </summary>
[RequireComponent(typeof(Match3BoardPiece))]
public class TileSpawnerBehavior : MonoBehaviour, ICustomSerializable {
	/// <summary>
	/// The path to the prefab used to mask the spawning area above the tile spawner. 
	/// Tiles will appear behind that area and fall down into place on this board piece.
	/// </summary>
	protected string prefabTopSpawnMaskPath = "Game/Board/BoardPieces/TileSpawnerMask";
	protected Vector3 maskOffset = new Vector3(0f, 1.05f, -5f);
	protected Transform topSpawnMaskTransform; 
	
	/// <summary>
	/// The cached spawn point for this tile spawner board piece.
	/// </summary>
	protected Vector3 spawnPoint;
	
	public delegate void TileSpawnerHandler(TileSpawnerBehavior sender);
	
	public static event TileSpawnerHandler OnTileSpawnerPieceEmpty;
	
	//Stores the time of the last immediate spawn processed
	public static float lastImmediateRuleSpawnTime = 0f;
	
	protected List<TileSpawnRule> immediateSpawnList;
	
	/// <summary>
	/// The tile spawn rules that will be executed on this 
	/// </summary>
	public List<TileSpawnRule> tileSpawnRules;
	public int currentSpawnRuleIdx = 0;
	
	[System.NonSerialized]
	public Match3BoardPiece boardPiece;
//	public bool canUseLastSpawnedTile = false;	
	
	protected Queue<Match3Tile> spawnQueue = new Queue<Match3Tile>(10);
	protected Match3Tile lastSpawnedTile = null;
	protected bool isSpawnQueueUpdating = false;
	
	
	void Awake() 
	{
		boardPiece = GetComponent<Match3BoardPiece>();
		
		// Mark the Match3BoardPiece this component is on as being a tile spawner.
		boardPiece.IsTileSpawner = true;
	}

	void Start()
	{
		// Cache the spawn point.
		spawnPoint = boardPiece.LocalPosition;
		spawnPoint.y += Match3BoardRenderer.vertTileDistance;

		// Spawn the depth mask above this spawner.
		topSpawnMaskTransform = (Instantiate(Resources.Load(prefabTopSpawnMaskPath)) as GameObject).transform;
		topSpawnMaskTransform.name = string.Format("SpawnMask_{0}", name);
		topSpawnMaskTransform.parent = transform.parent;
		topSpawnMaskTransform.localPosition = transform.localPosition;
		topSpawnMaskTransform.localPosition += maskOffset * Match3BoardRenderer.vertTileDistance;

//		Debug.LogWarning("[TileSpawnerBehavior] Start called...");
		
		immediateSpawnList = Match3BoardGameLogic.Instance.immediateSpawnList;
		
		boardPiece.OnTileChanged += OnBoardPieceTileChanged;
		
		InitComponent();
	}
	
	/// <summary>
	/// Inits this component. 
	/// It's safe to call it multiple times.
	/// </summary>
	void InitComponent() 
	{
		if (tileSpawnRules == null) {
			tileSpawnRules = new List<TileSpawnRule>();			
		}

		if (tileSpawnRules.Count == 0) 
		{
			// Create a default spawn rule if we don't have any spawning rule for this spawner.
			tileSpawnRules.Add(new TileSpawnRule(tileSpawnRules));
		}

		//Register the ownerList for those spawnTiles assigned through the editor
		for(int i = 0; i < tileSpawnRules.Count; i++)
		{
			if(tileSpawnRules[i].ownerList == null) {
				tileSpawnRules[i].ownerList = tileSpawnRules;
			}
		}
	}
	
	protected IEnumerator RaiseTileSpawnEventNextFrame(AbstractBoardPiece sender, AbstractTile newtile)
	{
		yield return null;
		
		OnBoardPieceTileChanged(sender, newtile);
	}
	
	protected void OnBoardPieceTileChanged(AbstractBoardPiece sender, AbstractTile newTile) {
		
		if ((sender as Match3BoardPiece).IsBlocked)
		{
			StartCoroutine(RaiseTileSpawnEventNextFrame(sender, newTile));
			return;
		}
		
		// Check if the new tile of the board piece is null.
		if (newTile != null) {
			return;
		}
		
		if (OnTileSpawnerPieceEmpty != null) {
			OnTileSpawnerPieceEmpty(this);
		}
	
//			Debug.Log(name + " => at frame: " + Time.frameCount);
		// Execute the next spawn rule from the list.
		
		if( immediateSpawnList.Count > 0 &&
		    Match3BoardRenderer.minDelayForceSpawnRules + lastImmediateRuleSpawnTime < Time.time)
		{
//				Debug.LogError("ImediateSpawnList has " + immediateSpawnList.Count + " items!");
			for(int i = 0; i < immediateSpawnList.Count; i++)
			{
//					Debug.LogError("Checking for [i] = " + i);
				for(int j = 0; j < boardPiece.eligibleSpawnList.Count; j++)
				{
//						Debug.LogError("Checking for [i][j]" + i + " " + j);
					if ( immediateSpawnList[i].ruleEntries[0].Equals(boardPiece.eligibleSpawnList[j]) )
					{
						SpawnImmediateRule(i);
						return;
					}
				}
			}
		}
//			
		if (tileSpawnRules.Count > 0) 
		{
//			tileSpawnRules[currentSpawnRuleIdx++].SpawnNewTile();
			SpawnNewTileInQueue(tileSpawnRules[currentSpawnRuleIdx++]);
			if (currentSpawnRuleIdx >= tileSpawnRules.Count) {
				currentSpawnRuleIdx = 0;
			}
		}
	}
	
	protected void SpawnImmediateRule(int ruleIndex)
	{
		lastImmediateRuleSpawnTime = Time.time;
//		immediateSpawnList[ruleIndex].SpawnNewTile();
		SpawnNewTileInQueue(immediateSpawnList[ruleIndex]);
		immediateSpawnList.RemoveAt(ruleIndex);
	}
	
	protected void SpawnNewTileInQueue(TileSpawnRule spawnRule)
	{
		Match3Tile newTile = spawnRule.SpawnNewTile();
		newTile.LocalPosition = spawnPoint;
		
		// Deactivate and hide the tile until the spawn queue is processed.
		newTile.gameObject.SetActive(false);

		spawnQueue.Enqueue(newTile);
		
		if ( !isSpawnQueueUpdating )
		{
			StartCoroutine(ProcessSpawnQueue());
		}
	}
		
	protected IEnumerator ProcessSpawnQueue() 
	{
		Match3Tile nextTile = null;
		Vector3 tileLocalPos = Vector3.zero;
		
		isSpawnQueueUpdating = true;
		
		while(spawnQueue.Count > 0)
		{			
			nextTile = spawnQueue.Peek();
										
			if (boardPiece.Tile == null)
			{				
				if (lastSpawnedTile == null || lastSpawnedTile.LocalPosition.y <= boardPiece.LocalPosition.y)
				{
					if (lastSpawnedTile != null && lastSpawnedTile.LocalPosition.y >= (boardPiece.LocalPosition.y - Match3BoardRenderer.halfVertTileDistance))
					{
						nextTile.gameObject.SetActive(true);

						nextTile.moveVel = Mathf.Clamp(lastSpawnedTile.moveVel - nextTile.initialVel, 0f, nextTile.maxVel);

						tileLocalPos = nextTile.LocalPosition;
						tileLocalPos.y = lastSpawnedTile.LocalPosition.y + Match3BoardRenderer.vertTileDistance;

						nextTile.LocalPosition = tileLocalPos;
					}
					else
					{
						nextTile.gameObject.SetActive(true);
						nextTile.LocalPosition = spawnPoint;
					}
					
					Match3BoardRenderer.Instance.AttachTileToBoardAt(boardPiece, nextTile, false, false, false);
					lastSpawnedTile = spawnQueue.Dequeue();
				}

			}
			
			yield return null;
		}
		
		isSpawnQueueUpdating = false;
	}
	
	#region ICustomSerializable implementation
	public void WriteToStream (System.IO.BinaryWriter writeStream)
	{
		throw new System.NotImplementedException ();
	}

	public void ReadFromStream (int fileVersion, System.IO.BinaryReader readStream)
	{
		throw new System.NotImplementedException ();
	}
	#endregion

}
