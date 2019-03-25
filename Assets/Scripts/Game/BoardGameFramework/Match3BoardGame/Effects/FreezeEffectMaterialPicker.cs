using UnityEngine;
using System.Collections;
using System;

public class FreezeEffectMaterialPicker : MonoBehaviour {
	
	public Renderer normalModel;
	public Renderer cutoffModel;
	
	[System.Serializable]
	public class FreezeColor {
		public string name;
		public Material normalMat;
		public Material cutoffMat;
	}
	
	public FreezeColor[] freezeEffectMaterials;
	
	public void AssignMaterialsForColor(TileColorType colorType) {
		for(int i = 0;  i < freezeEffectMaterials.Length; i++) {
			if(freezeEffectMaterials[i].name.Equals(Enum.GetName(typeof(TileColorType), colorType))) {
				normalModel.material = freezeEffectMaterials[i].normalMat;
				cutoffModel.material = freezeEffectMaterials[i].cutoffMat;
				break;
			}
		}
	}
}


	