using UnityEngine;


namespace LemonSpawn
{


    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]

    public class CRTScreen : MonoBehaviour
    {
        public Shader shader;
        private Material _material;

//        public Color tint = Color.white;
        public SerializedCRTSettings settings = new SerializedCRTSettings();



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
            mat.SetFloat("_pixels", settings.pixels);
            mat.SetFloat("_pixelsColorX", Screen.width);//  pixelsColor);
            mat.SetFloat("_pixelsColorY", Screen.height);//  pixelsColor);
            mat.SetFloat("_radialDistort", settings.radialDistort);
            mat.SetFloat("_vignetteDistance", settings.vignetteDistance);
            mat.SetFloat("_pixelBlend", settings.pixelBlend);
            mat.SetColor("_tint", settings.tint());
            mat.SetFloat("_tintStrength", settings.tintStrength);
            mat.SetFloat("_gamma", settings.gamma);
            mat.SetFloat("_correction", settings.correction);
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
}