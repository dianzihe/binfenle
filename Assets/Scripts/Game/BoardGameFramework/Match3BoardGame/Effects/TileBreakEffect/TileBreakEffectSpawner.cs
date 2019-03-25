using UnityEngine;
using System.Collections;

public class TileBreakEffectSpawner : TileDestroyEffect {
	
	protected override void Start ()
	{
		base.Start ();
		
		// Get the cached particles effect from the objects pool.
		GameObject particleEffectInstance = GameObjectsPool.Instance.objectsPool[lastSelectedMaterial] as GameObject;
		
		// Spawn cached particles effect.
		ParticleSystemManager.Instance.Emit(particleEffectInstance, cachedTransform.position, false);
		particleEffectInstance.GetComponent<TileBreakEffectController>().UpdateParticleCollisionPlane();
	}
}
