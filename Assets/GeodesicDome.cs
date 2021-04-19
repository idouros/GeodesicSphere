using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GeodesicDome : MonoBehaviour
{
	public int iterations = 1;

	private Vector3[] vertices;
	private Vector3[] verticesSpherical;
	private int[] triangles;

	public void Start()
	{
		InitOctahedron();
		verticesSpherical = CartesianToSpherical(vertices);

		for(int i = 2; i < iterations; i++)
        {
			Subdivide();
        }

		vertices = SphericalToCartesian(verticesSpherical);
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
			0, 1, 2,
			0, 2, 3, 
			0, 3, 4,
			0, 4, 1, 
			5, 2, 1,
			5, 3, 2,
			5, 4, 3,
			5, 1, 4
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
		List<Vector3> newVerticesSpherical = new List<Vector3>(verticesSpherical);

		for (int i = 0; i < triangles.Length; i++)
        {
			var idx1 = triangles[i * 3];
			var idx2 = triangles[i * 3 + 1];
			var idx3 = triangles[i * 3 + 2];
			var idx4 = k;

			var v1 = vertices[idx1];
			var v2 = vertices[idx2];
			var v3 = vertices[idx3];
			var v4 = new Vector3 ( 1, (v1[1]+v2[1]+v3[1])/3.0f, (v1[2]+v2[2]+v3[2])/3.0f );
			newVerticesSpherical.Add(v4);

			newTriangles.Add(idx1);
			newTriangles.Add(idx2);
			newTriangles.Add(idx4);

			newTriangles.Add(idx2);
			newTriangles.Add(idx3);
			newTriangles.Add(idx4);

			newTriangles.Add(idx3);
			newTriangles.Add(idx1);
			newTriangles.Add(idx4);

			k++;
        }

		verticesSpherical = newVerticesSpherical.ToArray();
		triangles = newTriangles.ToArray();
    }


	private Vector3 CartesianToSpherical(Vector3 v_in)
    {
		var x = v_in[0];
		var y = v_in[1];
		var z = v_in[2];

		var r = 1.0f; // (float)Math.Sqrt(x * x + y * y + z * z);
		var theta = (float)Math.Acos(z); //(float)Math.Acos(z / r);
		var phi = (float)Math.Atan2(y, x);

		//Debug.Log(r + " " + theta + " " + phi);

		return new Vector3 ( r, theta, phi );
	}

	private Vector3[] CartesianToSpherical(Vector3[] a_in)
    {
		int n = a_in.Length;
		Vector3[] a_out = new Vector3[n];
		for(int i = 0; i < n; i++)
        {
			a_out[i] = CartesianToSpherical(a_in[i]);
        }
		return a_out;
    }

	private Vector3 SphericalToCartesian(Vector3 v_in)
    {
		//var r = v_in[0];
		var theta = v_in[1];
		var phi = v_in[2];

		var x = (float)(Math.Sin(theta) * Math.Cos(phi));
		var z = (float)(Math.Sin(theta) * Math.Sin(phi));
		var y = (float)(Math.Cos(theta));

		//Debug.Log(x + " " + y + " " + z);

		return new Vector3 ( x, y, z );
	}

	private Vector3[] SphericalToCartesian(Vector3[] a_in)
    {
		int n = a_in.Length;
		Vector3[] a_out = new Vector3[n];
		for (int i = 0; i < n; i++)
		{
			a_out[i] = SphericalToCartesian(a_in[i]);
		}
		return a_out;
	}
}