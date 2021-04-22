using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOFView : MonoBehaviour
{
	Mesh mesh;
	private MeshFilter meshFilter = null;

	[Tooltip("The FOV degree")] [Range(0,360)] [SerializeField] private float fov = 90f;
	[Tooltip("The FOV throw rays count")] [Range(0, 500)] [SerializeField] private int rayCount = 2;

	[Tooltip("The FOV range of view")] [Range(0, 50)] [SerializeField] private float viewDistance = 10f;
	public float ViewDistance { get => viewDistance; set => viewDistance = value; }

	public LayerMask fovLayerMask = new LayerMask();

	private float startingAngle = 0f;
	private Vector3 origin;

	private void Start()
	{
		if(meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
		}
		origin = Vector3.zero;
		mesh = new Mesh();
		meshFilter.mesh = mesh;
	}

	private void LateUpdate()
	{
		BakeMesh();
	}

	private void BakeMesh()
	{

		float angle = startingAngle + fov /2;
		float angleIncrease = fov / rayCount;

		//Mesh Fields
		Vector3[] vertices = new Vector3[rayCount + 2];
		Vector2[] uv = new Vector2[vertices.Length];
		int[] triangles = new int[rayCount * 3];


		vertices[0] = origin;

		//Indexes
		int verticesIndex = 1;
		int trianglesIndex = 0;

		//Bake The Mesh
		for (int i = 0; i <= rayCount; i++)
		{
			Vector3 vertic = Vector3.zero;
			//Debug.DrawRay(origin, GetVectorFromAngle(angle) * viewDistance , Color.red);
			RaycastHit2D hit = Physics2D.Raycast(origin, GetVectorFromAngle(angle), ViewDistance, fovLayerMask);
			if (hit.collider == null)
			{
				//No hit
				vertic = origin + GetVectorFromAngle(angle) * ViewDistance;
			}
			else
			{
				//Hited a object
				vertic = hit.point;
			}

			vertices[verticesIndex] = vertic;

			if (i > 0)
			{
				triangles[trianglesIndex] = 0;
				triangles[trianglesIndex + 1] = verticesIndex - 1;
				triangles[trianglesIndex + 2] = verticesIndex;

				trianglesIndex += 3;
			}

			verticesIndex++;
			angle -= angleIncrease;
		}

		//Set the mesh
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
	}


	public void SetTheOrigin(Vector3 origin)
	{
		this.origin = origin;
	}

	public void SetAimDirection(Vector3 aimDirection)
	{
		startingAngle = GetAngleFromVectorFloat(aimDirection) - fov / 2;
	}

	#region Extentions

	private Vector3 GetVectorFromAngle(float angle)
	{
		float angleRad = angle * (Mathf.PI / 180f);
		return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
	}

	private float GetAngleFromVectorFloat(Vector3 dir)
	{
		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if (n < 0)
		{
			n += 360;
		}
		return n;
	}

	#endregion



}

