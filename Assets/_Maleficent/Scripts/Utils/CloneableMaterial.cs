using UnityEngine;
using System.Collections;

[System.Serializable]
public class CloneableMaterial {

	private Material _clonedMaterial;

	public Material originalMaterial;
	public Material clonedMaterial {
		get {
			if(_clonedMaterial == null) {
				_clonedMaterial = GameObject.Instantiate(originalMaterial) as Material;
			}
			return _clonedMaterial;
		}
	}

	public CloneableMaterial()
	{
			
	}

	public CloneableMaterial(Material _originalMaterial) {
		originalMaterial = _originalMaterial;
	}
}
