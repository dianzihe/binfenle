using UnityEngine;
using System.Collections;

//namespace PulseEngine {
	
/// <summary>
/// Text controller handling an array of text blocks.
/// </summary>
public class TextController : BlocksController<TextBlock>, PlayableController
{	
	/// <summary>
	/// Fades in the text with the given name.
	/// </summary>
	/// <param name='textName'>
	/// Text game object name.
	/// </param>
	public void FadeInText(string textName, float animationTime = TextBlock.DEFAULT_ANIMATION_TIME)
	{
		TextBlock textBlock = GetBlock(textName);
		if (textBlock) {
			textBlock.FadeIn(animationTime);
			RegisterFinishedEvent(textBlock);
		}
	}
		
	/// <summary>
	/// Fades out the text with the given name.
	/// </summary>
	/// <param name='textName'>
	/// Text game object name.
	/// </param>
	public void FadeOutText(string textName, float animationTime = TextBlock.DEFAULT_ANIMATION_TIME)
	{
		TextBlock textBlock = GetBlock(textName);
		if (textBlock) {
			textBlock.FadeOut(animationTime);
			RegisterFinishedEvent(textBlock);
		}
	}
}

//}
