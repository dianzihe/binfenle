using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PathDrawer : MonoBehaviour
{
	float alphaOn = 1f;
	float alphaOff = 1f;
	public Material materialOn;
	public Material materialOff;
	CustomLineRender2 lineRendererOn;
	CustomLineRender2 lineRendererOff;
	float lineWidthOn = 0.08f;
	float lineWidthOff = 0.08f;
	GameObject chaptersLineOn = null;
	GameObject chaptersLineOff = null;
	bool created = false;

	// animation part
	bool animate = false;
	float startTime;
	Vector3 posA;
	Vector3 posB;
	Vector3 pos;
	float duration = 1f;

	// Use this for initialization
	public void DrawPath (bool forceRedraw)
	{
		if(created && !forceRedraw)
			return;

		if(forceRedraw)
		{
			if(chaptersLineOn != null)
				Destroy(chaptersLineOn);
			if(chaptersLineOff != null)
				Destroy(chaptersLineOff);
		}

		CreateLine(true);
		CreateLine(false);

		foreach(Transform child in transform.Cast<Transform>().OrderBy(t=>Convert.ToInt32(t.name)))
		{
			Vector3 normal = -child.transform.forward;
			LoadLevelButton childButton = child.GetComponent<LoadLevelButton>();
			//Vector3 point = child.transform.localPosition - normal.normalized * 0.0125f;//-0.06f;//0.0125f; <- Constante Xabi
			Vector3 point = child.transform.position - normal.normalized * 0.0125f;//-0.06f;//0.0125f; <- Constante Xabi
			if(!childButton.unlocked || LoadLevelButton.lastButton == childButton)
			{
				lineRendererOff.AddPoint(point,normal);
			}
			if(childButton.unlocked)
			{
				lineRendererOn.AddPoint(point,normal);
			}
		}

		lineRendererOn.UpdateMesh();
		lineRendererOff.UpdateMesh();
		created = true;
	}

	public void InitLineAnimation()
	{
		// draw previous path and gets the "last" positio
		DrawPath(true);

		posA = lineRendererOn.GetVertex(lineRendererOn.GetNVertices() - 2);
		posB = lineRendererOn.GetVertex(lineRendererOn.GetNVertices() - 1);

		int lastIdx = lineRendererOn.GetNVertices() - 1;
		lineRendererOff.AddPoint(lineRendererOff.GetVertex(0), lineRendererOff.GetNormal(0), 0);
		lineRendererOn.ChangePoint(posA,lineRendererOn.GetNormal(lastIdx), lastIdx);
		lineRendererOff.ChangePoint(posA,lineRendererOff.GetNormal(0), 0);
		lineRendererOn.UpdateMesh();
		lineRendererOff.UpdateMesh();
	}

	public void DoLineAnimation()
	{
		animate = true;
		startTime = Time.time;
	}

	void Update()
	{
		if(animate)
		{
			float currentTime = Time.time;
			if(currentTime-startTime<duration)
			{
				float time = (currentTime-startTime)/duration;
				pos = posB*time + posA*(1-time);

				int lastIdx = lineRendererOn.GetNVertices() - 1;
				lineRendererOn.ChangePoint(pos,lineRendererOn.GetNormal(lastIdx), lastIdx);
				lineRendererOff.ChangePoint(pos,lineRendererOff.GetNormal(0), 0);
				lineRendererOn.UpdateMesh();
				lineRendererOff.UpdateMesh();
			}
			else
			{
				animate = false;
				lineRendererOff.RemovePoint(0);
				lineRendererOff.UpdateMesh();
			}
		}
	}

	void CreateLine(bool on)
	{
		GameObject chaptersLine;
		CustomLineRender2 lineRenderer;
		string pathName = on?"PathOn":"PathOff";
		chaptersLine = new GameObject(pathName);
		chaptersLine.AddComponent<CustomLineRender2>();
		chaptersLine.AddComponent<LinkerLineAlphaListener>();
		chaptersLine.layer = LayerMask.NameToLayer("GameGUI");
		chaptersLine.transform.parent = this.transform.parent;
		//chaptersLine.transform.localPosition = this.gameObject.transform.localPosition;
		chaptersLine.transform.position = Vector3.zero + Vector3.up * 10.0f;
		lineRenderer = chaptersLine.GetComponent<CustomLineRender2>();
		
		if(on)
		{
			lineRenderer.Init(materialOn,lineWidthOn,alphaOn);
			chaptersLineOn = chaptersLine;
			lineRendererOn = lineRenderer;
		}
		else
		{
			lineRenderer.Init(materialOff,lineWidthOff,alphaOff);
			chaptersLineOff = chaptersLine;
			lineRendererOff = lineRenderer;
		}
	}
}
