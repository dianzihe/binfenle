using UnityEngine;
using System.Collections;

public class IngameCrow : MonoBehaviour {

	public Transform pivotPortrait;
	public Transform pivotLandscape;

	private FSM fsm;
	public StateIdle        stateIdle;
	public StateFlying      stateFlying;
	public StateGettingBack stateGettingBack;
	public StateFreeze			stateFreeze;

	public AudioClip soundEffectFlyOut;
	public AudioClip soundeffectFlyIn;

	// Use this for initialization
	void Start () {
		if(MaleficentBlackboard.Instance.character < 2) {
			Destroy(gameObject);
			return;
		}
	
		fsm = gameObject.AddComponent< FSM >();
		fsm.ChangeState(stateIdle);

		MaleficentTools.ChangeLayerRecursively(gameObject,  "Effects");
		MaleficentTools.DoOnNextFrame(this, 
		    () => {
				CalculatePosition();
				ManaPowerBallController.OnManaPowerBallWillOpen += OnManaPowerBallWillOpen;
				ManaPowerBallController.OnManaPowerBallWillClose += OnManaPowerBallWillClose;
			}
		);

		OrientationListener.Instance.OnOrientationChanged += OnOrientationChanged;
	}
	
	void OnDestroy() {
		ManaPowerBallController.OnManaPowerBallWillOpen -= OnManaPowerBallWillOpen;
		ManaPowerBallController.OnManaPowerBallWillClose -= OnManaPowerBallWillClose;
	}

	private void OnOrientationChanged(ScreenOrientation _orientation) {
		MaleficentTools.DoOnNextFrame(this, 
      		() => {
					CalculatePosition();
				}
			);
	}

	private void CalculatePosition() {
		Transform pivot = (Screen.width > Screen.height) ? pivotLandscape : pivotPortrait;

		transform.position = MaleficentTools.ConvertPositionBetweenLayers(pivot.position, LayerMask.LayerToName(pivot.gameObject.layer), LayerMask.LayerToName(gameObject.layer), 15.0f);
		transform.rotation = pivot.rotation;
	}
	
	private void OnManaPowerBallWillOpen(ManaPowerBallController _manaPowerBallController) {
		FlyAway();
	}
	
	private void OnManaPowerBallWillClose(ManaPowerBallController _manaPowerBallController) {
		MaleficentTools.DoOnNextFrame(this, () => {
			ManaItem item = _manaPowerBallController.powerUpInvoker.itemPrefabManaItem;
			if(item  == null || (!(item is Crow2nd) && !(item is WolfHowl)))
			{
				GetBack();
			}
		});
	}
	
	public void FlyAway(System.Action _onDone = null) {
		stateFlying.gone = _onDone;
		fsm.ChangeState(stateFlying);

		SoundManager.Instance.PlayOneShot(soundEffectFlyOut);
	}
	
	public void GetBack(System.Action _onDone = null) {
		stateGettingBack.gotBack = _onDone;
		fsm.ChangeState(stateGettingBack);

		SoundManager.Instance.PlayOneShot(soundeffectFlyIn);
	}

	public void Freeze () {
		fsm.ChangeState(stateFreeze);
	}

	public void Idle () {
		fsm.ChangeState(stateIdle);
	}

	/*void Update() {
		if(Input.GetKeyDown(KeyCode.Alpha0)) {
			fsm.ChangeState(stateIdle);
		}

		if(Input.GetKeyDown(KeyCode.Alpha1)) {
			fsm.ChangeState(stateFlying);
		}

		if(Input.GetKeyDown(KeyCode.Alpha2)) {
			fsm.ChangeState(stateGettingBack);
		}
	}*/

	[System.Serializable]
	public class StateIdle : FSM.State {
		public Animation animation;
		public float anim01Chance = 0.5f;
		public float anim02Chance = 0.9f;
		public float anim03Chance = 1.0f;


		private void SelectAnim() {
			float p = Random.Range(0, 1.0f);
			if(p <= anim01Chance) {
				animation.CrossFade("Idle_01");
			} else if (p > anim01Chance && p <= anim02Chance) {
				animation.CrossFade("Idle_02");
			} else {
				animation.CrossFade("Idle_03");
			}
		}

		public override void Enter () {
			SelectAnim();
		}

		public override void Execute () {
			if(!animation.isPlaying) {
				SelectAnim();
			}
		}
	}

	[System.Serializable]
	public class StateFlying : FSM.State {
		public Animation animation;
		public System.Action gone;

		private bool isGone;

		public override void Enter () {
			animation.CrossFade("Salida_01");
			isGone = false;
		}

		public override void Execute () {
			if(!isGone) {
				if(!animation.isPlaying) {
					isGone = true;
					if(gone != null) {
						gone();
					}
				}
			}
		}
	}

	[System.Serializable]
	public class StateGettingBack : FSM.State {
		public Animation animation;
		public System.Action gotBack;
		
		public override void Enter () {
			animation.CrossFade("Entrada_01");
		}

		public override void Execute () {
			if(!animation.isPlaying) {
				if(gotBack != null) {
					gotBack();
				}
			
				ChangeState(fsm.GetComponent< IngameCrow >().stateIdle);
			}
		}
	}

	[System.Serializable]
	public class StateFreeze : FSM.State {
	}
}
