using UnityEngine;
using System.Collections;

public class ColorBombColorChange : MonoBehaviour {

	public Gradient colorGradient;
	public float time = 2.0f;
	public float globalAlpha = 0.5f;

	private float ellapsedTime = 0.0f;
	private Material myMaterial;

	void Start()
	{
		myMaterial = GetComponent<Renderer>().material;
	}


	void Update()
	{
		ellapsedTime += Time.deltaTime;
		if (ellapsedTime > time) {
			ellapsedTime -= time;
		}

		float t = ellapsedTime / time;
		Color newColor = colorGradient.Evaluate(t);
		newColor.a = globalAlpha;

		myMaterial.SetColor("_Color", newColor);
	}
}
