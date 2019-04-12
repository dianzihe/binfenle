using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DestroyTargetLabel : MonoBehaviour 
{
	public Match3BoardGameLogic gameLogic;
	public UITexture myTexture;
	public int destroyTilesIndex = 0;
	
	public GameObject targetScore;
	
	public Transform[] brothers;
	
	UILabel label;
	WinDestroyTiles winCondition;
	DestroyTilesPair destroyTiles;
	bool updatingValues = false;
	
	float deactivateOffset = 0f;
	
	protected WaitForEndOfFrame waitEndFrame;
	
	void Awake() 
	{
		waitEndFrame = new WaitForEndOfFrame();
	}
	
	void Start () 
	{
		winCondition = (gameLogic.winConditions as WinDestroyTiles);
		
		if (winCondition == null) {
			Destroy(transform.parent.parent.gameObject);
			return;
		}
		
		if (winCondition.destroyTiles.Length <= destroyTilesIndex) 
		{
			if (brothers != null && brothers.Length > 1) {
				deactivateOffset = (brothers[0].parent.localPosition.x - brothers[1].parent.localPosition.x) * 0.5f;
				UpdateBrothers(-deactivateOffset);
			}
			
			Destroy(transform.parent.gameObject);
			return;
		}
		
		if (targetScore != null) {
			targetScore.SetActive(false);
		}
		
		destroyTiles = winCondition.destroyTiles[destroyTilesIndex];
		Object thumbnail = destroyTiles.type.Thumbnail();

		if (typeof(Texture).IsAssignableFrom(thumbnail.GetType())) {
			myTexture.mainTexture = (Texture)thumbnail;
		}else if (typeof(Sprite).IsAssignableFrom(thumbnail.GetType())){
			myTexture.gameObject.AddComponent<SpriteRenderer>().sprite = (Sprite)thumbnail;
			Destroy(myTexture);
		}

		label = GetComponent<UILabel>();
		
//		Debug.LogWarning("Destroy tiles start: " + label.text);
		StartCoroutine("UpdateValues");
		
		Match3Tile.OnAnyTileDestroyed += OnTilesDestroyed;
		SnowTile.OnSnowWinDestroyCondition += OnTilesDestroyed;
	}
	
	void OnTilesDestroyed(Match3Tile tile)
	{
		if (!updatingValues) {
			StartCoroutine(UpdateValues());
		}
	}
	
	IEnumerator UpdateValues() 
	{
		updatingValues = true;
		yield return waitEndFrame;
		
		int remaining = Mathf.Max(0, destroyTiles.number - destroyTiles.current);
		label.text = remaining.ToString();
		
		if (remaining == 0) 
		{
			bool allDone = true;
			
			for (int i = 0; i < winCondition.destroyTiles.Length; ++i) 
			{
				allDone &= winCondition.destroyTiles[i].current >= winCondition.destroyTiles[i].number;
			}
			
			if (allDone && targetScore != null) 
			{
				targetScore.SetActive(true);
				Match3Tile.OnAnyTileDestroyed -= OnTilesDestroyed;
				SnowTile.OnSnowWinDestroyCondition -= OnTilesDestroyed;
				
				transform.parent.gameObject.SetActive(false);
			}
		}
		
		updatingValues = false;
	}
	
	void UpdateBrothers(float offset)
	{
		if (brothers != null && brothers.Length > 1) {
			foreach (Transform brother in brothers) {
				brother.parent.localPosition = brother.parent.localPosition + new Vector3(offset, 0f, 0f);
			}
		}
	}
	
	void OnDestroy()
	{
		Match3Tile.OnAnyTileDestroyed -= OnTilesDestroyed;
		SnowTile.OnSnowWinDestroyCondition -= OnTilesDestroyed;
	}
}
