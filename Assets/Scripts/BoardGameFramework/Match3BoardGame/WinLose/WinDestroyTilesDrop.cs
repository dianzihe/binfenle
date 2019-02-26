using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinDestroyTilesDrop : WinDestroyTiles {
	
	protected float[] numberOfDropTilesOnBoard;
	
	protected float numberOfDropTilesAll;
	
	public float maxNumberOfDropTilesOnBoard;
	
	//Wait Random.Range[ Low, High ]  seconds before adding a requested tile to the spawnList 
	public int MinTurnsForSpawn = 2;
	public int MaxTurnsForSpawn = 5;
	
	protected int nrOfMoves;
	protected int spawnTurnIndex;
	
	protected List<TileSpawnRule> immediateSpawnList;

	protected override void Awake()
	{
		base.Awake();
		numberOfDropTilesOnBoard = new float[(int)TileColorType.Count];
		
		nrOfMoves = 0;
		spawnTurnIndex = Random.Range(MinTurnsForSpawn, MaxTurnsForSpawn);
		
		DropTile.OnDropTileInit += ActionOnDropCreate;
		DropTile.OnDropTileDropped += ActionOnDropTileDropped;
	}
	
	protected override void Start()
	{
		base.Start();
		immediateSpawnList = Match3BoardGameLogic.Instance.immediateSpawnList;
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove += OnNewMove;
	}
	
	protected void ActionOnDropCreate(DropTile dropTile)
	{
//		Debug.LogError("!In " + dropTile.name + " ++");
		numberOfDropTilesOnBoard[(int)dropTile.TileColor]++;
		numberOfDropTilesAll++;
	}
	
	protected void ActionOnDropTileDropped(DropTile dropTile)
	{	
//		Debug.LogError("!In " + dropTile.name + " --");
		//Sync the current number of dropTiles of this color currenly on the board
		numberOfDropTilesOnBoard[(int)dropTile.TileColor]--;
		numberOfDropTilesAll--;
		
		if (numberOfDropTilesAll <= 0)
		{
			numberOfDropTilesAll = 0;
			nrOfMoves = spawnTurnIndex;
		}
	}
	
	public override string GetLoseReason()
	{
		if (AllDestroyed()) {
			return base.GetLoseReason();
		}
		else {
			return Language.Get("LOSE_DROP_TILES");
			//return "You didn't get all\nthe special tiles.";
		}
	}
	
	public override string GetObjectiveString()
	{
		return Language.Get("GAME_OBJECTIVE_DROP_TILES");
		//return "You have to get all\nthe special tiles by\ndropping them off\nthe board.";
	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return Language.Get("MAP_OBJECTIVE_DROP_TILES");
	}
	
	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		return "Drop";
	}
	
	protected override void OnDestroy()
	{
		DropTile.OnDropTileInit -= ActionOnDropCreate;
		DropTile.OnDropTileDropped -= ActionOnDropTileDropped;
		base.OnDestroy();
	}
	
	protected void AddToImmediateSpawnList(TileColorType dropTileColor)
	{
		TileSpawnRule tileSpawnRule = new TileSpawnRule();
		
		tileSpawnRule.numberOfExecutions = 1;
		tileSpawnRule.ownerList = immediateSpawnList;
		
		RuleEntry ruleEntry = tileSpawnRule.ruleEntries[0];
		
		ruleEntry.RuleTileType = typeof(DropTile);
		ruleEntry.ColorSelection = ColorSelectionMethod.ColorBased;
		ruleEntry.RuleTileColor = dropTileColor;
		ruleEntry.randomColor = false;
		
		immediateSpawnList.Add(tileSpawnRule);
		
//		Debug.LogError("[AddToImmediateSpawnList()]");
	}
	
	void OnNewMove()
	{
		nrOfMoves++;
			
		if (nrOfMoves >= spawnTurnIndex && numberOfDropTilesAll + immediateSpawnList.Count < maxNumberOfDropTilesOnBoard)
		{
			//destroyTiles.Shuffle();
			foreach (DestroyTilesPair pair in destroyTiles)
			{
//				Debug.LogError(pair.current + " " + numberOfDropTilesOnBoard[(int)pair.type.TileColor] + " < " + pair.number);
				if (pair.current + numberOfDropTilesOnBoard[(int)pair.type.TileColor] < pair.number) {
					AddToImmediateSpawnList(pair.type.TileColor);
					
					nrOfMoves = 0;
					spawnTurnIndex = Random.Range(MinTurnsForSpawn, MaxTurnsForSpawn);
					
					break;
				}
			}
		}
	}
}
