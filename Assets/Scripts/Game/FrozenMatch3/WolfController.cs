using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WolfController : MonoBehaviour
{
	private static WolfController instance = null;
	
	public List<WolfTile> allWolves;
	
	protected float newMoveCounter = 0;
	
	protected WaitForEndOfFrame waitEndOfFrame;
	
	public int noOfWolfTilesToCreate = 0;
	
	protected int nrOfJumpingWolves = 0;

	
	// Use this for initialization
	void Awake ()
	{
		waitEndOfFrame = new WaitForEndOfFrame();
			
		if (instance == null)
		{
			instance = this;
			
			allWolves = new List<WolfTile>();
			Match3BoardGameLogic.Instance.loseConditions.OnNewMove += OnNewMove;
			Match3BoardGameLogic.OnPostStableBoard += OnPostStableBoard;
			WolfTile.OnWolfTileFinishJump += OnWolfTileFinishJump;
		}
		else {
			Destroy(this);
		}
	}
	
	public void TryJump()
	{
		
		if (allWolves.Count == 0 || newMoveCounter == 0 )  //|| wolvesJumping
		{
			return;
		}
		
//		Debug.LogWarning("[WolfTile] [JUMPING] numWolves: " + allWolves.Count);
		
		nrOfJumpingWolves = allWolves.Count;
		
		for (int i = 0; i < allWolves.Count; i++) 
		{
			allWolves[i].turnsLeftToJump--;
		}
		
		// Mark the board stable state dirty because wolves are about to jump.
		Match3BoardGameLogic.Instance.IsBoardStable = false;
		
		// Move all wolf tiles
		for (int i = 0; i < allWolves.Count; i++)
		{
			if (allWolves[i].turnsLeftToJump <= 0 && allWolves[i].currentAnimState != WolfAnimState.Destroying)
			{
				allWolves[i].wolfIndex = i;
				allWolves[i].SetAnimState(WolfAnimState.Jumping);						
			}
		}
	}
	
	public static WolfController Instance
	{
		get 
		{
			return instance;
		}
	}
	
	protected void OnNewMove()
	{
			newMoveCounter++;
	}
	
	protected void OnWolfTileFinishJump(WolfTile wolf)
	{
		nrOfJumpingWolves--;
		
		if (nrOfJumpingWolves == 0)
		{
//			Debug.LogWarning("[WolfTile] [FINISHED JUMPING]");
			
//			Match3BoardGameLogic.Instance.IsBoardStable = false;
//	 		Match3BoardGameLogic.Instance.TryCheckStableBoard();
		}
	}
		
	protected void OnPostStableBoard()
	{
		StartCoroutine(OnStableBoardCoroutine());
	}
	
	protected IEnumerator OnStableBoardCoroutine()
	{	
//		Debug.LogWarning("[WolfController] [OnStableBoardCoroutine] Started on frameCount: " + Time.frameCount);
		
		yield return new WaitForEndOfFrame();
		
		while (BoardShuffleController.Instance.IsBoardReshuffling)
		{
			yield return null;
		}
		
		if (Match3BoardGameLogic.Instance.IsGameOver)
		{
			yield break;
		}
		
		TryJump();
		
		newMoveCounter = 0;
		
		if (noOfWolfTilesToCreate > 0)
		{
			WolfTile.CreateWolves(noOfWolfTilesToCreate);
			noOfWolfTilesToCreate = 0;
			
			Match3BoardGameLogic.Instance.IsBoardStable = false;
	 		Match3BoardGameLogic.Instance.TryCheckStableBoard();
		}
		
	}
	
	public void OnDestroy()
	{
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove -= OnNewMove;
		Match3BoardGameLogic.OnPostStableBoard -= OnPostStableBoard;
		WolfTile.OnWolfTileFinishJump -= OnWolfTileFinishJump;
	}
}
