using UnityEngine;
using System.Collections;

public class ClearInput : MonoBehaviour {
	ChangeCodeButton changeCB;
	// Use this for initialization
	void Start () {
		changeCB=GameObject.Find("UI Root Portrait/Camera/Anchor/Scaler/ChangeCode Panel Portrait/Input").GetComponent<ChangeCodeButton>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnClick(){
		changeCB.MyInput.text = null;
	}
}
