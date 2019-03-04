using UnityEngine;
using System.Collections;

public class QualityDisabler : MonoBehaviour {

	public GameObject[] gameObjectsToDisable;

	// Use this for initialization
	void Start () {
		if(QualitySettings.GetQualityLevel() == 0) {
			foreach(GameObject go in gameObjectsToDisable) {
				go.SetActive(false);
			}
		}
	}
}
