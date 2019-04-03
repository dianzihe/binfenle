using UnityEngine;
using System.Collections;

public class Crow : ManaItem {

	protected Match3Tile tileToDestroy;
	protected AbstractBoardPiece selectedBoardPiece;
	
	public BoardPieceTouchController touchController;
	public AudioClip crowSound;
	public GameObject selectionEffect;


	private Transform pivot;
	private Transform selectionEffectTransform;

	public override string ItemName {
		get {
			return "Crow";
		}
	}

	protected override void Awake()
	{
		base.Awake();
		selectionEffectTransform = (GameObject.Instantiate(selectionEffect) as GameObject).transform;
	}


	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);

		TileSwitchInput.Instance.DeactivateDrag();
		touchController.StartInputController();
		touchController.OnNewBoardPieceSelected += OnNewBoardPieceSelected;
	}
	
	public override void CancelUsingItem()
	{
		touchController.StopInputController();
		touchController.OnNewBoardPieceSelected -= OnNewBoardPieceSelected;
		
		TileSwitchInput.Instance.ActivateDrag();
		
		base.CancelUsingItem();
	}
	
	public void OnNewBoardPieceSelected(AbstractBoardPiece boardPiece, CustomInput.TouchInfo touchInfo)
	{
		if(BoardShuffleController.Instance.IsBoardReshuffling)
		{
			touchController.ClearLastSelection();
			return;
		}
		
		bool selectionSucces = false;
		
		selectedBoardPiece = boardPiece;
		tileToDestroy = boardPiece.Tile as Match3Tile;

		if(pivot == null) {
			pivot = (new GameObject("Pivot")).transform;
			pivot.parent = transform;
		}
		pivot.position = boardPiece.cachedTransform.position;

		effectPosition = pivot;
		effectPosition.transform.position = MaleficentTools.ConvertPositionBetweenLayers(effectPosition.transform.position, "DropTile", "Effects", 10.0f);


		if(boardPiece.Tile == null)
		{
			if(boardPiece is LayeredBoardPiece && (boardPiece as LayeredBoardPiece).NumLayers > 0 )
			{
				selectionSucces = true;
			}
		}
		else if (!tileToDestroy.IsMoving && tileToDestroy.IsDestructible && !tileToDestroy.IsDestroying && !(tileToDestroy as NormalTile).IsFrozen()) 
		{
			selectionSucces = true;
		}
		
		if(selectionSucces)
		{
			selectionEffectTransform.position = selectedBoardPiece.cachedTransform.position;
			selectionEffectTransform.gameObject.SetActive(true);

			SoundManager.Instance.Play("powerup_crow_sfx");
			AvatarParticles.Instance.ActivateEffect(AvatarParticlesEffectType.Crow);

			MaleficentMatch3Animations.PerformTileUserSelectionAnimation(boardPiece.Tile);

			touchController.StopInputController();
			touchController.OnNewBoardPieceSelected -= OnNewBoardPieceSelected;
			
			IngameCrow ingameCrow = GameObject.FindObjectOfType< IngameCrow >();
			if(ingameCrow != null) {
				//We need to call this here so the item cannot be cancelled
				if(OnCancelFinish != null) {
					OnCancelFinish();
					OnCancelFinish = null;
				}
			
				//Wail until Diaval leaves
				ingameCrow.FlyAway(StartItemEffects);
			} else {
				StartItemEffects();
			}
		}
	}
	
	protected override void DoItem()
	{
		if (tileToDestroy)
		{
			tileToDestroy.Destroy();
		}
		else if(selectedBoardPiece is LayeredBoardPiece)
		{
			(selectedBoardPiece as LayeredBoardPiece).NumLayers--;
		}
		
		TileSwitchInput.Instance.ActivateDrag();

		Destroy(selectionEffectTransform.gameObject);
		selectionEffectTransform = null;


		base.DoItem();
	}
	
	protected override void FinishUsingItem()
	{
		IngameCrow ingameCrow = GameObject.FindObjectOfType< IngameCrow >();
		if(ingameCrow != null) {
			MaleficentTools.DoAfterSeconds(this, 1.5f, () => {
				//Wail until Diaval gets back
				ingameCrow.GetBack(base.FinishUsingItem);
			});
		} else {
			base.FinishUsingItem();
		}
	}
	
	protected override void OnDestroy ()
	{
		base.OnDestroy ();
		
		touchController.StopInputController();
		touchController.OnNewBoardPieceSelected -= OnNewBoardPieceSelected;

		if (selectionEffectTransform) {
			Destroy(selectionEffectTransform.gameObject);
		}
	}
}
