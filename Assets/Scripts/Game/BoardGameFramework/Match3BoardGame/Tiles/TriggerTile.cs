using UnityEngine;
using System.Collections;

public abstract class TriggerTile : NormalTile
{
	[System.NonSerialized]
	public bool canTriggerAbility = true;
		
	// Just release the tile from the board piece, but don't destroy the actual game object 
	// as we need it for the coroutine to destroy the colored tiles.
	protected IEnumerator DelayedTileRelease(float time)
	{
		tileModelTransform.gameObject.SetActive(false);
		
//		ApplyPunchEffectToNeighbors(-1, 1, 1);
		
		yield return new WaitForSeconds(time);
		
		if (this == BoardPiece.Tile) {
			BoardPiece.Tile = null;
		}
	}
	
	/// <summary>
	/// Triggers the tile's ability. This can also vary depending on with which tile this trigger has been previously switched.
	/// </summary>
	public abstract void TriggerTileAbility();	
}

