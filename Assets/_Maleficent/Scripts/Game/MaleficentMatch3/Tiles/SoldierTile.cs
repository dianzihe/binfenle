using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class SoldierTile : NormalTile {

	private Animation modelAnimation;

	public GameObject movePrefabEffect;
	public GameObject spawnPrefabEffect;
	public GameObject soldierDestroyPrefabEffect;
	public Transform model;
	public Animation attackAnimation;
	public float moveAnimTime = 0.5f;
	public Texture targetThumbnail;
	public string moveSoundSfx;
	
	protected override void Awake ()
	{
		base.Awake ();
		moveSoundSfx = "soldier_move_sfx";
		modelAnimation = transform.Find("Model").GetComponent<Animation>();
		spawnPrefabEffect = movePrefabEffect;
	}
	
	public override void InitAfterAttachedToBoard ()
	{
		base.InitAfterAttachedToBoard ();
		RegisterNeighborBoardPieces(true);
	}

	public override void OnDestroy ()
	{
		RegisterNeighborBoardPieces(false);
		base.OnDestroy();
	}

	public bool CanMoveForward ()
	{
		bool res = false;
		Match3BoardPiece topBoardPiece = (BoardPiece as Match3BoardPiece).TopLink;
		if (topBoardPiece != null) {
			Match3Tile topTile = topBoardPiece.Tile as Match3Tile;
			if (topTile == null) {
				res = true;
			}else {
				System.Type topTileType = topTile.GetType();

				if (topTileType == typeof(SoldierTile)) {
					res = (topTile as SoldierTile).CanMoveForward();
				}else {
					res =  !(topTileType == typeof(ShieldTile) ||
					         topTileType == typeof(LockedTile));
				}
			}
		}

		return res;
	}

	public void MoveForward (System.Action _moveEndCallback) 
	{
		if(CanMoveForward()) {
			Match3BoardPiece topBoardPiece = (BoardPiece as Match3BoardPiece).TopLink;
			Vector3 targetPos = topBoardPiece.cachedTransform.position;
			
			RegisterNeighborBoardPieces(false);
			IsMoving = true;

			if (movePrefabEffect != null) {
				Transform effectInstance = (Instantiate(movePrefabEffect) as GameObject).transform;
				effectInstance.position = targetPos;
				destroyEffect = effectInstance.GetComponent<DestroyEffect>();
				Destroy(effectInstance.gameObject, destroyEffect.lifeTime);
			}

			PlayMoveAnimation();
			
			HOTween.To(transform, moveAnimTime, 
			           new TweenParms().Prop("position", targetPos)
			           .Ease(EaseType.EaseInQuart)
			           .OnComplete(OnTargetPosReached, _moveEndCallback)
			           );
		}
	}

	public void DestroyPreviousTile (Match3Tile _tile)
	{
		if(_tile != null && !(_tile is SoldierTile))
		{
			_tile.DisableTileLogic();
			Destroy(_tile.gameObject);
		}
	}

	public void PlayAttackAnimation ()
	{
		modelAnimation.Play("SoldierAttack");
	}

	public void PlayMoveAnimation ()
	{
		modelAnimation.Play("SoldierMoveForward");
	}

	private void RegisterNeighborBoardPieces(bool subscribe) 
	{
		Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
		
		for(int i = 0; i < match3BoardPiece.neighbors.Length; i+=2) {
			
			Match3BoardPiece boardPieceIterator = match3BoardPiece.neighbors[i];
			
			if(boardPieceIterator == null) {
				continue;
			}
			
			if(subscribe) {
				boardPieceIterator.OnTileDestroyed += OnNeighborDestroy;;
			}else {
				boardPieceIterator.OnTileDestroyed -= OnNeighborDestroy;
			}
		}
	}

	void OnNeighborDestroy (AbstractBoardPiece sender, AbstractTile _neighbor)
	{
		if(_neighbor != null && (_neighbor as NormalTile).IsMatched && _neighbor.GetType() != typeof(SoldierTile)) {
			Destroy();
		}
	}

	
	void OnTargetPosReached(TweenEvent eventData)
	{
		System.Action moveEndCallback = (System.Action)eventData.parms[0];

		SoundManager.Instance.Play(moveSoundSfx);

		Match3BoardPiece topBoardPiece = (BoardPiece as Match3BoardPiece).TopLink;
		Match3Tile topTile = topBoardPiece.Tile as Match3Tile;

		BoardPiece.Tile = null;
		DestroyPreviousTile(topTile);
		Match3BoardRenderer.Instance.AttachTileToBoardAt(topBoardPiece as Match3BoardPiece, this, false, false, false);
		IsMoving = false;

		if (moveEndCallback != null) {
			moveEndCallback();
		}
	}

	public override bool Freeze()
	{
		if (!IsDestroying) {
			return true;
		}
		
		return false;
	}
	
	protected override void TileDestroy(bool useEffect)
	{
		base.TileDestroy(false);

		SoundManager.Instance.Play("soldier_destroy_sfx");

		Transform effectInstance = (Instantiate(soldierDestroyPrefabEffect) as GameObject).transform;
		effectInstance.position = WorldPosition + new Vector3(0f, 0f, -2f);
		destroyEffect = effectInstance.GetComponent<DestroyEffect>();
		Destroy(effectInstance.gameObject, destroyEffect.lifeTime);
	}

	public override Object Thumbnail ()
	{
		return targetThumbnail;
	}
}
