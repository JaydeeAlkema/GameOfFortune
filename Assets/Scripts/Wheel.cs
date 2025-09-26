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
	[SerializeField] private Material HighlightMaterial;

	private float _sliceAngle;
	private SlicePrefab[] _slicePrefabs;
	private int _currentHighlighted = -1;

	private void Start()
	{
		if (NumberOfSlices <= 0) return;
		if (WheelParent == null)
		{
			Debug.LogError("WheelParent is not assigned.");
			return;
		}

		_slicePrefabs = new SlicePrefab[NumberOfSlices];
		_sliceAngle = 360f / NumberOfSlices;

		// Correct offset so slice 0 is centered at "up"
		float offset = 90f + _sliceAngle / 2f;

		for (int i = 0; i < NumberOfSlices; i++)
		{
			float start = offset - i * _sliceAngle;
			float end = offset - (i + 1) * _sliceAngle;

			Mesh sliceMesh = GenerateSlice(start, end, Radius, Segments);

			SlicePrefab newSlice = Instantiate(SlicePrefab, Vector3.zero, Quaternion.identity, WheelParent);
			newSlice.name = $"Slice {i}";

			Color sliceColor = i % 2 == 0 ? SliceColorA : SliceColorB;

			MeshFilter sliceMeshFilter = newSlice.GetMeshFilter();
			sliceMeshFilter.mesh = sliceMesh;

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

	private static Mesh GenerateSlice(float startAngle, float endAngle, float radius, int segments)
	{
		Mesh mesh = new();
		List<Vector3> vertices = new();
		List<int> triangles = new();

		// Center
		vertices.Add(Vector3.zero);

		// Arc points
		for (int i = 0; i <= segments; i++)
		{
			float t = i / (float)segments;
			float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
			vertices.Add(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius);
		}

		// Triangle fan
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

		// Normalize to 0–360
		float normalized = (360f + zRot) % 360f;

		// Add half a slice to align centers with the pin at the top
		normalized += _sliceAngle / 2f;

		// Wrap around in case it goes over 360
		normalized %= 360f;

		int index = Mathf.FloorToInt(normalized / _sliceAngle) % NumberOfSlices;
		if (index < 0) index += NumberOfSlices;
		return index;
	}

	/// <summary>
	///     Returns the index of the slice currently under the top pin.
	/// </summary>
	public int GetWinningSlice()
	{
		return GetSliceAtPin();
	}
}
