using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;

public class Match3TileEditor {
	public static int maxStatesLength = 8;
	public static GUIStyle labelStyle = null;
	public static bool tilesDebugDisplayOverride = true;
		
	public static Texture2D MakeTex(int width, int height, Color col) {

        Color[] pix = new Color[width*height];

        for(int i = 0; i < pix.Length; i++) {
            pix[i] = col;
		}
        Texture2D result = new Texture2D(width, height);

        result.SetPixels(pix);
        result.Apply();
		
        return result;

    }
		
	public static string AppendState(string _tileStates, string newState, int maxLength) {
		int lastNewLineIdx = _tileStates.LastIndexOf('\n');
		if (lastNewLineIdx < 0) {
			lastNewLineIdx = 0;
		}
		
		if (_tileStates.Length + newState.Length - lastNewLineIdx >= maxLength) {
			_tileStates += "\n";
		}
		
		_tileStates += newState;
		
		return _tileStates;
	}
	
	[MenuItem("CONTEXT/Match3Tile/Enable Debug Draw")]
	public static void EnableTilesDebugDraw() {
		tilesDebugDisplayOverride = true;
	}
	
	[MenuItem("CONTEXT/Match3Tile/Disable Debug Draw")]
	public static void DisableTilesDebugDraw() {
		tilesDebugDisplayOverride = false;
	}
	
	[DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.InSelectionHierarchy)]
	public static void OnDrawGizmos(Match3Tile tile, GizmoType gizmoType) {
		if ( !tile.canDrawGizmos || !tilesDebugDisplayOverride) {
			return;
		}
		
		string tileStates = "";
		
		if (tile.IsMoving) {
			tileStates = AppendState(tileStates, "M, ", maxStatesLength);
		}
		if (tile.IsDestroying) {
			tileStates = AppendState(tileStates, "D, ", maxStatesLength);
		}
		if (tile.debugPassiveGravity) {
			tileStates = AppendState(tileStates, "Pg, ", maxStatesLength);
		}
		if (tile.debugActiveGravity) {
			tileStates = AppendState(tileStates, "Ag, ", maxStatesLength);
		}
		if (tile.BoardPiece != null && !tile.HasReachedBoardPieceArea()) {
			tileStates = AppendState(tileStates, "Bd, ", maxStatesLength);
		}
		
		tileStates = tileStates.Trim();
		
		if (labelStyle == null) {
			labelStyle = new GUIStyle();
			labelStyle.normal.textColor = Color.yellow;
			labelStyle.fontSize = 14;
			labelStyle.normal.background = MakeTex(2, 2, Color.black);
		}
		/* 
		Handles.Label(tile.transform.position +  new Vector3(-tile.collider.bounds.extents.x, tile.collider.bounds.extents.y, 0f), 
					  tileStates, labelStyle);
		*/
		Matrix4x4 pushedMatrix = Gizmos.matrix;
		if (tile.transform.parent != null) {
			Gizmos.matrix = tile.transform.parent.localToWorldMatrix;
		}
		
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(tile.fallDestination, 0.25f);
		
		if (tile.transform.parent != null) {
			Gizmos.matrix = pushedMatrix;
		}
	}	
}
