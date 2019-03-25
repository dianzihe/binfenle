using UnityEngine;
using System.Collections;

public class TileBreakEffectController : MonoBehaviour {
	public Transform particleCollisionPlane;
	
	/// <summary>
	/// The tile renderers that use tile materials.
	/// </summary>
	public Renderer[] tileRenderers;
	
	
	public void UpdateTileRenderersMaterial(Material newMaterial)
	{
		for(int i = 0;i < tileRenderers.Length; i++) {
			tileRenderers[i].material = newMaterial;
		}
	}
		
	/// <summary>
	/// Updates the position of the particle collision plane from the particles effect of this script.
	/// </summary>
	public void UpdateParticleCollisionPlane() 
	{
		particleCollisionPlane.position = BottomCollider.Instance.cachedTransform.position;
	}
}
