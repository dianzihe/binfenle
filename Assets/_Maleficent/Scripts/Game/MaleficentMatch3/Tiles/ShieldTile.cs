using UnityEngine;
using System.Collections;

public class ShieldTile : SnowTile {

	protected override void Awake ()
	{
		resourcePath = "Game/Materials/Tiles/tile_material_shield";

		base.Awake ();
//
//		materialArray = new Material[materialArraySize];
//
//		for(int i = 0; i < numberOfSnowLayers; i++)
//		{
//			materialArray[i] = Resources.Load(resourcePath + (materialArraySize - numberOfSnowLayers + 1 + i)) as Material;
//		}
	}
}
