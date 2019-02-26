using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// TilesDictionary
/// 
/// Stores arrays of tiles based on their <see cref="System.Type"/> and their <see cref="TileColorType"/>.
/// Each <see cref="System.Type"/> key in the dictionary has an array of tiles indexed by their <see cref="TileColorType"/>.
/// </summary>
public class TilesDictionary {
	protected Dictionary<System.Type, Match3Tile[]> tilesDictionary;
	
	public TilesDictionary() {
		tilesDictionary = new Dictionary<System.Type, Match3Tile[]>(10);
	}
	
	public bool HasTileType(System.Type tileType, TileColorType tileColor) {
		if ( !tilesDictionary.ContainsKey(tileType) ) {
			return false;
		}
		
		Match3Tile[] tiles = tilesDictionary[tileType];
		if (tiles != null) {
			return tiles[(int)tileColor] != null;
		} else {
			return false;
		}
	}
	
	public Match3Tile this[System.Type tileType, TileColorType tileColor] {
		get {
			return tilesDictionary[tileType][(int)tileColor];
		}
		
		set {
			Match3Tile[] tiles;
			if (tilesDictionary.TryGetValue(tileType, out tiles) == false) {
				tiles = new Match3Tile[(int)TileColorType.Count];
			}
			
			tiles[(int)tileColor] = value;
			tilesDictionary[tileType] = tiles;
		}
	}
	
	/// <summary>
	/// Clears all the stored references.
	/// </summary>
	public void Clear() {
		tilesDictionary.Clear();
	}
	
}