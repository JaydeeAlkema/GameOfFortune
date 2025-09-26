using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Wheel : MonoBehaviour
{
	[SerializeField] private int NumberOfSlices = 8;
	[SerializeField] private float Radius = 5f;
	[SerializeField] private int Segments = 10;

	[SerializeField] private SlicePrefab SlicePrefab;
	[SerializeField] private Transform WheelParent;

	[SerializeField] private Material SliceMaterial;
	[SerializeField] private Color SliceColorA;
	[SerializeField] private Color SliceColorB;
	[SerializeField] private Color SliceHighlightColor;

	private float _sliceAngle;
	private SlicePrefab[] _slicePrefabs;
	private int _currentHighlighted = -1;

	private void Start()
	{
		GenerateWheel();
	}

	private void GenerateWheel()
	{
		if (NumberOfSlices <= 0) return;
		if (WheelParent == null)
		{
			Debug.LogError("WheelParent is not assigned.");
			return;
		}

		if (_slicePrefabs.Length > 0)
			for (int i = _slicePrefabs.Length - 1; i >= 0; i--)
			{
				if (_slicePrefabs[i] == null)
					continue;

				Destroy(_slicePrefabs[i].gameObject);
			}

		_slicePrefabs = new SlicePrefab[NumberOfSlices];
		_sliceAngle = 360f / NumberOfSlices;

		// Generate a single base slice mesh (centered upwards)
		Mesh baseSliceMesh = GenerateSlice(-_sliceAngle / 2f, _sliceAngle / 2f, Radius, Segments);

		for (int i = 0; i < NumberOfSlices; i++)
		{
			SlicePrefab newSlice = Instantiate(SlicePrefab, WheelParent.position, Quaternion.identity, WheelParent);
			newSlice.name = $"Slice {i}";

			// Position at center, rotate into place
			newSlice.transform.localPosition = Vector3.zero;
			newSlice.transform.localRotation = Quaternion.Euler(0, 0, -i * _sliceAngle);

			// Alternate colors
			Color sliceColor = i % 2 == 0 ? SliceColorA : SliceColorB;

			MeshFilter sliceMeshFilter = newSlice.GetMeshFilter();
			sliceMeshFilter.mesh = baseSliceMesh;

			MeshRenderer meshRenderer = newSlice.GetMeshRenderer();
			meshRenderer.material = SliceMaterial;
			meshRenderer.material.color = sliceColor;
			meshRenderer.rayTracingMode = RayTracingMode.Off;

			Highlighter highlighter = newSlice.GetHighlighter();
			highlighter.SetMeshRenderer(meshRenderer);
			highlighter.SetNormalColor(sliceColor);
			highlighter.SetHighlightedColor(SliceHighlightColor);

			_slicePrefabs[i] = newSlice;
		}
	}

	private void Update()
	{
		int index = GetSliceAtPin();
		if (index == _currentHighlighted) return;

		// Reset old
		if (_currentHighlighted >= 0)
			_slicePrefabs[_currentHighlighted].GetHighlighter().Unhighlight();

		// Highlight new
		SlicePrefab slicePrefab = _slicePrefabs[index];
		slicePrefab.GetHighlighter().Highlight();
		slicePrefab.GetAudioCue().Play();
		_currentHighlighted = index;
	}

	// Generates one wedge pointing up, spanning startAngle..endAngle around +Y
	private Mesh GenerateSlice(float startAngle, float endAngle, float radius, int segments)
	{
		Mesh mesh = new();
		List<Vector3> vertices = new();
		List<int> triangles = new();

		vertices.Add(Vector3.zero);

		for (int i = 0; i <= segments; i++)
		{
			float t = i / (float)segments;
			float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
			vertices.Add(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius);
		}

		for (int i = 1; i < vertices.Count - 1; i++)
		{
			triangles.Add(0);
			triangles.Add(i);
			triangles.Add(i + 1);
		}

		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		return mesh;
	}

	private int GetSliceAtPin()
	{
		if (NumberOfSlices <= 0) return -1;

		float zRot = WheelParent.eulerAngles.z;

		float normalized = (360f + zRot) % 360f;
		normalized += _sliceAngle / 2f;
		normalized %= 360f;

		int index = Mathf.FloorToInt(normalized / _sliceAngle) % NumberOfSlices;
		if (index < 0) index += NumberOfSlices;
		return index;
	}

	public int GetWinningSlice()
	{
		return GetSliceAtPin();
	}
}
