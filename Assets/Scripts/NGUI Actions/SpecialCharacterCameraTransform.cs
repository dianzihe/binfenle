using UnityEngine;
using System.Collections;

public class SpecialCharacterCameraTransform : MonoBehaviour 
{
	public Transform[] transforms;
	
	// Use this for initialization
	void Awake () 
	{
		int index = CharacterSpecialAnimations.CharIdx;
		
		if (index >= 0 && index < transforms.Length && transforms[index] != null)
		{
			transform.position = transforms[index].position;
			transform.rotation = transforms[index].rotation;
			transform.localScale = transforms[index].localScale;
		}
	}
}
