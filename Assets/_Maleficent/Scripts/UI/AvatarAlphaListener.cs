using UnityEngine;
using System.Collections;

public class AvatarAlphaListener : AlphaListener
{
	MeshRenderer mMeshRenderer;

	public void Awake()
	{
		mMeshRenderer = GetComponent<MeshRenderer>();
	}

	public override void SetAlpha(float alpha)
	{
		if(mMeshRenderer != null) mMeshRenderer.material.color = new Vector4(1f,1f,1f,alpha);
	}
}
