using UnityEngine;

public class Highlighter : MonoBehaviour
{
	private MeshRenderer _renderer;

	private Color _normalColor;
	private Color _highlightedColor;

	public void Highlight()
	{
		_renderer.material.color = _highlightedColor;
	}

	public void Unhighlight()
	{
		_renderer.material.color = _normalColor;
	}

	public void SetNormalColor(Color color)
	{
		_normalColor = color;
	}

	public void SetHighlightedColor(Color color)
	{
		_highlightedColor = color;
	}

	public void SetMeshRenderer(MeshRenderer meshRenderer)
	{
		_renderer = meshRenderer;
	}
}