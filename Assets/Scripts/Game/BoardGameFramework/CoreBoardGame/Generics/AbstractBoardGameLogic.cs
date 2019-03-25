using UnityEngine;
using System.Collections;

/// <summary>
/// AbstractBoardGameLogic
/// 
/// Board gameplay handles board input and gameplay rules like:
/// - when a tile should start falling (and how it should fall into place)
/// - how tiles should fall (slide to the left or drop down)
/// - when tiles should explode
/// - when the board should shuffle in case there are no possible matches.
/// - what power-up is active at a given time
/// It registers to callbacks from the BoardAnimations to correctly progress the tiles on the board, it
/// checks for tile matches and triggers corresponding events to help keep score, achievements and so on.
/// By extending this class you can theoretically make completelly different game logic on the board.
/// </summary>
public abstract class AbstractBoardGameLogic : MonoBehaviour {
	public AbstractWinCondition winConditions;
	public AbstractLoseCondition loseConditions;
	public AbstractBoardRenderer boardRenderer;
	public AbstractBoardAnimations boardAnimations;
	
	[System.NonSerialized]
	public BoardData boardData;
	
	[System.NonSerialized]
	public Transform cachedTransform;
	
	
	protected virtual void Awake() {
		cachedTransform = transform;
	}
	
	// Use this for initialization
	protected virtual void Start () {
	}

	public virtual void InitComponent() {
		boardRenderer.InitComponent();
		boardAnimations.InitComponent();
	}
	
	public abstract bool TryToMoveTile(AbstractTile srcTile, AbstractTile dstTile);
	
	public abstract bool TryToMoveTile(AbstractTile tile, TileMoveDirection moveDirection);
	
	public virtual void TileMoving(AbstractTile tile) { }
	
	public virtual void EndMoveTile(AbstractTile tile) { }
}
