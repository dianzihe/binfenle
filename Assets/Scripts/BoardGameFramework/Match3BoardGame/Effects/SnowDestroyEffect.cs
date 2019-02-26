using UnityEngine;
using System.Collections;

public class SnowDestroyEffect : DestroyEffect {
	
	public Renderer cachedModelRenderer;
	
	public void HideRenderer() {
		cachedModelRenderer.enabled = false;
	}
}
