using UnityEngine;
using System.Collections;

public class ElsaIceEffect : MonoBehaviour
{
	public GameObject effectPrefab;
	public float delay;
	public float lifeTime = 3f;
	
	protected PlayMakerFSM myFsm;
	
	public void SetFSM(PlayMakerFSM fsm)
	{
		myFsm = fsm;
	}
	
	public void CreateIceEffect(GameObject parentObj)
	{
		if (delay > 0f) {
			StartCoroutine(CreateIceEffectWithDelay(parentObj));
		}
		else {
			ActuallyCreateEffect(parentObj);
		}
	}
	
	IEnumerator CreateIceEffectWithDelay(GameObject parentObj)
	{
		yield return new WaitForSeconds(delay);
		if (myFsm.ActiveStateName == "Look at items New") {
			ActuallyCreateEffect(parentObj);
		}
	}
	
	void ActuallyCreateEffect(GameObject parentObj)
	{
		GameObject effect = GameObject.Instantiate(effectPrefab) as GameObject;
		Vector3 pos = effect.transform.localPosition;
		Quaternion rot = effect.transform.localRotation;
		Vector3 sca = effect.transform.localScale;
		
		effect.transform.parent = parentObj.transform.parent;
		effect.transform.localPosition = pos;
		effect.transform.localRotation = rot;
		effect.transform.localScale = sca;
		
		Destroy(effect, lifeTime);
	}
}

