using UnityEngine;
using System.Collections;

public class ShopBtn : MonoBehaviour {
	public PlayMakerFSM fsm;
	public string sendEvent ="";
	public static ShopBtn instanse;
	public ShopBtn()
	{
		instanse = this;
	}
	void OnDestroy()
	{
		instanse = null;
	}
	void OnClick()
	{
		shopshow ();
	}
	public void shopshow()
	{
		if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;
		ShopUINotifyer.getInstance ().IndexUI = 0;
		if(ExchangeUIFlag.getInstance().m_bShow==false)
		{
			return;
		}
		fsm.SendEvent(sendEvent);
	}
}
