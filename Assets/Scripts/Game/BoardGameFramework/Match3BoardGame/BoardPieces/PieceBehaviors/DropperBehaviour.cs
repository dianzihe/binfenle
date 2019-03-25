using UnityEngine;
using System.Collections;

/// <summary>
/// Dropper behaviour. For the moment this is used to indicate if a boardPiece allows tiles to be dropped
/// out of the board. In the future this can be used for scripting any visual indicators that might be used
/// to indicate the fact that this board piece is a 'drop point'.
/// </summary>

[RequireComponent(typeof(Match3BoardPiece))]
public class DropperBehaviour : MonoBehaviour {
	
	protected Match3BoardPiece boardPiece;
	
	protected string dropTileIndicatorResourcePath = "Game/Prefabs/Droptile_Indicator";
	protected Vector3 dropTileIndicatorLocalPosition = new Vector3(0, -0.5f, -10f);
	
	protected Transform dropTileIndicator;
		
	protected string dropTileMaskPath = "Game/Board/BoardPieces/DropTileMask";
	
	protected Vector3 maskOffset = new Vector3(0f, -1f, -5f);
	
	protected Transform topSpawnMaskTransform;
	
	protected void Awake()
	{
		boardPiece = GetComponent<Match3BoardPiece>();
		
		boardPiece.OnTileChanged += OnTileChanged;
		
		dropTileIndicator = (GameObject.Instantiate(Resources.Load(dropTileIndicatorResourcePath)) as GameObject).transform;
		dropTileIndicator.parent = boardPiece.transform;
		dropTileIndicator.localPosition = dropTileIndicatorLocalPosition;
		
		topSpawnMaskTransform = (GameObject.Instantiate(Resources.Load(dropTileMaskPath)) as GameObject).transform;
		
		topSpawnMaskTransform.name = "DropTileMask";
		topSpawnMaskTransform.parent = transform.parent;
		topSpawnMaskTransform.localPosition = transform.localPosition;
		topSpawnMaskTransform.localPosition += maskOffset * Match3BoardRenderer.vertTileDistance;
	}

	protected void OnTileChanged(AbstractBoardPiece sender, AbstractTile tile)
	{
		if(tile is DropTile)
		{
			DropTile dropTile = tile as DropTile;
			
			if(!dropTile.isSwitching && !dropTile.isDropping)
			{
				StartCoroutine(dropTile.Drop());
			}
		}
	}
}
