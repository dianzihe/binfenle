using UnityEngine;
using System.Collections;

public class LinkerLineAlphaListener : AlphaListener
{
	CustomLineRender2 mCustomLineRender;
	
	// Use this for initialization
	protected void Start () {
		base.Start();
		mCustomLineRender = GetComponent<CustomLineRender2>();
	}
	
	public override void SetAlpha(float alpha)
	{
		if(mCustomLineRender != null) mCustomLineRender.UpdateMesh(Mathf.Min(mCustomLineRender.alpha,alpha));
	}
}
