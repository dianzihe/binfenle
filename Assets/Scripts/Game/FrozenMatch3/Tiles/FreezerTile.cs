using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreezerTile : NormalTile {
	
	public static event System.Action<FreezerTile> OnFreezerTileCreated;
	public static event System.Action<FreezerTile> OnFreezerTileDestroyed;
	
	//Neighbors of interest for this freezer tile
	public List<Match3BoardPiece> freezerNeighbors = new List<Match3BoardPiece>(4);
	
	//Valid positions (indexes of neighbors) towards which this freezer can replicate
	public List<int> validReplicationList = new List<int>();
	public List<int> validReplicationListSpecial = new List<int>();
	
	public string createAnimationName;
	public string destroyAnimationName;
	
	/// <summary>
	///Use this material when the tile state is idle (after create anim & before destroy anim).
	///Facilitates batching of freezerTiles
	/// </summary>
	public Material idleMaterial;
	
	protected Match3BoardPiece boardPieceIterator;
	
	public Animation cachedScaleAnim;
	
	public override void InitComponent () {
		base.InitComponent();
		
		TileColor = TileColorType.None;
		
		cachedAnimation = cachedTransform.Find("Model").GetComponent<Animation>();
		tileModelRenderer = tileModelTransform.GetChild(2).GetComponent<Renderer>();
		
		if(FreezerController.Instance == null) {
			Match3BoardGameLogic.Instance.gameObject.AddComponent<FreezerController>();
		}

		FreezerController.Instance.allFreezers.Add(this);
	}
	
	public override void InitAfterAttachedToBoard ()
	{
		base.InitAfterAttachedToBoard ();
		
		RegisterNeighborBoardPieces(true);
	}
	
	public override RuleEntry DefaultRuleEntry {
		get {
			RuleEntry defaultRule = new RuleEntry();
			defaultRule.RuleTileType = GetType();
			defaultRule.randomColor = false;
			defaultRule.ColorSelection = ColorSelectionMethod.ColorBased;
			defaultRule.RuleTileColor = TileColorType.None;
			
			return defaultRule;
		}
	}
	
	public void OnNeighborDestroy(AbstractBoardPiece sender, AbstractTile neighbor)
	{	
		if(neighbor && (neighbor as NormalTile).IsMatched)
		{
			Destroy ();
			FreezerController.Instance.freezerDestroyedThisTurn = true;
			FreezerController.Instance.allFreezers.Remove(this);
			
			//Tile release
			//BoardPiece.Tile = null;
		}
	}
	
	public void RaiseOnFreezerTileCreateEvent(FreezerTile target)
	{
		if(OnFreezerTileCreated != null)
		{
			OnFreezerTileCreated(target);
		}
	}
	
	public void RaiseOnFreezerTileDestroyedEvent(FreezerTile target)
	{
		if(OnFreezerTileDestroyed != null)
		{
			OnFreezerTileDestroyed(target);
		}
	}
	
	private void RegisterNeighborBoardPieces(bool subscribe) {
		Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
		
		for(int i = 0; i < match3BoardPiece.neighbors.Length; i+=2) {
			
			boardPieceIterator = match3BoardPiece.neighbors[i];
			
			if(boardPieceIterator == null) {
				continue;
			}
		
			if(subscribe) {
				freezerNeighbors.Add(boardPieceIterator);
				boardPieceIterator.OnTileDestroyed += OnNeighborDestroy;
			} else {
				boardPieceIterator.OnTileDestroyed -= OnNeighborDestroy;
			}
		}
	}
	
	public void Replicate()
	{	
		int randomIndex = Random.Range(0, validReplicationList.Count);
		int randomIndexSpecial = Random.Range(0, validReplicationListSpecial.Count);
		Match3BoardPiece targetBoardPiece;
		
		if(validReplicationList.Count != 0)
		{
			targetBoardPiece = freezerNeighbors[validReplicationList[randomIndex]];
		}
		else
		{
			targetBoardPiece = freezerNeighbors[validReplicationListSpecial[randomIndexSpecial]];
		}
		
		NormalTile oldTile = targetBoardPiece.Tile as NormalTile;
		
		//Create new freezer tile
		Match3BoardRenderer.Instance.SpawnSpecificTileAt(targetBoardPiece.BoardPosition, typeof(FreezerTile), TileColorType.None, false);
		FreezerTile newFreezerTile = (targetBoardPiece.Tile as FreezerTile);
		
		//Raise CREATE event
		RaiseOnFreezerTileCreateEvent(newFreezerTile);
		
		newFreezerTile.StartCoroutine(newFreezerTile.PlayCreateAnimation());
		
		if(oldTile != null)
		{
			oldTile.DisableTileLogic();
			GameObject.Destroy(oldTile.gameObject);
		}
		
		Match3BoardGameLogic.Instance.IsBoardStable = false;
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
	
	protected IEnumerator PlayCreateAnimation()
	{
		cachedAnimation.Play(createAnimationName);
		
		yield return new WaitForSeconds(cachedAnimation[createAnimationName].length);
		yield return new WaitForEndOfFrame();
		
		tileModelRenderer.material = idleMaterial;
	}
	
	public void RefreshValidIndexList()
	{	
		validReplicationList.Clear();
		validReplicationListSpecial.Clear();
		
		for(int i = 0; i < freezerNeighbors.Count; i++)
		{	
			if (freezerNeighbors[i] != null && !(freezerNeighbors[i] as Match3BoardPiece).IsBlocked && CanReplicateOverBoardPiece(freezerNeighbors[i]))
			{
				if (IsSpecialTile(freezerNeighbors[i].Tile))
				{
					validReplicationListSpecial.Add(i);
				}
				else
				{
					validReplicationList.Add(i);
				}
			}
		}
	}
	
	protected bool IsSpecialTile(AbstractTile target)
	{
		if (target != null && (target is TriggerTile || target is SunTile))
		{
			return true;
		}
		
		return false;
	}
	
	protected bool CanReplicateOverBoardPiece(Match3BoardPiece targetBoardPiece)
	{
		Match3Tile tile = targetBoardPiece.Tile as Match3Tile;

		if (targetBoardPiece is EmptyBoardPiece)
		{
			return false;
		}
		
		if (tile != null && (
			!tile.IsDestructible ||
			tile.IsDestroying ||
			tile.IsMoving ||
			tile.IsTileSwitching ||
			tile.IsMatched ||
			(tile as NormalTile).IsFrozen()) )
		{
			return false;
		}
		
		return true;
	}
	
	public override bool Freeze()
	{
		if (!IsDestroying) {
			return true;
		}
		
		return false;
	}
	
	protected override void TileDestroy (bool useEffect)
	{
		//Raise DESTROY event.
		RaiseOnFreezerTileDestroyedEvent(this);
		
		FreezerController.Instance.freezerDestroyedThisTurn = true;
		FreezerController.Instance.allFreezers.Remove(this);
			
//		Tile release
//		BoardPiece.Tile = null;
		
		RegisterNeighborBoardPieces(false);
		
		ParticleSystem freezerParticleSystem = tileModelTransform.GetChild(0).GetComponent<ParticleSystem>();
		
		freezerParticleSystem.Stop();
		freezerParticleSystem.Play();
		
		cachedAnimation.Play(destroyAnimationName);
		cachedScaleAnim.Play(destroyAnimationName);
		
		useDestroyEffect =false;
		
		StartCoroutine(DelayedDestroy(cachedAnimation[destroyAnimationName].length));
	}
}
