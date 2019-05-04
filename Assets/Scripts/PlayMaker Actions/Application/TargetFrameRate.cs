using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory(ActionCategory.Application)]
//[Tooltip("Set the application target frame rate.")]
public class TargetFrameRate : FsmStateAction
{
	[RequiredField]
	public FsmInt targetFrameRate = 60;
	
	// Code that runs on entering the state.
	public override void OnEnter()
	{
		Application.targetFrameRate = targetFrameRate.Value;
		Finish();
	}
	
	// Perform custom error checking here.
	public override string ErrorCheck()
	{
		// Return an error string or null if no error.
		if (targetFrameRate.Value <= 0) {
			return "You must set a positive integer frame rate value!";
		}
		
		return null;
	}

}
