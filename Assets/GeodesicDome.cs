using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GeodesicDome : MonoBehaviour
{
	private int iterations = 4;

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
			new Vector3 (0, 0, 1), // North Pole
			new Vector3 (s2, -s2, 0),
			new Vector3 (s2, s2, 0),
			new Vector3 (-s2, s2, 0),
			new Vector3 (-s2, -s2,0),
			new Vector3 (0, 0, -1) // South Pole
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

			var v4 = new Vector3 ( (v1[0]+v2[0])/2.0f, (v1[1]+v2[1])/2.0f, (v1[2]+v2[2])/2.0f );
			var v5 = new Vector3 ( (v2[0]+v3[0])/2.0f, (v2[1]+v3[1])/2.0f, (v2[2]+v3[2])/2.0f );
			var v6 = new Vector3 ( (v3[0]+v1[0])/2.0f, (v3[1]+v1[1])/2.0f, (v3[2]+v1[2])/2.0f );
			
			int idx4, idx5, idx6;


			// TODO check vertex before adding		
			if(true)
			{
				idx4 = k;
				var l = (float)Math.Sqrt(v4[0]*v4[0] + v4[1]*v4[1] + v4[2]*v4[2]);
				var v4Projected = new Vector3(v4[0]/l, v4[1]/l, v4[2]/l);
				newVertices.Add(v4Projected);	
			}

			if(true)
			{
				idx5 = k+1;
				var l = (float)Math.Sqrt(v5[0]*v5[0] + v5[1]*v5[1] + v5[2]*v5[2]);
				var v5Projected = new Vector3(v5[0]/l, v5[1]/l, v5[2]/l);
				newVertices.Add(v5Projected);	
			}

			if(true)
			{
				idx6 = k+2;
				var l = (float)Math.Sqrt(v6[0]*v6[0] + v6[1]*v6[1] + v6[2]*v6[2]);
				var v6Projected = new Vector3(v6[0]/l, v6[1]/l, v6[2]/l);
				newVertices.Add(v6Projected);	
			}


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


			k = k + 3;
        }

		vertices = newVertices.ToArray();
		triangles = newTriangles.ToArray();
    }

	
}