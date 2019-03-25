using UnityEngine;
using System.Collections;

public class WinDestroyTilesWolf : WinDestroyTiles
{
//	[System.NonSerialized]
//	public int numWolfTilesOnBoard = 0;
	[System.NonSerialized]
	public int numWolfTilesToDestroy = 0;
	
	public bool isNormalWolf = true;

	public int maxWolvesOnBoard = 1;
	
	public bool autoCreateWolves = true;
	
	protected override void Awake()
	{
		base.Awake();

		WolfTile.OnWolfTileCreated += OnWolfTileCreated;
		WolfTile.OnWolfTileBeginDestroy += OnWolfTileDestroyed;
	}
	
	public void OnStart()
	{
		int maxWolvesOnBoard = (Match3BoardRenderer.Instance.winConditions as WinDestroyTilesWolf).maxWolvesOnBoard;
		numWolfTilesToDestroy = destroyTiles[0].number;
		
		WolfTile.isNormalWolf = isNormalWolf;
		
		if (autoCreateWolves)
		{
			WolfTile.CreateWolves(maxWolvesOnBoard);
		}
		
		Match3BoardGameLogic.OnStartGame -= OnStart;
	}
	
	protected override void Start()
	{
		base.Start();
		Match3BoardGameLogic.OnStartGame += OnStart;
	}
	
	protected void OnWolfTileCreated(WolfTile wolfTile)
	{
//		Debug.LogWarning("[WinDestroyTilesWolf] [OnWolfTileCreated] " + wolfTile.name);
		
		int numWolfTilesOnBoard = WolfController.Instance.allWolves.Count;
		
		if(numWolfTilesOnBoard < maxWolvesOnBoard)
		{
			WolfController.Instance.noOfWolfTilesToCreate = Mathf.Min(maxWolvesOnBoard - numWolfTilesOnBoard, numWolfTilesToDestroy - numWolfTilesOnBoard);
		} 
		else
		{
			WolfController.Instance.noOfWolfTilesToCreate = 0;
		}
	}
	
	protected void OnWolfTileDestroyed(WolfTile wolfTile) 
	{	
//		Debug.LogWarning("[WinDestroyTilesWolf] [OnWolfTileDestroyed] " + wolfTile.name);
		
		int numWolfTilesOnBoard = WolfController.Instance.allWolves.Count;
		numWolfTilesToDestroy--;
		
		if(numWolfTilesOnBoard < maxWolvesOnBoard)
		{
			WolfController.Instance.noOfWolfTilesToCreate = Mathf.Min(maxWolvesOnBoard - numWolfTilesOnBoard, numWolfTilesToDestroy - numWolfTilesOnBoard);
		} 
		else 
		{
			WolfController.Instance.noOfWolfTilesToCreate = 0;
		}	
	}
	
	public override string GetLoseReason() 
	{
		if ( AllDestroyed() ) {
			return base.GetLoseReason();
		}
		else {
			return Language.Get("LOSE_WOLVES");
		}
	}
	
	public override string GetObjectiveString()	
	{
		return Language.Get("GAME_OBJECTIVE_WOLVES");
	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions) 
	{
		return Language.Get("MAP_OBJECTIVE_WOLVES");
	}
	
	public override string GetLevelType (AbstractLoseCondition loseConditions) 
	{
		return "Wolf";
	}
	
	protected override void OnDestroy()
	{
		WolfTile.OnWolfTileCreated -= OnWolfTileCreated;
		WolfTile.OnWolfTileBeginDestroy -= OnWolfTileDestroyed;

		base.OnDestroy();
	}
}
