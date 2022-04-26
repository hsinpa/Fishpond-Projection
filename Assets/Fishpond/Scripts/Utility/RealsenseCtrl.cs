using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hsinpa.Realsense {
    public class RealsenseCtrl : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.RawImage debugTexture;

        [SerializeField, Range(0, 1)]
        private float depthmapThreshold;

        [SerializeField]
        private Material realsenseMat;

        [SerializeField]
        private Material customizeMat;

        private Texture rawDepthMapTexture;
        private RenderTexture grayDepthMapTexture;
        private RenderTexture filterTexture;

        public Texture texture
        {
            get
            {
                return rawDepthMapTexture;
            }
            set
            {
                if (rawDepthMapTexture == value)
                    return;

                rawDepthMapTexture = value;
                OnTexture(value);
            }
        }

        private void OnTexture(Texture p_texture) {
            Debug.Log("OnTexture " + p_texture.graphicsFormat);

            if (grayDepthMapTexture == null)
                grayDepthMapTexture = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 16, RenderTextureFormat.R16);

            if (filterTexture == null)
                filterTexture = TextureUtility.GetRenderTexture(p_texture.width, p_texture.height, 8, RenderTextureFormat.ARGB32);

            debugTexture.texture = filterTexture;
        }

        private void Update()
        {
            if (filterTexture == null || grayDepthMapTexture == null) return;

            Graphics.Blit(rawDepthMapTexture, grayDepthMapTexture, realsenseMat);
            Graphics.Blit(grayDepthMapTexture, filterTexture, customizeMat);
        }

    }
}
