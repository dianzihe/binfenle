using UnityEngine;
using UnityEditor;
using System.Collections;

public static class SoundManagerEditor
{
	
	[MenuItem("CONTEXT/SoundManager/Add Selected AudioClips")]
	public static void AddSelectedAudioClips(MenuCommand menuItem) 
	{
		SoundManager sndManager = menuItem.context as SoundManager;
		Object[] selectedObjects = Selection.objects;
		
		for(int i = 0; i < selectedObjects.Length; i++) 
		{
			AudioClip audioClip = selectedObjects[i] as AudioClip;
			if (audioClip != null)
			{
				sndManager.RegisterSound(new SoundEffect(audioClip));
			}
		}
	}
}
