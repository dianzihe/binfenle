using UnityEngine;
using System.Collections;

public enum AvatarParticlesEffectType
{
	YellowPixieDust,
	Crow,
	GreenMagic,
	CrowSwipe,
}

public class AvatarParticles : MonoBehaviour {

	public Transform effectLeftHand;
	public Transform effectRightHand;
	public Transform effectBottom;

	public GameObject yellowPixieDustEffect;
	public GameObject crowEffect;
	public GameObject greenMagicEffect;
	public GameObject crowSwipeEffect;

	private static AvatarParticles instance;
	public static AvatarParticles Instance {
		get  {
			return instance;
		}
	}

	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
	}
	
	void OnDestroy() {
		instance = null;
	}


	public void ActivateEffect(AvatarParticlesEffectType effect) {
		_SetEffectActive(effect, true);
	}


	private void _SetEffectActive(AvatarParticlesEffectType effect, bool active)
	{
		GameObject effectToActivate = null;
		Transform effectTransform = this.transform;

		switch (effect) {
		case (AvatarParticlesEffectType.YellowPixieDust):
			effectToActivate = yellowPixieDustEffect;
			effectTransform = effectRightHand;
			break;
		case (AvatarParticlesEffectType.Crow):
			effectToActivate = crowEffect;
			effectTransform = effectRightHand;
			break;
		case (AvatarParticlesEffectType.GreenMagic):
			effectToActivate = greenMagicEffect;
			effectTransform = effectRightHand;
			break;
		case (AvatarParticlesEffectType.CrowSwipe):
			effectToActivate = crowSwipeEffect;
			effectTransform = effectRightHand;
			break;
		}
		
		if (effectToActivate != null) {
			//effectToActivate.SetActive(active);

			GameObject effectCopy = GameObject.Instantiate(effectToActivate) as GameObject;
			Transform originalTransform = effectCopy.transform;

			effectCopy.transform.parent = effectTransform;
			effectCopy.transform.localPosition = originalTransform.position;

		} else {
			Debug.Log("[AvatarParticles] Trying to activate a null effect : " + effect);
		}
	}

}
