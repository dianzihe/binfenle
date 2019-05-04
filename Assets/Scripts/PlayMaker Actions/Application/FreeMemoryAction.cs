using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory(ActionCategory.Application)]
//[Tooltip("Frees all unusued unity assets and calls the garbage collector to free up unused memory.")]
public class FreeMemoryAction : FsmStateAction
{	
	// Code that runs on entering the state.
	public override void OnEnter()
	{
		FreeMemoryAction.FreeMemory();
		
		Finish();
	}
	
	public static void FreeMemory()
	{
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
		System.GC.WaitForPendingFinalizers();
	}
}
