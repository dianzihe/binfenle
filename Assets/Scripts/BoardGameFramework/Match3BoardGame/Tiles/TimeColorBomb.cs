using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class TimeColorBomb : ColorBombTile
{
	public float time = 2f;
	
	//Start Timer onIinit
	public override void InitComponent () {
		
		base.InitComponent();
	
		StartCoroutine(Timer());
	}
	
	//Timer coroutine
	protected IEnumerator Timer()
	{
		float timer = 0f;

		while (timer < time) {
			yield return null;
			timer += Time.deltaTime;
		}
		
		Destroy();
		
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
	
	//Picks and sets the destroyColor property randomly ( excluding colorException ) 
	public TileColorType SetRandomDestroyColor(TileColorType colorException) {
		
		int returnValue = Random.Range(1, (int)TileColorType.Count-1);
		
		if(returnValue == (int)colorException) {
			returnValue = Random.Range(0,1) == 0 ? Random.Range(1, (int)colorException-1) : Random.Range((int)colorException+1, (int)TileColorType.Count-1);
		}
		
		destroyColor = (TileColorType) returnValue;
		return (TileColorType)returnValue;
	}
}
