using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationTest : MonoBehaviour {

	public GameObject avatarPrefab;
	
	private GameObject avatar;
	private Animation anim;
	
	public List< AnimationClip > queuedAnims = new List< AnimationClip >();
	
	void Start() {
		avatar = (GameObject.Instantiate(avatarPrefab) as GameObject);
		anim = avatar.GetComponentInChildren< Animation >();
	}
	
	void Update() {
		if(!anim.isPlaying) {
			if(queuedAnims.Count > 0) {
				anim.CrossFade(queuedAnims[0].name);
				queuedAnims.RemoveAt(0);
			} else {
				anim.CrossFade("Idle_01");
			}
		}
	
		for(int key = (int)KeyCode.Alpha0; key < (int)KeyCode.Alpha9; ++key) {
			int idx = key - (int)KeyCode.Alpha0;
			/*switch(idx) {
				case 0: queuedAnims.Add(anim.GetClip("Idle_01")); break;
			}*/
			
			if(Input.GetKeyDown((KeyCode)key) && idx < anim.GetClipCount()) {
				queuedAnims.Add(GetAnimationByIndex(anim, idx));
			}
		}
	}
	
	private AnimationClip GetAnimationByIndex(Animation _anim, int _index) {
		int i = 0;
		foreach(AnimationState clip in _anim) {
			if(i == _index) {
				return _anim.GetClip(clip.name);
			}
			i ++;
		}
		return null;
	}
}
