using UnityEngine;
using System.Collections;

public class GameCheatsController : MonoBehaviour {
	protected System.Type selectedTile = null;
	
	
	// Use this for initialization
	void Start () {
		Match3Tile.OnTileTap += OnTileTapped;
	}
	
	public void OnGUI()
	{
		GUILayout.BeginVertical();
		{
			if ( GUILayout.Button("Win Level", GUILayout.Height(40f)) ) 
			{
				ScoreSystem.Instance.Score = 999999;
				Match3BoardGameLogic.Instance.winConditions.RaiseOnWinChecked();
			}
			
			GUILayout.Space(10f);
			
			if ( GUILayout.Button("Lose Level", GUILayout.Height(40f)) ) {
				ScoreSystem.Instance.Score = 1000;
				Match3BoardGameLogic.Instance.loseConditions.RaiseOnLoseChecked();
			}
			
			GUILayout.Space(10f);
			TileSelectionButton(typeof(BombTile));
			
			GUILayout.Space(10f);
			
			TileSelectionButton(typeof(DirectionalDestroyTile));
			
			GUILayout.Space(10f);
			TileSelectionButton(typeof(ColorBombTile));
			
		}
		GUILayout.EndVertical();
	}
	
	public void TileSelectionButton(System.Type tileType)
	{
		if ( selectedTile != tileType && GUILayout.Button(tileType.Name, GUILayout.Height(40f)) || 
			selectedTile == tileType && GUILayout.Button(tileType.Name + " [SELECTED]", GUILayout.Height(40f)) )
		{
			if (selectedTile == tileType) {
				selectedTile = null;
			}
			else {
				selectedTile = tileType;
			}
		}
	}
	
	public void OnTileTapped(AbstractTile tile) 
	{
		BoardCoord targetCoord = tile.BoardPiece.BoardPosition;
		
		if (selectedTile == typeof(BombTile)) 
		{
			GameObject.Destroy(tile.gameObject);
			BombTile newTile = Match3BoardRenderer.Instance.SpawnSpecificTileAt(targetCoord, selectedTile, TileColorType.None) as BombTile;
			newTile.TileColor = RuleEntry.genericColors[Random.Range(0, Match3BoardRenderer.maxNumBoardColors)];
			newTile.UpdateMaterial();
		}
		else if (selectedTile == typeof(DirectionalDestroyTile)) 
		{
			GameObject.Destroy(tile.gameObject);
			
			DirectionalDestroyTile newTile = null;
			if (Random.value > 0.5f) {
				newTile = Match3BoardRenderer.Instance.SpawnSpecificTileAt(targetCoord, typeof(RowDestroyTile), TileColorType.None) as DirectionalDestroyTile;
			}
			else {
				newTile = Match3BoardRenderer.Instance.SpawnSpecificTileAt(targetCoord, typeof(ColumnDestroyTile), TileColorType.None) as DirectionalDestroyTile;
			}
			
			newTile.TileColor = RuleEntry.genericColors[Random.Range(0, Match3BoardRenderer.maxNumBoardColors)];
			newTile.UpdateMaterial();
		}
		else if (selectedTile == typeof(ColorBombTile))
		{
			GameObject.Destroy(tile.gameObject);
			
			Match3BoardRenderer.Instance.SpawnSpecificTileAt(targetCoord, selectedTile, TileColorType.None);
		}
	}
	
	void OnDestroy()
	{
		Match3Tile.OnTileTap -= OnTileTapped;
	}
}
