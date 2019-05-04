using UnityEngine;
using System.Collections;

public class UIPanelAlphaListener : AlphaListener
{
	UIPanel mPanel;
	
	// Use this for initialization
	protected void Start () {
		base.Start();
		mPanel = GetComponent<UIPanel>();
	}

	public override void SetAlpha(float alpha)
	{
		if(mPanel != null) mPanel.alpha = alpha;
	}
}
