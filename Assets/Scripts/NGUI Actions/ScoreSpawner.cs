using UnityEngine;
using System.Collections;
using System;

public class ScoreSpawner : MonoBehaviour {
	public static ScoreSpawner instance = null;
	
	public UIRoot scoreUIRoot;
	public Camera scoreCamera;
	public Camera gameCamera;
	public GameObject scorePrefab;
	public int textSize;
	
	private Vector3 worldCoordScaleFactor = Vector3.right + Vector3.up;
	private Vector3 finalPosition;
	
	protected void Awake() {
		instance = this;
	}
	
	//MIO TODO: This his written in a hurry. Rewrite the smart way
	public void SpawnScore(Vector3 position, int value) {
		Transform scorePrefabTransform = (Instantiate(scorePrefab, position, Quaternion.identity) as GameObject).transform;
		scorePrefabTransform.parent = transform;

		finalPosition = gameCamera.WorldToViewportPoint(position);
		finalPosition = scoreCamera.ViewportToWorldPoint(finalPosition);
		finalPosition.Scale(worldCoordScaleFactor);
		
		scorePrefabTransform.position = finalPosition;
		scorePrefabTransform.localScale = new Vector3(textSize, textSize, 1);
		
		UILabel scoreUILabel = scorePrefabTransform.GetComponentInChildren<UILabel>();
		scoreUILabel.text = value.ToString();
		
		Destroy(scorePrefabTransform.gameObject, scorePrefabTransform.GetComponent<Animation>().clip.length);
	}
}
