using UnityEngine;
using System.Collections;

public class ColumnDestroyTile : DirectionalDestroyTile
{
	protected override void Awake ()
	{
		base.Awake();
		
		direction = DirectionalDestroyTile.Direction.Vertical;
	}
}

