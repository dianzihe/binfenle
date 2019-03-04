using UnityEngine;
using System.Collections;

public class LevelIndexLabel : MonoBehaviour 
{	
	UILabel myLabel;
	
	// Use this for initialization
	void Start () {
		myLabel = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
		myLabel.text = Language.Get("LEVEL_NAME") + " " + (MaleficentBlackboard.Instance.level);
	}
}
