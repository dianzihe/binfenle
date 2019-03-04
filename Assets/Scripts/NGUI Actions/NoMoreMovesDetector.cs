using UnityEngine;
using System.Collections;

public class NoMoreMovesDetector : MonoBehaviour
{
	public int maxTries = 3;
	public PlayMakerFSM targetFsm;
	public string fsmEvent = "NoMoves";
	
	protected int tries = 0;
	
	// Use this for initialization
	void Start()
	{
		BoardShuffleController.OnBoardShuffleWithMatch += OnBoardShuffleWithMatch;
	}
	
	void ResetDetector()
	{
		tries = 0;
	}
	
	void OnBoardShuffleWithMatch (bool hasMatch)
	{
		if (hasMatch) {
			tries = 0;
		}
		else {
			tries++;
			
			if (tries >= maxTries) {
				targetFsm.SendEvent(fsmEvent);
				BoardShuffleController.Instance.enabled = false;
				Match3Tile.OnAnyTileDestroyed += OnAnyTileDestroyed;
			}
		}
	}

	void OnAnyTileDestroyed (Match3Tile tile)
	{
		Match3Tile.OnAnyTileDestroyed -= OnAnyTileDestroyed;
		BoardShuffleController.Instance.enabled = true;
	}
	
	void OnDestroy()
	{
		Match3Tile.OnAnyTileDestroyed -= OnAnyTileDestroyed;
		BoardShuffleController.OnBoardShuffleWithMatch -= OnBoardShuffleWithMatch;
	}
}

