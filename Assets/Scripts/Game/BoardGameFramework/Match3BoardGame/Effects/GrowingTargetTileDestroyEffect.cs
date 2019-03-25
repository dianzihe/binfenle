using UnityEngine;
using System.Collections;

public class GrowingTargetTileDestroyEffect : MonoBehaviour {

	public Renderer cachedModelRenderer;
	
	public void HideRenderer() {
		cachedModelRenderer.enabled = false;
	}
}