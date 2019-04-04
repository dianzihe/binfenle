using UnityEngine;
using System.Collections;

public abstract class AbstractBoardRenderer : MonoBehaviour {
	public delegate void BoardRendererEventHandler(AbstractBoardRenderer sender);
	
	public static event BoardRendererEventHandler OnBoardStartedSetup;
	public static event BoardRendererEventHandler OnBoardFinishedSetup;
	
	public GameObject[] tilesPrefabs;
	/// <summary>
	/// The board pieces prefabs.
	/// </summary>
	public GameObject[] prefabsPieces;

	private BoardData boardData;

	[System.NonSerialized]
	public Transform cachedTransform;

	
	protected virtual void Awake() {
		cachedTransform = transform;
		boardData = GetComponent<BoardData>();
	}
		
	protected virtual void Start() {
		System.Console.WriteLine("AbstractBoardRenderer -> start->");
	}
	
	/// <summary>
	/// Sets up the board rendering. (eg. tiles setup and positioning, board positionining and rendering options, etc.)
	/// When this method gets called it raises the following events:
	/// <see cref="OnBoardStartedSetup"/> when the board begins the setup its tiles.
	/// <see cref="OnBoardFinishedSetup"/> when the board finished setting up the tiles.
	/// </summary>
	public virtual void InitComponent() {
		RaiseBoardStartedSetupEvent();
		
		SetupBoardTiles();
		SetupBoardRendering();
		
		RaiseBoardFinishedSetupEvent();
	}
	
	public virtual void RaiseBoardStartedSetupEvent() {
		if (OnBoardStartedSetup != null) {
			OnBoardStartedSetup(this);
		}
	}
	
	public virtual void RaiseBoardFinishedSetupEvent() {
		if (OnBoardFinishedSetup != null) {
			OnBoardFinishedSetup(this);
		}
	}

	public static void CalculateRecursiveAABB(GameObject parent, ref Bounds resultedBounds) {
		Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
		for(int i = 0; i < renderers.Length; i++) {
			if (renderers[i].transform.childCount > 0) {
				CalculateRecursiveAABB(renderers[i].gameObject, ref resultedBounds);
			} else {
				resultedBounds.Encapsulate(renderers[i].bounds);
			}
		}
	}
	
	public BoardData Board {
		get {
			return boardData;
		}
	}

	public abstract void SetupBoardTiles();
	
	/// <summary>
	/// Sets up the board tiles based on the currently loaded <see cref="BoardData"/>.
	/// </summary>
	protected abstract void SetupBoardRendering();	
}