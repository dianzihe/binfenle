using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MatchCounterBoardPiece : Match3BoardPiece {
	public static event System.Action<MatchCounterBoardPiece> OnMatchCounterBoardPieceInit;
	public static event System.Action<MatchCounterBoardPiece> OnNewMatch;
	public static event System.Action<MatchCounterBoardPiece> OnMatchesLimitReached;

	public int matchesLimit = 0;
	protected int matchesCount = 0;

	#region public

	public override void Awake () 
	{
		base.Awake();
		gameObject.AddComponent<BoxCollider>().center = Vector3.forward * 0.5f;
		gameObject.layer = LayerMask.NameToLayer("BoardPiece");
	}

	public int MatchesLimit
	{
		get {
			return matchesLimit;
		}
	}
	
	#endregion

	#region protected

	protected virtual void OnDestroy () 
	{
		OnTileDestroyed -= HandleOnTileDestroyed;
	}


	public override void InitComponent (AbstractBoardRenderer _boardRenderer)
	{
		base.InitComponent(_boardRenderer);

		OnTileDestroyed += HandleOnTileDestroyed;
		RaiseOnMatchCounterBoardPieceInit();
	}

	protected virtual void RaiseOnMatchCounterBoardPieceInit ()
	{
		if (OnMatchCounterBoardPieceInit != null) {
			OnMatchCounterBoardPieceInit(this);	
		}
	}

	protected virtual void RaiseOnNewMatch (Match3Tile tile) 
	{
		if(OnNewMatch != null) {
			OnNewMatch(this);
		}
	}

	protected virtual void RaiseOnMatchesLimitReached ()
	{
		if(OnMatchesLimitReached != null) {
			OnMatchesLimitReached(this);
		}
	}

	protected virtual bool IgnoresMatchesAfterLimitReached () {
		return true;
	}

	protected virtual bool IgnoreMatchedTile (AbstractTile tile) 
	{
		bool ignore = tile is  LockedTile /* || tile is SnowTile || tile is FreezerTile*/;
		return ignore;
	}

	protected virtual void HandleOnTileDestroyed (AbstractBoardPiece sender, AbstractTile tile)
	{
		PieceReached(sender, tile);
	}

	public virtual void PieceReached (AbstractBoardPiece sender, AbstractTile tile) 
	{
		if (!IgnoreMatchedTile(tile)) {
			if (matchesCount == matchesLimit && IgnoresMatchesAfterLimitReached ()) {
				return;
			}else {
				matchesCount++;
				RaiseOnNewMatch(tile as Match3Tile);
				if (matchesCount == matchesLimit) {
					RaiseOnMatchesLimitReached ();
				}
			}
		}
	}

	#endregion
}
