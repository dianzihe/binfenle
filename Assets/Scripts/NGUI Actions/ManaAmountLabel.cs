using UnityEngine;
using System.Collections;

public class ManaAmountLabel : MonoBehaviour
{
	UILabel label;
	
	void Start () 
	{
		label = GetComponent<UILabel>();
		ScoreSystem.Instance.OnManaUpdated += UpdateManaWon;
	}
	
	public void UpdateManaWon()
	{
		label.text = "+" + ScoreSystem.Instance.Mana.ToString();
	}

	void OnDestroy()
	{
		ScoreSystem.Instance.OnManaUpdated -= UpdateManaWon;
	}
}

