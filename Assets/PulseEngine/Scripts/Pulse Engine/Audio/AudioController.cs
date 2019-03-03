using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Narration controller handling an array of narration blocks.
/// </summary>
public class AudioController : BlocksController<AudioBlock>, PlayableController
{
	/// <summary>
	/// Plays the given narration block.
	/// </summary>
	/// <param name='narrationName'>
	/// Narration block name.
	/// </param>
	public void PlayNarration(string narrationName)
	{
		NarrationBlock narrationBlock = GetBlock(narrationName) as NarrationBlock;
		if (narrationBlock) {
			narrationBlock.Play();
			RegisterFinishedEvent(narrationBlock);
		}
	}
	
	/// <summary>
	/// Plays the given audio block.
	/// </summary>
	/// <param name='audioName'>
	/// Audio block name.
	/// </param>
	public void PlaySound(string audioName)
	{
		AudioBlock audioBlock = GetBlock(audioName);
		if (audioBlock) {
			audioBlock.Play();
			RegisterFinishedEvent(audioBlock);
		}
	}
	
	/// <summary>
	/// Plays the given music block.
	/// </summary>
	/// <param name='musicName'>
	/// Music block name.
	/// </param>
	public void PlayMusic(string musicName)
	{
		MusicBlock musicBlock = GetBlock(musicName) as MusicBlock;
		if (musicBlock) {
			musicBlock.Play();
			RegisterFinishedEvent(musicBlock);
		}
	}
}

//}
