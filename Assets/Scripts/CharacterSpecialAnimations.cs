using UnityEngine;
using System.Collections;

public class CharacterSpecialAnimations : MonoBehaviour 
{
	public PlayMakerFSM characterFSM;
		
	public static CharacterSpecialAnimations instance;

	public const float TIME_GET_ATTENTION = 20.0f;
	public float TimeGetAttention { get; set; } //Used from playmaker to do the special animation after 20 seconds without anything interesting happening
	
	public static int CharIdx {
		get {
			return MaleficentBlackboard.Instance.character;
		}
		set {
			MaleficentBlackboard.Instance.character = value;
		}
	}


	
	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start ()  {
		if (characterFSM == null) {
			characterFSM = GetComponent<PlayMakerFSM>();
		}
		
		Match3Tile.OnAnyTileDestroyed += OnTileDestroyed;
		ScoreSystem.Instance.OnScoreUpdated += OnScoreUpdated;
		//WinScore.OnNewStarReached += OnNewStarReached;
	}
	
	void OnTileDestroyed(Match3Tile tile) {
		TimeGetAttention = TIME_GET_ATTENTION; //Reset time get attention

		if ((tile is ColorBombTile) || (tile is BombTile) || (tile is DirectionalDestroyTile)) {
			//characterFSM.SendEvent("DoComboAnimation");
		}
	}
	
	void OnScoreUpdated() {
		if (ScoreSystem.Instance.multiplier == 5) {
			characterFSM.SendEvent("DoComboAnimation");
		}
	}
		
	// Update is called once per frame
	void Update () {
		TimeGetAttention -= Time.deltaTime;
	}
	
	/// <summary>
	/// Destroy event raised by Unity.
	/// </summary>
	void OnDestroy() {
		Match3Tile.OnAnyTileDestroyed -= OnTileDestroyed;
		ScoreSystem.Instance.OnScoreUpdated -= OnScoreUpdated;
		//WinScore.OnNewStarReached -= OnNewStarReached;
	}
}
