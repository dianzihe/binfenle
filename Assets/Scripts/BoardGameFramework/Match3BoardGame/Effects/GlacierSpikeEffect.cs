using UnityEngine;
using System.Collections;

public class GlacierSpikeEffect : DestroyEffect
{
	public void StartParticles()
	{
		Transform break1 = transform.Find("Break1");
		Transform break2 = transform.Find("Break2");
		break1.gameObject.SetActive(true);
		break2.gameObject.SetActive(true);
		
		break1.eulerAngles = break1.localEulerAngles + new Vector3(270f, 0f, 0f);
		break2.eulerAngles = break2.localEulerAngles + new Vector3(270f, 0f, 0f);
		
		Transform collisionPlane = transform.Find("Collision");
		Vector3 newPos = collisionPlane.position;
		//newPos.y = BottomCollider.Instance.PosY;
		collisionPlane.position = newPos;
	}
}

