using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SunTileSpawnController : MonoBehaviour
{
	public static SunTileSpawnController instance;
	
	protected List<TileSpawnRule> immediateSpawnList;
	
	protected float numberofSunTilesAll = 0;
	
	public int maxNrOnBoard;

	public int spawnProbability = 50;
	
	protected void Awake()
	{
		//Singleton MonoBehaviour
		if (instance == null)
		{
			instance = this;
		
			SunTile.OnSunTileCreated += OnSunTileCreated;
			SunTile.OnSunTileDestroyed += OnSunTileDestroyed;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
	protected void Start()
	{
		immediateSpawnList = Match3BoardGameLogic.Instance.immediateSpawnList;
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove += OnNewMove;
	}
		
	protected void OnSunTileCreated(SunTile sender)
	{
		numberofSunTilesAll++;
	}
	
	protected void OnSunTileDestroyed(SunTile sender)
	{
		numberofSunTilesAll--;
	}
	
	protected void AddToImmediateSpawnList()
	{
		TileSpawnRule tileSpawnRule = new TileSpawnRule();
		
		tileSpawnRule.numberOfExecutions = 1;
		tileSpawnRule.ownerList = immediateSpawnList;
		
		RuleEntry ruleEntry = tileSpawnRule.ruleEntries[0];
		
		ruleEntry.RuleTileType = typeof(SunTile);
		ruleEntry.ColorSelection = ColorSelectionMethod.ColorBased;
		ruleEntry.RuleTileColor = TileColorType.None;
		ruleEntry.randomColor = false;
		
		immediateSpawnList.Add(tileSpawnRule);
	}

	void OnNewMove()
	{
		if (numberofSunTilesAll < maxNrOnBoard && !Match3BoardGameLogic.Instance.IsGameOver)
		{
			if(Random.Range(0,101) < spawnProbability)
			{
				AddToImmediateSpawnList();
			}
		}
	}
	
	protected void OnDestroy()
	{
		SunTile.OnSunTileCreated -= OnSunTileCreated;
		SunTile.OnSunTileDestroyed -= OnSunTileDestroyed;
		Match3BoardGameLogic.Instance.loseConditions.OnNewMove -= OnNewMove;
	}
	
}
