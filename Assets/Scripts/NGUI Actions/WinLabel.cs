using UnityEngine;
using System.Collections;

public class WinLabel : MonoBehaviour
{
	public Match3BoardGameLogic gameLogic;
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		label.text = gameLogic.winConditions.GetWinString();
	}
	
	void Update () 
	{
		label.text = gameLogic.winConditions.GetWinString();
	}
}

