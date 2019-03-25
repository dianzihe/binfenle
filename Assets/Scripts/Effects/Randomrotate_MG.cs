using UnityEngine;
using System.Collections;

public class Randomrotate_MG : MonoBehaviour {
	
	void Start () {
		transform.Rotate(0,0,Random.Range(0f, 359f)); 
	}
	
}
