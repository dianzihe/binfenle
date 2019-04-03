using UnityEngine;
using System.Collections;
using System;

public abstract class SceneTransition : MonoBehaviour
{	
	/// <summary>
	/// Method to be called for scene enter transition
	/// </summary>
	/// <remarks>
	/// It must cast event onEnterFinished() when enter transition end
	/// </remarks>
	public abstract void Enter(System.Action _enterFinishedCallback);
	
	/// <summary>
	/// Method to be called for scene exit transition
	/// </summary>
	/// <remarks>
	/// It must cast event onExitFinished() when exit transition end
	/// </remarks>
	public abstract void Exit(System.Action _exitFinishedCallback);
}
