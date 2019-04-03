using UnityEngine;
using System.Collections;

public class WolfConfig : MonoBehaviour {

	public Camera cameraLandscape;
	public Camera cameraLandscapeWolf;
	public Camera cameraPortrait;
	public GameObject wolf;
	public ParticleSystem enteringParticles;
	public ParticleSystem leavingParticles;
	//public GameObject crow;
	//public ParticleSystem particles;
	
	public float timeStartPlatingAnim = 1.0f;
	public float timePlayEnteringParticles = 1.2f;
	public float timeStartHowlSound = 2.0f;
	public float timePlayLeavingParticles = 3.5f;
	public float timeMoveCameraBack = 4.15f;
	
	public float previousAspectRatio;
	public void Update() {
		float aspect = ((float)Screen.width / Screen.height);
		if(aspect != previousAspectRatio) {
			if(aspect < 1.0f) {
				//portrait
				cameraPortrait.gameObject.SetActive(true);
				cameraLandscapeWolf.gameObject.SetActive(false);
			} 
			if(aspect > 1.0f) {
				//landscape
				cameraPortrait.gameObject.SetActive(false);
				cameraLandscapeWolf.gameObject.SetActive(true);
			}
			previousAspectRatio = aspect;
		}
	}
}
