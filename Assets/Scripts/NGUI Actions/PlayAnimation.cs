using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour
{
	public void PlayTheAnimation()
	{
		//Debug.LogWarning("CANTA FUTU-TI MORTII MATII!");
		GetComponent<Animation>().Stop();
		GetComponent<Animation>().clip = GetComponent<Animation>()["PauseFadeOut"].clip;
		GetComponent<Animation>()["PauseFadeOut"].normalizedSpeed = 1f;
		GetComponent<Animation>().Play("PauseFadeOut");
		StartCoroutine(CEMORTIIMAAAAATIIIIIII());
	}
	
	IEnumerator CEMORTIIMAAAAATIIIIIII()
	{
		while (GetComponent<Animation>()["PauseFadeOut"].time < GetComponent<Animation>()["PauseFadeOut"].length) {
			GetComponent<Animation>()["PauseFadeOut"].time += Time.deltaTime;
			GetComponent<Animation>().Sample();
//			Debug.Log(animation["PauseFadeOut"].normalizedTime);
//			Debug.Log(animation["PauseFadeOut"].normalizedSpeed);
			yield return null;
		}
	}
}

