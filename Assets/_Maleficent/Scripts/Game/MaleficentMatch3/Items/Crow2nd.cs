using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class Crow2nd : ManaItem {

	public GameObject crowEffect;
	public GameObject swipeEffect;
	public AudioClip effectName;
	public AudioClip effectNameSwipe;
	public float swipeEffectDelay = 0.3f;

	private float animationStepDuration;
	private BoardPieceTouchController touchController;
	
	private List<Transform> highlightEffects;

	private AbstractBoardPiece firstSelectedBoardPiece;
	private AbstractBoardPiece secondSelectedBoardPiece;
	private Transform firstSelectionHighlightEffect;
	private Transform secondSelectionHighlightEffect;

	bool previousTile1SwitchBackStatus;
	bool previousTile2SwitchBackStatus;

	protected override void Awake () {
		base.Awake();
		touchController = GetComponent<BoardPieceTouchController>();

		firstSelectionHighlightEffect = (Instantiate(effectPrefab) as GameObject).transform;
		firstSelectionHighlightEffect.gameObject.SetActive(false);

		secondSelectionHighlightEffect = (Instantiate(effectPrefab) as GameObject).transform;
		secondSelectionHighlightEffect.gameObject.SetActive(false);

		animationStepDuration = ((effectPrefab.GetComponent<DestroyEffect>()).destroyTileTime)/2f;

		effectPrefab = null;
	}

	public override string ItemName {
		get {
			return "Crow2nd";
		}
	}

	protected override void OnDestroy ()
	{
		IngameCrow crow = GameObject.FindObjectOfType< IngameCrow >();
		if(crow != null) {
			crow.GetBack();
		}
		
	
		base.OnDestroy ();

		if(firstSelectionHighlightEffect != null) {
			Destroy(firstSelectionHighlightEffect.gameObject);
		}

		if (secondSelectionHighlightEffect != null) {
			Destroy(secondSelectionHighlightEffect.gameObject);
		}
		
		touchController.StopInputController();
		touchController.OnNewBoardPieceSelected -= HandleOnNewBoardPieceSelected;
	}

	public override bool CanBeUsed()
	{
		return !BoardShuffleController.Instance.IsBoardReshuffling;
	}

	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		effectPosition = transform;

		TileSwitchInput.Instance.DeactivateDrag();

//		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
//		gameLogic.unstableLock++;
//		gameLogic.TryCheckStableBoard();

		touchController.StartInputController();
		touchController.OnNewBoardPieceSelected += HandleOnNewBoardPieceSelected;
	}

	public override void CancelUsingItem()
	{
		touchController.StopInputController();

//		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
//		gameLogic.unstableLock--;
//		gameLogic.TryCheckStableBoard();

		touchController.OnNewBoardPieceSelected -= HandleOnNewBoardPieceSelected;
		
		TileSwitchInput.Instance.ActivateDrag();
		
		base.CancelUsingItem();
	}

	void HandleOnNewBoardPieceSelected (AbstractBoardPiece boardPiece, CustomInput.TouchInfo touchInfo)
	{
		if(!CanBeUsed()) {
			touchController.ClearLastSelection();
			return;
		}
		
		bool selectionSucces = false;
		
		AbstractBoardPiece selectedBoardPiece = boardPiece;
		Match3Tile tileToDestroy = boardPiece.Tile as Match3Tile;
		
	
		if(boardPiece.Tile != null)
		{
			bool tileIsStable = !tileToDestroy.IsMoving && !tileToDestroy.IsDestroying;
			if (tileToDestroy.IsUserMoveable && tileIsStable && !(tileToDestroy is DropTile)) 
			{
				selectionSucces = true;
			}
		}

		
		if(selectionSucces)
		{
			//SoundManager.Instance.PlayOneShot("icepick_sfx");
			SoundManager.Instance.PlayOneShot(effectName);

			MaleficentMatch3Animations.PerformTileUserSelectionAnimation(boardPiece.Tile);
		
			if(firstSelectedBoardPiece == null) {
				touchController.StopInputController();
				ShowCrow(selectedBoardPiece, false, () => {
					touchController.StartInputController();

					firstSelectedBoardPiece = selectedBoardPiece;
					firstSelectionHighlightEffect.position = firstSelectedBoardPiece.cachedTransform.position;
					firstSelectionHighlightEffect.gameObject.SetActive(true);
				});
			}else if (firstSelectedBoardPiece != selectedBoardPiece) {
				touchController.StopInputController();
				ShowCrow(selectedBoardPiece, true, () => {
					secondSelectedBoardPiece = selectedBoardPiece;
					secondSelectionHighlightEffect.position = secondSelectedBoardPiece.cachedTransform.position;
					secondSelectionHighlightEffect.gameObject.SetActive(true);

					touchController.OnNewBoardPieceSelected -= HandleOnNewBoardPieceSelected;

					SoundManager.Instance.PlayOneShot(effectNameSwipe);

					AvatarParticles.Instance.ActivateEffect(AvatarParticlesEffectType.CrowSwipe);
					StartItemEffects();
				});
			}
		}
	}

	private void ShowCrow(AbstractBoardPiece _boardPiece, bool _invert, System.Action _onFinish) {
		GameObject crowEffectInstance = GameObject.Instantiate(crowEffect) as GameObject;
		if(_invert) {
			crowEffectInstance.transform.localScale = new Vector3(crowEffectInstance.transform.localScale.x, crowEffectInstance.transform.localScale.y, -crowEffectInstance.transform.localScale.z);
			SoundManager.Instance.Play("crow_fly_right_sfx");
		}else {
			SoundManager.Instance.Play("crow_fly_left_sfx");
		}

		crowEffectInstance.GetComponent<Animation>()["_01"].speed = 2.0f;
		crowEffectInstance.transform.position = MaleficentTools.ConvertPositionBetweenLayers(
			_boardPiece.transform.position, 
			"Default", 
			"Effects", 
			10.0f
		);
		Destroy(crowEffectInstance, 3.0f);


		MaleficentTools.DoAfterSeconds(this, 1.0f, _onFinish);
	} 

	public override void ActuallyUsingItem ()
	{
		base.ActuallyUsingItem();

		Match3Tile tile1 =  firstSelectedBoardPiece.Tile as Match3Tile;
		Match3Tile tile2 =  secondSelectedBoardPiece.Tile as Match3Tile;
		tile1.IsMoving = true;
		tile1.IsTileSwitching = true;
		tile1.GravityEnabled = false;
		previousTile1SwitchBackStatus = tile1.SwitchBackOnMatchFail;
		tile1.SwitchBackOnMatchFail = false;
		tile2.IsMoving = true;
		tile2.IsTileSwitching = true;
		tile2.GravityEnabled = false;
		previousTile2SwitchBackStatus = tile2.SwitchBackOnMatchFail;
		tile2.SwitchBackOnMatchFail = false;

		Vector3 tile1OriginalScale = tile1.cachedTransform.localScale;
		Vector3 tile2OriginalScale = tile2.cachedTransform.localScale;

		SoundManager.Instance.Play("powerup_crowswipe_sfx");
		HOTween.To (tile1.cachedTransform, animationStepDuration, "localScale", Vector3.zero);

		Tweener scaleOut2nd = HOTween.To (tile2.cachedTransform, animationStepDuration, new TweenParms().Prop("localScale", Vector3.zero));

		scaleOut2nd.ApplyCallback(CallbackType.OnComplete, () =>{
			Vector3 position = tile1.LocalPosition;
			tile1.LocalPosition = tile2.LocalPosition;
			tile2.LocalPosition = position;

			HOTween.To (tile1.cachedTransform, animationStepDuration, "localScale", tile1OriginalScale);
			Tweener scaleIn2nd = HOTween.To (tile2.cachedTransform, animationStepDuration, new TweenParms().Prop("localScale", tile2OriginalScale));

			scaleIn2nd.ApplyCallback(CallbackType.OnComplete, () =>{
				Match3BoardGameLogic.Instance.BoardAnimations.RaiseEventTilesSwitchAnimFinished(true,Match3BoardGameLogic.Instance.BoardAnimations,tile1,tile2);
				tile1.RaiseEventTileSwitchAnimEnded(tile2);
				tile2.RaiseEventTileSwitchAnimEnded(tile1);
				tile1.SwitchBackOnMatchFail = previousTile1SwitchBackStatus;
				tile2.SwitchBackOnMatchFail = previousTile2SwitchBackStatus;
			});
		});
	}
	
	protected override void DoItem()
	{
		TileSwitchInput.Instance.ActivateDrag();

//		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
//		gameLogic.unstableLock--;
//		gameLogic.TryCheckStableBoard();

		firstSelectedBoardPiece = null;
		secondSelectedBoardPiece = null;
		base.DoItem();
	}

	public override void StartItemEffects()
	{
		StartCoroutine(_ShowSwipeEffectDelayed(swipeEffectDelay));

		base.StartItemEffects();
	}

	private IEnumerator _ShowSwipeEffectDelayed(float time)
	{
		yield return new WaitForSeconds(time);

		Vector3[] positions = {firstSelectedBoardPiece.transform.position, secondSelectedBoardPiece.transform.position};
		foreach (Vector3 position in positions) {
			GameObject selectionEffect = GameObject.Instantiate(swipeEffect) as GameObject;
			DestroyEffect selectionDestroy = selectionEffect.GetComponent<DestroyEffect>();
			//firstSelectionEffect.transform.position = MaleficentTools.ConvertPositionBetweenLayers(position, "BoardTile", "Effects", 10);
			selectionEffect.transform.position = position;
			
			
			Destroy (selectionEffect, selectionDestroy.lifeTime);
		}
	}
}
