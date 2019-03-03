using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//namespace PulseEngine {

/// <summary>
/// Scenes controller, handles the flow of the scenes in the application. 
/// It loads and destroys scens based on their states. Each scene must have a SceneBlock component.
/// </summary>
public class ScenesController : MonoBehaviour
{
	/// <summary>
	/// The scenes controlled by this controller. Stored as paths of the prefabs, to avoid loading them in memory when instancing this class.
	/// </summary>
	public string[] scenes;
	
	/// <summary>
	/// The loaded scenes. It would usually have one or two scenes in it, unless there is a more specific behavior needed.
	/// </summary>
	public List<SceneBlock> loadedScenes = new List<SceneBlock>();
		
	/// <summary>
	/// Unity method.
	/// Start this instance.
	/// </summary>
	public virtual void Start()
	{		
		if (scenes.Length > 0) {
			LoadScene(scenes[0]);
			StartScene(0);
		}
	}
	
	/// <summary>
	/// Gets the path of the scene with the given name from the list of scenes. 
	/// If the given scene is not in the list, it returns its name, maybe it is a custom dynamic path.
	/// </summary>
	/// <returns>
	/// The scene path.
	/// </returns>
	/// <param name='sceneName'>
	/// Scene name.
	/// </param>
	public string GetScenePath(string sceneName, out int sceneIndex)
	{
		for (int i = 0; i < scenes.Length; ++i) 
		{
			if (scenes[i].EndsWith(sceneName)) {
				sceneIndex = i;
				return scenes[i];
			}
		}
		
		sceneIndex = -1;
		return sceneName;
	}
	
	/// <summary>
	/// Loads the scene block with the given name by instantiating its prefab.
	/// </summary>
	/// <param name='blockName'>
	/// Block name.
	/// </param>
	public virtual void LoadScene(string sceneName)
	{
		int sceneIndex = -1;
		
		//TODO: Change to call to ResourceManager / ResourceWrapper
		GameObject newScene = (GameObject)GameObject.Instantiate(Resources.Load(GetScenePath(sceneName, out sceneIndex)) as GameObject);
		
		newScene.SetActive(false);
		newScene.name = newScene.name.Replace("(Clone)", "");
		
		SceneBlock sceneBlock = newScene.GetComponent<SceneBlock>();
		
		if (sceneBlock == null) {
			Debug.LogWarning("Trying to load scene with name: " + sceneName + " but it doesn't have a SceneBlock component. Adding the component at run-time.");
			sceneBlock = newScene.AddComponent<SceneBlock>();
		}
		
		loadedScenes.Add(sceneBlock);
		
		if (sceneBlock.nextScene == null || sceneBlock.nextScene == "") {
			sceneBlock.nextScene = scenes[(sceneIndex + 1) % scenes.Length];
		}
		if ((sceneBlock.previousScene == null || sceneBlock.previousScene == "") && sceneIndex > 0) {
			sceneBlock.previousScene = scenes[sceneIndex - 1];
		}
		
	}
	
	/// <summary>
	/// Called when a scene finishes playing its intro flow.
	/// </summary>
	/// <param name='scene'>
	/// The scene.
	/// </param>
	public void OnSceneIntroFinished(SceneBlock scene)
	{
		Debug.Log("Scene INTRO: " + scene.name);
		
		// If this is the current scene because the previous scene was destroyed, start the idle flow
		if (loadedScenes[0] == scene) {
			// We go from intro to outro automatically
			AdvanceFlow();
			
			// Preload the next scene
			if (loadedScenes[0].preloadNextScene) {
				LoadScene(loadedScenes[0].nextScene);
			}
		}
	}
	
	/// <summary>
	/// Called when a scene finishes playing its idle flow.
	/// </summary>
	/// <param name='scene'>
	/// The scene.
	/// </param>
	public void OnSceneIdleFinished(SceneBlock scene)
	{
		Debug.Log("Scene IDLE: " + scene.name);
		
		if (GenericSettings.Instance.Autoplay) {
			AdvanceFlow();
		}
	}
	
	/// <summary>
	/// Called when a scene finishes playing its outro flow.
	/// </summary>
	/// <param name='scene'>
	/// The scene.
	/// </param>
	public void OnSceneOutroFinished(SceneBlock scene)
	{
		Debug.Log("Scene OUTRO: " + scene.name);
		DestroyScene(scene);
		
		// If the outro flow finished after the intro flow of the next scene, start the idle flow of the next scene
		if (loadedScenes.Count < 1 || !loadedScenes[0].IsPlaying()) {
			AdvanceFlow();
			
			// Preload the next scene
			if (loadedScenes[0].preloadNextScene) {
				LoadScene(loadedScenes[0].nextScene);
			}
		}
	}
	
	/// <summary>
	/// Advances the flow of the scenes, based on their states.
	/// </summary>
	public void AdvanceFlow()
	{
		Debug.Log("Advance Flow");
		
		SceneBlock currentScene = loadedScenes[0];
		
		if (currentScene.State == FlowBlock.FlowState.Outro) {
			DestroyScene(currentScene);
			
			currentScene = loadedScenes[0];
		}
		
		currentScene.AdvanceFlow();
		
		if (currentScene.State == FlowBlock.FlowState.Idle) {
			currentScene.PlayIdle();
			currentScene.OnSceneIdleFinished += OnSceneIdleFinished;
		}
		else if (currentScene.State == FlowBlock.FlowState.Outro) {
			currentScene.PlayOutro();
			currentScene.OnSceneOutroFinished += OnSceneOutroFinished;
			
			if (loadedScenes.Count == 1) {
				LoadScene(currentScene.nextScene);
			}
			
			if (loadedScenes.Count > 1) {
				StartScene(1);
			}
		}
	}
	
	/// <summary>
	/// Advances the flow manually. Used when Autoplay is off.
	/// </summary>
	public void AdvanceFlowManually()
	{
		if (GenericSettings.Instance.Autoplay) {
			return;
		}
		
		foreach (SceneBlock sceneBlock in loadedScenes)
		{
			if (sceneBlock.IsPlaying()) {
				return;
			}
		}
		
		AdvanceFlow();
	}
	
	/// <summary>
	/// Starts the loaded scene at the given index.
	/// </summary>
	/// <param name='index'>
	/// Index in the list of loaded scenes.
	/// </param>
	protected void StartScene(int index)
	{
		if (loadedScenes.Count > index) {
			loadedScenes[index].gameObject.SetActive(true);
			loadedScenes[index].PlayIntro();
			loadedScenes[index].OnSceneIntroFinished += OnSceneIntroFinished;
		}
	}
	
	/// <summary>
	/// Destroys the scene.
	/// </summary>
	/// <param name='scene'>
	/// Scene.
	/// </param>
	protected void DestroyScene(SceneBlock scene)
	{
		loadedScenes.Remove(scene);
		Destroy(scene.gameObject);
	}
	
	/// <summary>
	/// Jumps to the given scene.
	/// </summary>
	/// <param name='sceneName'>
	/// Scene name.
	/// </param>
	public void JumpToScene(string sceneName)
	{
		while (loadedScenes.Count > 0) {
			DestroyScene(loadedScenes[0]);
		}
		
		LoadScene(sceneName);
		StartScene(0);
	}
	
	/// <summary>
	/// Jumps to the previous scene.
	/// </summary>
	public void JumpToPreviousScene()
	{
		if (GenericSettings.Instance.Autoplay) {
			return;
		}
		
		if (loadedScenes.Count > 0) {
			JumpToScene(loadedScenes[0].previousScene);
		}
		else {
			JumpToScene(scenes[0]);
		}
	}
	
	/// <summary>
	/// Called when the application is paused/resumed by the OS. Sets the timescale to almost 0 to pause all sounds, animations etc.
	/// </summary>
	/// <param name='pause'>
	/// Tells if the application is paused or resumed.
	/// </param>
	void OnApplicationPause(bool pause)
	{
		Time.timeScale = pause ? 0.00001f : 1f;
	}
}

//}