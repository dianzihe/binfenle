using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreezerController : MonoBehaviour {
	
	//Singleton Instance
	private static FreezerController instance = null;
	
	public List<FreezerTile> allFreezers;
	public bool freezerDestroyedThisTurn = false;
	
	protected WaitForEndOfFrame waitEndOfFrame;
	
	protected float newMoveCounter = 0;
	
	//Singleton Property
	public static FreezerController Instance
	{
		get
		{
			return instance;
		}
	}
	
	public void Awake()
	{
		waitEndOfFrame = new WaitForEndOfFrame();
			
		if(instance == null)
		{
			instance = this;
			allFreezers = new List<FreezerTile>(20);
			Match3BoardGameLogic.Instance.loseConditions.OnNewMove += OnNewMove;
			Match3BoardGameLogic.OnPostStableBoard += OnPostStableBoard;
		}
		else
		{
			Destroy (this);
		}
	}
	
	protected void OnNewMove()
	{
		newMoveCounter++;
		
		if(newMoveCounter > 1 )
		{
			OnPostStableBoard();
		}
	}
	
	protected void OnPostStableBoard()
	{
		if(newMoveCounter == 0)
		{
			return;
		}
		else
		{
			newMoveCounter--;
		}
		
		StartCoroutine(OnStableBoardCoroutine());
	}
	
	protected IEnumerator OnStableBoardCoroutine()
	{	
		yield return new WaitForEndOfFrame();
		
		while(BoardShuffleController.Instance.IsBoardReshuffling)
		{
			yield return null;
		}
		
		int index = Random.Range(0, allFreezers.Count);
		int count = 0;
		FreezerTile previousCandidate = null;
		
		if(freezerDestroyedThisTurn || Match3BoardGameLogic.Instance.IsGameOver)
		{
			freezerDestroyedThisTurn = false;
			yield break;
		}
		
		while(count < allFreezers.Count)
		{
//				Debug.LogWarning("Trying out index: " + index + "out of " + allFreezers.Count);
				allFreezers[index].RefreshValidIndexList();
//				Debug.LogWarning("Count: " + allFreezers[index].validReplicationList.Count);
				
				if(allFreezers[index].validReplicationList.Count != 0)
				{
					allFreezers[index].Replicate();
					yield break;
				}
				else
				{
					if(allFreezers[index].validReplicationListSpecial.Count != 0)
					{
						previousCandidate = allFreezers[index];
					}
					index++;
					index = index % allFreezers.Count;
					count++;
				}
		}
		
		if(previousCandidate != null)
		{
			previousCandidate.Replicate();
		}
	}
	
	public void OnDestroy()
	{
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove -= OnNewMove;
		Match3BoardGameLogic.OnPostStableBoard -= OnPostStableBoard;
	}
}
