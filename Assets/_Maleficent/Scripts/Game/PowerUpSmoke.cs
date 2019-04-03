using UnityEngine;
using System.Collections;

public class PowerUpSmoke : MonoBehaviour {

	[HideInInspector]public FSM fsm;
	public ShowFlames showFlames;
	public HideFlames hideFlames;

	public Material m;

	// Use this for initialization
	void Start () {
		fsm = gameObject.AddComponent< FSM >();
		fsm.ChangeState(hideFlames);
	}

	public void Show() {
		fsm.ChangeState(showFlames);
	}

	public void Hide() {
		fsm.ChangeState(hideFlames);
	}

	[System.Serializable]
	public class ShowFlames : FSM.State {
		public float speed = 10.0f;
		public float k;
		public float limit;
		public Light light;
		public BackgroundLoader backgroundLoader;

		public override void Enter () {
			fsm.gameObject.SetActive(true);
		}

		public override void Execute () {
			Vector3 pos = fsm.transform.position;
			pos.y += (limit - pos.y) * speed - k;
			fsm.transform.position = pos;

			light.intensity = Mathf.Lerp(light.intensity, 0.59f, 0.04f);
			foreach(Light l in backgroundLoader.lights) {
				l.intensity = Mathf.Lerp(l.intensity, 0.0f, 0.04f);
			}
		}
	}

	[System.Serializable]
	public class HideFlames : FSM.State {
		public float speed = 10.0f;
		public Light light;
		public BackgroundLoader backgroundLoader;

		public override void Execute () {
			fsm.transform.Translate(-Vector3.up * speed * Time.deltaTime);

			light.intensity = Mathf.Lerp(light.intensity, 0.0f, 0.04f);
			foreach(Light l in backgroundLoader.lights) {
				l.intensity = Mathf.Lerp(l.intensity, backgroundLoader.defaultLightIntensity, 0.04f);
			}

			if(fsm.transform.position.y < -137.1785f) {
				foreach(Light l in backgroundLoader.lights) {
					l.intensity = backgroundLoader.defaultLightIntensity;
				}
				fsm.gameObject.SetActive(false);
			}
		}
	}
}
