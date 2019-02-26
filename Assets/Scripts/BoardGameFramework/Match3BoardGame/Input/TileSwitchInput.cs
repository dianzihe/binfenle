using UnityEngine;
using System.Collections;

/// <summary>
/// TileSwitchInput
/// 
/// Handles input required for switching tiles on the Match3 board. 
/// This component communicates with the <see cref="Match3BoardGameLogic"/> to switch tiles by swiping
/// or by tapping one tile and then the other.
/// </summary>
public class TileSwitchInput : MonoBehaviour 
{		
	protected static TileSwitchInput instance;
	
	public delegate bool InputFilterDelegate(AbstractTile selectedTile, AbstractTile destinationTile, TileMoveDirection moveDirection);
	public InputFilterDelegate InputFilter;
	
	public Match3BoardGameLogic boardGameLogic;
	public DragRecognizer dragRecognizer;
	public TapRecognizer tapRecognizer;
	
	// Reference to the tile selected by a tap gesture. Used by the TapRecognizer component.
	protected AbstractTile tapSelectedTile;
	
	protected int inputDisableLock = 0;
	protected int dragLock = 0;
	
	
	public static TileSwitchInput Instance {
		get {
			return instance;
		}
	}
	
	void Awake()
	{
		instance = this;
	}
	
	void Start () {
		dragRecognizer = GetComponent<DragRecognizer>();
		tapRecognizer = GetComponent<TapRecognizer>();
		
		dragRecognizer.OnGesture += OnDragGesture;
		tapRecognizer.OnGesture += OnTapGesture;
	}
	
	public void DisableInput() 
	{
//		Debug.LogWarning("Disable input");
		inputDisableLock++;
		
		if (inputDisableLock > 0)
		{
			gameObject.SetActive(false);
		}
	}
	
	public void EnableInput()
	{
//		Debug.LogWarning("Enable input");
		inputDisableLock--;
		
		if (inputDisableLock <= 0)
		{
			inputDisableLock = 0;
			gameObject.SetActive(true);
		}
	}
	
	public bool IsEnabled
	{
		get {
			return enabled && gameObject.activeInHierarchy;
		}
	}
	
	public void DeactivateDrag()
	{
		dragLock++;
	}
	
	public void ActivateDrag()
	{
		if (dragLock > 0) {
			dragLock--;
		}
	}
	
	/// <summary>
	/// Raises the drag gesture event. This event is raised by FingerGestures's DragRecognizer component.
	/// Determine the selected tile and in which direction the user dragged it and tell the <see cref="Match3BoardGameLogic"/>
	/// that we've started moving that tile in that direction.
	/// </summary>
	/// <param name='eventData'>
	/// Event data.
	/// </param>
	public void OnDragGesture(DragGesture eventData) {		
		if (dragLock > 0) {
			tapSelectedTile = null;
			return;
		}
		
		// Check if we've selected any tile.
		if (eventData.StartSelection != null && eventData.StartSelection.layer == Match3Globals.Instance.layerBoardTile) {
			if (eventData.Phase == ContinuousGesturePhase.Started) {
				// Cancel the tap selected tile if we've done a drag gesture.
				tapSelectedTile = null;
				
				AbstractTile dragSelectedTile = eventData.StartSelection.GetComponent<AbstractTile>();
				TileMoveDirection moveDirection = Match3BoardGameLogic.GetTileMoveDirection(eventData.TotalMove);
				
				if (InputFilter == null || InputFilter(dragSelectedTile, null, moveDirection)) {
//					Debug.Log("Drag event started! Start Selection: " + eventData.StartSelection + " -> totalMove = " + eventData.TotalMove);
					if (boardGameLogic.TryToMoveTile(dragSelectedTile, moveDirection)) {
						//boardGameLogic.loseConditions.NewMove();
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Raises the tap gesture event. This event is raised by FingerGestures's TapRecognizer component.
	/// </summary>
	/// <param name='eventData'>
	/// Event data.
	/// </param>
	public void OnTapGesture(TapGesture eventData) {
		if (dragLock > 0) {
			tapSelectedTile = null;
		}
		
		// Process tap gesture only if the start selection is a board tile.
		if (eventData.StartSelection != null && eventData.StartSelection.layer == Match3Globals.Instance.layerBoardTile) 
		{
			// Check if there is no previously selected tile.
			if (tapSelectedTile == null) 
			{
//				Debug.Log("TapGesture selection started: " + eventData.StartSelection);
				tapSelectedTile = eventData.StartSelection.GetComponent<AbstractTile>();
//				Debug.Log("Tile tapped: " + tapSelectedTile);
				tapSelectedTile.RaiseEventTileTap();
			} 
			else 
			{
//				Debug.Log("TapGesture selected ended: " + eventData.StartSelection);
				AbstractTile newSelectedTile = eventData.StartSelection.GetComponent<AbstractTile>();
				
				// Tell the Match3BoardGameLogic to try and switch the first selected tile with this new selected tile.
				if ((InputFilter == null || InputFilter(tapSelectedTile, newSelectedTile, TileMoveDirection.Count)) && boardGameLogic.TryToMoveTile(tapSelectedTile, newSelectedTile) == false) 
				{
					// If the tiles couldn't be switched then make the new selected tile as the first selected tile.
					tapSelectedTile = newSelectedTile;
					newSelectedTile.RaiseEventTileTap();
				} 
				else 
				{
					// If the tiles were succesfully switched (not necessarilly matched) then reset the first selected tile.
					tapSelectedTile = null;
					//boardGameLogic.loseConditions.NewMove();
				}
			}
		}
	}
	
	/// <summary>
	/// Unity raised destroy event.
	/// </summary>
	void OnDestroy() {
		if (dragRecognizer != null) {
			// Unregister from events.
			dragRecognizer.OnGesture -= OnDragGesture;
		}
		
		if (tapRecognizer != null) {
			tapRecognizer.OnGesture -= OnTapGesture;
		}
	}
		
}
