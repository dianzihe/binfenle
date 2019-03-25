using UnityEngine;
using System.Collections;

public class RowDestroyTile : DirectionalDestroyTile
{
	protected override void Awake ()
	{
		base.Awake();
		
		direction = DirectionalDestroyTile.Direction.Horizontal;
	}
}

