using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class HotChocolate : BasicItem
{	
	public static event System.Action<HotChocolate> OnChocolateBeginFlowing;
	public static event System.Action<HotChocolate> OnChocolateStopFlowing;
	
	public GameObject prefabSelectionEffect;
	
	public GameObject hotChocolateMelter;
	
	public GameObject hotChocolateEffect;
	
	protected Vector3 hotChocolateEffectOffset = Vector3.up * 0.5f + Vector3.forward * - 8f;
	
	public GameObject hotChocolateEffect_Cup;
	
	protected int finishedTriggerCount = 0;
	public List<HotChocolateTriggerItem> hotChocolateTriggerItems = new List<HotChocolateTriggerItem>();
	public List<GameObject> hotChocolateMeltEffects = new List<GameObject>();
	
	public float selectionAnimationSpeed = 1f;
	
	public float postSelectionWaitTime = 0f;
	public float postEffectWaitTime = 0.2f;
	
	protected int maxNumberOfSelections = 1;
	
	protected AbstractBoardPiece selection;
	
	protected int boardNumRows;
	
	
	protected override void Awake()
	{
		base.Awake();
		boardNumRows = Match3BoardGameLogic.Instance.boardData.NumRows;
	}
	
	public override string ItemName 
	{
		get {
			return "HotChocolate";
		}
	}

	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		
		PossibleMatchesController.Instance.EnabledCount--;
		
		TileSwitchInput.Instance.DeactivateDrag();
		Match3Tile.OnTileTap += OnTileSelected;
	}
	
	protected IEnumerator StartUsingItem()
	{
		yield return null;
	}
	
	public override void CancelUsingItem()
	{	
//		RemoveAllSelections();
		
		PossibleMatchesController.Instance.EnabledCount++;
		TileSwitchInput.Instance.ActivateDrag();
		Match3Tile.OnTileTap -= OnTileSelected;	
		
		base.CancelUsingItem();
	}
	
	protected IEnumerator DoItemCoroutine()
	{
		yield return new WaitForSeconds(postSelectionWaitTime);
		yield return null;
		
		ActuallyUsingItem();
		
		//Lock boardPieces
		Match3BoardGameLogic.Instance.boardData.ApplyActionToAll( (boardPiece) => {
			(boardPiece as Match3BoardPiece).BlockCount++;
		});
		
		GameObject hotChocolateTriggerItemInstance = Instantiate(hotChocolateMelter, selection.cachedTransform.position, Quaternion.identity) as GameObject;
		HotChocolateTriggerItem hotChocolateTriggerItem = hotChocolateTriggerItemInstance.GetComponent<HotChocolateTriggerItem>();
		hotChocolateTriggerItems.Add(hotChocolateTriggerItemInstance.GetComponent<HotChocolateTriggerItem>());
		
		GameObject hotChocolateEffectInstance = Instantiate(hotChocolateEffect, selection.cachedTransform.position + hotChocolateEffectOffset, Quaternion.Euler(270f, 0f, 0f)) as GameObject;
		hotChocolateMeltEffects.Add(hotChocolateEffectInstance);
		hotChocolateEffectInstance.transform.parent = hotChocolateTriggerItemInstance.transform;
		hotChocolateTriggerItem.SetEffect(hotChocolateEffectInstance.transform, hotChocolateEffectInstance.transform.GetChild(0).GetComponent<ParticleSystem>());
		
		AdjustTriggerItems();
		
		RaiseOnChocolateBeginFlowing(this);
		
		for( int i = 0;  i < hotChocolateTriggerItems.Count; i++)
		{
			hotChocolateTriggerItems[i].OnTriggerFinishedMove += ActionOnTriggerFinishedMove;
			StartCoroutine(hotChocolateTriggerItems[i].Move());
		}
		
		while(finishedTriggerCount < hotChocolateTriggerItems.Count)
		{
			yield return null;
		}
		
		for(int i = 0; i < hotChocolateTriggerItems.Count; i++)
		{
			Destroy(hotChocolateTriggerItems[i].gameObject);
		}
		
		Match3BoardGameLogic.Instance.boardData.ApplyActionToAll( (boardPiece) => {
			(boardPiece as Match3BoardPiece).BlockCount--;
		});
		
		Match3BoardGameLogic.Instance.boardData.ApplyActionToAll( (boardPiece) => {
			(boardPiece as Match3BoardPiece).UpdateOrphanState();
		});
		
		StartCoroutine(EndOfItemBehaviour());
	}
	
	public void ActionOnTriggerFinishedMove(HotChocolateTriggerItem sender)
	{
		sender.OnTriggerFinishedMove -= ActionOnTriggerFinishedMove;
		finishedTriggerCount++;
	}
	
	protected IEnumerator EndOfItemBehaviour()
	{
		yield return new WaitForSeconds(postEffectWaitTime);
		
		RaiseOnChocolateStopFlowing(this);
		
		PossibleMatchesController.Instance.EnabledCount++;
		TileSwitchInput.Instance.ActivateDrag();
		
//		FinishUsingItem();
		base.DoItem();		
		
		DoDestroy();
	}
	
	/// <summary>
	/// Called after the input has been succesfully received.
	/// </summary>
//	protected void OnInputReceived()
//	{
//		Match3Tile.OnTileTap -= OnTileSelected;		
//		StartCoroutine(DoItemCoroutine());
//		isRunning = true;
//	}
	
	/// <summary>
	/// Adjusts the trigger items. Based on the length of the selection, manually specify the distance each triggerItem must fall.
	/// </summary>
	protected void AdjustTriggerItems()
	{
		for(int i = 0;  i < hotChocolateTriggerItems.Count; i++)
		{
			hotChocolateTriggerItems[i].numTilesTraveled = boardNumRows - selection.BoardPosition.row;
		}
		return;
	}
	
	/// <summary>
	/// This method handles and interprets any selected boardPiece. This method is registered to the TouchController.OnNewBoardPieceSelected Event.
	/// </param>
	protected void OnTileSelected(AbstractTile sender)
	{	
		if (BoardShuffleController.Instance.IsBoardReshuffling)
		{
			return;
		}
		
		selection = sender.BoardPiece;
		
		Match3Tile.OnTileTap -= OnTileSelected;	
		StartCoroutine(DoItemCoroutine());
		
		IsRunning = true;
	}
	
	protected void RaiseOnChocolateBeginFlowing(HotChocolate sender)
	{
		if(OnChocolateBeginFlowing != null)
		{
			OnChocolateBeginFlowing(sender);
		}
	}
	
	protected void RaiseOnChocolateStopFlowing(HotChocolate sender)
	{
		if(OnChocolateStopFlowing != null)
		{
			OnChocolateStopFlowing(sender);
		}
	}
	
	protected override void OnDestroy ()
	{
		base.OnDestroy ();
		
		Match3Tile.OnTileTap -= OnTileSelected;
	}
}
