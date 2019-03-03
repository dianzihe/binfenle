using UnityEngine;
using System.Collections;

//namespace PulseEngine {

/// <summary>
/// Animation controller handling an array of animation blocks.
/// </summary>
public class AnimationController : BlocksController<AnimationBlock>, PlayableController
{
	public void PlayAnimation(string animationName)
	{
		AnimationBlock animationBlock = GetBlock(animationName);
		if (animationBlock) {
			animationBlock.Play();
			RegisterFinishedEvent(animationBlock);
		}
	}
}

//}
