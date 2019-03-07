using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class Carrot : BasicItem
{	
	protected Transform shakeTarget;
	
	public AudioClip carrotThumpingAudioClip;
	
	public int nrOfTilesPerTurn = 3;
	public int nrOfTurns = 3;
	public float shakeDuration = 0.3f;
	
	public float[] delayBetweenTurns;
	
	public float postEffectWaitTime = 0.1f;
	
	protected List<NormalTile> tileList;
	protected List<NormalTile> prioritizedTileList;
	
	// Value between 1 and 100 (percentage)
	public int priorityListBias = 80;
	
	protected int randomTileIndex;
	
	public bool carrotBeginThumping = false;
	
	public override string ItemName
	{
		get {
			return "Carrot";
		}
	}
   
    protected override void Awake()
	{
		base.Awake();
		
		SoundManager.Instance.RegisterSound(new SoundEffect(carrotThumpingAudioClip, "carrot_thumping", 0));
	}
	
	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		
		tileList = new List<NormalTile>(10);
		prioritizedTileList = new List<NormalTile>();
		
		shakeTarget = Match3BoardGameLogic.Instance.match3Board;
		
//		TileSwitchInput.Instance.gameObject.SetActive(false);
		TileSwitchInput.Instance.DisableInput();
		
		if (tileList.Count != 0)
		{
			tileList.Clear();
		}
		
		PopulateTileList();
		DoItem();
	}
	
	public override void CancelUsingItem()
	{
//		TileSwitchInput.Instance.gameObject.SetActive(true);
		TileSwitchInput.Instance.EnableInput();
		
		base.CancelUsingItem();
	}
	
	public override bool CanBeUsed()
	{
		return !BoardShuffleController.Instance.IsBoardReshuffling;
	}
	
	protected override void DoItem()
	{
		ActuallyUsingItem();
		StartCoroutine(CarrotBehaviour());
	}
	
	protected IEnumerator CarrotBehaviour()
	{	
		int listBiasFactor;
		/*
		if(CharacterSpecialAnimations.CharIdx == 2)
		{
			CharacterSpecialAnimations.instance.characterFSM.SendEvent("PauseAnimation");
			CharacterSpecialAnimations.instance.characterFSM.SendEvent("Custom");
			CharacterSpecialAnimations.instance.characterFSM.FsmVariables.GetFsmGameObject("Character").Value.GetComponent<Animation>().CrossFade("usepower_01",  0.1f, PlayMode.StopAll);
		}
		*/
		for(int turnIndex = 0; turnIndex < nrOfTurns; turnIndex++)
		{
			yield return new WaitForSeconds(delayBetweenTurns[turnIndex]);
			
			PlayCarrotThumpingSnd();
			
//			Debug.LogError("EffectPosition: " + effectPosition.name);
			HOTween.Shake(shakeTarget, shakeDuration, new TweenParms().Prop("position", shakeTarget.position + Vector3.down * 0.25f), 1f, 1f);
			
			for(int i = 0 ; i < effectPosition.childCount; i++)
			{
				Transform t = effectPosition.GetChild(i);
				HOTween.Shake(t, shakeDuration, new TweenParms().Prop("position", t.position + Vector3.down * 0.06f), 0.5f, 1f);
			}
			
			for(int tileIndex = 0; tileIndex < nrOfTilesPerTurn; tileIndex++)
			{
				listBiasFactor = Random.Range(1, 101);
				
				if(listBiasFactor < priorityListBias && prioritizedTileList.Count != 0)
				{
					randomTileIndex = Random.Range(0, prioritizedTileList.Count);
					prioritizedTileList[randomTileIndex].Destroy();
					prioritizedTileList.RemoveAt(randomTileIndex);
				}
				else
				{
					randomTileIndex = Random.Range(0, tileList.Count);
					tileList[randomTileIndex].Destroy();
					tileList.RemoveAt(randomTileIndex);
				}
			}	
		}
		
		yield return new WaitForSeconds(postEffectWaitTime);
		//CharacterSpecialAnimations.instance.characterFSM.SendEvent("FinishedCustom");
		
		base.DoItem();
		
//		TileSwitchInput.Instance.gameObject.SetActive(true);
		TileSwitchInput.Instance.EnableInput();
		
		DoDestroy();
	}
	
	protected void PopulateTileList()
	{
		Match3BoardGameLogic.Instance.boardData.ApplyActionToAll((boardPiece) => 
		{
			NormalTile tile = boardPiece.Tile as  NormalTile;
			
			if (tile)
			{
				if (IsPrioritizedTile(tile))
				{
					prioritizedTileList.Add(tile);
				}
				
				if (tile.GetType() == typeof(NormalTile))
				{
					tileList.Add(tile);
				}
			}
		});
	}
	
	protected bool IsPrioritizedTile(NormalTile tile)
	{
		if (tile is SnowTile || tile is FreezerTile || tile is LockedTile || tile is WolfTile)
		{
			return true;
		}
		
		return false;
	}
	
	protected void PlayCarrotThumpingSnd()
	{	
		if(carrotBeginThumping == false)
		{
			carrotBeginThumping = true;
			SoundManager.Instance.PlayOneShot(SoundManager.Instance["carrot_thumping"]);
		}
	}
}
