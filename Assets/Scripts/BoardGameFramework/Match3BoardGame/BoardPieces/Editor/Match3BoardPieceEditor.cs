using UnityEngine;
using UnityEditor;
using System.Collections;

public class Match3BoardPieceEditor {
	public static int maxStatesLength = 9;
	public static GUIStyle labelStyle = null;
	public static bool showBoardPiecesStates = false;
	public static bool showBoardLinks = false;

	[MenuItem("CONTEXT/Match3BoardPiece/Show Board States")]
	public static void ShowBoardStates() {
		showBoardPiecesStates = true;
	}
	
	[MenuItem("CONTEXT/Match3BoardPiece/Hide Board States")]
	public static void HideBoardStates() {
		showBoardPiecesStates = false;
	}
	
	[MenuItem("CONTEXT/Match3BoardPiece/Show Board Links")]
	public static void ShowBoardLinks() {
		showBoardLinks = true;
	}
	
	[MenuItem("CONTEXT/Match3BoardPiece/Hide Board Links")]
	public static void HideBoardLinks() {
		showBoardLinks = false;
	}

	[DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.SelectedOrChild) ]
	public static void OnDrawGizmos(Match3BoardPiece boardPiece, GizmoType gizmoType) {
		if ( !boardPiece.canDrawGizmos ) {
			return;
		}
		
		if (showBoardPiecesStates) {
			string states = "";
			states = Match3TileEditor.AppendState(states, string.Format("[{0},{1}], ", boardPiece.BoardPosition.row, boardPiece.BoardPosition.col), maxStatesLength);
			
			if (boardPiece.IsBlocked) {
				states = Match3TileEditor.AppendState(states, "B,", maxStatesLength);
			}
			if (boardPiece.IsTemporaryOrphan) {
				states = Match3TileEditor.AppendState(states, "Ot,", maxStatesLength);
			}
			if (boardPiece.IsOrphan) {
				states = Match3TileEditor.AppendState(states, "O,", maxStatesLength);
			}			
			if (boardPiece.IsEmpty) {
				states = Match3TileEditor.AppendState(states, "E,", maxStatesLength);
			} else if (boardPiece.IsEmpty && boardPiece.AllowTileFalling) {
				states = Match3TileEditor.AppendState(states, "EF, ", maxStatesLength);
			}
			
			if (boardPiece.IsBorderPiece) {
				states = Match3TileEditor.AppendState(states, "Br,", maxStatesLength);
			}
			if (boardPiece.LockCount > 0) {
				states = Match3TileEditor.AppendState(states, "L" + boardPiece.LockCount + ",", maxStatesLength);
			}
			if (boardPiece.IsTileSpawner) {
				states = Match3TileEditor.AppendState(states, "T,", maxStatesLength);
			}
			
			states = states.Trim();
			
			if (labelStyle == null) {
				labelStyle = new GUIStyle();
				labelStyle.normal.textColor = Color.yellow;
				labelStyle.fontSize = 14;
				labelStyle.normal.background = Match3TileEditor.MakeTex(2, 2, Color.blue);
			}
			
			Handles.Label(boardPiece.transform.position + new Vector3(-0.5f, 0.5f, 0f), states, labelStyle);
		}
		
		if (showBoardLinks) {
			if (boardPiece != null && boardPiece.links != null) {
				Gizmos.color = Color.green;
				Matrix4x4 pushedMatrix = Gizmos.matrix;
				
				if (boardPiece.transform.parent) { 
					Gizmos.matrix = boardPiece.transform.parent.localToWorldMatrix;
				}
	
				for(int i = 0; i < boardPiece.links.Length; i++) {
					if (boardPiece.links[i] != null) {
						Gizmos.DrawLine(boardPiece.LocalPosition, boardPiece.links[i].LocalPosition);
					}
				}
				
				Gizmos.matrix = pushedMatrix;
			}
		}
	}	
}
