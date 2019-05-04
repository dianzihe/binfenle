using UnityEngine;
using System.Collections;

public class ChangeCodeButton : MonoBehaviour {
	public UIInput MyInput;
	public UILabel changelable;
	//public UIInput input;
	//public string changeName;
	//string changeService;
	public bool ischange=false;
	static int count=0;
	// Use this for initialization
	void Start () {
		//changelable.text="hwf";
		//MyInput.validator += OnValidator;
	}

	// Update is called once per frame
	void Update () {
		//string	changeName = changelable.text;//获取用户输入的兑换码信息
		//Debug.Log("count="+count);
		//Debug.Log ("changeName=" + changeName);
		//count++;


	}
	void OnClick(){
		//MyInput
	}

	/*
	char OnValidator(string currentText, char nextChar)
	{
		Debug.Log(currentText + nextChar);
		return nextChar;
	}
	*/
}
