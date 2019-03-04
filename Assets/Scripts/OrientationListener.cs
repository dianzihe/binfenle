using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrientationListener : MonoBehaviour 
{
	private static OrientationListener instance;	
	
	public delegate void OrientationChangedEventHandler(ScreenOrientation newOrientation);
	
	public event OrientationChangedEventHandler OnOrientationChanged;
	
	public List<GameObject> activateOnPortrait;
	public List<GameObject> activateOnLandscape;
	
	public List<Behaviour> activateScriptsOnPortrait;
	public List<Behaviour> activateScriptsOnLandscape;
	
	public List<RepositionPair> repositionOnPortrait;
	public List<RepositionPair> repositionOnLandscape;
	
	private int screenWidth;
	private int screenHeight;
	
	public static OrientationListener Instance {
		get {
			if (instance) {
				return instance;
			} 
		

			instance = GameObject.FindObjectOfType(typeof(OrientationListener)) as OrientationListener;
			
			if (instance) {
				return instance;
			}

			GameObject instanceObj = new GameObject("Orientation Listener");
			instance = instanceObj.AddComponent<OrientationListener>();
			
			return instance;
		}
	}

	public static bool IsInstantiated ()
	{
		return instance != null;
	}
	
	// Use this for initialization
	void Awake ()
	{

		instance = this;
		
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		
		if (activateOnPortrait == null) {
			activateOnPortrait = new List<GameObject>();
		}
		if (activateOnLandscape == null) {
			activateOnLandscape = new List<GameObject>();
		}
		if (activateScriptsOnPortrait == null) {
			activateScriptsOnPortrait = new List<Behaviour>();
		}
		if (activateScriptsOnLandscape == null) {
			activateScriptsOnLandscape = new List<Behaviour>();
		}
		if (repositionOnPortrait == null) {
			repositionOnPortrait = new List<RepositionPair>();
		}
		if (repositionOnLandscape == null) {
			repositionOnLandscape = new List<RepositionPair>();
		}


	}

	void Start()
	{
		HandleObjects();
	}
	
	private void HandleObjects()
	{
		foreach (GameObject gameObj in activateOnPortrait) {
			gameObj.SetActive(Screen.width <= Screen.height);
		}
		
		foreach (GameObject gameObj in activateOnLandscape) {
			gameObj.SetActive(Screen.width > Screen.height);
		}
		
		foreach (Behaviour behavior in activateScriptsOnPortrait) {
			behavior.enabled = Screen.width <= Screen.height;
		}
		
		foreach (Behaviour behavior in activateScriptsOnLandscape) {
			behavior.enabled = Screen.width > Screen.height;
		}
		
		if (Screen.width >= Screen.height) {
			foreach (RepositionPair pair in repositionOnLandscape) {
				pair.obj.transform.position = pair.pos;
			}
		}
		
		if (Screen.width < Screen.height) {
			foreach (RepositionPair pair in repositionOnPortrait) {
				pair.obj.transform.position = pair.pos;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Screen.width != screenWidth || Screen.height != screenHeight) { //orientation has changed
			screenWidth = Screen.width;
			screenHeight = Screen.height;
			
			HandleObjects();
			
			if (OnOrientationChanged != null) {
				OnOrientationChanged(Screen.width > Screen.height ? ScreenOrientation.Landscape : ScreenOrientation.Portrait);
			}
		}
	}
}

[System.Serializable]
public class RepositionPair 
{	
	public GameObject obj;
	public Vector3 pos;
}