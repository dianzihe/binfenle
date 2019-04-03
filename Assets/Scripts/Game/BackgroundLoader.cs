using UnityEngine;
using System.Collections;

public class BackgroundLoader : MonoBehaviour
{	
	[HideInInspector]public Light[] lights;
	[HideInInspector]public float defaultLightIntensity;

	public PowerUpSmoke powerUpSmoke;

	void Start() {
		lights = new Light[2];

		GameObject bgLandscape = GameObject.Instantiate(Resources.Load("Game/Backgrounds/Landscape/BGLandscape_" + MaleficentBlackboard.Instance.levelBg.ToString("000"))) as GameObject;
		if(OrientationListener.IsInstantiated())
			OrientationListener.Instance.activateOnLandscape.Add(bgLandscape);
		lights[0] = bgLandscape.GetComponentInChildren< Light >();
		defaultLightIntensity = lights[0].intensity;
		bgLandscape.SetActive(Screen.width > Screen.height);
		bgLandscape.transform.parent = transform;

		GameObject bgPortrait = GameObject.Instantiate(Resources.Load("Game/Backgrounds/Portrait/BGPortrait_" + MaleficentBlackboard.Instance.levelBg.ToString("000"))) as GameObject;
		if(OrientationListener.IsInstantiated())
			OrientationListener.Instance.activateOnPortrait.Add(bgPortrait);
		lights[1] = bgPortrait.GetComponentInChildren< Light >();
		bgPortrait.SetActive(Screen.width <= Screen.height);
		bgPortrait.transform.parent = transform;
	}


	void Update() {
		if(Input.GetKeyDown(KeyCode.Q)) {
			powerUpSmoke.fsm.ChangeState(powerUpSmoke.showFlames);
		}
		if(Input.GetKeyDown(KeyCode.A)) {
			powerUpSmoke.fsm.ChangeState(powerUpSmoke.hideFlames);
		}
	}
}

