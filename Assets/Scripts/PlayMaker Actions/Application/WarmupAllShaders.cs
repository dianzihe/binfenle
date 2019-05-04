using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory(ActionCategory.Application)]
//[Tooltip("Warms up all currently loaded shaders to avoid in-game hickup. ")]
public class WarmupAllShaders : FsmStateAction
{	
	// Code that runs on entering the state.
	public override void OnEnter()
	{
		Shader.WarmupAllShaders();
		Finish();
	}
}
