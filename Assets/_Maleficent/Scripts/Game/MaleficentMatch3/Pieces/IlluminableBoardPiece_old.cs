using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class IlluminableBoardPiece_old : MatchCounterBoardPiece {
	/*
	static IlluminableBoardPiece_old firstIlluminatedPiece = null;

	public Renderer backModel;
	public Texture finalStageTexture;

	public float defaultAlpha = 0.35f;
	public float alphaIncrementAnimationDuration = 0.2f;
	public int points = 1000;
	
	private float alphaIncrement;
	private Material backModelMaterial;


	public override void Awake() {
		base.Awake();

		alphaIncrement = (1 - defaultAlpha)/matchesLimit;
		backModelMaterial = backModel.material;
		SetAlpha(defaultAlpha, false);
	}

	void Update () {
		if (IsLighten() && firstIlluminatedPiece != null && firstIlluminatedPiece != this) {
			backModelMaterial.color = firstIlluminatedPiece.backModelMaterial.color;
		}
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

	protected override void RaiseOnNewMatch () 
	{
		base.RaiseOnNewMatch();

		if(IsLighten()) {
			tile.Points = tile.Points + points;
		}

		float alpha = defaultAlpha + matchesCount * alphaIncrement;
		SetAlpha(alpha, true);
	}

	private void SetAlpha(float alpha, bool animated) {
		Color color = backModelMaterial.color;
		color.a = alpha;

		if(animated) {
			TweenParms parms = new TweenParms().Prop ("color", color).Ease (EaseType.Linear).OnComplete(OnCompleteAlphaChangeAnimation);
			HOTween.To (backModelMaterial, alphaIncrementAnimationDuration, parms);
		}else {
			backModelMaterial.color = color;
			OnCompleteAlphaChangeAnimation();
		}
	}

	private void OnCompleteAlphaChangeAnimation() {
		if(IsLighten()) {
			backModelMaterial.mainTexture = finalStageTexture;

			Transform backModelTransform = backModel.transform;
			Vector3 originalScale = backModel.transform.localScale;
			Vector3 expandScale = 0.5f * Vector3.one;
			Vector3 shrinkScale = -0.8f * Vector3.one;

			Sequence sequence = new Sequence();
			sequence.Append(HOTween.To(backModelTransform, 0.2f, "localScale", expandScale, true));
			sequence.Append(HOTween.To(backModelTransform, 0.15f, "localScale", shrinkScale, true));
			sequence.Append(HOTween.To(backModelTransform, 0.1f, "localScale", originalScale, false));
			sequence.ApplyCallback(CallbackType.OnComplete, OnCompleteScaleAnimation);

			sequence.Play();
		}
	}

	private void OnCompleteScaleAnimation () {

		if (firstIlluminatedPiece == null) {

			firstIlluminatedPiece = this;

			Color colorInc = new Color(0, 0, 0, 0.9f);
			TweenParms parms = new TweenParms().Prop ("color", colorInc, true).Ease (EaseType.Linear).Loops(-1, LoopType.Yoyo);
			HOTween.To (backModelMaterial, 1f, parms);
		}
	}

*/
}
