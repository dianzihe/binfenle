using UnityEngine;
using System.Collections;

public class Pauser : MonoBehaviour
{
	public PlayMakerFSM fsm;
	public string pauseEvent = "AppPause";
	
	void OnApplicationPause(bool pause)
	{
		if (pause == false) {
			fsm.SendEvent(pauseEvent);
		}
	}
}

