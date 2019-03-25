using UnityEngine;
using System.Collections;

public class TileBombDestroyEffect : DestroyEffect {
	public Transform destroyTrigger;
	public float explosionSpreadTime = 0.2f;
	public float delayAfterExplosionSpread = 0.2f;
	/// <summary>
	/// How many layer of tiles around the current tile to explode. (1 layer means a 3x3 explosion)
	/// </summary>
	public float numNeighborLayersToExplode = 1f;
	
	protected Match3Tile effectOwner;
	protected float targetScaleFactor;	

	[System.NonSerialized]
	public TilesTriggerListener triggerListener;
	
	
	public void InitBombDestroyEffect(Match3Tile owner, Vector3 destroyTriggerPos) {
		effectOwner = owner;
		
		// Setup destroy trigger
		destroyTrigger.position = destroyTriggerPos;
		triggerListener = destroyTrigger.gameObject.GetComponent<TilesTriggerListener>();
			
		StartCoroutine(StartTriggerScale(explosionSpreadTime));
	}

	IEnumerator StartTriggerScale(float duration) {
		float t = 0f;
		float effectDuration = 1f / explosionSpreadTime;
		float horizTileSize = Match3BoardRenderer.horizTileDistance;
		float vertTileSize = Match3BoardRenderer.vertTileDistance;
		// Start the scale effect with the trigger at 75% of the size of the origin tile size.
		Vector3 startScaleSize = new Vector3(horizTileSize * 0.75f, vertTileSize * 0.75f, 1f);
		Vector3 finalScaleSize = new Vector3(horizTileSize * numNeighborLayersToExplode * 2f, 
											 vertTileSize * numNeighborLayersToExplode * 2f, 1f);
		destroyTrigger.localScale = startScaleSize;
		
		while(true) {
			destroyTrigger.localScale = Vector3.Lerp(startScaleSize, finalScaleSize, t);
			// The stop condition is here so when t == 1f we will update the target scale to the exact final scale.
			if (t >= 1) {
				break;
			}
			t = Mathf.Clamp01(t + Time.deltaTime * effectDuration);
			
			yield return null;
		}
		
		yield return new WaitForSeconds(delayAfterExplosionSpread);
		
		if (OnEffectFinished != null) {
			OnEffectFinished(this);
		}
	}
}
