using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//namespace PulseEngine {

/// <summary>
/// Scene block. Represents a scene in the application, usually loaded by instantiating the prefab.
/// </summary>
public class SceneBlock : FlowBlock
{
	/// <summary>
	/// The next scene to play when this block is finished.
	/// </summary>
	public string nextScene;
	
	/// <summary>
	/// The previous scene to play if you need to go back.
	/// </summary>
	public string previousScene;
	
	/// <summary>
	/// Tells if the next scene should be preloaded. 
	/// Should be false if this scene's behavior can change what the next scene will be.
	/// </summary>
	public bool preloadNextScene = true;
	
	/// <summary>
	/// The flow controller of this scene block. Each scene must have a flow controller. 
	/// If it wasn't assigned or the component added on the object, a component will be added at runtime.
	/// </summary>
	public FlowController flowController;
	
	/// <summary>
	/// The playable controllers of this scene block. If it isn't populated, the playable controllers on this game object will be added.
	/// Usually includes a TextController and a NarrationController.
	/// </summary>
	public List<PlayableController> playableControllers;
	
	/// <summary>
	/// The number of controllers that finished playing. Used to raised the corresponding finish events, based on this scene's state.
	/// </summary>
	protected int finishedControllers = 0;
		
	#region Events Delegates
	public delegate void ScenePlayingFinishedEventHandler(SceneBlock scene);
	#endregion Events Delegates
	
	/// <summary>
	/// Occurs when the scene finishes playing its intro state.
	/// </summary>
	public event ScenePlayingFinishedEventHandler OnSceneIntroFinished;
	
	/// <summary>
	/// Occurs when the scene finishes playing its idle state.
	/// </summary>
	public event ScenePlayingFinishedEventHandler OnSceneIdleFinished;
	
	/// <summary>
	/// Occurs when the scene finishes playing its outro state.
	/// </summary>
	public event ScenePlayingFinishedEventHandler OnSceneOutroFinished;
	
	/// <summary>
	/// Unity method.
	/// Awake this instance. 
	/// </summary>
	public override void Awake ()
	{
		base.Awake ();
		
		if (flowController == null) {
			flowController = GetComponent<FlowController>();
			
			if (flowController == null) {
				// No flow controller found on this scene, create a new one that will control only this scene.
				flowController = gameObject.AddComponent<FlowController>();
				flowController.blocks = new FlowBlock[]{this};
			}
		}
		
		if (playableControllers == null) {
			playableControllers = new List<PlayableController>();
		}
		
		if (playableControllers.Count == 0) {
			MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
			
			foreach (MonoBehaviour script in scripts) {
				if (typeof(PlayableController).IsInstanceOfType(script)) {
					playableControllers.Add(script as PlayableController);
				}
			}
		}
	}
	
	/// <summary>
	/// Plays the intro animation.
	/// </summary>
	public void PlayIntro()
	{
		flowController.PlayIntroFlow();
		flowController.OnIntroFinished += OnFlowAnimationFinished;
	}
	
	/// <summary>
	/// Plays the idle animation.
	/// </summary>
	public void PlayIdle()
	{
		flowController.PlayIdleFlow();
		flowController.OnIdleFinished += OnFlowAnimationFinished;
	}
	
	/// <summary>
	/// Plays the outro animation.
	/// </summary>
	public void PlayOutro()
	{
		flowController.PlayOutroFlow();
		flowController.OnOutroFinished += OnFlowAnimationFinished;
	}
	
	/// <summary>
	/// Called when the flow controller finishes playing a state.
	/// </summary>
	/// <param name='theController'>
	/// The flow controller.
	/// </param>
	public void OnFlowAnimationFinished(FlowController theController)
	{
		finishedControllers = 0;
		
		foreach (PlayableController controller in playableControllers) 
		{
			if (controller.IsPlaying()) {
				controller.OnPlayingFinished += OnControllerFinished;
			} 
			else {
				finishedControllers++;
			}
		}
		
		if (finishedControllers >= playableControllers.Count) {
			DoAllControllersFinished();
		}
	}
	
	/// <summary>
	/// Called when a playable controller finishes playing.
	/// </summary>
	/// <param name='controller'>
	/// Controller.
	/// </param>
	public void OnControllerFinished(PlayableController controller)
	{
		controller.OnPlayingFinished -= OnControllerFinished;
		
		finishedControllers++;
		
		if (finishedControllers >= playableControllers.Count) {
			DoAllControllersFinished();
		}
	}
	
	/// <summary>
	/// Called when all the controllers finished playing. It will raise events based on this scene's state.
	/// </summary>
	public void DoAllControllersFinished() 
	{
		switch (state) {
		case FlowBlock.FlowState.Intro :
			if (OnSceneIntroFinished != null) {
				OnSceneIntroFinished(this);
			}
			break;
		case FlowBlock.FlowState.Idle :
			if (OnSceneIdleFinished != null) {
				OnSceneIdleFinished(this);
			}
			break;
		case FlowBlock.FlowState.Outro :
			if (OnSceneOutroFinished != null) {
				OnSceneOutroFinished(this);
			}
			break;
		}
	}
	
	public override bool IsPlayingSelf ()
	{
		bool controllersFinished = true;
	
		foreach (PlayableController controller in playableControllers) 
		{
			if (!(typeof(AudioController)).IsInstanceOfType(controller) && controller.IsPlaying()) {
				controllersFinished = false;
				break;
			}
		}
		
		return !controllersFinished || base.IsPlayingSelf();
	}
}

//}