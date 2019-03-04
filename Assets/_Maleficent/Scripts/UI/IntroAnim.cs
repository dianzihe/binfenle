using UnityEngine;
using System.Collections;

public class IntroAnim : MonoBehaviour {
	public Renderer r;

	public Vector2 tiling;
	public Vector2 offset;

	public Animation crowAnimation;
	public float crowAnimationSpeed = 1.0f;

	void Start() {
		if(crowAnimation != null)
			crowAnimation["Logo_01"].speed = crowAnimationSpeed;
	}

	void Update() {
		if(r != null) {
			if(r.sharedMaterial != null) {
				r.sharedMaterial.SetTextureScale ("_AlphaMask", tiling);
				r.sharedMaterial.SetTextureOffset("_AlphaMask", offset);
			}
		}
	}
}
