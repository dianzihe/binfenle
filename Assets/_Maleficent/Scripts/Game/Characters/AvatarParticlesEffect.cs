using UnityEngine;
using System.Collections;

public class AvatarParticlesEffect : MonoBehaviour {

	public float duration = 1.0f;

	void Start()
	{
		Destroy(this.gameObject, duration);
	}

}
