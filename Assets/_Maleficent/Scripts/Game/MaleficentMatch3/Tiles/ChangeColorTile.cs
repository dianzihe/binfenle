using UnityEngine;
using System.Collections;

// This class is just a test that we did to understand tiles
// It is a Tile that changes its color after falling accumulatedFalls
public class ChangeColorTile : NormalTile {

	public Material[] coloredMaterials;

	public int numFallingPieces = 2;
	
	private int accumulatedFalls = 0;
	

	
	public override void RaiseOnBoardPieceChanged() {
		base.RaiseOnBoardPieceChanged();
		
		if(!IsTileSwitching) {
			
			accumulatedFalls ++;
			Debug.Log("Piece changed to " + BoardPiece +" "+ accumulatedFalls);
			if(accumulatedFalls == numFallingPieces) {
				ChangeColor(Random.Range(0, coloredMaterials.Length - 1));
				accumulatedFalls = 0;
			}
		}
	}
	
	public void ChangeColor(int _color) {
		TileColor = (TileColorType)(_color + 1);
		tileModelTransform.GetComponent<Renderer>().material = coloredMaterials[(int)_color];
	}
}
