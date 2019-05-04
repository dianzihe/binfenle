using UnityEngine;
using System.Collections;

public class ExchangeCodePanelManager : MonoBehaviour 
{
	public UIEventListener m_btnExchangeCodeListener=null;
	public PlayMakerFSM m_Fsm=null;
	public string m_strSendEvent = "ExchangeCodePanelIn";

	// Use this for initialization
	void Start () 
	{
		if(m_btnExchangeCodeListener!=null)
		{
			m_btnExchangeCodeListener.onClick=OnExchangeCodeBtnNotify;
		}
	}
	void OnExchangeCodeBtnNotify(GameObject go)
	{
		if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
			return;

		if(m_Fsm!=null)
		{
			m_Fsm.SendEvent(m_strSendEvent);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
