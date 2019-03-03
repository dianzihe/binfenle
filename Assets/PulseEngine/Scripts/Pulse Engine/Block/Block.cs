using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//namespace PulseEngine {

/// <summary>
/// Block that can handle other child Blocks and provides various options for parsing them.
/// </summary>
public class Block : MonoBehaviour, IIndexedList
{
	/// <summary>
	/// Types of parsing.
	/// </summary>
	public enum ParseType : int {
		/// <summary>
		/// Returns the child Block at the current index.
		/// </summary>
		Current,
		/// <summary>
		/// Returns the child Block at the current index and increments the current index.
		/// </summary>
		CurrentAdvance,
		/// <summary>
		/// Returns the child Block at the current index and increments the current index, restarting from 0 if the end is reached.
		/// </summary>
		CurrentAdvanceLoop,
		/// <summary>
		/// Returns a random child Block and updates the current index to point at that Block.
		/// </summary>
		Random,
		/// <summary>
		/// Returns the child Block at the current index and increments the current index. Parse should be called continuously until it returns false.
		/// </summary>
		Sequence,
		/// <summary>
		/// Returns the child Block at the current index and increments the current index. Parse should be called continuously until it returns false. 
		/// At this basic level it's the same as Sequence, but it helps differentiate the two when more complex behavior is needed.
		/// </summary>
		All,
	}	
		
	/// <summary>
	/// Indicates how this Block's children should be parsed.
	/// </summary>
	public ParseType parseType = ParseType.Current;
		
	/// <summary>
	/// The active block. If it is not null, ingore parsing and return this block as the result of the parse method.
	/// Can be used to implement various non-linear behaviors.
	/// </summary>
	public Block activeBlock;
	
	/// <summary>
	/// If it doesn't have 0 element, during Parse() it sets the value blocks from each pair as active blocks on each corresponding target blocks.
	/// Example: On target block at index 0 sets as active block the value block at index 0.
	/// </summary>
	public BlockPair[] setActiveBlocks;
	
	/// <summary>
	/// The events to send. When the corresponding method matching an event's type, that event's message will be sent to its target with its string parameter.
	/// </summary>
	public BlockSendEvent[] eventsToSend;
	
	[System.NonSerialized]
	public Transform cachedTransform;
	
	/// <summary>
	/// The children of the same type of Block as this instance, found directly under this game object in the hierarchy.
	/// </summary>
	protected List<Block> children;
	
	/// <summary>
	/// The index of the current parsed Block.
	/// </summary>
	protected int currentIndex = 0;
	
	/// <summary>
	/// Gets or sets the index of the current parsed Block.
	/// </summary>
	/// <value>
	/// The index of the current parsed Block.
	/// </value>
	public int CurrentIndex {
		get {
			return currentIndex;
		}
		set {
			currentIndex = value;
		}
	}
	
	/// <summary>
	/// Unity method.
	/// Awake this instance.
	/// </summary>
	public virtual void Awake()
	{
		cachedTransform = transform;
		
		children = new List<Block>();
		
		UpdateChildren();
		UpdateSetActiveBlocksWithExtendedType();
	}
	
	/// <summary>
	/// Updates the set active blocks based on the extended type of this instance.
	/// </summary>
	protected void UpdateSetActiveBlocksWithExtendedType()
	{
		if (setActiveBlocks == null) {
			setActiveBlocks = new BlockPair[0];
		}
		
		for (int i = 0; i < setActiveBlocks.Length; ++i) 
		{
			if (setActiveBlocks[i].targetBlock.GetType() != this.GetType()) {
				setActiveBlocks[i].targetBlock = (Block)setActiveBlocks[i].targetBlock.GetComponent(this.GetType());
				if (setActiveBlocks[i].targetBlock == null) {
					Debug.LogError("Set Active Blocks on object: " + name + " has bad target block at index: " + i + ". The provided object doesn't have a component of type: " + this.GetType()); 
				}
			}
			
			if (setActiveBlocks[i].valueBlock.GetType() != this.GetType()) {
				setActiveBlocks[i].valueBlock = (Block)setActiveBlocks[i].valueBlock.GetComponent(this.GetType());
				if (setActiveBlocks[i].valueBlock == null) {
					Debug.LogError("Set Active Blocks on object: " + name + " has bad value block at index: " + i + ". The provided object doesn't have a component of type: " + this.GetType()); 
				}
			}
		}
	}
	
	/// <summary>
	/// Updates the children list with all children of the same type of Block as this instance, found directly under this game object in the hierarchy.
	/// </summary>
	public void UpdateChildren()
	{
		children.Clear();
		
		int childCount = cachedTransform.childCount;
		
		for (int i = 0; i < childCount; ++i) 
		{
			Block child = cachedTransform.GetChild(i).GetComponent(this.GetType()) as Block;
			
			if (child) {
				children.Add(child);
			}
		}
	}
	
	/// <summary>
	/// Parse this Block's children, depending on its parse type.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the method should be called again after the resulting child Block is processed; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='result'>
	/// The resulting child Block to be processed.
	/// </param>
	/// <typeparam name='T'>
	/// The specific type of this Block.
	/// </typeparam>
	public bool Parse<T>(out T result) where T : Block
	{
		if (activeBlock != null) {
			result = (T)activeBlock;
			activeBlock = null;
			return false;
		}
		
		if (children.Count == 0) {
			result = (T)this;
			return false;
		}
		
		int selectedIndex = currentIndex;
		bool shouldContinue = false;
		
		if (parseType == ParseType.Current) {
			//do nothing, the selected index remains the current index
		}
		else if (parseType == ParseType.CurrentAdvance) {
			//advance the current index for any possible future calls
			if (currentIndex < children.Count - 1) {
				currentIndex++;
			}
		}
		else if (parseType == ParseType.CurrentAdvanceLoop) {
			//advance the current index for any possible future calls, starting from 0 if the end is reached
			currentIndex = (currentIndex + 1) % children.Count;
		}
		else if (parseType == ParseType.Random) {
			//choose a random index which will also be set as the current index
			currentIndex = selectedIndex = Random.Range(0, children.Count);
		}
		else if (parseType == ParseType.Sequence || parseType == ParseType.All) {
			//check if we haven't reached the end of the list
			if (currentIndex < children.Count - 1) {
				//advance the current index for the next call
				currentIndex++;
				shouldContinue = true;
			}
		}
		
		if (selectedIndex < children.Count) {
			result = (T)children[selectedIndex];
			return shouldContinue;
		} 
		else {
			Debug.LogWarning("Block parse error: the result is null for Block: " + gameObject.name + " with parse type: " + parseType);
			result = null;
			return false;
		}
	}
	
	/// <summary>
	/// The number of direct children in this object's hierarchy with the same type of Block as itself.
	/// </summary>
	/// <returns>
	/// The number of Block children.
	/// </returns>
	public int IndexCount()
	{
		return children.Count;
	}
	
	/// <summary>
	/// Sets the active blocks. Call this in what methods you need to temporarily modify the next parsed blocks, for example from Play() but not from Stop() or Pause().
	/// </summary>
	public void SetActiveBlocks()
	{
		if (setActiveBlocks != null && setActiveBlocks.Length > 0) {
			for (int i = 0; i < setActiveBlocks.Length; i++) 
			{
				setActiveBlocks[i].targetBlock.activeBlock = setActiveBlocks[i].valueBlock;
			}
		}
	}
	
	/// <summary>
	/// Sends the events of the given type.
	/// </summary>
	/// <param name='type'>
	/// The events type.
	/// </param>
	public void SendEvents(string type)
	{
		if (eventsToSend != null && eventsToSend.Length > 0) {
			for (int i = 0; i < eventsToSend.Length; i++) 
			{
				Debug.Log("Event: " + eventsToSend[i].message + " " + eventsToSend[i].parameters);
				if (eventsToSend[i].type == type) {
					Debug.Log("Sent");
					eventsToSend[i].target.SendMessage(eventsToSend[i].message, eventsToSend[i].parameters);
				}
			}
		}
	}
	
	/// <summary>
	/// Destroy this instance.
	/// </summary>
	public void Destroy()
	{
		Destroy(this);
	}
	
	/// <summary>
	/// Destroys the game object that hold this component.
	/// </summary>
	public void DestroyGameObject()
	{
		Destroy(gameObject);
	}
}


/// <summary>
/// Block pair composed of a target and a value.
/// </summary>
[System.Serializable]
public class BlockPair
{
	/// <summary>
	/// The target block.
	/// </summary>
	public Block targetBlock;
	
	/// <summary>
	/// The value block.
	/// </summary>
	public Block valueBlock;
}


/// <summary>
/// Helper class that holds info for when a block needs to send an event.
/// </summary>
[System.Serializable]
public class BlockSendEvent
{
	/// <summary>
	/// The type determines when this event should be sent. Each function that calls SendEvents() must also pass its type and if those 2 types match, the event is sent.
	/// </summary>
	public string type;
	
	/// <summary>
	/// The target to which the event will be sent.
	/// </summary>
	public GameObject target;
	
	/// <summary>
	/// The event message to be sent to the target.
	/// </summary>
	public string message;
	
	/// <summary>
	/// The parameters of the event message.
	/// </summary>
	public string parameters;
}

//}
