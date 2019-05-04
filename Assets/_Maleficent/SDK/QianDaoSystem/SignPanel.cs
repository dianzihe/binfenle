using UnityEngine;
using System.Collections;

public class SignPanel : MonoBehaviour {
	public UIEventListener closebtn=null;
	public PlayMakerFSM pfsm;
	public string closeevent;
	public static SignPanel instanse=null;
	public static SignPanel getInstanse()
	{
		return instanse;
	}
	void Awake()
	{
		instanse = this;
		instanse.gameObject.SetActive (false);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (closebtn != null) {
			closebtn.onClick=OnCloseClick;
		}
	}
	void OnCloseClick(GameObject go)
	{
		pfsm.SendEvent (closeevent);
		instanse.gameObject.SetActive (false);
	}
}
