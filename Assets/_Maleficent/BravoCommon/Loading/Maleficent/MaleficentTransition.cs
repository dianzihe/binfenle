using UnityEngine;
using System.Collections;

public class MaleficentTransition : SceneTransition {

	public float t;

	public Renderer bgTransition;
	public Renderer greenMenuFog;
	public Renderer menuMagic1;
	public Renderer menuMagic2;
	public UILabel loadingText;

	public void Start() {
		SetAlpha(bgTransition.material, "_Color", 0.0f);
		SetAlpha(greenMenuFog.material, "_Color", 0.0f);
		SetAlpha(menuMagic1.material, "_Color", 0.0f);
		SetAlpha(menuMagic2.material, "_Color", 0.0f);

		loadingText.enabled = false; //Fuck you NGUI! (if you don't do this the label will be shown)
		loadingText.alpha = 0.0f;
	}

	private void SetAlpha(Material m, string _color, float _alpha){
		Color c = m.GetColor(_color);
		c.a = _alpha;
		m.SetColor(_color, c);
	}

	public void Update() {
		SetAlpha(bgTransition.sharedMaterial, "_Color", t);
		SetAlpha(greenMenuFog.sharedMaterial, "_Color", t);
		SetAlpha(menuMagic1.sharedMaterial, "_Color", t);
		SetAlpha(menuMagic2.sharedMaterial, "_Color", t);

		loadingText.enabled = true;
		if(t>0.8f)
		loadingText.alpha = t;

	}

	public override void Enter(System.Action _enterFinishedCallback) {
		GetComponent<Animation>().CrossFade("transitionIn");
		MaleficentTools.DoAfterSeconds(this, 1.0f, _enterFinishedCallback);
	}

	public override void Exit(System.Action _exitFinishedCallback) {
		GetComponent<Animation>().CrossFade("transitionOut");
		MaleficentTools.DoAfterSeconds(this, 1.0f, _exitFinishedCallback);
	}
}
