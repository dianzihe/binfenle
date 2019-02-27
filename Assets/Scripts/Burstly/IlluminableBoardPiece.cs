using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class IlluminableBoardPiece : MatchCounterBoardPiece {

	static IlluminableBoardPiece firstIlluminatedPiece = null;

	public Renderer backModel;
	public int points = 1000;
	public Material[] stagesMaterials;
	public Color beatAnimationFromColor;
	public Color beatAnimationToColor;

	
	public override void Awake() {
		base.Awake();
		matchesLimit = 1;
		UpdateMaterial();
	}
	
	protected override void  OnDestroy () {
		if(firstIlluminatedPiece == this) {
			firstIlluminatedPiece = null;
		}

		base.OnDestroy();
	}

	public bool IsLighten() 
	{
		return matchesCount == matchesLimit;
	}

	protected override void RaiseOnNewMatch (Match3Tile _tile) 
	{
		base.RaiseOnNewMatch(_tile);
		UpdateMaterial();
		if(IsLighten()) {

			if(_tile != null) {
				_tile.Points = _tile.Points + points;
			}
			
			Transform backModelTransform = backModel.transform;
			Vector3 originalScale = backModel.transform.localScale;
			Vector3 expandScale = 0.5f * Vector3.one;
			Vector3 shrinkScale = -0.8f * Vector3.one;

			//FIX: final scale to match non-illuminable 
			originalScale = Vector3.one * 1.678f;
			
			Sequence sequence = new Sequence();
			sequence.Append(HOTween.To(backModelTransform, 0.2f, "localScale", expandScale, true));
			sequence.Append(HOTween.To(backModelTransform, 0.15f, "localScale", shrinkScale, true));
			sequence.Append(HOTween.To(backModelTransform, 0.1f, "localScale", originalScale, false));
			
			sequence.Play();

			SoundManager.Instance.Play("illuminable_sfx");
		}
	}


	private void UpdateMaterial()
	{
		if(IsLighten()) {

			if (firstIlluminatedPiece == null) {
				firstIlluminatedPiece = this;
				
				CloneableMaterial cloneableMaterial = new CloneableMaterial(stagesMaterials[matchesLimit]);
				backModel.sharedMaterial = cloneableMaterial.clonedMaterial;
			}else {
				backModel.sharedMaterial = firstIlluminatedPiece.backModel.sharedMaterial;
			}
		
			GetComponent<Collider>().enabled = false;
		}else {
			backModel.sharedMaterial = stagesMaterials[Mathf.Min(matchesCount, stagesMaterials.Length - 1)];
		}
	}
}
