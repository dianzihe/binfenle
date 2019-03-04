using UnityEngine;
using System.Collections;

public class LevelDestroyTargets : MonoBehaviour
{
	public UITexture myTexture;
	public int destroyTilesIndex = 0;
	
	public Transform[] brothers;
	
	UILabel label;
	float deactivateOffset = 0f;
	
	public void UpdateValues(AbstractWinCondition condition) 
	{
		if (label == null) {
			label = GetComponent<UILabel>();
			if (brothers != null && brothers.Length > 1) {
				deactivateOffset = (brothers[0].parent.localPosition.x - brothers[1].parent.localPosition.x) * 0.5f;
			}
		}
		
		WinDestroyTiles winCondition = (condition as WinDestroyTiles);
		
		if (!label.enabled) {
			UpdateBrothers(deactivateOffset);
		}
		
		if (winCondition == null || winCondition.destroyTiles.Length <= destroyTilesIndex) {
			myTexture.gameObject.SetActive(false);
			label.enabled = false;
			UpdateBrothers(-deactivateOffset);
			return;
		}
		
		myTexture.gameObject.SetActive(true);
		label.enabled = true;
		
		DestroyTilesPair destroyTiles = winCondition.destroyTiles[destroyTilesIndex];
		
		myTexture.mainTexture = destroyTiles.type.transform.Find("Model").GetComponent<Renderer>().sharedMaterial.mainTexture;

		string key = "Level" + MaleficentBlackboard.Instance.level + "Destroy" + destroyTilesIndex;
		if (TweaksSystem.Instance.intValues.ContainsKey(key)) {
			label.text = TweaksSystem.Instance.intValues[key].ToString();
		}
		else {
			label.text = destroyTiles.number.ToString();
		}


	}
	
	void UpdateBrothers(float offset)
	{
		if (brothers != null && brothers.Length > 1) {
			foreach (Transform brother in brothers) {
				brother.parent.localPosition = brother.parent.localPosition + new Vector3(offset, 0f, 0f);
			}
		}
	}
}

