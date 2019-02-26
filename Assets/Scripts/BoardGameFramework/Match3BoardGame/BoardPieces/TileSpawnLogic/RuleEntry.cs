using UnityEngine;
using System.Collections;

public enum TileSpawnType
{
	NormalTile,
	LockedTile,
	SnowTile,
	DropTile,
	BombTile,
	ColorBombTile,
	FreezerTile,
	ColumnDestroyTile,
	RowDestroyTile,
	WolfTile,
	SunTile,
	EaterTile,
	SliderTile,
	AreaBlockerTile,
	StubbornTile,
	ShieldTile,
	GrowingTargetTile,
	SoldierTile
}

public enum GenericColorType
{
	GenericColor1 = 0,
	GenericColor2,
	GenericColor3,
	GenericColor4,
	GenericColor5,
	GenericColor6,
	Count,
}

public enum ColorSelectionMethod
{
	ColorBased,
	Generic,
}

[System.Serializable]
public class RuleEntry
{	
	/// <summary>
	/// The generic colors that will be randomly assigned at run-time based on how many maximum colors the board will have on it.
	/// </summary>
	public static TileColorType[] genericColors = new TileColorType[(int)GenericColorType.Count];

	//Fields that need to be assigned trough the inspector or trough the ctor----------------
	[SerializeField]
	protected TileSpawnType spawnType;
	
	[SerializeField]
	protected TileColorType spawnColor;
	
	[SerializeField]
	protected ColorSelectionMethod colorSelection;

	[SerializeField]
	protected GenericColorType genericColor;

	[SerializeField]
	public bool randomColor = true;
	//--------------------------------------------------------------------------------------
	
	
//	protected System.Type spawnTileType;
	protected static System.Type[] cachedTypeList;
	
#region Constructors
	/// <summary>
	/// Initializes the <see cref="RuleEntry"/> class.
	/// Create a cache of System.Type for each type registered in the <see cref="TileSpawnType"/> enum.
	/// </summary>
	static RuleEntry()
	{
		string[] spawnTypeNames = System.Enum.GetNames(typeof(TileSpawnType));
		cachedTypeList = new System.Type[spawnTypeNames.Length];
		
		for(int i = 0; i < spawnTypeNames.Length; i++)
		{
			cachedTypeList[i] = System.Type.GetType(spawnTypeNames[i]);
		}
	}
	
	public RuleEntry() 
	{
		colorSelection = ColorSelectionMethod.Generic;
		spawnType = TileSpawnType.NormalTile;
//		spawnTileType = cachedTypeList[(int)spawnType];
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="RuleEntry"/> class.
	//  The color of the newly spawned tile will be randomly selected from the available colors.
	/// </summary>
	/// </param>
	public RuleEntry(TileSpawnType _spawnType)
	{ 
		spawnType = _spawnType;
//		spawnTileType = cachedTypeList[(int)spawnType];
	}
	
	public RuleEntry(System.Type _type, TileColorType _spawnColor)
	{
		RuleTileType = _type;
		spawnColor = _spawnColor;
//		spawnTileType = cachedTypeList[(int)spawnType];
	}
	
	public RuleEntry(TileSpawnType _spawnType, TileColorType _spawnColor, ColorSelectionMethod _colorSelection, bool _randomColor)
	{
		spawnType = _spawnType;
		spawnColor = _spawnColor;
		colorSelection = _colorSelection;
		randomColor = _randomColor;
	}

#endregion

#region Properties
	public GenericColorType GenericColor {
		get {
			return genericColor;
		}
		set {
			genericColor = value;
		}
	}	

	public ColorSelectionMethod ColorSelection {
		get {
			return colorSelection;
		}
		set {
			colorSelection = value;
		}
	}	

	public System.Type RuleTileType 
	{
		get 
		{
			return cachedTypeList[(int)spawnType];
		}
		set 
		{
			spawnType = (TileSpawnType)System.Enum.Parse(typeof(TileSpawnType), value.Name);
		}
	}
	
	public TileColorType RuleTileColor 
	{
		get
		{
			if (colorSelection == ColorSelectionMethod.ColorBased)
			{
//				return randomColor == false ? spawnColor : RuleEntry.genericColors[Random.Range(0, Match3BoardRenderer.maxNumBoardColors)]; //(TileColorType)Random.Range(1, (int)TileColorType.Count);
				return randomColor == false ? spawnColor : RuleEntry.genericColors[ProbabilisticSpawnController.Instance.GetRandomTileColor()];
			}
			else 
			{
				return randomColor == false ? RuleEntry.genericColors[(int)genericColor] : RuleEntry.genericColors[ProbabilisticSpawnController.Instance.GetRandomTileColor()];
			}
		}
		set
		{
			spawnColor = value;
		}
	}	
#endregion
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
	
	public override bool Equals (object obj)
	{
		if (obj == null) 
		{
			return false;
		}
		
		RuleEntry entryParam = null;
		
		if (obj is RuleEntry)
		{
			entryParam = obj as RuleEntry;
			if(entryParam.RuleTileType == this.RuleTileType)
			{
				if(entryParam.RuleTileColor == this.RuleTileColor || entryParam.randomColor || this.randomColor)
				{
					return true;
				}
			}
		}
		
		return false;
	}
	
}