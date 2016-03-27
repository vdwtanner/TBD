using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Crosshair : MonoBehaviour {
	private Material material;
	public Shader shader;
	public Texture2D crosshair;
	public float alphaCutoff;
	// Use this for initialization
	void Awake () {
		material = new Material(shader);
		material.SetTexture("_CrosshairTex", crosshair);
		material.SetFloat("_AlphaCutoff", alphaCutoff);
	}
	

	void OnRenderImage (RenderTexture src, RenderTexture dst) {
		material.SetTexture("_CrosshairTex", crosshair);
		material.SetFloat("_AlphaCutoff", alphaCutoff);
		Graphics.Blit(src, dst);
	}
}
