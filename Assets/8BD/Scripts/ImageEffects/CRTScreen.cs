using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class CRTScreen : MonoBehaviour
{
    public Shader shader;
    private Material _material;

    [Range(0, 1024)]
    public float pixels = 320f;
    [Range(-1, 1)]
    public float radialDistort = 1.0f;
    [Range(0, 0.5f)]
    public float vignetteDistance = 0.2f;
    [Range(0, 1)]
    public float tintStrength = 0.5f;
    [Range(0, 1)]
    public float pixelBlend = 0.5f;
    //    [Range(0, 1)]
    public Color tint = Color.white;




    protected Material material
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
            return _material;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null) return;
        Material mat = material;
        mat.SetFloat("_pixels", pixels);
        mat.SetFloat("_pixelsColor", Screen.width);//  pixelsColor);
        mat.SetFloat("_radialDistort", radialDistort);
        mat.SetFloat("_vignetteDistance", vignetteDistance);
        mat.SetFloat("_pixelBlend", pixelBlend);
        mat.SetColor("_tint", tint);
        mat.SetFloat("_tintStrength", tintStrength);
        Graphics.Blit(source, destination, mat, 0);
    }

    void OnDisable()
    {
        if (_material)
        {
            DestroyImmediate(_material);
        }
    }
}