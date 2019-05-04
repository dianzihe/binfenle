using UnityEngine;
using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;

public class MusicController : MonoBehaviour 
{
	protected static MusicController instance = null;
	
	/// <summary>
	/// The name of the global FSM variable that will store the reference for this GameObject at init.
	/// Leave empty if you don't want this to be set.
	/// </summary>
	public string gameObjFsmGlobalVar = "";
	
	[System.NonSerialized]
	public AudioSource audioSrc;
	
	
	void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("[MusicController] Multiple instances of this singleton found! In scene: " + Application.loadedLevelName);
			Destroy(gameObject);
			return;
		}
		
		instance = this;
		audioSrc =  GetComponent<AudioSource>();
		DontDestroyOnLoad(gameObject);
		
		UpdatePlaymakerReferences();

		//IPodPlayerEventsHandler.OnIPodPlayStateChanged += OnIPodPlayStateChangedEvent;
		//IPodPlayerEventsHandler.OnIPodPlayStatusChecked += OnIPodPlayStatusCheckedEvent;
	}
	
	public void UpdatePlaymakerReferences()
	{
		if ( !string.IsNullOrEmpty(gameObjFsmGlobalVar) )
		{
			FsmGameObject fsmGameObj = FsmVariables.GlobalVariables.GetFsmGameObject(gameObjFsmGlobalVar);
			if (fsmGameObj != null && !fsmGameObj.IsNone) {
				fsmGameObj.Value = gameObject;
			}
			else {
				Debug.LogWarning("[MusicController] Invalid FSM Global Variable specified: gameObjFsmGlobalVar = " + gameObjFsmGlobalVar);
			}
		}
	}
	
	public static MusicController Instance
	{
		get
		{
			if (instance == null) 
			{
				GameObject container = new GameObject("MusicController", typeof(MusicController));
				DontDestroyOnLoad(container);
			}

			return instance;
		}
	}
	
	public void OnIPodPlayStateChangedEvent(bool isPlaying)
	{
		audioSrc.volume = !isPlaying ? 0f : 1f;
	}
	
	public void OnIPodPlayStatusCheckedEvent()
	{
		//audioSrc.volume = IPodPlayerEventsHandler.Instance.IsIPodPlaying ? 0f : 1f;
	}
	
	void OnDestroy()
	{
		//IPodPlayerEventsHandler.OnIPodPlayStateChanged -= OnIPodPlayStateChangedEvent;
		//IPodPlayerEventsHandler.OnIPodPlayStatusChecked -= OnIPodPlayStatusCheckedEvent;
	}
		
}