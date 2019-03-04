using UnityEngine;
using System.Collections;

public class TimeRefillLabel : MonoBehaviour 
{
	UILabel label;
	public UILabel timerLabel;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		label.text = timerLabel.text;
	}
}
