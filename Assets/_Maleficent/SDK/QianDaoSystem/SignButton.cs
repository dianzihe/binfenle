using UnityEngine;
using System.Collections;

public class SignButton : MonoBehaviour {

	public PlayMakerFSM fsm;
	public string sendEvent ="SignClose";
	
	void OnClick()
	{
		if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;
		SignPanel.instanse.gameObject.SetActive (true);
		if(ExchangeUIFlag.getInstance().m_bShow==false)
		{
			return;
		}
		fsm.SendEvent(sendEvent);
		ColliderManager.ToggleCollider (false);
		//print("click");
	}
}
