using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

public class ScriptFsmEventsListener : MonoBehaviour {	
	protected PlayMakerFSM[] fsmComponents;
		
	void Awake() {
		fsmComponents = GetComponents<PlayMakerFSM>();
	}
	
	public void SendFsmEvent(string eventName)
	{
		for(int i = 0; i < fsmComponents.Length; i++) 
		{
//			Debug.Log("Sending event " + eventName + " to FSM: " + fsmComponent.name);
			fsmComponents[i].Fsm.Event(eventName);
		}
	}	
}
