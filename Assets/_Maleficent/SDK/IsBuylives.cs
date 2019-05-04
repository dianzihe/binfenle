using UnityEngine;
using System.Collections;

public class IsBuylives : MonoBehaviour {
	public bool isshow=false;
	public PlayMakerFSM fsm;
	public string sendEvent = "GameShopBegin";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Buylives()
	{
		isshow = true;
		isshowbuy (isshow);
	}
	public void NotShow()
	{
		isshow = false;
		isshowbuy (isshow);
	}
	void isshowbuy(bool showshop)
	{
		if (showshop) {
			print("show");
			if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
				return;
			ShopUINotifyer.getInstance ().IndexUI = 2;
			fsm.SendEvent(sendEvent);
		}
	}
}
