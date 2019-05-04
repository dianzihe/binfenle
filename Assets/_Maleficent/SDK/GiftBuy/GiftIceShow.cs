using UnityEngine;
using System.Collections;

public class GiftIceShow : MonoBehaviour {
	public PlayMakerFSM fsm;
	public string sendEvent ="GiftShow";
	// Use this for initialization
	void Start () {
		if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;
		if(ExchangeUIFlag.getInstance().m_bShow==false)
		{
			return;
		}
		if (GiftIsShow.getinstanse ().showgift == 2) {
			fsm.SendEvent(sendEvent);	
		}

	}
	
	// Update is called once per frame
	void Update () {
	}
}
