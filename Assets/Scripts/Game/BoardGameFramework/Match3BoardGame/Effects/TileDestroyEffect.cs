using UnityEngine;
using System.Collections;

public class TileDestroyEffect : DestroyEffect
{
	public Material[] coloredMaterials;
	
	public Renderer[] renderersForMaterialUpdate;
	
	/// <summary>
	/// The last selected material when <see cref="UpdateMaterial(Material newMaterial)"/> is called.
	/// </summary>
	[System.NonSerialized]
	public Material lastSelectedMaterial;
	
	
	public void UpdateMaterial(TileColorType tileColor)
	{
		int index = (int)tileColor - 1;
		if (index >= 0 && index < coloredMaterials.Length) {
			UpdateMaterial(coloredMaterials[index]);
		}
		else {
			UpdateMaterial(null);
		}
	}
	
	public void UpdateMaterial(Material newMaterial)
	{
		if (newMaterial != null) 
		{
			for(int i = 0; i < renderersForMaterialUpdate.Length; i++)  {
				renderersForMaterialUpdate[i].material = newMaterial;
			}
			lastSelectedMaterial = newMaterial;
		}		
	}
	
}

