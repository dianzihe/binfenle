using UnityEngine;
using System.Collections;

public class Blackboard : MonoBehaviour {
	
	public static GameObject instance = null;
	public static GameObject Instance {
		get {
			if(instance == null) {
				instance = (Instantiate(Resources.Load("Blackboard")) as GameObject);
				if(instance == null) {
					Debug.Log("Blackboard prefab not found. Please create a Blackboard and place it on Resource folder");
				} else {
					instance.name = "Blackboard";
					DontDestroyOnLoad(instance);
				}
			} 
			
			return instance;
		}
	}
	
}
