using UnityEngine;
using System.Collections;

public class ChangeCodeShowController : MonoBehaviour {
	public GameObject Infoshow,Shopshow,Storeshow,Liveshow,Giftshow;
	int count=0;
	public bool showui;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void showotherUI()
	{
		showui = true;
		EnableCollider (showui);
	}
	public void notshowotherUI()
	{
		showui = false;
		EnableCollider (showui);
	}
	void EnableCollider(bool bEnable)
	{
		Infoshow.GetComponent<BoxCollider> ().enabled = bEnable;
		Shopshow.GetComponent<BoxCollider> ().enabled = bEnable;
		Storeshow.GetComponent<BoxCollider> ().enabled = bEnable;
		Liveshow.GetComponent<BoxCollider> ().enabled = bEnable;
		Giftshow.GetComponent<BoxCollider> ().enabled = bEnable;
	}

}
