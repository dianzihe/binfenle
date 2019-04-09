using UnityEngine;
using System.Collections;

public class AlphaSetter : MonoBehaviour {

	public float alpha = 1f;
	Color spriteColor;
	// Use this for initialization
	void Start ()
	{
		spriteColor = GetComponent<UISprite>().color;
	}
	
	// Update is called once per frame
	void Update ()
	{
		GetComponent<UISprite>().color = new Color(spriteColor.r,spriteColor.r,spriteColor.g,alpha);
	}
}
