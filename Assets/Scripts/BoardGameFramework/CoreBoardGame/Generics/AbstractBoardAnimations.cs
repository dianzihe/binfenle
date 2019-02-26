using UnityEngine;
using System.Collections;

public abstract class AbstractBoardAnimations : MonoBehaviour {
	public AbstractBoardRenderer boardRenderer;
	
	[System.NonSerialized]
	public BoardData boardData;
	
	
	protected virtual void Awake() {
		
	}
	
	protected virtual void Start() {
	}
	
	public virtual void InitComponent() {
//		Debug.Log("[AbstractBoardAnimations] InitComponent...");
		boardData = boardRenderer.Board;
	}
	
}

