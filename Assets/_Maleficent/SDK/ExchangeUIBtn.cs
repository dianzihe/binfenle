using UnityEngine;
using System.Collections;

public class ExchangeUIBtn : MonoBehaviour 
{
	public PlayMakerFSM fsm;
	public string sendEvent ="";
	
	void OnClick()
	{
		if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;
		if(ExchangeUIFlag.getInstance().m_bShow==false)
		{
			return;
		}
		fsm.SendEvent(sendEvent);
	}
}
