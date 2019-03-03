using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Flow controller. Handles an array of flow blocks and can change their state and play their animation. Raises an event when all blocks finish playing their current state.
/// </summary>
public class FlowController : BlocksController<FlowBlock>
{
	/// <summary>
	/// The number of finished blocks.
	/// </summary>
	protected int finishedBlocks = 0;
	
	#region Events Delegates
	public delegate void FlowPlayingFinishedEventHandler(FlowController flowController);
	#endregion Events Delegates
	
	/// <summary>
	/// Occurs when all blocks finish playing their intro state.
	/// </summary>
	public event FlowPlayingFinishedEventHandler OnIntroFinished;
	
	/// <summary>
	/// Occurs when all blocks finish playing their idle state.
	/// </summary>
	public event FlowPlayingFinishedEventHandler OnIdleFinished;
	
	/// <summary>
	/// Occurs when all blocks finish playing their outro state.
	/// </summary>
	public event FlowPlayingFinishedEventHandler OnOutroFinished;
	
	/// <summary>
	/// Plays the given state on all controlled flow blocks.
	/// </summary>
	/// <param name='state'>
	/// Flow state.
	/// </param>
	public void PlayFlow(FlowBlock.FlowState state)
	{
		Debug.Log("Playing flow: " + state + " on scene: " + name);
		finishedBlocks = 0;
		
		foreach (FlowBlock currentBlock in blocks) 
		{
			currentBlock.PlayState(state);
			currentBlock.OnPlayingFinished += OnFlowBlockFinished;
		}
	}
	
	/// <summary>
	/// Plays the intro flow on all controlled flow blocks.
	/// </summary>
	public void PlayIntroFlow()
	{
		PlayFlow(FlowBlock.FlowState.Intro);
	}
	
	/// <summary>
	/// Plays the idle flow on all controlled flow blocks.
	/// </summary>
	public void PlayIdleFlow()
	{
		PlayFlow(FlowBlock.FlowState.Idle);
	}
	
	/// <summary>
	/// Plays the outro flow on all controlled flow blocks.
	/// </summary>
	public void PlayOutroFlow()
	{
		PlayFlow(FlowBlock.FlowState.Outro);
	}
		
	/// <summary>
	/// Raised when the give flow block finishes playing. Used to test if all flow blocks finished playing and, if true, to raise the corresponding event.
	/// </summary>
	/// <param name='block'>
	/// The block that finished playing.
	/// </param>
	public void OnFlowBlockFinished(PlayableBlock block)
	{
		finishedBlocks++;
		
		if (finishedBlocks == blocks.Length) {
			// All finished, raise corresponding event
			FlowBlock.FlowState state = (block as FlowBlock).State;
			
			Debug.Log("Finished flow: " + state + " on scene: " + name);
			
			switch (state) {
			case FlowBlock.FlowState.Intro :
				if (OnIntroFinished != null) {
					OnIntroFinished(this);
				}
				break;
			case FlowBlock.FlowState.Idle :
				if (OnIdleFinished != null) {
					OnIdleFinished(this);
				}
				break;
			case FlowBlock.FlowState.Outro :
				if (OnOutroFinished != null) {
					OnOutroFinished(this);
				}
				break;
			}
		}
	}
}

