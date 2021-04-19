using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GeodesicDome : MonoBehaviour
{
	private int iterations = 3;

	private Vector3[] vertices;
	private int[] triangles;

	public void Start()
	{
		InitOctahedron();

		for(int i = 0; i < iterations; i++)
        {
			Debug.Log("Iteration : " + i);
			Debug.Log("Starting with " + vertices.Length + " vertices and " + triangles.Length/3 + " face(s).");
			Subdivide();
			Debug.Log("Ended up with " + vertices.Length + " vertices and " + triangles.Length/3 + " face(s).");
        }

		CreateFinalMesh();
	}

	private void InitOctahedron()
	{
		float s2 = (float)(1.0 / Math.Sqrt(2.0));
		vertices = new Vector3[] {
			new Vector3 (0, 0, 1), 
			new Vector3 (s2, -s2, 0),
			new Vector3 (s2, s2, 0),
			new Vector3 (-s2, s2, 0),
			new Vector3 (-s2, -s2,0),
			new Vector3 (0, 0, -1) 
		};

		triangles = new int[] {
			0, 2, 1,
			0, 3, 2, 
			0, 4, 3,
			0, 1, 4, 
			5, 1, 2,
			5, 2, 3,
			5, 3, 4,
			5, 4, 1
		};
	}

	private void CreateFinalMesh()
    {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}


	private Vector3 ProjectToUnitSphere(Vector3 v)
	{
		var l = (float)Math.Sqrt(v[0]*v[0] + v[1]*v[1] + v[2]*v[2]);
		return new Vector3(v[0]/l, v[1]/l, v[2]/l);		
	}

	private void AddVertexIfNotPresent(int existingIndex, ref int newIndex, Vector3 v, ref int k, ref List<Vector3> newVertices)
	{
		if(existingIndex == -1)
		{
			newIndex = k;
			newVertices.Add(v);
			k = k + 1;
		}
		else
		{
			newIndex = existingIndex;
		}
	}

	private bool AreNearlyEqual(Vector3 v1, Vector3 v2)
	{
		var dx = v1[0] - v2[0];
		var dy = v1[1] - v2[1];
		var dz = v1[2] - v2[2];
		var d = Math.Sqrt(dx*dx + dy*dy + dz*dz);
		if (d < 1e-8) 
		{
			return true;
		} 
		return false;
	}

	private int VertexAlreadyExists(Vector3 v0, List<Vector3> a)
	{
		for(int i = 0; i < a.Count; i++)
		{
			var v = a[i];
			if(AreNearlyEqual(v0,v))
			{
				return i;
			}
		}
		return -1;
	}

	private void Subdivide()
    {
		int k = vertices.Length;
		List<int> newTriangles = new List<int>();
		List<Vector3> newVertices = new List<Vector3>(vertices);

		for (int i = 0; i < (triangles.Length/3); i++)
        {
			var idx1 = triangles[i * 3];
			var idx2 = triangles[i * 3 + 1];
			var idx3 = triangles[i * 3 + 2];

			var v1 = vertices[idx1];
			var v2 = vertices[idx2];
			var v3 = vertices[idx3];

			var v4Interpolated = new Vector3 ( (v1[0]+v2[0])/2.0f, (v1[1]+v2[1])/2.0f, (v1[2]+v2[2])/2.0f );
			var v5Interpolated = new Vector3 ( (v2[0]+v3[0])/2.0f, (v2[1]+v3[1])/2.0f, (v2[2]+v3[2])/2.0f );
			var v6Interpolated = new Vector3 ( (v3[0]+v1[0])/2.0f, (v3[1]+v1[1])/2.0f, (v3[2]+v1[2])/2.0f );
			
			var v4Projected = ProjectToUnitSphere(v4Interpolated);
			var v5Projected = ProjectToUnitSphere(v5Interpolated);
			var v6Projected = ProjectToUnitSphere(v6Interpolated);

			int v4p_idx = VertexAlreadyExists(v4Projected, newVertices);
			int v5p_idx = VertexAlreadyExists(v5Projected, newVertices);
			int v6p_idx = VertexAlreadyExists(v6Projected, newVertices);

			int idx4 = -1, idx5 = -1, idx6 = -1;

			AddVertexIfNotPresent(v4p_idx, ref idx4, v4Projected, ref k, ref newVertices);
			AddVertexIfNotPresent(v5p_idx, ref idx5, v5Projected, ref k, ref newVertices);
			AddVertexIfNotPresent(v6p_idx, ref idx6, v6Projected, ref k, ref newVertices);

			newTriangles.Add(idx1);
			newTriangles.Add(idx4);
			newTriangles.Add(idx6);

			newTriangles.Add(idx4);
			newTriangles.Add(idx2);
			newTriangles.Add(idx5);

			newTriangles.Add(idx5);
			newTriangles.Add(idx3);
			newTriangles.Add(idx6);

			newTriangles.Add(idx4);
			newTriangles.Add(idx5);
			newTriangles.Add(idx6);
        }

		vertices = newVertices.ToArray();
		triangles = newTriangles.ToArray();
    }

	
}