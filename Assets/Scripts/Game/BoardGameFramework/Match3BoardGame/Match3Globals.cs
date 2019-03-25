using UnityEngine;
using System.Collections;

public class Match3Globals {
	private static Match3Globals instance = null;
	
	public WaitForEndOfFrame waitEndFrame;
	
	/* Physics related */
	public int layerBoardTile = -1;
	
	
	public static Match3Globals Instance {
		get {
			if (instance == null) {
				instance = new Match3Globals();
			}
			return instance;
		}
	}
	
	public Match3Globals() {
		waitEndFrame = new WaitForEndOfFrame();
		
		layerBoardTile = LayerMask.NameToLayer("BoardTile");
	}
}