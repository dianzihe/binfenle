using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class Torch : BasicItem {
	
	public GameObject prefabSelectionEffect;
		
	public float selectionAnimationSpeed = 1f;
	
	public float torchMovementSpeed = 5f;
	
	public float postSelectionWaitTime = 0.5f;
	
	public float postFadeInWaitTime = 0.2f;
	public float preFadeOutWaitTime = 0.2f;
	
	public int maxNumberOfSelections = 10;
	
	[System.NonSerialized]
	public string fadeInAnimation = "effect_torch_fadeIn";
	
	protected int critNumberOfSelections;
	
	protected BoardPieceTouchController touchController;
	
	protected List<AbstractBoardPiece> selectionList;
	
	protected int selectionCount = 0;
	
	protected List<Transform> selectionEffectList;
	
	protected Transform torchEffectInstance;
	
	protected Animation torchEffectAnimation;
	
	protected int pathIndex = 0;
	
	
	protected override void Awake()
	{
		base.Awake();
		
		critNumberOfSelections = maxNumberOfSelections * 2;
		
		selectionList = new List<AbstractBoardPiece>(10);
		selectionEffectList = new List<Transform>();
		selectionCount = 0;
		
		touchController = GetComponent<BoardPieceTouchController>();
		touchController.OnNewBoardPieceSelected += OnNewBoardPieceSelected;
	}
	
	public override string ItemName {
		get {
			return "Torch";
		}
	}

	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		
		PossibleMatchesController.Instance.EnabledCount--;
		TileSwitchInput.Instance.DisableInput();
	}
	
	public override void CancelUsingItem()
	{	
		RemoveAllSelections();
		
		PossibleMatchesController.Instance.EnabledCount++;
		TileSwitchInput.Instance.EnableInput();
		
		base.CancelUsingItem();
	}
	
	protected IEnumerator DoItemCoroutine()
	{
		yield return new WaitForSeconds(postSelectionWaitTime);
		
		ActuallyUsingItem();
		
		SoundManager.Instance.PlayOneShot("torch_sfx");
		
		//Lock boardPieces
		Match3BoardGameLogic.Instance.boardData.ApplyActionToAll( (boardPiece) => {
			(boardPiece as Match3BoardPiece).BlockCount++;
		});
		
		for(int i = 0; i < selectionEffectList.Count; i++)
		{
			if(selectionEffectList[i] != null)
			{
				GameObject.Destroy(selectionEffectList[i].gameObject);
			}
		}
		
		selectionEffectList.Clear();
		selectionCount = 0;
		
		torchEffectInstance = (GameObject.Instantiate(effectPrefab, selectionList[pathIndex].cachedTransform.position + effectOffset, Quaternion.identity) as GameObject).transform;
		torchEffectAnimation = torchEffectInstance.GetComponent<Animation>();
		
		torchEffectAnimation.Play();
		
		yield return new WaitForSeconds(torchEffectAnimation.clip.length + postFadeInWaitTime);
		
		if (selectionList[pathIndex].Tile != null)
		{
			selectionList[pathIndex].Tile.Destroy();
		}
		
		HOTween.To(torchEffectInstance, torchMovementSpeed, 
											new TweenParms().Prop("position", selectionList[pathIndex+1].transform.position + effectOffset)
											.Ease(EaseType.Linear)
											.SpeedBased()
											.OnComplete(ActionOnTweenComplete)
										 );
	}
	
	protected void ActionOnTweenComplete()
	{
		pathIndex++;
		AbstractTile tile = selectionList[pathIndex].Tile;
		
		if(tile != null)
		{
			selectionList[pathIndex].Tile.Destroy();
		}
		
		if(pathIndex == selectionList.Count - 1)
		{	
			PossibleMatchesController.Instance.EnabledCount++;
			
			Match3BoardGameLogic.Instance.boardData.ApplyActionToAll( (boardPiece) => {
				(boardPiece as Match3BoardPiece).BlockCount--;
			});
			
			Match3BoardGameLogic.Instance.boardData.ApplyActionToAll( (boardPiece) => {
				(boardPiece as Match3BoardPiece).UpdateOrphanState();
			});
			
			StartCoroutine(EndOfPathMovement());
			
			return;
		}
		
		HOTween.To(torchEffectInstance, torchMovementSpeed, 
				   new TweenParms().Prop("position", selectionList[pathIndex+1].transform.position + effectOffset)
        		   .Ease(EaseType.Linear)
 				   .SpeedBased()
       			   .OnComplete(ActionOnTweenComplete));
	}
	
	protected IEnumerator EndOfPathMovement()
	{
		yield return new WaitForSeconds(preFadeOutWaitTime);
		
		torchEffectAnimation[fadeInAnimation].normalizedTime = 1f;
		torchEffectAnimation[fadeInAnimation].speed = -1f;
		torchEffectAnimation.Play(fadeInAnimation);
		
		yield return new WaitForSeconds(torchEffectInstance.GetComponent<Animation>().clip.length);
		
		GameObject.Destroy(torchEffectInstance.gameObject);
		
//		FinishUsingItem();
		base.DoItem();
		TileSwitchInput.Instance.EnableInput();
		DoDestroy();
	}
	
	protected void OnNewBoardPieceSelected(AbstractBoardPiece boardPiece, CustomInput.TouchInfo touchInfo)
	{	
		if (IsRunning)
		{
			return;
		}
		
		if (BoardShuffleController.Instance.IsBoardReshuffling) 
		{
			touchController.ClearLastSelection();
			return;
		}
		
		bool isLinked = false;
		AbstractBoardPiece lastSelection = null;
		
		//Cache reference to the last selection
		if(selectionList.Count > 0)
		{
			lastSelection = selectionList[selectionList.Count-1];
		}
		
//		if(boardPiece.GetType() != typeof(EmptyBoardPiece))
//		{
		//Undo selection
		if (selectionList.Count >= 2 && selectionList[selectionList.Count-2] == boardPiece)
		{	
			RemoveSelection(selectionList.Count-1);
			return;
		}
		
		//Check link between new and last selection
		if (lastSelection) {
			isLinked = lastSelection.IsAdjacentTo(boardPiece);
		}
		
		//Add current tile to the selection or start a new one
		if(selectionCount < maxNumberOfSelections) // && !selectionList.Contains(boardPiece)
		{
			if(!isLinked) 
			{
				if(selectionList.Contains(boardPiece))
				{
					return;
				}
				RemoveAllSelections();
			}
			
			AddSelection(boardPiece);
		}
		
		if(selectionCount == maxNumberOfSelections || selectionList.Count == critNumberOfSelections)
		{
			touchController.StopInputController();
			StartCoroutine(DoItemCoroutine());
			IsRunning = true;
		}
//		}
	}
	
	protected void AddSelection(AbstractBoardPiece boardPiece)
	{
		Transform selection = null;
//		Debug.LogWarning("Add selection: " + boardPiece.BoardPosition.row + " " + boardPiece.BoardPosition.col);
		
		if(!selectionList.Contains(boardPiece))
		{
			selection = (GameObject.Instantiate(prefabSelectionEffect) as GameObject).transform;
			
			selection.parent = boardPiece.cachedTransform;
			selection.localPosition = new Vector3(0f, 0f, -1f);
			selection.GetComponent<Animation>()["effect_nextitem3"].speed = selectionAnimationSpeed;
			
			if ( !(boardPiece is EmptyBoardPiece) || boardPiece.Tile != null )
			{
				selectionCount++;
			}	
		}
		
		selectionEffectList.Add(selection);
		selectionList.Add(boardPiece);
		
	}
	
	protected void RemoveSelection(int targetIndex)
	{	
//		Debug.LogWarning("Remove selection: " + selectionList[targetIndex].BoardPosition.row + " " + selectionList[targetIndex].BoardPosition.col);
		
		if(selectionEffectList[targetIndex])
		{	
			GameObject.Destroy(selectionEffectList[targetIndex].gameObject);         
			selectionCount--;
		}
		
		selectionList.RemoveAt(targetIndex);
		selectionEffectList.RemoveAt(targetIndex);
	}
	
	protected void RemoveAllSelections()
	{	
		for( int i = 0; i < selectionList.Count; i++)
		{
			
			if(selectionEffectList[i])
			{
				GameObject.Destroy(selectionEffectList[i].gameObject);
			}
		}
		
		selectionEffectList.Clear();
		selectionList.Clear();
		selectionCount = 0;
	}
}
