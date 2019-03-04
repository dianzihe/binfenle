using UnityEngine;
using System.Collections;

public class LivesButton : MonoBehaviour 
{
	public PlayMakerFSM fsm;
	//public string sendEvent = "GameShopBegin";
	public string sendEvent = "Lives";
	
	UISprite mySprite;
	
	void Awake()
	{
		mySprite = gameObject.GetComponent<UISprite>();
	}
	
	void OnClick()
	{

		if (LivesSystem.lives < 5) {
			if(BookAnimations.Instance.currentState != BookAnimations.BookAnimationsState.fixedState)
				return;
			if(ExchangeUIFlag.getInstance().m_bShow==false)
			{
				return;
			}	
			if (mySprite.enabled)// && LivesSystem.lives != LivesSystem.maxLives) 
			{
				fsm.SendEvent(sendEvent);
			}
		}
		//ShopUINotifyer.getInstance ().IndexUI = 2;



	}
}
