using UnityEngine;
using System.Collections;

public class ObjectiveLevel : MonoBehaviour
{
	public Match3BoardGameLogic gameLogic;
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		label.text = gameLogic.winConditions.GetObjectiveString();
	}
}

