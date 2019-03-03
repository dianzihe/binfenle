using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//namespace PulseEngine {

/// <summary>
/// Input manager. Handles all input in the app. Register input cameras to it.
/// </summary>
public class InputManager : MonoBehaviour
{
	protected static InputManager instance = null;
	
	/// <summary>
	/// Tells if an instance object has already been created so it doesn't create a new one on destroy.
	/// </summary>
	public static bool createdInstance = false;
	
	/// <summary>
	/// The input cameras. The order they are added to the list is also the order of their processing.
	/// </summary>
	public List<Camera> inputCameras;
	
	/// <summary>
	/// The modal stack. Register modal items to it. The first item is the one with the highest priority.
	/// </summary>
	protected List<ModalItem> modalStack;
	
	[System.NonSerialized]
	public InputWrapper cachedInputWrapper;
	
	/// <summary>
	/// Gets the instance of this singleton. Creates a game object for itself if needed.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static InputManager Instance {
		get {
			if (instance == null && !createdInstance) {
				GameObject inputObj = new GameObject("InputManager");
				instance = inputObj.AddComponent<InputManager>();
				createdInstance = true;
			}

			return instance;
		}
	}
	
	/// <summary>
	/// Unity method.
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if (instance == null) {
			instance = this;
			createdInstance = true;
		}
		else {
			Debug.LogWarning("Input Manager already exists. Destroying this instance: " + name);
			Destroy(this);
		}
		
		cachedInputWrapper = InputWrapper.Instance;
		
		cachedInputWrapper.OnTapBegan += OnTapBegan;
		cachedInputWrapper.OnTapEnded += OnTapEnded;
		cachedInputWrapper.OnSwipeLeftRight += OnSwipeLeftRight;
		cachedInputWrapper.OnSwipeRightLeft += OnSwipeRightLeft;
		
		if (inputCameras == null) {
			inputCameras = new List<Camera>();
		}
		
		if (modalStack == null) {
			modalStack = new List<ModalItem>();
		}
	}
	
	/// <summary>
	/// Registers a new input camera. The registered cameras are parsed in the order they were added, unless you specify index to alter that order.
	/// </summary>
	/// <param name='inputCamera'>
	/// Input camera.
	/// </param>
	/// <param name='index'>
	/// Optional index for the parsing order.
	/// </param>
	public void RegisterInputCamera(Camera inputCamera, int index = -1)
	{
		if (index < 0 || index >= inputCameras.Count) {
			inputCameras.Add(inputCamera);
		}
		else {
			inputCameras.Insert(index, inputCamera);
		}
	}
	
	/// <summary>
	/// Unregisters the input camera.
	/// </summary>
	/// <param name='inputCamera'>
	/// Input camera.
	/// </param>
	public void UnregisterInputCamera(Camera inputCamera)
	{
		inputCameras.Remove(inputCamera);
	}
	
	/// <summary>
	/// Registers a new modal item. It is added to the top of the stack unless an index is specified.
	/// </summary>
	/// <param name='item'>
	/// New modal item.
	/// </param>
	/// <param name='index'>
	/// Index.
	/// </param>
	public void RegisterModalItem(ModalItem item, int index = 0)
	{
		if (index < 0 || index >= modalStack.Count) {
			modalStack.Add(item);
		}
		else {
			modalStack.Insert(index, item);
		}
	}
	
	/// <summary>
	/// Unregisters the modal item.
	/// </summary>
	/// <param name='item'>
	/// Item.
	/// </param>
	public void UnregisterModalItem(ModalItem item)
	{
		modalStack.Remove(item);
	}
	
	/// <summary>
	/// Gets the input block at the given position.
	/// </summary>
	/// <returns>
	/// The input block.
	/// </returns>
	/// <param name='touchPos'>
	/// Touch position.
	/// </param>
	protected InputBlock GetInputBlock(Vector2 touchPos)
	{
		foreach (Camera inputCamera in inputCameras)
		{
			InputBlock block = GetInputBlock(inputCamera, touchPos);
			if (block != null) {
				if (modalStack.Count > 0) {
					foreach (ModalItem modalItem in modalStack) 
					{
						if ((modalItem.layerMask | inputCamera.cullingMask) != 0) {
							if (modalItem.inputBlock == block || modalItem.inputBlock.HasChild(block)) {
								return block;
							}
							
							break;
						}
					}
				}
				else {
					return block;
				}
			}
		}
		
		return null;
	}
	
	/// <summary>
	/// Gets the input block from the input camera at the given position.
	/// </summary>
	/// <returns>
	/// The input block.
	/// </returns>
	/// <param name='inputCamera'>
	/// Input camera.
	/// </param>
	/// <param name='touchPos'>
	/// Touch position.
	/// </param>
	public InputBlock GetInputBlock(Camera inputCamera, Vector2 touchPos)
	{
		Component inputComponent = cachedInputWrapper.GetInputComponent(inputCamera, touchPos);
		
		if (inputComponent != null) {
			return inputComponent.GetComponent<InputBlock>();
		}
		
		return null;
	}
	
	/// <summary>
	/// Raises the tap began event for the tapped input block.
	/// </summary>
	/// <param name='touchPos'>
	/// Touch position.
	/// </param>
	protected void OnTapBegan(Vector2 touchPos)
	{
		InputBlock block = GetInputBlock(touchPos);
		
		if (block != null) {
			block.OnTapBegan(touchPos);
		}
	}
	
	/// <summary>
	/// Raises the tap ended event for the tapped input block.
	/// </summary>
	/// <param name='touchPos'>
	/// Touch position.
	/// </param>
	protected void OnTapEnded(Vector2 touchPos)
	{
		InputBlock block = GetInputBlock(touchPos);
		
		if (block != null) {
			block.OnTapEnded(touchPos);
		}
	}
	
	/// <summary>
	/// Raises the swipe from left to right event for the swiped input block.
	/// </summary>
	/// <param name='startPos'>
	/// Start swipe position.
	/// <param name='endPos'>
	/// End swipe position.
	/// </param>
	protected void OnSwipeLeftRight(Vector2 startPos, Vector2 endPos)
	{
		Debug.Log("gugu");
		InputBlock block = GetInputBlock(startPos);
		
		if (block != null) {
			block.OnSwipeLeftRight(startPos, endPos);
		}
	}
	
	/// <summary>
	/// Raises the swipe from right to left event for the swiped input block.
	/// </summary>
	/// <param name='startPos'>
	/// Start swipe position.
	/// <param name='endPos'>
	/// End swipe position.
	/// </param>
	protected void OnSwipeRightLeft(Vector2 startPos, Vector2 endPos)
	{
		InputBlock block = GetInputBlock(startPos);
		
		if (block != null) {
			block.OnSwipeRightLeft(startPos, endPos);
		}
	}
	
	/// <summary>
	/// Unity method.
	/// Update this instance.
	/// </summary>
	protected void Update()
	{
		cachedInputWrapper.CheckInput();
	}
}

/// <summary>
/// Helper class that stores info about a modal input block.
/// </summary>
public class ModalItem
{
	/// <summary>
	/// The input block.
	/// </summary>
	public InputBlock inputBlock;
	
	/// <summary>
	/// The layer mask. Determines what layers the input block is modal over.
	/// </summary>
	public int layerMask;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ModalItem"/> class.
	/// </summary>
	/// <param name='_inputBlock'>
	/// _input block.
	/// </param>
	/// <param name='_layerMask'>
	/// _layer mask.
	/// </param>
	public ModalItem(InputBlock _inputBlock, int _layerMask) 
	{
		inputBlock = _inputBlock;
		layerMask = _layerMask;
	}
}

//}