using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Blocks controller handling an array of the given type of blocks.
/// </summary>
public class BlocksController<T> : MonoBehaviour, IIndexedList where T : Block
{
	/// <summary>
	/// The blocks controlled by this blocks controller.
	/// </summary>
	public T[] blocks;
	
	/// <summary>
	/// The index of the current block.
	/// </summary>
	protected int currentIndex = -1;
	
	/// <summary>
	/// Occurs when the blocks controlled by this instance finish playing.
	/// </summary>
	public event PlayableControllerDelegates.PlayingFinishedEventHandler OnPlayingFinished;
		
	/// <summary>
	/// Gets or sets the index of the current block.
	/// </summary>
	/// <value>
	/// The index of the current block.
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
	/// The number of blocks handled by this blocks controller.
	/// </summary>
	/// <returns>
	/// The number of blocks.
	/// </returns>
	public int IndexCount ()
	{
		return blocks.Length;
	}
	
	/// <summary>
	/// Gets the block with the given name and returns it as the given type.
	/// </summary>
	/// <returns>
	/// The block.
	/// </returns>
	/// <param name='blockName'>
	/// Block name.
	/// </param>
	/// <typeparam name='T'>
	/// The type of the block.
	/// </typeparam>
	public T GetBlock(string blockName)
	{
		foreach (T block in blocks) 
		{
			if (block.name == blockName) {
				return block;
			}
		}
		
		return null;
	}
		
	/// <summary>
	/// Registers this controller to be notified when the given block finishes playing.
	/// </summary>
	/// <param name='block'>
	/// The block.
	/// </param>
	public void RegisterFinishedEvent(PlayableBlock block)
	{
		block.OnPlayingFinished += OnBlockFinished;
	}
		
	/// <summary>
	/// Raised when a block finishes playing.
	/// </summary>
	/// <param name='block'>
	/// The block that finished playing.
	/// </param>
	public void OnBlockFinished(PlayableBlock block)
	{
		if (!IsPlaying()) {
			if (OnPlayingFinished != null && typeof(PlayableController).IsInstanceOfType(this)) {
				OnPlayingFinished((PlayableController)this);
			}
		}
	}
	
	/// <summary>
	/// Determines whether this instance has any block playing. If this instance doesn't control playable blocks, it returns false.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance has any block playing; otherwise, <c>false</c>.
	/// </returns>
	public bool IsPlaying()
	{
		bool isPlaying = false;
		
		foreach (T block in blocks) 
		{
			if (typeof(PlayableBlock).IsInstanceOfType(block) && (block as PlayableBlock).IsPlaying() && !(block as PlayableBlock).IsLooped()) {
				isPlaying = true;
				break;
			}
		}
		
		return isPlaying;
	}
}

//}
