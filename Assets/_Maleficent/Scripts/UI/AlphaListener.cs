using UnityEngine;
using System.Collections;

public class AlphaListener : MonoBehaviour
{
	protected void Start()
	{
		BookAnimations.Instance.OnFadingChapter += SetAlpha;
	}
	
	public virtual void SetAlpha(float alpha)
	{
	}

	protected virtual void OnDestroy()
	{
		BookAnimations.Instance.OnFadingChapter -= SetAlpha;
	}
}
