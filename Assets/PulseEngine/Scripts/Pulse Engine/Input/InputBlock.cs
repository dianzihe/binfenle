using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Input block that can receive input and send the corresponding messages.
/// </summary>
public class InputBlock : Block
{
	/// <summary>
	/// Tells if this input block is modal.
	/// </summary>
	public bool modal = false;
	
	/// <summary>
	/// The layer mask used if this block is modal.
	/// </summary>
	public LayerMask modalMask = 0;
	
	/// <summary>
	/// The modal item created when registering this block as a modal block.
	/// </summary>
	protected ModalItem modalItem;
	
	/// <summary>
	/// Unity method.
	/// Start this instance.
	/// </summary>
	public virtual void Start()
	{
		if (modal) {
			modalItem = new ModalItem(this, modalMask.value);
			InputManager.Instance.RegisterModalItem(modalItem);
		}
	}
	
	/// <summary>
	/// Called when a tap begins on this block.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	public void OnTapBegan(Vector2 position)
	{
		SendEvents("TapBegan");
	}
	
	/// <summary>
	/// Called when a tap ends on this block.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	public void OnTapEnded(Vector2 position)
	{
		SendEvents("TapEnded");
	}
	
	/// <summary>
	/// Called when a swipe from left to right is performed on this block.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	public void OnSwipeLeftRight(Vector2 startPosition, Vector2 endPosition)
	{
		SendEvents("SwipeLeftRight");
	}
	
	/// <summary>
	/// Called when a swipe from right to left is performed on this block.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	public void OnSwipeRightLeft(Vector2 startPosition, Vector2 endPosition)
	{
		SendEvents("SwipeRightLeft");
	}
	
	/// <summary>
	/// Determines whether this instance has the specified block as a child. It is recursive through all children.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance has the specified block as a child; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='block'>
	/// The block.
	/// </param>
	public bool HasChild(InputBlock block)
	{
		foreach (InputBlock childBlock in children)
		{
			if (block == childBlock || childBlock.HasChild(block)) {
				return true;
			}
		}
			
		return false;
	}
	
	/// <summary>
	/// Unity method.
	/// Called when the object is destroyed.
	/// </summary>
	public void OnDestroy()
	{
		if (modal) {
			if (InputManager.Instance) {
				InputManager.Instance.UnregisterModalItem(modalItem);
			}
		}
	}
}

