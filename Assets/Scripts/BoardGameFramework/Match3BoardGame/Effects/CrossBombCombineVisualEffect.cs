using UnityEngine;
using System.Collections;

public class CrossBombCombineVisualEffect : VisualEffect {
	public Material[] tileColorMaterials;
	public Animation[] runningAnims;
	
	public float horizontalAnimTime = 0.5f;

	/// <summary>
	/// The sum of all the "runningAnims" animation lengths.
	/// </summary>
	[System.NonSerialized]
	public float totalAnimTime;
		
	/// <summary>
	/// The dynamic renderers that will have their material changed to one of the materials from the "tileColorMaterials" list.
	/// </summary>
	public Renderer[] dynamicRenderers;
	
	
	protected override void Awake () {
		base.Awake ();
		
		// Sum up all "runningAnims" animation lengths.
		for(int i = 0; i < runningAnims.Length; i++) {
			totalAnimTime += runningAnims[i].clip.length;
		}
	}
	
	public void UpdateMaterial(TileColorType tileColor) {
		int index = (int)tileColor - 1;
		
		if (index >= 0 && index < tileColorMaterials.Length) {
			UpdateMaterial(tileColorMaterials[index]);
		}
		else {
			UpdateMaterial(null);
		}
	}
	
	public void UpdateMaterial(Material newMaterial) {
		for(int i = 0; i < dynamicRenderers.Length; i++) {
			dynamicRenderers[i].material = newMaterial;
		}
	}

}
