using UnityEngine;
using System.Collections;

public class PauseAnim : MonoBehaviour {

	public MeshRenderer mr;

	public float t;
	public Color color0;
	public Color color1;

	private Material mat;
	

	// Use this for initialization
	void Start () {
		mat = mr.GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		mat.SetColor("_color", new Color(Mathf.Lerp(color0.r, color1.r, t), Mathf.Lerp(color0.g, color1.g, t), Mathf.Lerp(color0.b, color1.b, t)));
		mat.SetFloat("_bwBlend", t);
	}
}
