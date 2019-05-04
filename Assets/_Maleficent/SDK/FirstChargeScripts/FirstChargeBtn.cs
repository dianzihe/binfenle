using UnityEngine;
using System.Collections;

public class FirstChargeBtn : MonoBehaviour 
{
	public PlayMakerFSM fsm;
	public string sendEvent ="";
	
	void OnClick()
	{
		if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;
		/*
		ShopUINotifyer.getInstance ().IndexUI = 0;
		if(ExchangeUIFlag.getInstance().m_bShow==false)
		{
			return;
		}
		*/

		fsm.SendEvent(sendEvent);
	}
}
