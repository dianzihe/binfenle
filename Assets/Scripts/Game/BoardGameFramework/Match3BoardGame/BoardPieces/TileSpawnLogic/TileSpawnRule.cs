using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// TileSpawnRule
/// 
/// Handles what kind of tile it's allowed to spawn at a certain time.
/// Each <see cref="Match3BoardPiece"/> has at least one initial TileSpawnRule that will define what tile should be initially on it.
/// A <see cref="TileSpawnerBehavior"/> has a list of TileSpawnRules that define how tiles will be spawned from it over time.
/// </summary>
[System.Serializable]
public class TileSpawnRule 
{
	/// <summary>
	/// The tile spawned by this RuleEntry is going to be ignored by the matches undo system. (MatchesUndoer, PossibleMatchesGenerator, etc.)
	/// </summary>
	public bool isTileIgnoredByAntiMatchSystems;

	/// <summary>
	/// The owner list to which this tile spawning rule belongs to.
	/// </summary>
	[System.NonSerialized]
	public List<TileSpawnRule> ownerList;
	
	/// <summary>
	/// Define rule entries ( [tileType, tileColor] ) for this spawn rule.
	/// A rule entry means what this rule is going to randomly spawn.
	/// </summary>
	public List<RuleEntry> ruleEntries;
	
	//Last spawned rule, type, entry
	[System.NonSerialized]
	public RuleEntry spawnedEntry = null;
	
	[System.NonSerialized]
	public System.Type spawnedType = null;
	
	[System.NonSerialized]
	public TileColorType spawnedColor = TileColorType.None;
	
	/// <summary>
	/// If this flag is true this spawn rule must spawn a random tile from the ones available in its bitmaks but be different from the last spawned type/color.
	/// </summary>
	public bool spawnSameTypeAsPrevious = false;
	
	//Number of spawns allowed before this spawn rule is removed
	public int numberOfExecutions;
	
	
	public TileSpawnRule()
	{
		if (ruleEntries == null)
		{
			ruleEntries = new List<RuleEntry>();
			ruleEntries.Add(new RuleEntry());
		}
		
		numberOfExecutions = -1;
	}
	
	public TileSpawnRule(List<TileSpawnRule> _ownerList) : this()
	{
		ownerList = _ownerList;
	}
		
	/// <summary>
	/// Spawns the new tile.
	/// </summary>
	/// <param name='targetPiece'>
	/// Target piece.
	/// </param>
	public Match3Tile SpawnNewTile(bool isBoardSetup = false)
	{
		Match3Tile tileResult = null;
		
		// Early out if rule
		if(ruleEntries == null || ruleEntries.Count == 0)
		{
			return tileResult;
		}

		if (numberOfExecutions == 0)
		{
			EndSpawnRule();
			return tileResult;
		}
		
		//Select random rule entry and acquire its info
		spawnedEntry = ruleEntries[Random.Range(0, ruleEntries.Count)];
		spawnedType = spawnedEntry.RuleTileType;
		spawnedColor = spawnedEntry.RuleTileColor;
		
		if(ownerList != null && ownerList.Count > 0)
		{
			numberOfExecutions--;
		}

//		bool offBoard = false;
//
//		if(ownerList != null)
//		{
//			if (ownerList.Count == 0)
//			{
//				offBoard = false;
//			}
//			else
//			{
//				offBoard = true;
//				numberOfExecutions--;
//			}
//		}

		//Spawn tile as described by the spawn rule
//		Debug.LogWarning("[SpawnRule] Spawning [" + spawnedType.ToString() + "] [" + spawnedColor.ToString() + "] [offboard:" + offBoard + "]");
		
		if (typeof(BombTile) == spawnedType)
		{
			BombTile spawnedTile = Match3BoardRenderer.Instance.SpawnSpecificTile(spawnedType, TileColorType.None, isBoardSetup) as BombTile;
//			BombTile spawnedTile = Match3BoardRenderer.Instance.SpawnSpecificTileAt(targetPiece.BoardPosition, spawnedType, TileColorType.None, offBoard, isBoardSetup) as BombTile;
			spawnedTile.TileColor = spawnedColor;
			spawnedTile.UpdateMaterial();
			tileResult = spawnedTile;
		}
		else if (typeof(DirectionalDestroyTile).IsAssignableFrom(spawnedType))
		{
			DirectionalDestroyTile spawnedTile = Match3BoardRenderer.Instance.SpawnSpecificTile(spawnedType, TileColorType.None, isBoardSetup) as DirectionalDestroyTile;
//			DirectionalDestroyTile spawnedTile = Match3BoardRenderer.Instance.SpawnSpecificTileAt(targetPiece.BoardPosition, spawnedType, TileColorType.None, offBoard, isBoardSetup) as DirectionalDestroyTile;
			spawnedTile.TileColor = spawnedColor;
			spawnedTile.UpdateMaterial();
			tileResult = spawnedTile;
		}

		else if (typeof(AreaBlockerTile).IsAssignableFrom(spawnedType)) {
			tileResult = Match3BoardRenderer.Instance.SpawnSpecificTile(spawnedType, TileColorType.None, isBoardSetup) as AreaBlockerTile; 
		}

		else if (typeof(SliderTile).IsAssignableFrom(spawnedType)) {
			tileResult = Match3BoardRenderer.Instance.SpawnSpecificTile(spawnedType, TileColorType.None, isBoardSetup) as SliderTile;
		}

		else if (typeof(EaterTile).IsAssignableFrom(spawnedType)) {
			tileResult = Match3BoardRenderer.Instance.SpawnSpecificTile(spawnedType, TileColorType.None, isBoardSetup) as EaterTile;
		}

		else if (typeof(StubbornTile).IsAssignableFrom(spawnedType)) {
			tileResult = Match3BoardRenderer.Instance.SpawnSpecificTile(spawnedType, TileColorType.None, isBoardSetup) as StubbornTile;
		}

		else if (typeof(GrowingTargetTile).IsAssignableFrom(spawnedType)) {
			tileResult = Match3BoardRenderer.Instance.SpawnSpecificTile(spawnedType, TileColorType.None, isBoardSetup) as GrowingTargetTile;
		}

		else if (typeof(SoldierTile).IsAssignableFrom(spawnedType)) {
			tileResult = Match3BoardRenderer.Instance.SpawnSpecificTile(spawnedType, TileColorType.None, isBoardSetup) as SoldierTile;
		}

		else
		{
//			tileResult = Match3BoardRenderer.Instance.SpawnSpecificTileAt(targetPiece.BoardPosition, spawnedType, spawnedColor, offBoard, isBoardSetup);
			tileResult = Match3BoardRenderer.Instance.SpawnSpecificTile(spawnedType, spawnedColor, isBoardSetup);
		}
		
		if (tileResult != null) {
			tileResult.IsTileIgnoredByAntiMatchSystems = isTileIgnoredByAntiMatchSystems;
		}
		
		return tileResult;
	}
	
	public bool ShouldSpawnedTileIgnoreAntiMatchSystems()
	{
		if (ruleEntries.Count == 1 && !ruleEntries[0].randomColor)
		{
			return true;
		}
		
		return false;
	}	
		
	public void EndSpawnRule()
	{
		if (ownerList == null) 
		{
			return;
		}
		
		int listIdx = ownerList.IndexOf(this);
		
		if (listIdx >= 0) {
			ownerList.RemoveAt(listIdx);
		}
	}
	
	/// <summary>
	/// Compares the ruleEntries List of this SpawnRule with the List given as a param.
	/// </summary>
	/// <returns>
	/// A RuleEntry list containing those Entries compatible with / found in both ruleEntryLists.
	/// </returns>
	public List<RuleEntry> CompareRuleEntries(List<RuleEntry> paramList)
	{
		List<RuleEntry> returnList = null;
		for(int i = 0; i < ruleEntries.Count; i++)
		{
			for( int j = 0; j < paramList.Count; j++)
			{
				if(ruleEntries[i].Equals(paramList[j]))
				{
					if(returnList == null)
					{
						returnList = new List<RuleEntry>(5);
					}
					returnList.Add(ruleEntries[i]);
				}
			}
		}
		
		return returnList;
	}
	
	#region Tile Color related utility methods
	public static TileColorType GetRandomTileColor() {
		return RuleEntry.genericColors[Random.Range(0, Match3BoardRenderer.maxNumBoardColors)];
	}

	public static TileColorType GetRandomTileColorDifferentFrom(TileColorType ignoreTileColor) {
		List<TileColorType> randomBag = new List<TileColorType>(Match3BoardRenderer.maxNumBoardColors);
		
		// Fill bag with all available tile colors different from the ignore tile color
		for(int i = 0; i < Match3BoardRenderer.maxNumBoardColors; i++)
		{
			if (RuleEntry.genericColors[i] != ignoreTileColor) {
				randomBag.Add(RuleEntry.genericColors[i]);
			}
		}
		
		return randomBag[Random.Range(0, randomBag.Count)];
	}
	
	public static TileColorType GetRandomTileColorDifferentFrom(TileColorType[] ignoreTileColors) {
		if (ignoreTileColors.Length >= Match3BoardRenderer.maxNumBoardColors) {
//			Debug.LogWarning("This shouldn't happen!!! Trying to ignore all available colors for new random tile color! WTF!");
		}
		
		List<TileColorType> randomBag = new List<TileColorType>(Match3BoardRenderer.maxNumBoardColors);
		
		// Fill bag with maximum available tile colors different from ignored tile colors.
		for(int i = 0; i < Match3BoardRenderer.maxNumBoardColors; i++)
		{
			bool validColor = true;

			for(int j = 0; j < ignoreTileColors.Length; j++) 
			{
				if (RuleEntry.genericColors[i] == ignoreTileColors[j]) {
					validColor = false;
					break;
				}
			}

			if (validColor) {
				randomBag.Add(RuleEntry.genericColors[i]);
			}
		}
		
		if (randomBag.Count > 0) {
			return randomBag[Random.Range(0, randomBag.Count)];
		}
		else {
			return TileColorType.None;
		}
	}
	
	#endregion
}


