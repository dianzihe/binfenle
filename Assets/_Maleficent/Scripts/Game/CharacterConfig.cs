using UnityEngine;
using System.Collections;

public class CharacterConfig : MonoBehaviour {

	public CameraManager cameraLandscapeManager;
	public CameraManager cameraPortraitManager;
	public Camera cameraLandscape;
	public Camera cameraPortrait;
	
	private static CharacterConfig instance;
	public static CharacterConfig Instance {
		get {
			return instance;
		}
	}
	
	
	public void Start() {
		if(instance != null) {
			Debug.LogError("There are two CharacterConfig on this scene!!!");
		}
	
		instance = this;
	}
	
	public void OnDestroy() {
		instance = null;
	}
}
