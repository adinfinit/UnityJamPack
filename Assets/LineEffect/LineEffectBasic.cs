using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LineEffectBasic : MonoBehaviour
{
    public float low = 1.0f;
	public float high = 8.0f;

	public Color line = new Color(0f, 0f, 0f);
	public Color background = new Color(1f, 1f, 1f);

	public Shader shader;
	private Material material;

	// Creates a private material used to the effect
	void Awake ()
	{
		shader = Shader.Find ("Hidden/LineEffectBasic");
		material = new Material (shader);
	}

	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (material == null) {
			material = new Material (shader);
		}

		material.SetFloat ("_Low", low);
		material.SetFloat ("_High", high);

		material.SetColor ("_LineColor", line);
		material.SetColor ("_BackgroundColor", background);

		Graphics.Blit (source, destination, material);
	}
}