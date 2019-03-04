using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderBufferSize : MonoBehaviour 
{
	protected RenderTexture landscapeBuffer;
	protected RenderTexture portraitBuffer;
	
	public List<Camera> cameras;
	public Renderer[] renderers;

	public float Size{
		set {
			foreach (Camera cam in cameras) {
				HorizontalFOVCamera horizontalFOVCamera = cam.GetComponent< HorizontalFOVCamera >();
				if(horizontalFOVCamera == null) {
					cam.rect = new Rect(0f, 0f, value, value);
				} else {
					if(Screen.width > Screen.height) {
						cam.rect = new Rect(cam.rect.x, cam.rect.y, horizontalFOVCamera.landscapeNormalizedViewPort.width * value, horizontalFOVCamera.landscapeNormalizedViewPort.height * value);
					} else {
						cam.rect = new Rect(cam.rect.x, cam.rect.y, horizontalFOVCamera.portraitNormalizedViewPort.width * value, horizontalFOVCamera.portraitNormalizedViewPort.height * value);
					}
				}
			}
		}
	}

	void Awake () 
	{
		UpdateBuffers();
	}
	
	void Start()
	{
		Size = 1.0f;
		UpdateSize();
		
		OrientationListener.Instance.OnOrientationChanged += OrientationChanged;
	}

	void OnDestroy() 
	{
		landscapeBuffer.Release();
		portraitBuffer.Release();
		Destroy(landscapeBuffer);
		Destroy(portraitBuffer);
	}

	void UpdateBuffers()
	{
		int width = Mathf.Max(Screen.width, Screen.height);
		int height = Mathf.Min(Screen.width, Screen.height);
		
		if (landscapeBuffer == null || width != landscapeBuffer.width || height != landscapeBuffer.height) {
			if (landscapeBuffer != null) {
				Destroy(landscapeBuffer);
			}
			landscapeBuffer = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);
			landscapeBuffer.Create();
		}
		if (portraitBuffer == null || height != portraitBuffer.width || width != portraitBuffer.height) {
			if (portraitBuffer != null) {
				Destroy(portraitBuffer);
			}
			portraitBuffer = new RenderTexture(height, width, 32, RenderTextureFormat.ARGB32);
			portraitBuffer.Create();
		}
	}
	
	public void OrientationChanged(ScreenOrientation newOrientation) 
	{
		//StartCoroutine(UpdateSizeNextFrame());
		UpdateSize();
	}
	
	IEnumerator UpdateSizeNextFrame() 
	{
		// Wait for a frame to pass to make sure the cameras have been updated
		yield return null;
		UpdateSize();
	}

	// Update is called once per frame
	void UpdateSize () 
	{		
		UpdateBuffers();
		
		RenderTexture bufferToUse = (Screen.width > Screen.height) ? landscapeBuffer : portraitBuffer;
		
		float aspectRatio = (float)Screen.width / (float)Screen.height;
		
		foreach (Camera cam in cameras) {
			if(cam.GetComponent<IgnoreForRenderToTexture>() != null && cam.GetComponent<IgnoreForRenderToTexture>().ignore)
			{
			//	continue;
				cam.targetTexture = null;
			}
			else
				cam.targetTexture = bufferToUse;

			if (cam.rect == new Rect(0f, 0f, 1f, 1f)) {
				cam.aspect = aspectRatio;
			} 
			else {
				cam.ResetAspect();
				cam.ResetProjectionMatrix();
			}
		}

		foreach(Renderer r in renderers) {
			//Update texture
			r.material.mainTexture = bufferToUse;
			Camera renderCam = r.transform.parent.GetComponent<Camera>();
			Transform xForm = r.transform;

			//Update position (really? do we need this?)
			Vector3 newPos = renderCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
			newPos.z = xForm.position.z;
			xForm.position = newPos;
			

			//Update scale
			Vector3 newSize = xForm.localScale;
			newSize.z = (renderCam.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y - 
			             renderCam.ViewportToWorldPoint(Vector3.zero).y) / renderCam.transform.localScale.y;
			if (renderCam.orthographic) {
				newSize.z /= renderCam.orthographicSize / 10f;
			}
			newSize.x = newSize.z * Screen.width / (float)Screen.height;
			xForm.localScale = newSize;
		}
	}
	
	public void DeactivateCameras()
	{
		for (int i = 0; i < cameras.Count; ++i) {
			cameras[i].gameObject.SetActive(false);
		}
	}

	// called from FSM
	public void RemoveCamera(GameObject cam)
	{
		if(cam.GetComponent<IgnoreForRenderToTexture>())
			cam.GetComponent<IgnoreForRenderToTexture>().ignore = true;
		UpdateSize();
	}

	// called from FSM
	public void AddCamera(GameObject cam)
	{
		if(cam.GetComponent<IgnoreForRenderToTexture>())
			cam.GetComponent<IgnoreForRenderToTexture>().ignore = false;
		UpdateSize();
	}
}
