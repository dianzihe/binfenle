using UnityEngine;
using System.Collections;

public class EmptyBoardPiece : Match3BoardPiece {
	
//	// pairs of: (x, y) uv tile coord, and (width, height) uv offset
//	public Rect[] uvRects = null;
//	private int uvRectIdx = -1;
//	private Vector2[] uvs;
//	private Mesh mesh;
//	
//	[System.NonSerialized]
//	public Transform modelTransform;
//	
//	
//	public override void Awake () {
//		base.Awake ();
//
//		modelTransform = cachedTransform.Find("Model");
//		mesh = modelTransform.gameObject.GetComponent<MeshFilter>().mesh;
//		uvs = mesh.uv;
//	}
//	
//	public override void InitComponent (BoardData owner, int row, int column) {
//		base.InitComponent(owner, row, column);
//
////		Debug.Log("UVS list for : " + name);
////		for(int i = 0; i < uvs.Length; i++) {
////			Debug.Log("uvs[" + i +"] = " + uvs[i]);
////		}
//		RandomUvRect();
//	}
//	
//	/// <summary>
//	/// Pick a randoms the uv rect from the specified list of <see cref="uvRects"/> and apply it to the current mesh.
//	/// </summary>
//	void RandomUvRect() {
//		if (uvRects == null) {
//			return;
//		}
//
//		// Pick random uv Rect
//		uvRectIdx = Random.Range(0, uvRects.Length);
//		
//		// Bottom-Left uv
//		uvs[0] = new Vector2(uvRects[uvRectIdx].x, uvRects[uvRectIdx].y);
//		// Top-Right uv
//		uvs[1] = new Vector2(uvRects[uvRectIdx].x + uvRects[uvRectIdx].width, uvRects[uvRectIdx].y + uvRects[uvRectIdx].height);
//		// Bottom-Right uv
//		uvs[2] = new Vector2(uvRects[uvRectIdx].x + uvRects[uvRectIdx].width, uvRects[uvRectIdx].y);
//		// Top-Left uv
//		uvs[3] = new Vector2(uvRects[uvRectIdx].x, uvRects[uvRectIdx].y + uvRects[uvRectIdx].height);
//		
//		// Update mesh uvs
//		mesh.uv = uvs;
//	}
//	
////	public override void Start ()
////	{
////		base.Start ();
////		StartCoroutine(TestAnim());
////	}
////	
////	IEnumerator TestAnim() {
////		while(true) {
////			yield return new WaitForSeconds(2f);
////			
////			RandomUvRect();
////		}
////	}

}
