using UnityEngine;
using System.Collections.Generic;
using System;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GeodesicDome : MonoBehaviour
{
	public int iterations = 3;

	private UnityEngine.Vector3[] vertices; 
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

		// Volume calculations
		var volumeOfDome = Volume();
		var volumeOfUnitSphere = (4.0f / 3.0f) * (float)Math.PI;
		Debug.Log("Volume BEFORE adjustment : " + volumeOfDome);
		Debug.Log("Volume of CIRCUMSCRIBING (Unit) Sphere : " + volumeOfUnitSphere);
		Debug.Log("Dome volume is " + ((volumeOfDome/volumeOfUnitSphere)*100.0).ToString("F4") + "% of the volume of the circumscribing sphere");

		CreateFinalMesh();
	}

	private void AdjustVolume(float startVolume, float targetVolume)
	{
		var factor = (float)(Math.Sqrt(targetVolume/startVolume));
		for(int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = vertices[i] * factor;
		}
	}

	private float Volume()
	{
		var totalVolume = 0.0f;
		for (int i = 0; i < (triangles.Length/3); i++)
        {
			var idx1 = triangles[i * 3];
			var idx2 = triangles[i * 3 + 1];
			var idx3 = triangles[i * 3 + 2];

			var v1 = vertices[idx1];
			var v2 = vertices[idx2];
			var v3 = vertices[idx3];

			Matrix4x4 m = new Matrix4x4();
			m.SetColumn(0, new Vector4(v1.x, v1.y, v1.z, 1));
			m.SetColumn(1, new Vector4(v2.x, v2.y, v2.z, 1));
			m.SetColumn(2, new Vector4(v3.x, v3.y, v3.z, 1));
			m.SetColumn(3, new Vector4(0, 0, 0, 1));
			var partialVolume = (float)Math.Abs(m.determinant / 6.0);
			
			totalVolume += partialVolume;
		}
		return totalVolume;
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

	private int VertexAlreadyExists(Vector3 v0, List<Vector3> a)
	{
		for(int i = 0; i < a.Count; i++)
		{
			var v = a[i];
			if(v0 == v)
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

			int idx4 = -1, idx5 = -1, idx6 = -1;

			// Calculate and add the three new vertices
			var v4Interpolated = UnityEngine.Vector3.Lerp(v1, v2, 0.5f);
			var v4Projected = UnityEngine.Vector3.Normalize(v4Interpolated);
			int v4p_idx = VertexAlreadyExists(v4Projected, newVertices);
			AddVertexIfNotPresent(v4p_idx, ref idx4, v4Projected, ref k, ref newVertices);

			var v5Interpolated = UnityEngine.Vector3.Lerp(v2, v3, 0.5f);
			var v5Projected = UnityEngine.Vector3.Normalize(v5Interpolated);
			int v5p_idx = VertexAlreadyExists(v5Projected, newVertices);
			AddVertexIfNotPresent(v5p_idx, ref idx5, v5Projected, ref k, ref newVertices);
			
			var v6Interpolated = UnityEngine.Vector3.Lerp(v3, v1, 0.5f);
			var v6Projected = UnityEngine.Vector3.Normalize(v6Interpolated);
			int v6p_idx = VertexAlreadyExists(v6Projected, newVertices);
			AddVertexIfNotPresent(v6p_idx, ref idx6, v6Projected, ref k, ref newVertices);

			// Add the indices to form three new triangles to replace the original one

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