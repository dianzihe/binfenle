using UnityEngine;
using System.Collections;

public class StarAnimation : MonoBehaviour 
{
	public int starIndex;
	
	protected GameObject thatsMySon;
	
	void Start () 
	{
		thatsMySon = transform.GetChild(0).gameObject;
		thatsMySon.SetActive(false);
		
		WinScore.OnNewStarReached += UpdateStars;
	}
	
	void UpdateStars(int count) 
	{
		if (count == starIndex) {
			thatsMySon.SetActive(true);
			thatsMySon.GetComponent<Animation>().Play();
		}
	}
	
	void OnDestroy()
	{
		WinScore.OnNewStarReached -= UpdateStars;
	}
}
