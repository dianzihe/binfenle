using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class SoldiersController : MonoBehaviour {

	const string spawnerIndicatorResourcePath = "Game/Prefabs/Droptile_Indicator";

	protected static SoldiersController instance;

	private bool moving = false;

	public Vector3 spawnerIndicatorLocalPosition = new Vector3(0, -0.68f, -20f);
	public Vector3 spawnerIndicatorRotation = new Vector3(0, 0f, 180f);
	protected WaitForEndOfFrame waitEndOfFrame;
	protected int movesAccum = 0;
	protected int turnsAccum = 0;
	protected List<BoardCoord> spawnCoords;

	public int turnsBetweenWaves = 2;
	public float spawnSoldierAnimationDuration = 0.15f;
	
	
	protected void Awake () 
	{
		waitEndOfFrame = new WaitForEndOfFrame();

		spawnCoords = new List<BoardCoord>();
	
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove += HandleOnNewMove;
		Match3BoardGameLogic.OnPostStableBoard += HandleOnPostStableBoard;
		Match3BoardGameLogic.OnStartGame += HandleOnStartGame;
		Match3Tile.OnAnyTileDestroyed += HandleOnAnyTileDestroyed;;
	}
			
	protected void OnDestroy () 
	{
		Match3BoardGameLogic.OnPostStableBoard -= HandleOnPostStableBoard;
		Match3BoardGameLogic.OnStartGame -= HandleOnStartGame;
		Match3Tile.OnAnyTileDestroyed -= HandleOnAnyTileDestroyed;
	}

	protected IEnumerator NewMove()
	{
		if (!moving) {
			moving = true;

			Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
			BoardData boardData = gameLogic.boardData;

			TileSwitchInput.Instance.DisableInput();
			
			yield return waitEndOfFrame;
			while(BoardShuffleController.Instance.IsBoardReshuffling) {
				yield return null;
			}
			yield return waitEndOfFrame;

			gameLogic.unstableLock++;
			gameLogic.SetBoardEnabledState(false);
			bool turnInMovement = false;

			System.Action endOfFullMovementCallback = () => {
				moving = false;
				movesAccum = 0;
				gameLogic.SetBoardEnabledState(true);
				gameLogic.IsBoardStable = false;
				gameLogic.unstableLock--;
				gameLogic.TryCheckStableBoard();
				TileSwitchInput.Instance.EnableInput();
 			};

			System.Action endOfTurnMovementCallback = () => {
				turnInMovement = false;
				turnsAccum++;

				if (turnsAccum >= turnsBetweenWaves) {
					turnsAccum -= turnsBetweenWaves;

					System.Action callback = movesAccum == 0? endOfFullMovementCallback:null;
					SpawnNewSoldiersWave(callback);
				}else if (movesAccum == 0) {
					endOfFullMovementCallback();
				}
			};

			while (movesAccum > 0) {

				while (turnInMovement) {
					yield return waitEndOfFrame;
				}

				movesAccum--;

				if (!gameLogic.IsGameOver) {
					List<SoldierTile> tilesToMove = new List<SoldierTile>(boardData.NumColumns);
					for (int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++) {
						for (int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) {
							Match3BoardPiece boardPiece = boardData[rowIdx, colIdx] as Match3BoardPiece;
							SoldierTile tile = boardPiece.Tile as SoldierTile;
							if (tile != null && tile.CanMoveForward()) {
								tilesToMove.Add(tile);
							}
						}
					}

					if (tilesToMove.Count == 0) {
						endOfTurnMovementCallback();
					}else {
						for (int i = 0; i < tilesToMove.Count; i++) {
							
							SoldierTile tile = tilesToMove[i];
							System.Action callback = null;
							
							if (i == tilesToMove.Count - 1) {
								callback = endOfTurnMovementCallback;
							}

							turnInMovement = true;
							tile.MoveForward(callback);
						}
					}
				}else {
					endOfFullMovementCallback();
					break;
				}
			}
		}
	}

	protected void SpawnNewSoldiersWave (System.Action _endOfSpawnCallback = null)
	{
		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
		BoardData boardData = gameLogic.boardData;
		Match3BoardRenderer boardRenderer = gameLogic.boardRenderer as Match3BoardRenderer;

		int nSoldiers = spawnCoords.Count;
		Sequence sequence = new Sequence();

		for (int i = 0; i < nSoldiers; i++) {
			Match3BoardPiece targetBoardPiece = boardData[spawnCoords[i]] as Match3BoardPiece;
			Match3Tile existingTile = targetBoardPiece.Tile as Match3Tile;

			if (existingTile == null || 
			    (existingTile.GetType() != typeof(SoldierTile) &&  existingTile.GetType() != typeof(LockedTile) && existingTile.GetType() != typeof(ShieldTile))) {
				SoldierTile newTile = boardRenderer.SpawnSpecificTile(typeof(SoldierTile), TileColorType.None) as SoldierTile;
				Match3Tile tileToDestroy = (targetBoardPiece.Tile as Match3Tile);
				boardRenderer.AttachTileToBoardAt(targetBoardPiece, newTile, false);

				newTile.IsMoving = true;
				sequence.Append(HOTween.From(newTile.cachedTransform, spawnSoldierAnimationDuration, 
				                             new TweenParms().Prop("localScale", Vector3.zero).OnStart(() => {
					if (newTile.spawnPrefabEffect != null) {
						newTile.PlayMoveAnimation();
						Transform effectInstance = (Instantiate(newTile.spawnPrefabEffect) as GameObject).transform;
						effectInstance.position = targetBoardPiece.cachedTransform.position;
						DestroyEffect destroyEffect = effectInstance.GetComponent<DestroyEffect>();
						Destroy(effectInstance.gameObject, destroyEffect.lifeTime);
					}
				}).OnComplete(()=> {
					SoundManager.Instance.Play("soldier_appear_sfx");
					newTile.IsMoving = false;
					newTile.DestroyPreviousTile(tileToDestroy);
				})));
			}
		}

		if (_endOfSpawnCallback != null) {
			if (sequence.isEmpty) {
				_endOfSpawnCallback();
			}else {
				sequence.ApplyCallback(CallbackType.OnComplete, ()=>{
					_endOfSpawnCallback();
				});
			}
		}

		if(!sequence.isEmpty) {
			sequence.Play();
		}
	}
	

	protected int GetLowestEmptyBoardPieceRow (int _colIdx) 
	{
		BoardData boardData = Match3BoardGameLogic.Instance.boardData;
		int res = 0;
		for(int rowIdx = boardData.NumRows - 1; rowIdx >= 0; rowIdx--) {
			AbstractBoardPiece piece = boardData[rowIdx, _colIdx];
			if(piece.GetType() != typeof(EmptyBoardPiece)) {
				res = rowIdx;
				break;
			}
		}
		return res;
	}
	

	#region Event Handlers
	
	protected void HandleOnStartGame ()
	{
		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
		BoardData boardData = gameLogic.boardData;

		GameObject spawnerIndicatorPrefab = Resources.Load(spawnerIndicatorResourcePath) as GameObject;

		for (int colIdx = 0; colIdx < boardData.NumColumns; colIdx++) {
			for(int rowIdx = 0; rowIdx < boardData.NumRows; rowIdx++) {
				Match3BoardPiece piece = boardData[rowIdx, colIdx] as Match3BoardPiece;
				Match3Tile tile = piece.Tile as SoldierTile;

				if(tile != null) {
					BoardCoord cord = new BoardCoord() {
						row = GetLowestEmptyBoardPieceRow(colIdx),
						col = colIdx
					};

					spawnCoords.Add(cord);

					Transform spawnerIndicator = (GameObject.Instantiate(spawnerIndicatorPrefab) as GameObject).transform;
					spawnerIndicator.parent = boardData[boardData.NumRows - 1, colIdx].transform;
					spawnerIndicator.localPosition = spawnerIndicatorLocalPosition;
					spawnerIndicator.eulerAngles = spawnerIndicatorRotation;

					break;
				}
			}
		}
	}

	protected void HandleOnAnyTileDestroyed (Match3Tile tile)
	{
		if (tile.GetType() == typeof(SoldierTile)) {
			movesAccum = Mathf.Max(movesAccum - 1, 0);
		}
	}

	protected void HandleOnPostStableBoard ()
	{
		if(movesAccum > 0 && !moving) {
			StartCoroutine(NewMove());
		}
	}

	protected void HandleOnNewMove ()
	{
		movesAccum++;
	}


	#endregion

}
