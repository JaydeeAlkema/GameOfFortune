using UnityEngine;

public class SlicePrefab : MonoBehaviour
{
	[SerializeField] private MeshRenderer MeshRenderer;
	[SerializeField] private MeshFilter MeshFilter;
	[SerializeField] private Highlighter Highlighter;
	[SerializeField] private AudioCue AudioCue;

	public MeshRenderer GetMeshRenderer()
	{
		return MeshRenderer;
	}

	public MeshFilter GetMeshFilter()
	{
		return MeshFilter;
	}

	public Highlighter GetHighlighter()
	{
		return Highlighter;
	}

	public AudioCue GetAudioCue()
	{
		return AudioCue;
	}
}