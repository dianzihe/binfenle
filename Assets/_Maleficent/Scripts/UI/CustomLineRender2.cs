using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomLineRender2 : MonoBehaviour
{
	public float width;
	public Material material;
	public float alpha = 1f;

	
	private List< Vector3 > vertices = new List<Vector3>();
	private List< Vector3 > normals  = new List<Vector3>();
	MeshRenderer meshRenderer;
	MeshFilter   meshFilter;
	private Mesh mesh;
	/*
	void Awake ()
	{
		meshFilter   = gameObject.AddComponent< MeshFilter >();
		meshRenderer = gameObject.AddComponent< MeshRenderer >();
		mesh = meshFilter.mesh;
		if(material != null)
			meshRenderer.material = Instantiate(material) as Material;
		mesh.MarkDynamic();
	}
	*/
	
	public Vector3 GetVertex(int _n) {
		return transform.TransformPoint(vertices[_n]);
	}
	
	public int GetNVertices() {
		return vertices.Count;
	}
	
	public Vector3 GetNormal(int _n) {
		return transform.TransformPoint(normals[_n]);
	}
	
	public int GetNNormals() {
		return normals.Count;
	}
	
	public void Init(Material material,float width,float alpha)
	{
		meshFilter   = gameObject.AddComponent< MeshFilter >();
		meshRenderer = gameObject.AddComponent< MeshRenderer >();
		mesh = meshFilter.mesh;
		meshRenderer.material = Instantiate(material) as Material;
		this.width = width;
		this.material = material;
		this.alpha = alpha;
	}

	public void AddPoint(Vector3 _point, Vector3 _normal,int _idx = -1)
	{
		_point = transform.InverseTransformPoint(_point);
		_normal = transform.InverseTransformDirection(_normal);
	
		if(_idx == -1)
		{
			vertices.Add(_point);
			normals.Add(_normal);
		}
		else
		{
			List <Vector3> newVertexs = new List<Vector3>();
			List <Vector3> newNormals = new List<Vector3>();

			newVertexs.AddRange(vertices.GetRange(0,_idx));
			newNormals.AddRange(normals.GetRange(0,_idx));

			newVertexs.Add(_point);
			newNormals.Add(_normal);
			
			newVertexs.AddRange(vertices.GetRange(_idx,vertices.Count-_idx));
			newNormals.AddRange(normals.GetRange(_idx,normals.Count-_idx));
			
			vertices.Clear();
			vertices.AddRange(newVertexs);
			normals.Clear();
			normals.AddRange(newNormals);
		}
	}

	public void ChangePoint(Vector3 _point, Vector3 _normal,int _idx)
	{
		_point = transform.InverseTransformPoint(_point);
		_normal = transform.InverseTransformDirection(_normal);
	
		vertices[_idx]= _point;
		normals[_idx]=  _normal;
	}

	public void RemovePoint(int _idx)
	{
		vertices.RemoveAt(_idx);
		normals.RemoveAt(_idx);
	}

	 public void UpdateMesh() {	
		UpdateMesh(alpha);
	}

	public void UpdateMesh(float alpha) {	
		if( vertices.Count > 0) {
			Vector3[] meshVertices = new Vector3[vertices.Count * 2 + 2];
			Vector2[] meshUV       = new Vector2[vertices.Count * 2 + 2];
			Color[]   meshColors   = new Color  [vertices.Count * 2 + 2];
			int[]     meshTriangles= new int[(vertices.Count - 1) * 6];
			float halfW = width * 0.5f;
			float tileY = 0.0f;		
			
			//vertices, uv and colors
			Vector3 min = new Vector3( 9999999.0f,  9999999.0f,  9999999.0f);
			Vector3 max = new Vector3(-9999999.0f, -9999999.0f, -9999999.0f);
			for(int i = 0; i < vertices.Count; ++i) {
				Vector3 dir = Vector3.zero;
				if(i == 0) {
					if(vertices.Count >= 2) {
						dir = vertices[1] - vertices[0];
					}
				} else if(i == 1 || i == vertices.Count - 1) {
					dir = vertices[i] - vertices[i - 1];
				} else {
					dir = ((vertices[i] - vertices[i - 1]) + (vertices[i + 1] - vertices[i])) * 0.5f;
				}
				Vector3 tan = Vector3.Cross(dir, normals[i]).normalized;
				
				//vertices
				meshVertices[i * 2    ]	= vertices[i] - halfW * tan;
				meshVertices[i * 2 + 1]	= vertices[i] + halfW * tan;
				
				//uv
				tileY += dir.magnitude / width;
				meshUV[i * 2]     = new Vector2(0.0f, tileY);
				meshUV[i * 2 + 1] = new Vector2(1.0f, tileY);
				
				//colors
				Color color = Color.white;
				color.a = alpha;
				meshColors[i * 2] = meshColors[i * 2 + 1] = color;
			}
			
			//triangles
			for(int i = 0; i < vertices.Count - 1; ++i) {
				meshTriangles[i * 6    ] = (i * 2)    ;
				meshTriangles[i * 6 + 1] = (i * 2) + 1;
				meshTriangles[i * 6 + 2] = (i * 2) + 3;
				
				meshTriangles[i * 6 + 3] = (i * 2);    
				meshTriangles[i * 6 + 4] = (i * 2) + 3;
				meshTriangles[i * 6 + 5] = (i * 2) + 2;
			}

			meshVertices[meshVertices.Length - 2] = new Vector3(-10.0f, -10.0f, -10.0f);
			meshVertices[meshVertices.Length - 1] = new Vector3( 10.0f,  10.0f,  10.0f);
			
			mesh.Clear();
			mesh.vertices = meshVertices;
			mesh.uv = meshUV;
			mesh.colors = meshColors;
			mesh.triangles = meshTriangles;
		}
	}

	public void SetAlpha(float alpha)
	{
		Color[] newMeshColors = new Color[vertices.Count * 2];
		for(int i=0;i<mesh.colors.Length;i++)
		{
			Color newColor = mesh.colors[i];
			// cannot get more alpha than 
			newColor.a = alpha;
			newMeshColors[i] = newColor;
		}
		mesh.colors = newMeshColors;
	}

	public void Clear() {
		vertices.Clear();
		normals.Clear();
	}
}
