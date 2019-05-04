using UnityEngine;
using System.Collections;

public class SignServerReadText : MonoBehaviour {
	public static SignServerReadText instanse=null;
	public string TimeResult;
	public static SignServerReadText GetInstanse()
	{
		return instanse;
	}
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
		instanse = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void ReadTimeFromServer(string result)
	{
		TimeResult = result;
	}
}
