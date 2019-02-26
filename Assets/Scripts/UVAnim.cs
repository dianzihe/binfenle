using UnityEngine;
using System.Collections;
using System;

public class UVAnim : MonoBehaviour {
	// horizontal tile count
	public int horizTileCount = 1;
	// vertical tile count
	public int vertTileCount = 1;
	
	public int framesPerSecond = 10;
	
	public bool changeOnlyOffset = false;
	
	// Indicates whether to play the animation just once and then stops the animations
	// It can be played again with Play()
	public bool playOnce = false;
	
	// Indicates whether to play on start
	public bool playAutomatically = true;
	
	// Indicates whether to destroy this GAMEOBJECT! after the animation has run once
	// Works only IF playOnce is activated
	public bool autoDestroy = false;
	
	// (Optional) Set the gameobject that has the required material you want to animate
	public Renderer targetRenderer;
	
	// Here you can override the texture channel that is animated (for example you could animate a texture channel with the name _BumpMap)
	public string animatedTextureChannel = "_MainTex";
		
	// Callback for when animation is finished 
	public Action<UVAnim> onPlayFinished;
	
	private bool isPlaying;
	private float uIndex;
	private float vIndex;
	private int frameIdx = 0;
	private Vector2 frameUVSize = Vector2.zero;
	private Vector2 frameUVOffset = Vector2.zero;
	
	private float startTime;
	
	void Awake() {
		if (targetRenderer == null) {
			targetRenderer = GetComponent<Renderer>();
		}
		isPlaying = false;
	}
	
	void Start () {
		//Initialize to display the first frame
		
		if (playAutomatically) {
			PlayUVAnim();
		}
	}
	
	public void PlayUVAnim () {
		startTime = 1f / framesPerSecond;
		UpdateUVs();
		isPlaying = true;
	}
				
	private void UpdateUVs() {
	    // repeat when exhausting all frames
//	    frameIdx = frameIdx % (horizTileCount * vertTileCount);
	
	    // split into horizontal and vertical index
	    uIndex = frameIdx % horizTileCount;
	    vIndex = frameIdx / horizTileCount;
	
	    // Size of every tile
	    frameUVSize.x = 1.0f / horizTileCount;
		frameUVSize.y = 1.0f / vertTileCount;
	    	
	    // build offset
	    // v coordinate is the bottom of the image in opengl so we need to invert.
	    frameUVOffset.x = uIndex * frameUVSize.x;
		if (changeOnlyOffset) {
			frameUVOffset.y = 1.0f - vIndex * frameUVSize.y;
		} else {
			frameUVOffset.y = 1.0f - frameUVSize.y - vIndex * frameUVSize.y;
		}
	   
		foreach (Material mat in targetRenderer.materials) {
			//TODO: generalize script correctly!!!
		    //mat.mainTextureOffset = frameUVOffset;
			mat.SetTextureOffset(animatedTextureChannel, frameUVOffset);
			if (!changeOnlyOffset) {
		    	//mat.mainTextureScale = frameUVSize;
				mat.SetTextureScale(animatedTextureChannel, frameUVSize);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (isPlaying) {
			startTime += Time.deltaTime;
		   // Calculate index
		    frameIdx = (int)(startTime * framesPerSecond);
			if (frameIdx >= horizTileCount * vertTileCount) {
				if (onPlayFinished != null) {
					onPlayFinished(this);	
				}

				if(playOnce) {
					if (autoDestroy) {
						Destroy(gameObject);
					} else {
						isPlaying = false;	
					}
					return;
				} else {
					frameIdx = frameIdx % (horizTileCount * vertTileCount);	
				}
			}
			
		    UpdateUVs();
		}
	}
}
