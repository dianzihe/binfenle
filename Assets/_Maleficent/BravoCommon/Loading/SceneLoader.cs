using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manage scene transisitons and free resources loading an empty scene before loading the new one.
/// </summary>
public class SceneLoader : MonoBehaviour 
{
	private static LinkedList< string > nextScenes = new LinkedList<string>();
	private static bool 	alreadyLoading 	= false;
	private static bool 	additiveLoad 	= false;
	private static bool 	showResourcesBetweenScenes = false;
	
	private static SceneTransition sceneTransition;
	
	private static SceneLoader	   sceneLoader;
	
	#region MONOBEHAVIORS
	
	/// <summary>
	/// Visual debug on editor for the scene
	/// </summary>
	void OnGUI()
	{
		if(Debug.isDebugBuild &&  showResourcesBetweenScenes)
		{
			ResourceDisplayer.DrawInfo();
			if(GUILayout.Button("Continue"))
			{
				//sceneLoader.StartCoroutine(Load());
				Load ();
			}
		}
	}
	
	#endregion
	
	#region PUBLIC LOADING METHODS

	public static void LoadScene(string _nextLevel, bool _additive, GameObject _transitionType = null)
	{	
		LoadScene(new string[] {_nextLevel}, _additive, _transitionType);
	}
	
	public static void LoadScene(string[] _nextLevels, bool _additive, GameObject _transitionType = null)
	{	
		// Avoid several calls to the loader (it happens)
		if(alreadyLoading && !_additive)
		{
			Debug.LogError("Trying to load scene non additively while a current loading is still in progress");
			return;
		}		
		
		if(!alreadyLoading)
		{
			if(_transitionType == null)
			{
				_transitionType = Instantiate(Resources.Load("MaleficentTransition", typeof(GameObject))) as GameObject;
				if(_transitionType == null)
				{
					Debug.LogError("Transitions/DefaultTransition prefab must exist in a resources directory in order this class works");
				}
			}
			sceneTransition = _transitionType.GetComponent< SceneTransition >();
			if(sceneTransition == null)
			{
				Debug.LogError(_transitionType.name+" prefab must have a SceneTransition script attached"); 
			}
			
			// Set loading config
			alreadyLoading 	= true;
			
			foreach(string level in _nextLevels)
				nextScenes.AddLast(level);
			additiveLoad 	= _additive;
			
			
			// Disable all inputs
			EnableInput(false);
			
			// Start loading scene process
			sceneLoader 	= new GameObject("LoadingScene").AddComponent<SceneLoader>();
			_transitionType.transform.parent = sceneLoader.transform;
			DontDestroyOnLoad(sceneLoader.gameObject);
			DontDestroyOnLoad(sceneTransition);
			DontDestroyOnLoad(_transitionType);
			
			
			// First Transition from splash screen over first scene, setting a Callback on complete to 'startLoading()'
			sceneTransition.Enter(sceneLoader.StartLoading);
		}
		else
		{
			foreach(string level in _nextLevels)
				nextScenes.AddLast(level);
		}
	}
	
	#endregion
	
	#region LOADING STEPS
	
	/// <summary>
	/// Load an empty level before the new one to clean unity assets cache
	/// </summary>
	public void StartLoading()
	{		
		// Unloads the previous scene now (loading an empty scene)
		if(!additiveLoad) {
			Application.LoadLevelAsync("Empty");
			Resources.UnloadUnusedAssets();
		}
		
		// Begin to load additively next scene
		if(!Debug.isDebugBuild || !showResourcesBetweenScenes) 
		{
			//sceneLoader.StartCoroutine(Load());
			Load();
		}
	}
	
	/// <summary>
	/// Load desired scenes
	/// </summary>
	public void Load()
	{		
		Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
		
		//yield return new WaitForEndOfFrame();
		
		Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;	
		// Load all the new scenes and wait until their completion
		for(LinkedListNode< string > it = nextScenes.First; it != null; it = it.Next)
		{
			AsyncOperation async;
			string stage = it.Value;
			if(stage == nextScenes.First.Value)
			{
				if(additiveLoad) {
					async = Application.LoadLevelAdditiveAsync(stage);	
				} else {
					//async = Application.LoadLevelAsync(stage);	
					Application.LoadLevel(stage);	
				}
			}
			else {	
				async = Application.LoadLevelAdditiveAsync(stage);
			}
			
			//async.priority = 0;
			//yield return async;
		}
		// Final Transition when scene get loaded from splash, setting a Callback on complete to 'finishAndFreeResources()'
		sceneTransition.Exit(EndLoading);
	}
	
	/// <summary>
	/// Free memory
	/// </summary>
	public void EndLoading()
	{	
		// Disable all inputs
		EnableInput(true);
		
		// Unload loading resources
		Destroy(sceneTransition);
		sceneTransition = null;
		Destroy(sceneLoader.gameObject);
		Resources.UnloadUnusedAssets();
		
		
		// Reset statics to default state
		alreadyLoading = false;
		nextScenes.Clear();
		
		Time.timeScale = 1.0f;
	}
	
	#endregion
	
	#region CONTROL METHODS
	
	/// <summary>
	/// Disable all scene inputs
	/// </summary>
	public static void EnableInput(bool _enable)
	{
		foreach(Object inputManager in GameObject.FindObjectsOfType(typeof(InputManager)))
		{
			MonoBehaviour gameObj = inputManager as MonoBehaviour;
			
			gameObj.GetComponent<InputManager>().enabled = _enable;
		}
	}
	
	#endregion
}
