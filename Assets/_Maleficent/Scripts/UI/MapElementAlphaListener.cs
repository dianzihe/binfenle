using UnityEngine;
using System.Collections;

public class MapElementAlphaListener : AlphaListener
{
	bool destroyed = false;

	public override void SetAlpha(float alpha)
	{
		if(destroyed)
			return;

		foreach(Material material in GetComponent<Renderer>().materials)
			if(material.HasProperty("_Alpha"))
				material.SetFloat("_Alpha",alpha);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		destroyed = true;
	}
}
