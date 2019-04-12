using UnityEngine;
using System.Collections;

public class LightPiecesTargetGUI : AbstractLevelTargetGUI {

	private WinDestroyLightPieces winDestroyLightPieces;
	public UILabel remainingMatchesLabel;

	protected override void Start() 
	{
		base.Start();
		if(winCondition != null) {
			winDestroyLightPieces = winCondition as WinDestroyLightPieces;
			WinDestroyLightPieces.OnLightMatchesNeededUpdated += HandleOnLightMatchesNeededUpdated;
		}
	}
	
	protected void OnDestroy() 
	{
		WinDestroyLightPieces.OnLightMatchesNeededUpdated -= HandleOnLightMatchesNeededUpdated;
	}


	protected void HandleOnLightMatchesNeededUpdated (WinDestroyLightPieces obj)
	{
		TargetValuesChanged();
	}

	protected override System.Type WinConditionType() 
	{
		return typeof(WinDestroyLightPieces);
	}

	protected override bool MainTargetCompleted()
	{
		return winDestroyLightPieces.AllPiecesLighten();
	}

	protected override void UpdateGUI()
	{
		remainingMatchesLabel.text = string.Format("{0}", winDestroyLightPieces.notLightenPieces);
	}
}
