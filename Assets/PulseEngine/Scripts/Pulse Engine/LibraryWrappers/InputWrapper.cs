using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Input wrapper that handles raising input events. Call CheckInput() to test for input events and register to this instance to be notified.
/// Extend and override the instance to use a custom input library.
/// </summary>
public class InputWrapper
{
	/// <summary>
	/// The singleton instance.
	/// </summary>
	protected static InputWrapper instance = null;
	
	/// <summary>
	/// The raycast distance for the raycasts performed to determine input receivers.
	/// </summary>
	public static float RAYCAST_DISTANCE = 4096f;
	
	/// <summary>
	/// The maximum time for a swipe to be performed. If it is bigger then this, it must be a drag movement.
	/// </summary>
	public static float SWIPE_MAX_TIME = 1f;
	
	/// <summary>
	/// The minimum screen percentage for a swipe.
	/// </summary>
	public static float SWIPE_MIN_SCREEN = 0.2f;
	
	#region Event Delegates
	public delegate void TapEventHandler(Vector2 position);
	public delegate void MoveEventHandler(Vector2 startPosition, Vector2 endPosition);
	#endregion Event Delegates
	
	/// <summary>
	/// Occurs when a tap begins.
	/// </summary>
	public event TapEventHandler OnTapBegan;
	
	/// <summary>
	/// Occurs when a tap ends.
	/// </summary>
	public event TapEventHandler OnTapEnded;
	
	/// <summary>
	/// Occurs when a swipe is performed from left to right.
	/// </summary>
	public event MoveEventHandler OnSwipeLeftRight;
	
	/// <summary>
	/// Occurs when a swipe is performed from right to left.
	/// </summary>
	public event MoveEventHandler OnSwipeRightLeft;
	
	/// <summary>
	/// Gets the singleton instance for this class.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static InputWrapper Instance {
		get {
			if (instance == null) {
				instance = new InputWrapper();
			}
			
			return instance;
		}
	}
	
	/// <summary>
	/// Gets the input component from the given game object. The wrapper is the only one who knows what component it needs to get. 
	/// This function is primarily used to cache the result on the caller.
	/// </summary>
	/// <returns>
	/// The input component.
	/// </returns>
	/// <param name='gameObj'>
	/// The game object from which to get the input component.
	/// </param>
	public virtual Component GetInputComponent(GameObject gameObj)
	{
		return gameObj.GetComponent<Collider>();
	}
	
	/// <summary>
	/// Checks the input. Call this at every frame or just when you needed. Register to this instance's events to get notified when they happen.
	/// </summary>
	public void CheckInput()
	{
		for (int i = 0; i < CustomInput.touchCount; ++i) {
			CustomInput.TouchInfo touch = CustomInput.GetTouch(i);
			
			if (touch.phase == TouchPhase.Began) {
				if (OnTapBegan != null) {
					OnTapBegan(touch.position);
				}
			}
			else if (touch.phase == TouchPhase.Ended) {
				if (OnTapEnded != null) {
					OnTapEnded(touch.position);
				}
				
				if (Time.time - touch.beginTime < SWIPE_MAX_TIME) {
					if (OnSwipeLeftRight != null && touch.position.x - touch.beginPosition.x >= SWIPE_MIN_SCREEN * Screen.width) {
						OnSwipeLeftRight(touch.beginPosition, touch.position);
					}
					if (OnSwipeRightLeft != null && touch.position.x - touch.beginPosition.x <= -SWIPE_MIN_SCREEN * Screen.width) {
						OnSwipeRightLeft(touch.beginPosition, touch.position);
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Gets the input component for the given camera, at the given position.
	/// </summary>
	/// <returns>
	/// The input component.
	/// </returns>
	/// <param name='inputCamera'>
	/// Input camera.
	/// </param>
	/// <param name='screenPos'>
	/// Screen position.
	/// </param>
	public Component GetInputComponent(Camera inputCamera, Vector2 screenPos)
	{
		RaycastHit raycastHit;
        
		if (Physics.Raycast(inputCamera.ScreenPointToRay(screenPos), out raycastHit, RAYCAST_DISTANCE, inputCamera.cullingMask)) {
			return raycastHit.collider;
		}
			
		return null;
	}
	
	/// <summary>
	/// Enables or disables the given input component.
	/// </summary>
	/// <param name='renderComponent'>
	/// Input component.
	/// </param>
	/// <param name='enabled'>
	/// Tells if the input component should be enabled or disabled.
	/// </param>
	public virtual void SetEnabled(Component inputComponent, bool enabled)
	{
		(inputComponent as Collider).enabled = enabled;
	}
}

//}