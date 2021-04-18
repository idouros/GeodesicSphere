using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GeodesicDome : MonoBehaviour
{

	void Start()
	{
		CreateOctahedron();
	}

	private void CreateOctahedron()
	{
		double s2 = 1.0 / Math.Sqrt(2.0);
		Vector3[] vertices = {
			new Vector3 (0, 0, 1),
			new Vector3 (1, -1, 0),
			new Vector3 (1, 1, 0),
			new Vector3 (-1, 1, 0),
			new Vector3 (-1, -1,0),
			new Vector3 (0, 0, -1)
		};

		int[] triangles = {
			0, 1, 2,
			0, 2, 3, 
			0, 3, 4,
			0, 4, 1, 
			5, 2, 1,
			5, 3, 2,
			5, 4, 3,
			5, 1, 4
		};

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		//mesh.Optimize();
		mesh.RecalculateNormals();
	}
}