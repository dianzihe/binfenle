using UnityEngine;
using System.Collections;

public class AreaBlockerTargetGUI : AbstractLevelTargetGUI {

	private WinDestroyAreaBlocker winDestroyAreaBlocker;
	public UILabel remainingTilesLabel;

	protected override void Start() 
	{
		base.Start();
		if(winCondition != null) {
			winDestroyAreaBlocker = winCondition as WinDestroyAreaBlocker;

			AreaBlockerTile.OnAreaBlockerDestroyed += HandleOnAreaBlockerDestroyed;

		}
	}

	protected void OnDestroy() 
	{
		AreaBlockerTile.OnAreaBlockerDestroyed -= HandleOnAreaBlockerDestroyed;
	}

	protected override System.Type WinConditionType ()
	{
		return typeof(WinDestroyAreaBlocker);
	}

	protected override bool MainTargetCompleted() 
	{
		return winDestroyAreaBlocker.AllAreaBlockersDestroyed();
	}

	protected override void UpdateGUI() 
	{
		remainingTilesLabel.text = string.Format("{0}", winDestroyAreaBlocker.RemainigAreaBlockers());
	}

	private void HandleOnAreaBlockerDestroyed (AreaBlockerTile obj)
	{
		TargetValuesChanged();
	}
}
